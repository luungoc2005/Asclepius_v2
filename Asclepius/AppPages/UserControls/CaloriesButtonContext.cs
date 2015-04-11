using Asclepius.User;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asclepius.AppPages.UserControls
{
    class CaloriesButtonContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        User.AppUser _selectedUser;
        Helpers.CaloriesCounterHelper _counter;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public CaloriesButtonContext(User.AppUser user)         
        {
            this._selectedUser = user;
            _counter = new Helpers.CaloriesCounterHelper(_selectedUser);
            Helpers.StepCounterHelper.Instance.OnStepCounterReport += Instance_OnStepCounterReport;
        }

        void Instance_OnStepCounterReport(Record record)
        {
            if (_selectedUser != null && _selectedUser == User.AccountsManager.Instance.CurrentUser)
            {
                OnPropertyChanged(null);
            }
        }

        public int TotalCalories
        {
            get
            {
                DailyRecord record;
                record = _selectedUser.FindRecord(DateTime.Now, true);
                if (record==null)
                {
                    return 0;
                }
                else
                {
                    DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                    int total = 0;
                    //24 hrs a day
                    for (int a = 0; a < 24; a++)
                    {
                        var hrec = record.GetHourlyRecord(a, true);
                        int walking = (hrec == null ? 0 : _counter.GetWalkingCalorieBurn(hrec));
                        int hourly = (hrec == null ? ((a > DateTime.Now.Hour) ? 0 : _counter.CalcHourlyBMR(startDate)) : _counter.AdjustedBMR(hrec));
                        total += hourly + walking;

                        startDate += TimeSpan.FromHours(1);
                    }

                    return total;
                }
            }
        }

        public double Percentage
        {
            get
            {
                return Math.Min(((double)TotalCalories / (double)_counter.GetDailyCalorieGoal()), 1);
            }
        }
    }
}
