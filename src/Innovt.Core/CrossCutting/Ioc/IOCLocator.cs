﻿using System;
using System.Linq;
using System.Reflection;
using Innovt.Core.Exceptions;

namespace Innovt.Core.CrossCutting.Ioc
{
    public static class IOCLocator
    {
        //private static  readonly IoCLocator instance = new IoCLocator();

        private static IContainer container;

        public static void Initialize(IContainer mainContainer)
        {
            container = mainContainer ?? throw new ArgumentNullException(nameof(mainContainer));
        }

        private static void ThowExceptionIfContainerIsNotInitialized()
        {
            if(container==null)
                throw new CriticalException("IOC Container not initialized");
        }

        public static void Register<T>(T type)
        {
            ThowExceptionIfContainerIsNotInitialized();

            container.Register(type);
        }
        
        public static object Resolve(Type type)
        {
            ThowExceptionIfContainerIsNotInitialized();

            return container.Resolve(type);
        }

        public static TService Resolve<TService>(Type type)
        {
            ThowExceptionIfContainerIsNotInitialized();

            return container.Resolve<TService>(type);
        }
        
        public static TService Resolve<TService>(string intanceKey)
        {
            ThowExceptionIfContainerIsNotInitialized();

            return container.Resolve<TService>(intanceKey);
        }

        public static TService Resolve<TService>(Type type,string intanceKey)
        {
            ThowExceptionIfContainerIsNotInitialized();

            return container.Resolve<TService>(type,intanceKey);
        }


        // public TService Resolve<TService>(Type type,string instanceKey)

        public static TService Resolve<TService>()
        {
            ThowExceptionIfContainerIsNotInitialized();

            return container.Resolve<TService>();
        }

        public static void Release(object inst)
        {
            ThowExceptionIfContainerIsNotInitialized();
            container.Release(inst);
        }

        public static void AddModule(IOCModule module)
        {
            if (module == null) throw new ArgumentNullException(nameof(module));

            ThowExceptionIfContainerIsNotInitialized();

            container.AddModule(module);
        }

        public static void AddModuleFromAssembly(Assembly assembly)
        {
            ThowExceptionIfContainerIsNotInitialized();

            var moduleType = typeof(IOCModule);
            var modules = assembly.GetTypes().Where(t => t.GetTypeInfo().IsSubclassOf(moduleType)).ToList();

            foreach (var module in modules)
            {
                var moduleInstance = (IOCModule) Activator.CreateInstance(module);

                container.AddModule(moduleInstance);
            }
        }

        public static void CheckConfiguration()
        {
            ThowExceptionIfContainerIsNotInitialized();
            container.CheckConfiguration();
        }
    }
}