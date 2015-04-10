using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;

namespace Asclepius.AppPages.UserControls
{
    public partial class StepsButton : UserControl
    {
        public event EventHandler ButtonClick;

        public StepsButton()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonClick != null) ButtonClick(sender,e);
        }

    }
}
