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
    public partial class HealthPage : PhoneApplicationPage
    {
        Models.HealthPageModel Model = new Models.HealthPageModel();

        public HealthPage()
        {
            InitializeComponent();
            this.DataContext = Model;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.NavigationService.Navigate(new Uri("/AppPages/HubPage.xaml", UriKind.Relative));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/AppPages/NewSnapshotPage.xaml", UriKind.Relative));
        }
    }
}