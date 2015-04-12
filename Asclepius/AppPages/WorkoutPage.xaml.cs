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
    public partial class WorkoutPage : PhoneApplicationPage
    {
        public Models.WorkoutPageModel Model = new Models.WorkoutPageModel();

        public WorkoutPage()
        {
            InitializeComponent();
            Model.SelectedUser = User.AccountsManager.Instance.CurrentUser;
            this.DataContext = Model;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/AppPages/WorkoutTimerPage.xaml", UriKind.Relative));
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.NavigationService.Navigate(new Uri("/AppPages/HubPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_Back(object sender, EventArgs e)
        {
            Model.SelectedDay += 1;
            UpdateAppBar();
        }

        private void ApplicationBarIconButton_Click_Next(object sender, EventArgs e)
        {
            Model.SelectedDay -= 1;
            UpdateAppBar();
        }


        private void UpdateAppBar()
        {
            ApplicationBarIconButton btn1 = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            ApplicationBarIconButton btn2 = (ApplicationBarIconButton)ApplicationBar.Buttons[1];

            btn1.IsEnabled = Model.IsPreviousAvailable();
            btn2.IsEnabled = (Model.SelectedDay != 0);
        }

    }
}