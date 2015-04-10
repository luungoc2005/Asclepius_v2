using Asclepius.AppPages.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Asclepius.Common
{
    public class CommonMethods
    {
        public static void PromptExitApplication()
        {
            if (MessageBox.Show("Are you sure you want to quit?", "Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Application.Current.Terminate();
            }
        }

        public static UserPageModel mainModel;

        public static double CalcDistance(uint steps, double stepsLength)
        {
            return Math.Round((stepsLength / 100000) * (steps), 2);
        }

        public static string FormatDate(DateTime date)
        {
            CultureInfo ci = new CultureInfo("en-GB");

            string format = "D";
            return date.ToString(format, ci);
        }

        public static int CalcAge(DateTime reference, DateTime birthDate)
        {
            DateTime now = reference;
            int age = now.Year - birthDate.Year;
            if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day)) age--;
            return age;
        }
    }
}
