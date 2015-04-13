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
    public partial class HubPage : PhoneApplicationPage
    {
        public HubPage()
        {
            InitializeComponent();
            this.DataContext = Common.CommonMethods.mainModel;

            stepsButton.ButtonClick += stepsButton_ButtonClick;
            stepsButton.DataContext = new UserControls.StepsButtonContext(User.AccountsManager.Instance.CurrentUser);

            caloriesButton.ButtonClick += caloriesButton_ButtonClick;
            caloriesButton.DataContext = new UserControls.CaloriesButtonContext(User.AccountsManager.Instance.CurrentUser);

            workoutButton.DataContext = new UserControls.WorkoutButtonContext(User.AccountsManager.Instance.CurrentUser);
            workoutButton.ButtonClick += WorkoutButton_ButtonClick;
        }

        void caloriesButton_ButtonClick(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/AppPages/CaloriesPage.xaml", UriKind.Relative));
        }

        void stepsButton_ButtonClick(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/AppPages/StepsPage.xaml", UriKind.Relative));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/AppPages/ProfilePage.xaml", UriKind.Relative));
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            User.AccountsManager.Instance.SaveUser();
            if (MessageBox.Show("Are you sure you want to sign out?", "Prompt", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                this.NavigationService.Navigate(new Uri("/AppPages/LoginPage.xaml", UriKind.Relative));
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            stepsButton.IsUpdating = true;
            caloriesButton.IsUpdating = true;
            workoutButton.IsUpdating = true;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            stepsButton.IsUpdating = false;
            caloriesButton.IsUpdating = false;
            workoutButton.IsUpdating = false;
        }

        private void GadgetButton_ButtonClick(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/AppPages/HealthPage.xaml", UriKind.Relative));
        }

        private void WorkoutButton_ButtonClick(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/AppPages/WorkoutPage.xaml", UriKind.Relative));
        }

        private void FriendsButton_ButtonClick(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/AppPages/FriendsPage.xaml", UriKind.Relative));            
        }
    }
}