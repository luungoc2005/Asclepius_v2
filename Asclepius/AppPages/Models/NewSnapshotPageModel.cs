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
    public class NewSnapshotPageModel : INotifyPropertyChanged
    {
        GadgetHelper gadgetHelper = GadgetHelper.Instance;
        Snapshot newSnap = new Snapshot();

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public NewSnapshotPageModel()
        {
            gadgetHelper.HeartRateChanged += gadgetHelper_HeartRateChanged;
        }

        void gadgetHelper_HeartRateChanged(float value)
        {
            HeartRate = Convert.ToInt32(value);
        }

        double _heartrate;
        float[] _samples = new float[20];
        int _count = 0;
        public double HeartRate
        {
            get
            {
                return _heartrate;
            }
            set
            {
                if (_heartrate == 0 || value < _heartrate) _heartrate = value;
                _samples[_count] = (float)value;
                _count += 1;
                if (_count == _samples.Length) _count = 0;

                float _min = _samples.Min();
                _heartrate = (_min == 0 ? ((_heartrate == 0 || value < _heartrate) ? value : _heartrate) : _min);
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

        public event PropertyChangedEventHandler PropertyChanged;
        
        public double Weight
        {
            get
            {
                return newSnap.Weight;
            }
            set
            {
                newSnap.Weight = value;
                OnPropertyChanged("Weight");
            }
        }

        public double Height
        {
            get
            {
                return newSnap.Height;
            }
            set
            {
                newSnap.Height = value;
                OnPropertyChanged("Height");
            }
        }

        public double UserHeartRate
        {
            get
            {
                return newSnap.HeartRate;
            }
            set
            {
                newSnap.HeartRate = (int)value;
                OnPropertyChanged("UserHeartRate");
            }
        }

        public void CreateNewSnapshot()
        {
            AppUser user = AccountsManager.Instance.CurrentUser;
            if (user != null)
            {
                user.Snapshots.Insert(0, newSnap);
                AccountsManager.Instance.SaveUser();
            }
        }
    }
}
