[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(WebRegistrationForm.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(WebRegistrationForm.App_Start.NinjectWebCommon), "Stop")]

namespace WebRegistrationForm.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Ninject.Web.Common.WebHost;
    using WebRegistrationForm.DataAccess;
    using WebRegistrationForm.Service;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application.
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            //Services 
            kernel.Bind<IWebRegistrationFormService>().To<WebRegistraionFormService>();
            kernel.Bind<ISignUpFormServices>().To<signUpFormServices>();
            kernel.Bind<ILogInFormService>().To<logInFormService>();
            kernel.Bind<IDashboardService>().To<dashboardService>();

            //DataAccess
            kernel.Bind<IWebRegistrationFormDataAccess>().To<WebRegistrationFormDataAccess>();
            kernel.Bind<ISignUpFormDataAccess>().To<signUpFormDataAccess>();
            kernel.Bind<ILogInFormDataAccess>().To<logInFormDataAccess>();
            kernel.Bind<IDashboardDataAccess>().To<dashboardDataAccess>();

            
        }
    }
}