using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.Configuration;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.WebHost.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using ServiceStack.Redis;
using ServiceStack.ServiceInterface.Validation;
using ServiceStack.Logging;
using ServiceStack.Logging.Support.Logging;
using ServiceStack.Logging.Elmah;
using ServiceStack.MiniProfiler;
using ServiceStack.MiniProfiler.Data;
using ServiceStack.ServiceInterface.Admin;
using ServiceStack.Redis.Messaging;
using Common;

namespace WebApplication.ServiceStack
{
	public class Global : System.Web.HttpApplication
	{
		public class ServiceStackAppHost : AppHostBase
		{
			public ServiceStackAppHost()
				: base("ProteinTracker", typeof(EntryService).Assembly)
			{
			}

			public override void Configure(Funq.Container container)
			{
				//to inject third-party IoC (for example for NInject use SrviceStack.ContainerAdapter.NInject)
				//IKernel kernel=new StandartKernel();
				//kernel.Bind<TrackedDataRepository>().ToSelf();
				//container.Adapter=new NinjectContainerAdapter(kernel);  -> provide a adapter layer for NInject to use in Funq


				Plugins.Add(new AuthFeature(() => new AuthUserSession(),
							new IAuthProvider[] { new BasicAuthProvider() , 
								new TwitterAuthProvider(new AppSettings())}));

				Plugins.Add(new RegistrationFeature());

				//register validators
				Plugins.Add(new ValidationFeature());
				container.RegisterValidators(typeof(Common.Entry).Assembly, typeof(EntryService).Assembly);


				//request logs
				Plugins.Add(new RequestLogsFeature()); // added ability to view request via http:/..../requestlogs

				//cache registration 
				container.Register<ICacheClient>(new MemoryCacheClient());
				container.Register<IRedisClientsManager>(new PooledRedisClientManager("localhost:6379"));
				//container.Register<ICacheClient>(r => (ICacheClient)r.Resolve<IRedisClientsManager>().GetCacheClient());

				var userRepository = new InMemoryAuthRepository();
				container.Register<IUserAuthRepository>(userRepository);

				string hash;
				string salt;
				new SaltedHash().GetHashAndSaltString("password1", out hash, out salt);

				userRepository.CreateUserAuth(new UserAuth()
				{
					Id = 1,
					DisplayName = "Joe user",
					Email = "joe@user.com",
					UserName = "jsuser",
					LastName = "jname",
					PasswordHash = hash,
					Salt = salt,
					Roles = new List<string> { RoleNames.Admin }//,
					//Permissions = new List<string> { "GetStatus", "AddStatus" }
				}, "password1");


				//automatically inject in all public properties
				container.RegisterAutoWired<TrackedDataRepository>().ReusedWithin(Funq.ReuseScope.Default);
				container.RegisterAutoWired<TrackedDataRepository2>().ReusedWithin(Funq.ReuseScope.Default);

				var dbConFactory = new OrmLiteConnectionFactory(HttpContext.Current.Server.MapPath("~/App_Data/data.txt"), SqliteDialect.Provider)
				{
					ConnectionFilter = x => new ProfiledDbConnection(x, Profiler.Current)
				};
				container.Register<IDbConnectionFactory>(dbConFactory);

				SetConfig(new EndpointHostConfig { DebugMode = true });

				var mqService = new RedisMqServer(container.Resolve<IRedisClientsManager>());
				mqService.RegisterHandler<Entry>(ServiceController.ExecuteMessage);
				mqService.Start();

			}
		}

		protected void Application_Start(object sender, EventArgs e)
		{
			new ServiceStackAppHost().Init();
		}

		protected void Session_Start(object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			if (Request.IsLocal)
				Profiler.Start();
		}

		protected void Application_EndRequest(object sender, EventArgs e)
		{
			Profiler.Stop();
		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{

		}

		protected void Application_Error(object sender, EventArgs e)
		{

		}

		protected void Session_End(object sender, EventArgs e)
		{

		}

		protected void Application_End(object sender, EventArgs e)
		{

		}
	}
}