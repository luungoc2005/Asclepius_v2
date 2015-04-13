using Asclepius.User;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Asclepius.AppPages.Models
{
    public class DailyStepsPageModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        User.AppUser _selectedUser;

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

        public DailyStepsPageModel()
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
        
        bool _isWalking;

        public bool IsWalking
        {
            get
            {
                return _isWalking;
            }
            set
            {
                _isWalking = value;
                OnPropertyChanged("IsWalking");
            }
        }

        public uint TotalSteps
        {
            get
            {
                return (uint)selectedTotalSteps;
            }
        }

        public uint RunningSteps
        {
            get
            {
                return (uint)selectedRunningSteps;
            }
        }

        public double Distance
        {
            get
            {
                return Common.CommonMethods.CalcDistance(TotalSteps, User.AccountsManager.Instance.CurrentUser.StepLength);
            }
        }

        public uint WalkTime { get; set; }
        public uint RunTime { get; set; }

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
            //return SelectedUser.FindRecord(DateTime.Now - TimeSpan.FromDays(SelectedDay + 1),true) != null;
            DateTime _date = DateTime.Now - TimeSpan.FromDays(SelectedDay + 1);
            foreach (DailyRecord r in SelectedUser.DailyRecords)
            {
                if (r.Date <= _date) return true;
            }
            return false;
        }

        private void UpdateData()
        {
            ResetValues();

            DailyRecord record;
            List<Record> tempList;
            record = SelectedUser.FindRecord(DateTime.Now - TimeSpan.FromDays(_day), true);
            if (record != null)
            {
                tempList = record.Records;

                if (tempList != null && tempList.Count > 0)
                {
                    for (int i = 0; i < 24; i++)
                    {
                        var hrec = record.GetHourlyRecord(i, true);

                        if (hrec != null)
                        {
                            selectedTotalSteps += hrec.WalkingStepCount + hrec.RunningStepCount;
                            selectedRunningSteps += hrec.RunningStepCount;
                            WalkTime += hrec.WalkTime;
                            RunTime += hrec.RunTime;
                        }

                        areaChartData[i].value2 = (hrec == null ? 0 : hrec.WalkingStepCount + hrec.RunningStepCount) +
                            (i > 0 ? areaChartData[i - 1].value2 : 0);
                    }
                }
            }

            OnPropertyChanged(null);
            ChartData.UpdateCollection();
        }

        private void ResetValues()
        {
            selectedTotalSteps = 0;
            selectedRunningSteps = 0;
            WalkTime = 0;
            RunTime = 0;

            for (int i = 0; i < 24; i++)
            {
                areaChartData[i].value2 = 0;
            }
        }

        private int selectedTotalSteps;
        private int selectedRunningSteps;

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
