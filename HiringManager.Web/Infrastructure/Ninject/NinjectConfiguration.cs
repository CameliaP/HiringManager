﻿using System.Web;
using HiringManager.Web.Ninject;
using Microsoft.Owin.Security;
using Ninject;
using Simple.Validation.Ninject;
using Ninject.Extensions.Conventions;

namespace HiringManager.Web.Infrastructure.Ninject
{
    public class NinjectConfiguration

    {
        public void Configure(IKernel kernel)
        {
            kernel.Load(new GeneralModule());
            kernel.Load(new SimpleValidationNinjectModule());

            kernel.Bind(configuration => configuration
                .FromAssemblyContaining<NinjectConfiguration>()
                .SelectAllClasses()
                .InNamespaces("HiringManager.Web.Mappers")
                .BindAllInterfaces()
                )
                ;

            kernel.Bind<IAuthenticationManager>()
                .ToMethod(context => HttpContext.Current.GetOwinContext().Authentication);


        }
    }
}