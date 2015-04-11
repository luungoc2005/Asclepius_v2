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
    public partial class NewSnapshotPage : PhoneApplicationPage
    {
        Models.NewSnapshotPageModel Model = new Models.NewSnapshotPageModel();

        public NewSnapshotPage()
        {
            InitializeComponent();
            this.DataContext = Model;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainPivot.SelectedIndex += 1;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
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

        private void txtHeight_GotFocus(object sender, RoutedEventArgs e)
        {
            txtHeight.SelectAll();
        }

        private void txtHeight_LostFocus(object sender, RoutedEventArgs e)
        {
            txtHeight.Visibility = Visibility.Collapsed;
            tbHeight.Visibility = Visibility.Visible;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            txtHeight.Visibility = Visibility.Visible;
            tbHeight.Visibility = Visibility.Collapsed;
            txtHeight.Focus();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            txtHeart.Visibility = Visibility.Visible;
            tbHeart.Visibility = Visibility.Collapsed;
            txtHeart.Focus();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            Model.CreateNewSnapshot();
            this.NavigationService.Navigate(new Uri("/AppPages/HealthPage.xaml", UriKind.Relative));
        }

        private void txtHeart_GotFocus(object sender, RoutedEventArgs e)
        {
            txtHeart.SelectAll();
        }

        private void txtHeart_LostFocus(object sender, RoutedEventArgs e)
        {
            txtHeart.Visibility = Visibility.Collapsed;
            tbHeart.Visibility = Visibility.Visible;
        }

    }
}