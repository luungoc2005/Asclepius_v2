using Asclepius.User;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asclepius.AppPages.Models
{
    public class WeeklyStepsPageModel : INotifyPropertyChanged
    {
        AppUser _selectedUser;

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
                SelectedWeek = 0;
            }
        }

        public WeeklyStepsPageModel()
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
            DateTime selectedWeek = FirstDayOfWeek(DateTime.Now - (TimeSpan.FromDays(7 * (_day + 1))));

            for (int i = 0; i < 7; i++)
            {
                if (SelectedUser.FindRecord(selectedWeek + TimeSpan.FromDays(i), true) != null) return true;
            }

            return false;
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
                    ChartData[i].value2 = sumRec.WalkingStepCount + sumRec.RunningStepCount;
                }
            }

            OnPropertyChanged(null);
            ChartData.UpdateCollection();
        }

        public string FirstDay { get { return Common.CommonMethods.FormatDate(FirstDayOfWeek(DateTime.Now - (TimeSpan.FromDays(7 * _day)))); } }
        public string LastDay { get { return Common.CommonMethods.FormatDate(LastDayOfWeek(DateTime.Now - (TimeSpan.FromDays(7 * _day)))); } }

        public uint TotalSteps
        {
            get
            {
                return (uint)WeeklyData.Sum(r => r.WalkingStepCount + r.RunningStepCount);
            }
        }

        private List<Record> WeeklyData = new List<Record>();

        public double Distance
        {
            get
            {
                return Common.CommonMethods.CalcDistance(TotalSteps, User.AccountsManager.Instance.CurrentUser.StepLength);
            }
        }

        public uint AverageSteps
        {
            get
            {
                return WeeklyData.Count == 0 ? 0 : (uint)Math.Round((double)TotalSteps / WeeklyData.Count);
            }
        }

        public uint TotalTime
        {
            get
            {
                return (uint)WeeklyData.Sum(r => r.WalkTime + r.RunTime);
            }
        }
    }
}
