﻿namespace DbReader.Tests
{
    using System;

    using Should;
    using Should.Core.Assertions;

    using Xunit.Extensions;

    public class ConstructorSelectorTests
    {
        [Theory, InjectData]
        public void Execute_PublicParameterLess_ReturnsConstructor(IConstructorSelector parameterlessConstructorSelector)
        {
            parameterlessConstructorSelector.Execute(typeof(ClassWithPublicParameteressConstructor)).ShouldNotBeNull();
        }

        [Theory, InjectData]
        public void Execute_PrivateParameterLess_ThrowsException(IConstructorSelector parameterlessConstructorSelector)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => parameterlessConstructorSelector.Execute(typeof(ClassWithPrivateParameterlessConstructor)));
        }
    }

    public class ClassWithPublicParameteressConstructor
    {
        public ClassWithPublicParameteressConstructor()
        {
        }
    }

    public class ClassWithPrivateParameterlessConstructor
    {
        private ClassWithPrivateParameterlessConstructor()
        {
        }
    }
}