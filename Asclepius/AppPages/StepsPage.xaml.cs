using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace Asclepius.AppPages
{
    public partial class StepsPage : PhoneApplicationPage
    {
        Models.DailyStepsPageModel dayModel = new Models.DailyStepsPageModel();
        Models.WeeklyStepsPageModel weekModel = new Models.WeeklyStepsPageModel();

        public StepsPage()
        {
            InitializeComponent();
            this.DataContext = dayModel;
            dayModel.SelectedUser = User.AccountsManager.Instance.CurrentUser;
            UpdateAppBar();
        }

        private void ApplicationBarIconButton_Click_Back(object sender, EventArgs e)
        {
            switch (pivot1.SelectedIndex)
            {
                case 0:
                    dayModel.SelectedDay += 1;
                    break;
                default:
                    weekModel.SelectedWeek += 1;
                    break;
            }
            UpdateAppBar();
        }

        private void ApplicationBarIconButton_Click_Next(object sender, EventArgs e)
        {
            switch (pivot1.SelectedIndex)
            {
                case 0:
                    dayModel.SelectedDay -= 1;
                    break;
                default:
                    weekModel.SelectedWeek -= 1;
                    break;
            }
            UpdateAppBar();
        }


        private void UpdateAppBar()
        {
            ApplicationBarIconButton btn1 = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            ApplicationBarIconButton btn2 = (ApplicationBarIconButton)ApplicationBar.Buttons[1];

            switch (pivot1.SelectedIndex)
            {
                case 0:
                    btn1.IsEnabled = dayModel.IsPreviousAvailable();
                    btn2.IsEnabled = (dayModel.SelectedDay != 0);
                    break;
                case 1:                    
                    btn1.IsEnabled = weekModel.IsPreviousAvailable();
                    btn2.IsEnabled = (weekModel.SelectedWeek != 0);
                    break;
                default:
                    break;
            }
        }

        private void pivot1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (pivot1.SelectedIndex)
            {
                case 0:                    
                    this.DataContext = dayModel;
                    dayModel.SelectedUser = User.AccountsManager.Instance.CurrentUser;
                    UpdateAppBar();
                    break;
                case 1:
                    this.DataContext = weekModel;
                    weekModel.SelectedUser = User.AccountsManager.Instance.CurrentUser;
                    UpdateAppBar();
                    break;
                default:
                    break;
            }
        }
    }
}