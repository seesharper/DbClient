﻿namespace DbReader.Tests
{
    using System;
    using System.Collections;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;

    using DbReader.Interfaces;

    using Should;

    public class KeyReaderTests
    {
        public KeyReaderTests()
        {
            DbReaderOptions.KeySelector = info => info.IsDefined(typeof(KeyAttribute));
        }

        [ScopedTheory, InjectData]        
        public void ShouldReadSingleKey(IKeyReader keyReader)
        {
            var dataRecord = new { KeyProperty = 42 }.ToDataRecord();
            IStructuralEquatable key = keyReader.Read(typeof(ClassWithSingleKey), dataRecord, string.Empty);
            key.ShouldEqual(Tuple.Create(42));
        }

        [ScopedTheory, InjectData]
        public void ShouldReadCompositeKey(IKeyReader keyReader)
        {
            var dataRecord = new { FirstKeyProperty = 42, SecondKeyProperty = 84 }.ToDataRecord();
            IStructuralEquatable key = keyReader.Read(typeof(ClassWithCompositeKey), dataRecord, string.Empty);
            key.ShouldEqual(Tuple.Create(42, 84));
        }        
    }


    public class ClassWithSingleKey
    {
        [Key]
        public int KeyProperty { get; set; }
    }

    public class ClassWithCompositeKey
    {
        [Key]
        public int FirstKeyProperty { get; set; }
        [Key]
        public int SecondKeyProperty { get; set; }
    }
}