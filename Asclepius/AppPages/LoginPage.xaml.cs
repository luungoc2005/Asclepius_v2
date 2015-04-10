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
    public partial class LoginPage : PhoneApplicationPage
    {
        Models.LoginPageModel Model = new Models.LoginPageModel();

        public LoginPage()
        {
            InitializeComponent();
            this.DataContext = Model;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            Common.CommonMethods.PromptExitApplication();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string parameter = string.Empty;
            if (NavigationContext.QueryString.TryGetValue("file", out parameter))
            {
                Model = new Models.LoginPageModel(parameter);
                this.DataContext = Model;
            }

            base.OnNavigatedTo(e);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            pbPass.Focus();
        }

        private void pbPass_GotFocus(object sender, RoutedEventArgs e)
        {
            pbPass.SelectAll();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/AppPages/NewAccountPage.xaml", UriKind.Relative));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/AppPages/AccountsSwitcher.xaml", UriKind.Relative));
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (Model.LoginButtonClick()) this.NavigationService.Navigate(new Uri("/AppPages/HubPage.xaml", UriKind.Relative));
        }
    }
}