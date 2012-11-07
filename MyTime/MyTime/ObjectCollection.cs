using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MyTime
{
    public class ObjectCollection : Collection<object>
    {
        public ObjectCollection() { }

        public ObjectCollection(IEnumerable collection)
        {
            foreach (object obj in collection) {
                Add(obj);
            }
        }
    }
}
