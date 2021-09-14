﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Aidan.Common.Core.Attributes;
using Aidan.Common.Core.Constants;
using Aidan.Common.Core.Enum;
using Aidan.Common.AspNetCoreExtensions;
using Aidan.Common.Core.Interfaces.Excluded;

namespace Aidan.Common.Configuration
{
    public static class ModuleExtensions
    {
        private static IServiceCollection Bind( IServiceCollection services, Type [ ] types, string layer, string rootNamespace )
        {
            RegisterServices( services, GetServiceTypes( GetInterfaces( types, layer, rootNamespace ), types ) );
            RegisterFactories( services, GetFactories( GetInterfaces( types, layer, rootNamespace ) ) );
            return services;
        }

        private static void RegisterViewModels( IServiceCollection services, Type [ ] types )
        {
            var viewModels = types.Where( x => x.Name.EndsWith( "ViewModel" ) );
            foreach( var viewModel in viewModels )
            {
                services.AddTransient( viewModel );
            }
        }
        
        private static void RegisterFactories( IServiceCollection services, Type [ ] factories )
        {
            foreach( var factory in factories ) services.AddFactory( factory );
        }

        private static void RegisterServices( IServiceCollection services,
            List<((Type contract, ServiceLifetime scope), Type implementation)> serviceTypes )
        {
            foreach( var ((contract, scope), implementation) in serviceTypes )
                services.Add( new ServiceDescriptor( contract, implementation, scope ) );
        }

        private static List<((Type contract, ServiceLifetime scope), Type implementation)> GetServiceTypes(
            Type [ ] interfaces, Type [ ] types )
        {
            var serviceTypes = new List<((Type contract, ServiceLifetime scope), Type implementation)>( );
            foreach( var iInterface in interfaces )
            {
                var lifetime = ServiceLifetime.Transient;
                try { AddService( types, iInterface, lifetime, serviceTypes ); }
                catch( Exception )
                {
                    // service not found
                }
            }

            return serviceTypes;
        }

        private static void AddService( Type [ ] types, Type iInterface, ServiceLifetime lifetime,
            List<((Type contract, ServiceLifetime scope), Type implementation)> serviceTypes )
        {
            var service = types
                .First( x =>
                    x.GetInterfaces( ).Any( type =>
                        $"{type.Namespace}.{type.Name}" == $"{iInterface.Namespace}.{iInterface.Name}" ) &&
                    x.IsClass );
            try
            {
                var serviceAttribute = iInterface
                    .GetCustomAttributes<ServiceAttribute>( )
                    .First( );
                lifetime = GetServiceLifetime( serviceAttribute, lifetime );
            }
            catch( Exception )
            {
                // service does not have custom scope
            }

            serviceTypes
                .Add( ( ( iInterface, lifetime ), service ) );
        }

        private static ServiceLifetime GetServiceLifetime( ServiceAttribute serviceAttribute, ServiceLifetime lifetime )
        {
            switch( serviceAttribute.Scope )
            {
                case ServiceLifetimeEnum.Scoped:
                    lifetime = ServiceLifetime.Scoped;
                    break;
                case ServiceLifetimeEnum.Singleton:
                    lifetime = ServiceLifetime.Singleton;
                    break;
                case ServiceLifetimeEnum.Transient:
                    lifetime = ServiceLifetime.Transient;
                    break;
            }

            return lifetime;
        }

        private static Type [ ] GetFactories( Type [ ] interfaces )
        {
            var factories = interfaces
                .Where( x => x.GetInterfaces( ).Any( subType => subType == typeof( IFactory ) ) )
                .ToArray( );
            return factories;
        }

        private static Type [ ] GetInterfaces( Type [ ] types, string layer, string rootNamespace )
        {
            var interfaces = types
                .Where( type =>
                {
                    try
                    {
                        return type.Namespace.Contains( $"{rootNamespace}.{layer}.{ApplicationConstants.ContractNamespace}" ) &&
                               type.IsInterface;
                    }
                    catch( Exception ) { return false; }
                } )
                .ToArray( );
            return interfaces;
        }

        private static Type [ ] GetTypes( string layer, string rootNamespace )
        {
            var types = AppDomain
                .CurrentDomain
                .GetAssemblies( )
                .SelectMany( x => x.GetTypes( ) )
                .Where( x =>
                {
                    try { return x.Namespace.Contains( $"{rootNamespace}.{layer}" ); }
                    catch( Exception ) { return false; }
                } )
                .ToArray( );
            return types;
        }

        public static IServiceCollection BindServices( this IServiceCollection serviceCollection,
            ApplicationLayerEnum applicationLayer,
            Action [ ] initializers,
            string rootNamespace )
        {
            var types = GetTypes( applicationLayer.ToString( ), rootNamespace );
            Bind( serviceCollection, types , applicationLayer.ToString( ), rootNamespace );
            if( applicationLayer == ApplicationLayerEnum.UI )
            {
                RegisterViewModels( serviceCollection, types );
            }

            return serviceCollection;
        }
            
    }
}