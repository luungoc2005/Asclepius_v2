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
        public double HeartRate
        {
            get
            {
                return _heartrate;
            }
            set
            {
                _heartrate = value;
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
