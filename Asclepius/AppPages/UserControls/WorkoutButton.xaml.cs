using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Threading;

namespace Asclepius.AppPages.UserControls
{
    public partial class WorkoutButton : UserControl
    {
        public event EventHandler ButtonClick;

        DispatcherTimer updateTimer = new DispatcherTimer();

        public WorkoutButton()
        {
            InitializeComponent();
            updateTimer.Interval = TimeSpan.FromMilliseconds(100);
            updateTimer.Tick += updateTimer_Tick;
            updateTimer.Start();
        }

        public bool IsUpdating
        {
            get
            {
                return updateTimer.IsEnabled;
            }
            set
            {
                if (value) updateTimer.Start();
                else updateTimer.Stop();
            }
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            barCore.Height = (double)Percentage * barMain.ActualHeight;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonClick != null) ButtonClick(sender, e);
        }

        public double Percentage
        {
            get { return (double)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Percentage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register("Percentage", typeof(double), typeof(StepsButton), new PropertyMetadata(0.0));
    }
}
