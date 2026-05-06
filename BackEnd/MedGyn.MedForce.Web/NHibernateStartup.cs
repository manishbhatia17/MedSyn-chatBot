using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Data.Mappings;
using MedGyn.MedForce.Data.Repositories;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using System;

namespace MedGyn.MedForce.Web
{
	public static class NHibernateStartup
	{
		public static IServiceCollection AddNHibernate(this IServiceCollection services, string connectionString)
		{
			ISessionFactory sessionFactory;
			try
			{
				sessionFactory = Fluently.Configure()
				.Database(MsSqlConfiguration.MsSql2012.ConnectionString(connectionString))
				.Mappings(m => m.FluentMappings.AddFromAssemblyOf<UserMapping>())
				.BuildSessionFactory();
			}
			catch (Exception)
			{
				// I don't know what the point of this is, because if it catches and falls in here, nothing works

				sessionFactory = Fluently.Configure()
				.Database(MsSqlConfiguration.MsSql2012.ConnectionString(connectionString))
				.Mappings(m => m.FluentMappings.AddFromAssemblyOf<Program>())
				.BuildSessionFactory();
			}

			services.AddSingleton(sessionFactory);
			services.AddScoped(factory => sessionFactory.OpenSession());
			services.AddScoped<IDbContext, DbContext>();

			return services;
		}
	}
}
