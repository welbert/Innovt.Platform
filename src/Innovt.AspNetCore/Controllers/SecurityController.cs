﻿using Innovt.AspNetCore.ViewModel;
using Innovt.Core.CrossCutting.Log;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;


namespace Innovt.AspNetCore.Controllers
{
    [Obsolete("Please do not use it anymore!:)")]
    public class SecurityController : Controller
    {
        private readonly IActionDescriptorCollectionProvider actionDescriptorProvider;

        protected ILogger Logger { get; }

        public SecurityController(IActionDescriptorCollectionProvider actionDescriptorProvider, ILogger logger)
        {
            this.actionDescriptorProvider = actionDescriptorProvider ??
                                            throw new ArgumentNullException(nameof(actionDescriptorProvider));
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected IList<MvcControllerViewModel> GetControllers()
        {
            var controllers = new List<MvcControllerViewModel>();

            //from web this code
            var items = actionDescriptorProvider
                .ActionDescriptors.Items
                .Where(descriptor => descriptor.GetType() == typeof(ControllerActionDescriptor))
                .Select(descriptor => (ControllerActionDescriptor)descriptor)
                .GroupBy(descriptor => descriptor.ControllerTypeInfo.FullName)
                .ToList();

            foreach (var actionDescriptors in items)
            {
                if (!actionDescriptors.Any())
                    continue;

                var actionDescriptor = actionDescriptors.First();


                if (!actionDescriptor.MethodInfo.IsPublic)
                    continue;

                var controllerTypeInfo = actionDescriptor.ControllerTypeInfo;

                var currentController = new MvcControllerViewModel()
                {
                    Area = controllerTypeInfo.GetCustomAttribute<AreaAttribute>()?.RouteValue,
                    DisplayName = controllerTypeInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName,
                    Name = actionDescriptor.ControllerName
                };

                var actions = new List<MvcActionViewModel>();

                foreach (var descriptor in actionDescriptors.GroupBy
                    (a => a.ActionName).Select(g => g.First()))
                {
                    var methodInfo = descriptor.MethodInfo;
                    actions.Add(new MvcActionViewModel
                    {
                        Controller = currentController.Name,
                        Name = descriptor.ActionName,
                        DisplayName = methodInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName,
                        Method = methodInfo.GetCustomAttribute<HttpMethodAttribute>()?.Name ?? "GET"
                    });
                }

                currentController.AddActions(actions);
                controllers.Add(currentController);
            }


            return controllers;
        }
    }
}