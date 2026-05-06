using System.Collections.Generic;
using System.Threading.Tasks;
using MedGyn.MedForce.Common.Configurations;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Facade.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MedGyn.MedForce.Web.Controllers
{
	public class AccountController : BaseController
	{
		private readonly IAccountFacade _accountFacade;
		private readonly ISecurityFacade _securityFacade;
		private readonly AppSettings _appSettings;

		public AccountController(IAccountFacade accountFacade, ISecurityFacade securityFacade, IOptions<AppSettings> appSettingsOptionsAccessor)
		{
			_accountFacade = accountFacade;
			_securityFacade = securityFacade;
			_appSettings = appSettingsOptionsAccessor.Value;
		}

		[HttpGet]
		public IActionResult Login(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			var model = new LoginViewModel()
			{
				BuildVersion = _appSettings.BuildVersion,
				BuildTimestamp = _appSettings.BuildTimestamp.ToLongDateString()
			};

			return View(model);
		}

		[HttpPost]
		public IActionResult Login(LoginViewModel model, string returnUrl)
		{
			var validatedModel = _accountFacade.Login(model);
			if (!validatedModel.Success)
			{
				return View(validatedModel);
			}

			if (validatedModel.ForcePasswordReset)
			{
				return RedirectToAction("ChangePassword", new { email = validatedModel.Email, model.RememberMe });
			}

			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}

			if (_securityFacade.IsAuthorized(new List<SecurityKeyEnum> {
				SecurityKeyEnum.CustomerOrderView,
				SecurityKeyEnum.CustomerOrderEdit,
				SecurityKeyEnum.CustomerOrderRescindOrder,
				SecurityKeyEnum.CustomerOrderExport,
				SecurityKeyEnum.CustomerOrderCustomersSeeAll,
				SecurityKeyEnum.CustomerOrderDomesticManagerApproval,
				SecurityKeyEnum.CustomerOrderDomesticVPApproval,
				SecurityKeyEnum.CustomerOrderDomesticCustomersSeeAll,
				SecurityKeyEnum.CustomerOrderDomesticDistributorManagerApproval,
				SecurityKeyEnum.CustomerOrderDomesticDistributorVPApproval,
				SecurityKeyEnum.CustomerOrderDomesticDistributorSeeAll,
				SecurityKeyEnum.CustomerOrderInternationalManagerApproval,
				SecurityKeyEnum.CustomerOrderInternationalVPApproval,
				SecurityKeyEnum.CustomerOrderInternationalSeeAll,
				SecurityKeyEnum.CustomerOrderShippable,
				SecurityKeyEnum.CustomerOrderShipRescind,
				SecurityKeyEnum.CustomerOrderFulfillment,
				SecurityKeyEnum.CustomerOrderFulfillmentRescind
				}))
				return RedirectToAction("Index", "CustomerOrder");

			if (_securityFacade.IsAuthorized(new List<SecurityKeyEnum> {
				SecurityKeyEnum.PurchaseOrderView,
				SecurityKeyEnum.PurchaseOrderEdit,
				SecurityKeyEnum.PurchaseOrderDomesticView,
				SecurityKeyEnum.PurchaseOrderDomesticEdit,
				SecurityKeyEnum.PurchaseOrderDomesticVPApproval,
				SecurityKeyEnum.PurchaseOrderDomesticDistributionView,
				SecurityKeyEnum.PurchaseOrderDomesticDistributionEdit,
				SecurityKeyEnum.PurchaseOrderDomesticDistributionVPApproval,
				SecurityKeyEnum.PurchaseOrderInternationalView,
				SecurityKeyEnum.PurchaseOrderInternationalEdit,
				SecurityKeyEnum.PurchaseOrderInternationalVPApproval,
				SecurityKeyEnum.PurchaseOrderReceive
				}))
				return RedirectToAction("Index", "PurchaseOrder");

			if (_securityFacade.IsAuthorized(new List<SecurityKeyEnum> {
				SecurityKeyEnum.CustomerView,
				SecurityKeyEnum.CustomerEdit,
				SecurityKeyEnum.CustomerSeeAll,
				SecurityKeyEnum.ExportCustomerList
				}))
				return RedirectToAction("Index", "Customer");

			if (_securityFacade.IsAuthorized(new List<SecurityKeyEnum> {
				SecurityKeyEnum.ProductView,
				SecurityKeyEnum.ProductEdit,
				SecurityKeyEnum.ExportProductList,
				SecurityKeyEnum.InventoryAdjust,
				SecurityKeyEnum.PurchaseOrderEdit
				}))
				return RedirectToAction("Index", "Product");

			if (_securityFacade.IsAuthorized(new List<SecurityKeyEnum> { SecurityKeyEnum.VendorView, SecurityKeyEnum.VendorEdit }))
				return RedirectToAction("Index", "Vendor");

			if (_securityFacade.IsAuthorized(new List<SecurityKeyEnum> { SecurityKeyEnum.UserView, SecurityKeyEnum.UserEdit }))
				return RedirectToAction("Index", "User");
			if (_securityFacade.IsAuthorized(new List<SecurityKeyEnum> { SecurityKeyEnum.RoleView, SecurityKeyEnum.RoleEdit }))
				return RedirectToAction("Index", "Role");

			if (_securityFacade.IsAuthorized(new List<SecurityKeyEnum> { SecurityKeyEnum.CodeTypes, SecurityKeyEnum.Codes }))
				return RedirectToAction("Index", "CodeType");


			return RedirectToAction("Login");
		}

		public IActionResult Logout()
		{
			_accountFacade.Logout();
			return RedirectToAction("Login");
		}

		public IActionResult ImpersonateUser(int id, string name)
		{
			//_accountFacade.ImpersonateUser(id, name);
			return RedirectToAction("Tasks", "Home");
		}

		[HttpGet]
		public IActionResult ResetPassword()
		{
			return View(new ResetPasswordViewModel());
		}

		[HttpPost]
		public IActionResult ResetPassword(ResetPasswordViewModel model)
		{
			_accountFacade.ResetPassword(model);

			return RedirectToAction("Login");
		}

		[HttpGet]
		public IActionResult ChangePassword(string email, bool isRememberMe)
		{
			var model = _accountFacade.CheckChangePassword(email);

			if (model.Email == null)
			{
				View();
			}

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
		{
			var validatedModel = await _accountFacade.ChangePassword(model);
			if (!validatedModel.Success)
			{
				return View(validatedModel);
			}

			return RedirectToAction("Login");
		}

		public override UnauthorizedResult Unauthorized()
		{
			return new UnauthorizedResult();
		}
	}
}
