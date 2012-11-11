// ***********************************************************************
// Assembly         : MyTimeDatabaseLib
// Author           : trevo_000
// Created          : 11-10-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-10-2012
// ***********************************************************************
// <copyright file="Main.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Reflection;
using System.Text.RegularExpressions;

namespace MyTimeDatabaseLib
{
    /// <summary>
    /// Class Main
    /// </summary>
    public class Main
    {
        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string GetVersion() { return Regex.Match(Assembly.GetExecutingAssembly().FullName, @"Version=(?<version>[\d\.]*)").Groups["version"].Value; }
    }
}