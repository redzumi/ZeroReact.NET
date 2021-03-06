﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace ZeroReact
{
    public interface IReactScopedContext
    {
        ReactComponent CreateComponent<T>(string componentName) where T: ReactComponent;

        void GetInitJavaScript(TextWriter writer);
    }

    public class ReactScopedContext : IDisposable, IReactScopedContext
    {
        protected readonly IList<ReactComponent> _components = new List<ReactComponent>();

        private readonly IServiceProvider _serviceProvider;

        public ReactScopedContext(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public virtual ReactComponent CreateComponent<T>(string componentName) where T: ReactComponent
        {
            var component = ActivatorUtilities.CreateInstance<T>(_serviceProvider);

            component.ComponentName = componentName;

            _components.Add(component);

            return component;
        }

        public virtual void GetInitJavaScript(TextWriter writer)
        {
            foreach (var component in _components)
            {
                if (!component.ServerOnly)
                {
                    component.RenderJavaScript(writer);
                    writer.WriteLine(';');
                }
            }
        }

        public void Dispose()
        {
            foreach (var component in _components)
            {
                component.Dispose();
            }
        }
    }
}
