// ***********************************************************************
// Assembly         : MyTimeDatabaseLib
// Author           : trevo_000
// Created          : 11-06-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-06-2012
// ***********************************************************************
// <copyright file="TimeDataItemNotFoundException.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace MyTimeDatabaseLib
{
    /// <summary>
    /// Class TimeDataItemNotFoundException
    /// </summary>
    public class TimeDataItemNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeDataItemNotFoundException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public TimeDataItemNotFoundException(string message) : base(message) { }
    }
}