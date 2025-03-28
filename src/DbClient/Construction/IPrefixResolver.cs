﻿/*****************************************************************************   
    The MIT License (MIT)
    Copyright (c) 2014 bernhard.richter@gmail.com
    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:
    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
******************************************************************************
    DbClient version 1.0.0.1
    https://github.com/seesharper/DbClient
    http://twitter.com/bernhardrichter
******************************************************************************/
namespace DbClient.Construction
{
    using System.Data;
    using System.Reflection;

    /// <summary>
    /// Represents a class that is capable of resolving the prefix 
    /// for a navigation property.
    /// </summary>
    public interface IPrefixResolver
    {
        /// <summary>
        /// Returns the prefix for the given <paramref name="navigationProperty"/>.
        /// </summary>
        /// <param name="navigationProperty">The property for which to get the prefix.</param>
        /// <param name="dataRecord">The <see cref="IDataRecord"/> that represents the available fields/columns.</param>
        /// <param name="currentPrefix">The current prefix that the resolved prefix will be appended to.</param>
        /// <returns>A <see cref="string"/> value that represents the property prefix.</returns>
        string GetPrefix(PropertyInfo navigationProperty, IDataRecord dataRecord, string currentPrefix);    
    }
}