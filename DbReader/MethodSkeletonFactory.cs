﻿namespace DbReader
{
    using System;

    public class MethodSkeletonFactory : IMethodSkeletonFactory
    {
        private readonly Func<Type, Type[], IMethodSkeleton> factoryDelegate;

        public MethodSkeletonFactory(Func<Type, Type[], IMethodSkeleton> factoryDelegate)
        {
            this.factoryDelegate = factoryDelegate;
        }

        public IMethodSkeleton GetMethodSkeleton(Type returnType, Type[] parameterTypes)
        {
            return factoryDelegate(returnType, parameterTypes);
        }
    }
}