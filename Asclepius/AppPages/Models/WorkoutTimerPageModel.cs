using Asclepius.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Asclepius.AppPages.Models
{
    public class WorkoutTimerPageModel : INotifyPropertyChanged
    {
        User.Record _record;
        public event PropertyChangedEventHandler PropertyChanged;
        Helpers.CaloriesCounterHelper _counter;
        GadgetHelper gadgetHelper = GadgetHelper.Instance;

        private ObservableCollection<ValuePair> heartChartData;
        public ObservableCollection<ValuePair> HeartChartData { get { return heartChartData; } }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        void gadgetHelper_GadgetStateChanged(bool isconnected)
        {
            OnPropertyChanged("IsGadgetConnected");
        }
        
        void gadgetHelper_HeartRateChanged(float value)
        {
            HeartRate = Convert.ToInt32(value);
        }

        double _heartrate;
        public double HeartRate
        {
            get
            {
                return _heartrate;
            }
            set
            {
                _heartrate = value;
                UpdateHeartChart(value);
                OnPropertyChanged("HeartRate");
                OnPropertyChanged("IsHeartUpdating");
            }
        }

        public bool IsHeartUpdating
        {
            get
            {
                return (_heartrate == 0);
            }
        }

        private void UpdateHeartChart(double value)
        {
            if (value != 0)
            {
                for (int i = 0; i < heartChartData.Count; i++)
                {
                    if (heartChartData[i].value2 == 0)
                    {
                        heartChartData[i].value2 = value;
                    }
                }

                for (int i = 0; i < heartChartData.Count - 1; i++)
                {
                    heartChartData[i].value2 = heartChartData[i + 1].value2;
                }

                heartChartData[19].value2 = value;

                heartChartData.HaltUpdate = false;
                heartChartData.UpdateCollection();
                heartChartData.HaltUpdate = true;
            }
        }

        DispatcherTimer workoutTimer;
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

        public WorkoutTimerPageModel()
        {
            workoutTimer = new DispatcherTimer();
            workoutTimer.Interval = TimeSpan.FromSeconds(1);
            workoutTimer.Tick += workoutTimer_Tick;
            _record = new User.Record();
            _record.StartRecord();
            _counter = new Helpers.CaloriesCounterHelper(User.AccountsManager.Instance.CurrentUser);
            gadgetHelper.HeartRateChanged += gadgetHelper_HeartRateChanged;
            gadgetHelper.GadgetStateChanged += gadgetHelper_GadgetStateChanged;

            heartChartData = new ObservableCollection<ValuePair>();
            for (int i = heartChartData.Count; i < 20; i++)
            {
                heartChartData.Add(new ValuePair(i, 0));
            }

            var _user = User.AccountsManager.Instance.CurrentUser;
            if (_user != null)
            {
                bool _bFound = false;
                foreach (User.DailyRecord daily in _user.DailyRecords)
                {
                    if (_bFound) break;
                    foreach (User.Record record in daily.Records)
                    {
                        if (record.ActivityType != 0)
                        {
                            SelectedIndex = ActivityTypes.FindIndex(r => r.Equals(Common.CommonMethods.activityTypes[record.ActivityType]));
                            _bFound = true;
                            break;
                        }
                    }
                }
            }
        }

        public void StartWorkout()
        {
            WorkoutStarted = true;
            StartWorkoutTimer();
            PopupDisplaying = false;
        }
        
        public void StartWorkoutTimer()
        {
            workoutTimer.Start();
        }

        public void StopWorkoutTimer()
        {
            workoutTimer.Stop();
        }

        void workoutTimer_Tick(object sender, EventArgs e)
        {
            SecondsElapsed += 1;
            OnPropertyChanged("CaloriesBurned");
            //int newSecs = _value + (int)workoutTimer.Interval.TotalMilliseconds;
            //if (newSecs > 1000)
            //{
            //    ProgressBarValue = 0;
            //    SecondsElapsed += 1;
            //}
            //else
            //{
            //    _value = newSecs;
            //    ProgressBarValue = newSecs;
            //}
        }

        public int _seconds;
        public int SecondsElapsed
        {
            get
            {
                return _seconds;
            }
            set
            {
                _seconds = value;
                _record.WalkTime += 1;
                OnPropertyChanged("SecondsElapsed");
            }
        }

        public int _value;
        public int ProgressBarValue
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnPropertyChanged("ProgressBarValue");
            }
        }

        public int CaloriesBurned
        {
            get
            {
                return (int)((double)_counter.RawBMR() * 1.157407407407407e-5 * (double)SecondsElapsed
                    * (SelectedActivity == null ? 1 : SelectedActivity.metaEquivalent)); //BMR per second
            }
        }

        public List<Common.CommonMethods.ActivityType> listActivities;
        public List<Common.CommonMethods.ActivityType> ActivityTypes
        {
            get
            {
                if (listActivities == null)
                {
                    listActivities = Common.CommonMethods.activityTypes.Values.OrderBy(x => x.description).ToList();
                }
                return listActivities;
            }
        }

        int _selectedIndex = 0;
        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                _selectedIndex = value;
                if (SelectedActivity != null)
                {
                    _record.ActivityType = Common.CommonMethods.activityTypes.First(x => x.Value.Equals(SelectedActivity)).Key;
                }
                OnPropertyChanged(null);
            }
        }

        public Common.CommonMethods.ActivityType SelectedActivity
        {
            get
            {
                return SelectedIndex == -1 ? null : ActivityTypes[SelectedIndex];
            }
        }

        public string SelectedDescription
        {
            get
            {
                return (SelectedActivity == null ? "" : SelectedActivity.description);
            }
        }

        bool _workoutStarted;
        public bool WorkoutStarted
        {
            get
            {
                return _workoutStarted;
            }
            set
            {
                _workoutStarted = value;
                OnPropertyChanged("WorkoutStarted");
            }
        }

        bool _popupDisplaying = true;
        public bool PopupDisplaying
        {
            get
            {
                return _popupDisplaying;
            }
            set
            {
                _popupDisplaying = value;
                OnPropertyChanged("PopupDisplaying");
            }
        }

        bool _workoutPaused;
        public bool WorkoutPaused
        {
            get
            {
                return _workoutPaused;
            }
            set
            {
                _workoutPaused = value;
                if (value)
                {
                    StopWorkoutTimer();
                    PopupDisplaying = true;
                }
                else
                {
                    StartWorkoutTimer();
                    PopupDisplaying = false;
                }
                OnPropertyChanged("WorkoutPaused");
            }
        }

        public void SaveRecord()
        {
            var user = User.AccountsManager.Instance.CurrentUser;
            _record.StopRecord();
            if (user != null)
            {
                user.Today.Records.Insert(0, _record);
            }
            User.AccountsManager.Instance.SaveUser();
        }
        
    }
}
