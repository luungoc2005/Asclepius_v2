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
    public partial class NewAccountPage : PhoneApplicationPage
    {
        Models.NewAccountPageModel Model = new Models.NewAccountPageModel();

        public NewAccountPage()
        {
            InitializeComponent();
            this.DataContext = Model;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            if ((User.AccountsManager.Instance).listFiles().Length == 0)
            {
                Common.CommonMethods.PromptExitApplication();
            }
            else
            {
                this.NavigationService.Navigate(new Uri("/AppPages/LoginPage.xaml", UriKind.Relative));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainPivot.SelectedIndex += 1;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            txtName.Visibility = Visibility.Visible;
            tbName.Visibility = Visibility.Collapsed;
            txtName.Focus();
        }

        private void txtName_LostFocus(object sender, RoutedEventArgs e)
        {
            txtName.Visibility = Visibility.Collapsed;
            tbName.Visibility = Visibility.Visible;
        }

        private void txtName_GotFocus(object sender, RoutedEventArgs e)
        {
            txtName.SelectAll();
        }

        private void txtEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            txtEmail.Visibility = Visibility.Collapsed;
            tbEmail.Visibility = Visibility.Visible;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            txtEmail.Visibility = Visibility.Visible;
            tbEmail.Visibility = Visibility.Collapsed;
            txtEmail.Focus();
        }

        private void txtEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            txtEmail.SelectAll();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            pbPass1.Visibility = Visibility.Visible;
            tbPass1.Visibility = Visibility.Collapsed;
            pbPass1.Focus();
        }

        private void pbPass1_GotFocus(object sender, RoutedEventArgs e)
        {
            pbPass1.SelectAll();
        }

        private void pbPass1_LostFocus(object sender, RoutedEventArgs e)
        {
            pbPass1.Visibility = Visibility.Collapsed;
            tbPass1.Visibility = Visibility.Visible;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            pbPass2.Visibility = Visibility.Visible;
            tbPass2.Visibility = Visibility.Collapsed;
            pbPass2.Focus();
        }

        private void pbPass2_GotFocus(object sender, RoutedEventArgs e)
        {
            pbPass2.SelectAll();
        }

        private void pbPass2_LostFocus(object sender, RoutedEventArgs e)
        {
            pbPass2.Visibility = Visibility.Collapsed;
            tbPass2.Visibility = Visibility.Visible;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            txtWeight.Visibility = Visibility.Visible;
            tbWeight.Visibility = Visibility.Collapsed;
            txtWeight.Focus();
        }

        private void txtWeight_GotFocus(object sender, RoutedEventArgs e)
        {
            txtWeight.SelectAll();
        }

        private void txtWeight_LostFocus(object sender, RoutedEventArgs e)
        {
            txtWeight.Visibility = Visibility.Collapsed;
            tbWeight.Visibility = Visibility.Visible;
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            txtHeight.Visibility = Visibility.Visible;
            tbHeight.Visibility = Visibility.Collapsed;
            txtHeight.Focus();
        }

        private void txtHeight_GotFocus(object sender, RoutedEventArgs e)
        {
            txtHeight.SelectAll();
        }

        private void txtHeight_LostFocus(object sender, RoutedEventArgs e)
        {
            txtHeight.Visibility = Visibility.Collapsed;
            tbHeight.Visibility = Visibility.Visible;
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            Model.CreateAccount();
            Asclepius.Helpers.AppSettings.DefaultUserfile = Model.user.FileName;

            this.NavigationService.Navigate(new Uri("/AppPages/LoginPage.xaml", UriKind.Relative));
        }

    }
}