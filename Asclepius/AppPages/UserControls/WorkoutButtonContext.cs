using Asclepius.User;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asclepius.AppPages.UserControls
{
    class WorkoutButtonContext : INotifyPropertyChanged
    {
        User.AppUser _selectedUser;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public WorkoutButtonContext(User.AppUser user)
        {
            this._selectedUser = user;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int TotalTime
        {
            get
            {
                List<Record> tempList = _selectedUser.Today.Records;
                return tempList.Sum((Record r) => { return (int)(r.ActivityType == 0 ? 0 : r.Duration); });
            }
        }

        public double Percentage
        {
            get
            {
                return Math.Min(((double)TotalTime / 7200), 1);
            }
        }
    }
}
