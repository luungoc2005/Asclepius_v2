using Asclepius.User;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asclepius.AppPages.Models
{
    public class WeeklyCaloriesPageModel : INotifyPropertyChanged
    {
        AppUser _selectedUser;
        Helpers.CaloriesCounterHelper _counter;

        private ObservableCollection<ValuePair> areaChartData = new ObservableCollection<ValuePair>();
        public ObservableCollection<ValuePair> ChartData { get { return areaChartData; } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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
                SelectedWeek = 0;
            }
        }

        public WeeklyCaloriesPageModel()
        {
            Helpers.StepCounterHelper.Instance.OnStepCounterReport += Instance_OnStepCounterReport;
            for (int i = 0; i < 7; i++)
            {
                areaChartData.Add(new ValuePair(i, 0));
            }
        }

        public DateTime FirstDayOfWeek(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Sunday) return date;
            for (int i = 0; i < 7; i++)
            {
                var temp = date - TimeSpan.FromDays(i);
                if (temp.DayOfWeek == DayOfWeek.Sunday) return temp;
            }
            return date;
        }

        public DateTime LastDayOfWeek(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday) return date;
            for (int i = 0; i < 7; i++)
            {
                var temp = date + TimeSpan.FromDays(i);
                if (temp.DayOfWeek == DayOfWeek.Saturday) return temp;
            }
            return date;
        }

        private void Instance_OnStepCounterReport(User.Record record)
        {
            if (_selectedUser != null && SelectedWeek == 0 && _selectedUser == User.AccountsManager.Instance.CurrentUser)
            {
                if (record.WalkingStepCount + record.RunningStepCount > 0)
                {
                    UpdateData();
                }
            }
        }

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

        int _day = 0;
        public int SelectedWeek
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
            DateTime selectedWeek = LastDayOfWeek(DateTime.Now - (TimeSpan.FromDays(7 * (_day + 1))));
            //DateTime _date = DateTime.Now - TimeSpan.FromDays(SelectedDay + 1);
            foreach (DailyRecord r in SelectedUser.DailyRecords)
            {
                if (r.Date <= selectedWeek) return true;
            }
            return false;
            //for (int i = 0; i < 7; i++)
            //{
            //    if (SelectedUser.FindRecord(selectedWeek + TimeSpan.FromDays(i), true) != null) return true;
            //}

            //return false;
        }

        private void UpdateData()
        {
            DateTime selectedWeek = FirstDayOfWeek(DateTime.Now - (TimeSpan.FromDays(7 * _day)));
            WeeklyData.Clear();

            for (int i = 0; i < 7; i++)
            {
                DailyRecord record;
                record = SelectedUser.FindRecord(selectedWeek + TimeSpan.FromDays(i), true);

                ChartData[i].value1 = (selectedWeek + TimeSpan.FromDays(i)).Day;
                ChartData[i].value2 = 0;

                if (record != null)
                {
                    Record sumRec = record.SummarizeRecord();
                    WeeklyData.Add(sumRec);

                    DateTime startDate = new DateTime(record.Date.Year, record.Date.Month, record.Date.Day, 0, 0, 0);

                    //24 hrs a day
                    for (int a = 0; a < 24; a++)
                    {
                        var hrec = record.GetHourlyRecord(a, true);
                        int walking = (hrec == null || hrec.ActivityType != 0 ? 0 : _counter.GetWalkingCalorieBurn(hrec));
                        int hourly = (hrec == null ? ((startDate.Date == DateTime.Now.Date && a > DateTime.Now.Hour) ? 0 : _counter.CalcHourlyBMR(startDate)) : _counter.AdjustedBMR(hrec));

                        areaChartData[i].value2 += hourly + walking;
                        
                        startDate += TimeSpan.FromHours(1);
                    }

                    foreach (Record hrec in record.Records)
                    {
                        int activity = (hrec == null || hrec.ActivityType == 0 ? 0 : (int)((double)_counter.RawBMR() * 1.157407407407407e-5 * (double)hrec.WalkTime
                        * hrec.RealActivity.metaEquivalent)); //BMR per second
                        areaChartData[i].value2 += activity;
                    }
                }
            }

            OnPropertyChanged(null);
            ChartData.UpdateCollection();
        }

        public string FirstDay { get { return Common.CommonMethods.FormatDate(FirstDayOfWeek(DateTime.Now - (TimeSpan.FromDays(7 * _day)))); } }
        public string LastDay { get { return Common.CommonMethods.FormatDate(LastDayOfWeek(DateTime.Now - (TimeSpan.FromDays(7 * _day)))); } }

        private List<Record> WeeklyData = new List<Record>();

        public uint TotalCalories
        {
            get
            {
                return (uint)areaChartData.Sum(x => x.value2);
            }
        }

        public uint AverageCalories
        {
            get
            {
                return (uint)(areaChartData.Sum(x => x.value2) / WeeklyData.Count);
            }
        }
    }
}
