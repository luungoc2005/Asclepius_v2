using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;

namespace Asclepius.AppPages.UserControls
{
    public partial class StepsButton : UserControl
    {
        public event EventHandler ButtonClick;

        public StepsButton()
        {
            InitializeComponent();
            this.Loaded += StepsButton_Loaded;
        }

        void StepsButton_Loaded(object sender, RoutedEventArgs e)
        {
            var button = this as StepsButton;
            button.barCore.Height = (double)Percentage * button.barMain.ActualHeight;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonClick != null) ButtonClick(sender,e);
        }

        public double Percentage
        {
            get { return (double)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Percentage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register("Percentage", typeof(double), typeof(CaloriesButton), new PropertyMetadata(0.0,
                new PropertyChangedCallback(ProgressPropertyChanged)));

        private static void ProgressPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var button = sender as StepsButton;
            if (button != null) button.barCore.Height = (double)e.NewValue * button.barMain.ActualHeight;
        }
    }
}
