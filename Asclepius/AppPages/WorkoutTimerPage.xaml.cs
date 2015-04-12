using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Asclepius.AppPages
{
    public partial class WorkoutTimerPage : PhoneApplicationPage
    {
        Models.WorkoutTimerPageModel Model = new Models.WorkoutTimerPageModel();

        public WorkoutTimerPage()
        {
            InitializeComponent();
            this.DataContext = Model;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Model.StopWorkoutTimer();
            MessageBoxResult result = MessageBox.Show("Are you sure you want to quit this workout session?\nYour record will not be saved", "Prompt", MessageBoxButton.OKCancel);
            if (result==MessageBoxResult.OK)
            {
                this.NavigationService.Navigate(new Uri("/AppPages/WorkoutPage.xaml", UriKind.Relative));
            }
            else
            {
                Model.StartWorkoutTimer();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Model.StartWorkout();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Model.WorkoutPaused = true;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/AppPages/WorkoutPage.xaml", UriKind.Relative));
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Model.WorkoutPaused = false;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Model.StopWorkoutTimer();
            MessageBoxResult result = MessageBox.Show("Are you sure you want to stop this workout session?", "Prompt", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                Model.SaveRecord();
                this.NavigationService.Navigate(new Uri("/AppPages/WorkoutPage.xaml", UriKind.Relative));
            }
            else
            {
                Model.StartWorkoutTimer();
            }
        }
    }
}