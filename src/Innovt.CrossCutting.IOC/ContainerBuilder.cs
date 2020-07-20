﻿using Innovt.Core.CrossCutting.Ioc;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Innovt.CrossCutting.IOC
{
    public class ContainerBuilder
    { 
        private IContainer container = null;

        public ContainerBuilder(IServiceCollection services)
        { 
            this.container = new Container(services);
        }

        public IServiceProvider GetServiceProvider()
        {
            return container;
        }
    }

}
