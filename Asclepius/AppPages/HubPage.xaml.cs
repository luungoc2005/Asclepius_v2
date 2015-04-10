﻿using System;
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
            User.AccountsManager.Instance.SaveUser();
            this.NavigationService.Navigate(new Uri("/AppPages/LoginPage.xaml", UriKind.Relative));
        }
    }
}