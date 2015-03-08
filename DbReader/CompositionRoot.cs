﻿namespace DbReader
{
    using System;
    using System.Collections;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;

    using DbReader.Interfaces;
    using DbReader.LightInject;
    using DbReader.Readers;

    using IConstructorSelector = DbReader.Interfaces.IConstructorSelector;
    using IMethodSkeleton = DbReader.Interfaces.IMethodSkeleton;
    using IPropertySelector = DbReader.Interfaces.IPropertySelector;

    public class CompositionRoot : ICompositionRoot
    {
        void ICompositionRoot.Compose(IServiceRegistry serviceRegistry)
        {                        
            serviceRegistry.Register<IMethodSkeletonFactory, MethodSkeletonFactory>(new PerContainerLifetime());
            serviceRegistry.Register(typeof(IReaderMethodBuilder<>), typeof(PropertyReaderMethodBuilder<>), "PropertyReaderMethodBuilder");
            serviceRegistry.Register(typeof(IReaderMethodBuilder<>), typeof(ConstructorReaderMethodBuilder<>), "ConstructorReaderMethodBuilder");
            //serviceRegistry.Register(typeof(IReader<>), typeof(PropertyReader<>), "PropertyReader");            
            serviceRegistry.Register<Type, Type[], IMethodSkeleton>((factory, returnType, parameterTypes) => new DynamicMethodSkeleton(returnType, parameterTypes));
            serviceRegistry.Register<IFieldSelector, FieldSelector>(new PerScopeLifetime());
            serviceRegistry.Register<IPropertySelector, SimplePropertySelector>("SimplePropertySelector", new PerContainerLifetime());
            serviceRegistry.Register<IPropertySelector, ManyToOnePropertySelector>("ManyToOnePropertySelector", new PerContainerLifetime());
            serviceRegistry.Register<IPropertySelector, OneToManyPropertySelector>("OneToManyPropertySelector", new PerContainerLifetime());

            serviceRegistry.Register<IPropertyMapper>(factory => new PropertyMapper(factory.GetInstance<IPropertySelector>("SimplePropertySelector"), factory.GetInstance<IFieldSelector>()), "PropertyMapper", new PerScopeLifetime());
            serviceRegistry.Decorate<IPropertyMapper, CachedPropertyMapper>();
            
            serviceRegistry.Register<IMethodSelector, MethodSelector>(new PerContainerLifetime());

            serviceRegistry.Register<IConstructorSelector, ParameterlessConstructorSelector>("ParameterlessConstructorSelector", new PerContainerLifetime());
            serviceRegistry.Register<IConstructorSelector, FirstConstructorSelector>("FirstConstructorSelector", new PerContainerLifetime());
            serviceRegistry.Decorate(typeof(IConstructorSelector), typeof(ConstructorValidator), sr => sr.ImplementingType == typeof(ParameterlessConstructorSelector));

            serviceRegistry.Register(typeof(IManyToOneMethodBuilder<>), typeof(ManyToOneMethodBuilder<>), "ManyToOneMethodBuilder", new PerScopeLifetime());
            serviceRegistry.Register<IPrefixResolver, PrefixResolver>(new PerScopeLifetime());           

            serviceRegistry.Register(typeof(IInstanceReader<>), typeof(InstanceReader<>), new PerScopeLifetime());
            serviceRegistry.Decorate(typeof(IInstanceReader<>), typeof(CachedInstanceReader<>));


            serviceRegistry.Register<Func<Type, object>>(factory => (type) => factory.GetInstance(type));
            serviceRegistry.Register<Func<Type, IReaderMethodBuilder<IStructuralEquatable>>>(
                factory => (type) => CreateConstructorReaderMethodBuilder(type, factory), new PerContainerLifetime());

            serviceRegistry.Register<IPropertyMapper, KeyPropertyMapper>("KeyPropertyMapper", new PerScopeLifetime());
            serviceRegistry.Decorate<IPropertyMapper, PropertyTypeValidator>();
            serviceRegistry.Decorate(typeof(IPropertyMapper), typeof(KeyPropertyMapperValidator), sr => sr.ImplementingType == typeof(KeyPropertyMapper));
            

            serviceRegistry.Register(factory => IsKeyProperty());


            serviceRegistry.Register<IKeyReader, KeyReader>(new PerScopeLifetime());
            serviceRegistry.Register<IKeyReaderMethodBuilder, KeyReaderMethodBuilder>(new PerScopeLifetime());

            serviceRegistry.Register(typeof(IManyToOneMethodBuilder<>), typeof(ManyToOneMethodBuilder<>), new PerScopeLifetime());
            serviceRegistry.Register(typeof(IOneToManyMethodBuilder<>), typeof(OneToManyMethodBuilder<>), new PerScopeLifetime());

            serviceRegistry.Register(typeof(IInstanceReaderMethodBuilder<>), typeof(InstanceReaderMethodBuilder<>), new PerScopeLifetime());

            serviceRegistry.Register<IOrdinalSelector, OrdinalSelector>();

            serviceRegistry.Register(factory => DbReaderOptions.ParameterParser, new PerContainerLifetime());

            serviceRegistry.Register<ICommandFactory, CommandFactory>(new PerContainerLifetime());

            serviceRegistry.Register<IArgumentProvider, ArgumentProvider>(new PerContainerLifetime());

            serviceRegistry.Register<IPropertySelector, ReadablePropertySelector>("ReadablePropertySelector", new PerContainerLifetime());
        }

        private static Func<PropertyInfo, bool> IsKeyProperty()
        {
            return
                p =>
                p.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase)
                || p.Name.Equals(p.DeclaringType.Name + "Id", StringComparison.InvariantCultureIgnoreCase);
        }

        private static IReaderMethodBuilder<IStructuralEquatable> CreateConstructorReaderMethodBuilder(Type type, IServiceFactory factory)
        {
            Type closedGenericMethodBuilderType = typeof(IReaderMethodBuilder<>).MakeGenericType(type);
            return
                (IReaderMethodBuilder<IStructuralEquatable>)
                factory.GetInstance(closedGenericMethodBuilderType, "ConstructorReaderMethodBuilder");
        }
    }
}