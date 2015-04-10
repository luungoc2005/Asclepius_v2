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
    public partial class ProfilePage : PhoneApplicationPage
    {
        public ProfilePage()
        {
            InitializeComponent();
            this.DataContext = Common.CommonMethods.mainModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            txtEmail.Visibility = Visibility.Visible;
            tbEmail.Visibility = Visibility.Collapsed;
            txtEmail.Focus();
        }

        private void txtEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            txtEmail.Visibility = Visibility.Collapsed;
            tbEmail.Visibility = Visibility.Visible;
        }
        
        private void txtEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            txtEmail.SelectAll();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/AppPages/HubPage.xaml", UriKind.Relative));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to sign out?", "Prompt", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                this.NavigationService.Navigate(new Uri("/AppPages/LoginPage.xaml", UriKind.Relative));
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Common.CommonMethods.mainModel.ChangeUserAvatar();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

        }
    }
}