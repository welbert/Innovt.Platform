// INNOVT TECNOLOGIA 2014-2021
// Author: Michel Magalh�es
// Project: Innovt.CrossCutting.IOC
// Solution: Innovt.Platform
// Date: 2021-06-02
// Contact: michel@innovt.com.br or michelmob@gmail.com

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Innovt.CrossCutting.IOC
{
    public class ServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            return new(services);
        }

        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            return containerBuilder.GetServiceProvider();
        }
    }
}