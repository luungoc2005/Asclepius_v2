using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Asclepius.Resources;
using Asclepius.User;
using Asclepius.AppPages.Models;

namespace Asclepius
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (((AccountsManager)User.AccountsManager.Instance).listFiles().Length == 0)
            {
                this.NavigationService.Navigate(new Uri("/AppPages/NewAccountPage.xaml", UriKind.Relative));
            }
            else
            {
                if (Helpers.AppSettings.DefaultUserfile!="" && Helpers.AppSettings.DefaultPassword != "")
                {
                    LoginPageModel Model = new LoginPageModel();
                    if (Model.LoginButtonClick())
                    {
                        this.NavigationService.Navigate(new Uri("/AppPages/HubPage.xaml", UriKind.Relative));
                    }
                    else
                    {
                        this.NavigationService.Navigate(new Uri("/AppPages/LoginPage.xaml", UriKind.Relative));
                    }
                }
                else
                {
                    this.NavigationService.Navigate(new Uri("/AppPages/LoginPage.xaml", UriKind.Relative));
                }
            }
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}