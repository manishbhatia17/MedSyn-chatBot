using System;
using System.Linq;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.Configurations;
using MedGyn.MedForce.Facade.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace MedGyn.MedForce.Web.Middleware
{
    
    public class ChatWidgetAccessFilter : IAsyncActionFilter
    {
        private readonly ChatWidgetSettings _settings;
        private readonly IMemoryCache _cache;

        public ChatWidgetAccessFilter(IOptions<ChatWidgetSettings> settings, IMemoryCache cache)
        {
            _settings = settings.Value;
            _cache = cache;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!IsWithinRateLimit(context))
            {
                context.Result = new ObjectResult("Too many requests. Please slow down and try again shortly.") { StatusCode = 429 };
                return;
            }

            if (!IsTokenValid(context))
            {
                context.Result = new UnauthorizedObjectResult("Invalid or missing widget token.");
                return;
            }

            var request = context.ActionArguments.Values.OfType<IChatWidgetRequest>().FirstOrDefault();
            if (!IsOriginAllowed(context, request?.CompanyId))
            {
                context.Result = new ObjectResult("This embed is not authorized for this site.") { StatusCode = 403 };
                return;
            }

            await next();
        }

        private bool IsTokenValid(ActionExecutingContext context)
        {
            if (string.IsNullOrWhiteSpace(_settings.SharedToken))
                return true;

            var providedToken = context.HttpContext.Request.Headers["X-Widget-Token"].ToString();
            return string.Equals(providedToken, _settings.SharedToken, StringComparison.Ordinal);
        }

        private bool IsOriginAllowed(ActionExecutingContext context, string companyId)
        {
            if (_settings.AllowedEmbeds == null || _settings.AllowedEmbeds.Count == 0)
                return true;

            var origin = context.HttpContext.Request.Headers["Origin"].ToString();
            if (string.IsNullOrWhiteSpace(origin))
                origin = context.HttpContext.Request.Headers["Referer"].ToString();

            if (string.IsNullOrWhiteSpace(origin) || string.IsNullOrWhiteSpace(companyId))
                return false;

            return _settings.AllowedEmbeds.Any(e =>
                string.Equals(e.CompanyId, companyId, StringComparison.OrdinalIgnoreCase) &&
                origin.IndexOf(e.Domain, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private bool IsWithinRateLimit(ActionExecutingContext context)
        {
            var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var window = DateTime.UtcNow.Ticks / (_settings.WindowSeconds * TimeSpan.TicksPerSecond);
            var cacheKey = $"chatwidget_ratelimit_{ip}_{window}";

            var count = _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_settings.WindowSeconds);
                return 0;
            });

            count++;
            _cache.Set(cacheKey, count, TimeSpan.FromSeconds(_settings.WindowSeconds));

            return count <= _settings.MaxRequestsPerWindow;
        }
    }
}
