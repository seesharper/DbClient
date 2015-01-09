﻿namespace DbReader.Tests
{
    using System;
    using Should;
    using Xunit;

    public class ValueConverterTests
    {
        static ValueConverterTests() 
        {
            ValueConverter.Register((record, i) => new CustomValueType(record.GetInt32(i)));
        }
        
        [Fact]
        public void GetExecuteMethod_ReturnsMethodInfo()
        {
            ValueConverter.GetConvertMethod(typeof(CustomValueType)).ShouldNotBeNull();
        }

        [Fact]
        public void CanConvert_KnownType_ReturnsTrue()
        {
            ValueConverter.CanConvert(typeof(CustomValueType)).ShouldBeTrue();
        }

        [Fact]
        public void CanConvert_UnknownType_ReturnsFalse()
        {
            ValueConverter.CanConvert(typeof(string)).ShouldBeFalse();
        }

        [Fact]
        public void Convert_ReturnsConvertedValue()
        {            
            var dataRecord = new { SomeColumn = 42 }.ToDataRecord();
            var convertedValue = ValueConverter.Convert<CustomValueType>(dataRecord, 0);
            convertedValue.Value.ShouldEqual(42);
        }
    }
}