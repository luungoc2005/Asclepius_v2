using Asclepius.Helpers;
using Asclepius.User;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asclepius.AppPages.Models
{
    public class HealthPageModel : INotifyPropertyChanged
    {
        GadgetHelper gadgetHelper = GadgetHelper.Instance;
        AccountsManager manager = AccountsManager.Instance;
        AppUser user;

        private ObservableCollection<ValuePair> heartChartData;
        public ObservableCollection<ValuePair> HeartChartData { get { return heartChartData; } }

        public List<Snapshot> ListSnapshots
        {
            get
            {
                return user.Snapshots;
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

        void gadgetHelper_TemperatureChanged(float value)
        {
            Temperature = Math.Round(value, 1);
        }

        void gadgetHelper_HeartRateChanged(float value)
        {
            HeartRate = Convert.ToInt32(value);
        }

        public HealthPageModel()
        {
            heartChartData = new ObservableCollection<ValuePair>();
            for (int i = heartChartData.Count; i < 20; i++)
            {
                heartChartData.Add(new ValuePair(i, 0));
            }

            user = manager.CurrentUser;
            gadgetHelper.HeartRateChanged += gadgetHelper_HeartRateChanged;
            gadgetHelper.TemperatureChanged += gadgetHelper_TemperatureChanged;
            gadgetHelper.GadgetStateChanged += gadgetHelper_GadgetStateChanged;

            OnPropertyChanged("ListSnapshots");
        }

        double _temperature; double _heartrate;
        public double Temperature
        {
            get
            {
                return _temperature;
            }
            set
            {
                _temperature = value;
                OnPropertyChanged("Temperature");
                OnPropertyChanged("IsTempUpdating");
            }
        }

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

        public bool IsTempUpdating
        {
            get
            {
                return (_temperature == 0);
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

                for (int i = 0; i < heartChartData.Count-1; i++)
                {
                    heartChartData[i].value2 = heartChartData[i + 1].value2;
                }

                heartChartData[19].value2 = value;

                heartChartData.HaltUpdate = false;
                heartChartData.UpdateCollection();
                heartChartData.HaltUpdate = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
