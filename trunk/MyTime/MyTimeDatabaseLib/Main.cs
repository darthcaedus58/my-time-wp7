using System;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MyTimeDatabaseLib
{
    public class Main
    {
        public static string GetVersion() { return Regex.Match(Assembly.GetExecutingAssembly().FullName, @"Version=(?<version>[\d\.]*)").Groups["version"].Value; }
    }
}
