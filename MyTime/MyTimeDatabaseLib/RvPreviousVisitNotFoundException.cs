// ***********************************************************************
// Assembly         : MyTimeDatabaseLib
// Author           : trevo_000
// Created          : 11-05-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-05-2012
// ***********************************************************************
// <copyright file="RvPreviousVisitNotFoundException.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace MyTimeDatabaseLib
{
    /// <summary>
    /// Class RvPreviousVisitNotFoundException
    /// </summary>
    public class RvPreviousVisitNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RvPreviousVisitNotFoundException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public RvPreviousVisitNotFoundException(string message) : base(message) { }
    }
}