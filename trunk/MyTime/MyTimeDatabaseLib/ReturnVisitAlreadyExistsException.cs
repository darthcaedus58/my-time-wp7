// ***********************************************************************
// Assembly         : MyTimeDatabaseLib
// Author           : trevo_000
// Created          : 11-03-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-05-2012
// ***********************************************************************
// <copyright file="ReturnVisitAlreadyExistsException.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace MyTimeDatabaseLib
{
    /// <summary>
    /// Class ReturnVisitAlreadyExistsException
    /// </summary>
    public class ReturnVisitAlreadyExistsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnVisitAlreadyExistsException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="id">The id.</param>
        public ReturnVisitAlreadyExistsException(string message, int id) : base(message) { ItemId = id; }
        /// <summary>
        /// Gets the item id.
        /// </summary>
        /// <value>The item id.</value>
        public int ItemId { get; private set; }
    }
}