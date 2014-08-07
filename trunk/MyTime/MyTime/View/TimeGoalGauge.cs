using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;

namespace FieldService.View
{
    [TemplatePart (Name = "PART_Gauge", Type = typeof(LinearGaugeRange))]
    public class TimeGoalGauge : Control
    {

        public static readonly DependencyProperty GoalValueProperty = DependencyProperty.Register(
            "GoalValue", typeof(double), typeof(TimeGoalGauge), new PropertyMetadata(default(double), GoalValuePropertyChanged));

        private static void GoalValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as TimeGoalGauge;
            if (c == null) return;

            if (c.GoalValue < c.ValueMet) return;

            var rounding = 10.0;
            var max = c.GoalValue;
            if (max <= 15) rounding = 2.0;


            max = max*1.5;
            var lbl = max < rounding ? rounding : Math.Round((max / 4) / rounding) * rounding;
            max = lbl*4;

            c.MaxGaugeValue = max;
            c.LabelStepValue = lbl;
            c.TickStepValue = Math.Round(lbl/4.0);
            c.MajorTickStepValue = c.LabelStepValue/c.TickStepValue;
            c.MaxValueLessGoalValue = max - (double) c.GoalValue;
        }

        public double GoalValue
        {
            get { return (double) GetValue(GoalValueProperty); }
            set { SetValue(GoalValueProperty, value); }
        }

        public static readonly DependencyProperty ValueMetProperty = DependencyProperty.Register(
            "ValueMet", typeof (double), typeof (TimeGoalGauge), new PropertyMetadata(default(double), ValueMetPropertyChanged));

        private static void ValueMetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as TimeGoalGauge;
            if (c == null) return;
            var rounding = 10.0;
            if (c.MaxGaugeValue == 0.0 || (double) e.NewValue <= c.MaxGaugeValue) return;

            var max = (double) e.NewValue;
            if (max <= 15) rounding = 2.0;


            max = max*1.5;
            var lbl = max < rounding ? rounding : Math.Round((max/4)/rounding)*rounding;
            max = lbl*4;

            c.MaxGaugeValue = max;
            c.LabelStepValue = lbl;
            c.TickStepValue = Math.Round(lbl/4.0);
            c.MajorTickStepValue = c.LabelStepValue/c.TickStepValue;
            c.MaxValueLessGoalValue = max - (double) c.GoalValue;

            c.GoalValue += 0.0001;
        }

        public double ValueMet
        {
            get { return (double) GetValue(ValueMetProperty); }
            set { SetValue(ValueMetProperty, value); }
        }

        private static readonly DependencyProperty MaxGaugeValueProperty = DependencyProperty.Register(
            "MaxGaugeValue", typeof(double), typeof(TimeGoalGauge), new PropertyMetadata(default(double)));

        private double MaxGaugeValue
        {
            get { return (double) GetValue(MaxGaugeValueProperty); }
            set { SetValue(MaxGaugeValueProperty, value); }
        }

        private static readonly DependencyProperty LabelStepValueProperty = DependencyProperty.Register(
            "LabelStepValue", typeof(double), typeof(TimeGoalGauge), new PropertyMetadata(default(double)));

        private double LabelStepValue
        {
            get { return (double) GetValue(LabelStepValueProperty); }
            set { SetValue(LabelStepValueProperty, value); }
        }

        private static readonly DependencyProperty TickStepValueProperty = DependencyProperty.Register(
            "TickStepValue", typeof(double), typeof(TimeGoalGauge), new PropertyMetadata(default(double)));

        private double TickStepValue
        {
            get { return (double) GetValue(TickStepValueProperty); }
            set { SetValue(TickStepValueProperty, value); }
        }

        private static readonly DependencyProperty MaxValueLessGoalValueProperty = DependencyProperty.Register(
            "MaxValueLessGoalValue", typeof(double), typeof(TimeGoalGauge), new PropertyMetadata(default(double)));

        private double MaxValueLessGoalValue
        {
            get { return (double) GetValue(MaxValueLessGoalValueProperty); }
            set { SetValue(MaxValueLessGoalValueProperty, value); }
        }

        private static readonly DependencyProperty MajorTickStepValueProperty = DependencyProperty.Register(
            "MajorTickStepValue", typeof (double), typeof (TimeGoalGauge), new PropertyMetadata(default(double)));

        private double MajorTickStepValue
        {
            get { return (double) GetValue(MajorTickStepValueProperty); }
            set { SetValue(MajorTickStepValueProperty, value); }
        }

        public TimeGoalGauge()
        {
            DefaultStyleKey = typeof (TimeGoalGauge);
        }
    }
}
