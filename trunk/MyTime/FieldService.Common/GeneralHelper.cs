using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace FieldService.Common
{
    public class GeneralHelper
    {
        public static TimeSpan RoundTime(TimeSpan t, int roundTimeIncrement)
        {
            var ts =
                TimeSpan.FromMinutes(roundTimeIncrement *
                                     Math.Ceiling(t.TotalMinutes / roundTimeIncrement));
            float m = float.Parse(t.TotalMinutes.ToString()) % roundTimeIncrement;
            if (m <= (roundTimeIncrement / 2.0))
                t = TimeSpan.FromMinutes(ts.TotalMinutes - roundTimeIncrement);
            else t = ts;

            if (t.TotalMinutes < roundTimeIncrement)
                return new TimeSpan(0, 0, roundTimeIncrement);

            return t;
        }
    }
}
