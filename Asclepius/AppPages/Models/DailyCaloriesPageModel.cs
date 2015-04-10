using Asclepius.User;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asclepius.AppPages.Models
{
    public class DailyCaloriesPageModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        User.AppUser _selectedUser;
        Helpers.CaloriesCounterHelper _counter;

        public class ValuePair
        {
            public int value1 { get; set; }
            public double value2 { get; set; }
            public ValuePair(int val1, double val2)
            {
                value1 = val1;
                value2 = val2;
            }
        }

        private ObservableCollection<ValuePair> areaChartData = new ObservableCollection<ValuePair>();
        public ObservableCollection<ValuePair> ChartData { get { return areaChartData; } }

        public DailyCaloriesPageModel()
        {
            Helpers.StepCounterHelper.Instance.OnStepCounterReport += Instance_OnStepCounterReport;
            for (int i = 0; i < 24; i++)
            {
                areaChartData.Add(new ValuePair(i, 0));
            }
        }

        void Instance_OnStepCounterReport(Record record)
        {
            if (_selectedUser != null && SelectedDay==0 && _selectedUser == User.AccountsManager.Instance.CurrentUser)
            {
                if (record.WalkingStepCount + record.RunningStepCount > 0)
                {
                    UpdateData();
                }
            }
        }

        public AppUser SelectedUser
        {
            get
            {
                return _selectedUser;
            }
            set
            {
                _selectedUser = value;
                _counter = new Helpers.CaloriesCounterHelper(_selectedUser);
                SelectedDay = 0;
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        int _day = 0;
        public int SelectedDay
        {
            get
            {
                return _day;
            }
            set
            {
                _day = value;
                //day changing
                if (_day >= 0)
                {
                    UpdateData();
                }
            }
        }

        public bool IsPreviousAvailable()
        {
            return SelectedUser.FindRecord(DateTime.Now - TimeSpan.FromDays(SelectedDay + 1),true) != null;
        }

        private void UpdateData()
        {
            ResetValues();

            DailyRecord record;
            List<Record> tempList;
            DateTime date = (DateTime.Now - TimeSpan.FromDays(_day));
            record = SelectedUser.FindRecord(date, true);
            if (record != null)
            {
                tempList = record.Records;

                DateTime startDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);

                if (tempList != null && tempList.Count > 0)
                {
                    for (int i = 0; i < 24; i++)
                    {
                        var hrec = record.GetHourlyRecord(i, true);
                        int walking = (hrec == null ? 0 : _counter.GetWalkingCalorieBurn(hrec));
                        int hourly = (hrec == null ? ((startDate.Date==DateTime.Now.Date && i > date.Hour) ? 0 : _counter.CalcHourlyBMR(startDate)) : _counter.AdjustedBMR(hrec));
                        areaChartData[i].value2 = hourly + walking;

                        _walkingCalories += walking;
                        _passiveCalories += hourly;

                        startDate += TimeSpan.FromHours(1);
                    }
                }
            }

            OnPropertyChanged(null);
            ChartData.UpdateCollection();
        }

        int _walkingCalories;
        int _passiveCalories;
        public int TotalCalories
        {
            get
            {
                return WalkingCalories+PassiveCalories;
            }
        }

        public int WalkingCalories
        {
            get
            {
                return _walkingCalories;
            }
        }

        public int PassiveCalories
        {
            get
            {
                return _passiveCalories;        
            }
        }

        private void ResetValues()
        {
            _walkingCalories = 0;
            _passiveCalories = 0;
            for (int i = 0; i < 24; i++)
            {
                areaChartData[i].value2 = 0;
            }
        }

        public string DayText
        {
            get
            {
                if (_day == 0)
                    return "Today";
                else if (_day == 1)
                    return "Yesterday";
                else
                {
                    DateTime time = DateTime.Now - TimeSpan.FromDays(_day);
                    return Common.CommonMethods.FormatDate(time);
                }
            }
        }
    }
}
