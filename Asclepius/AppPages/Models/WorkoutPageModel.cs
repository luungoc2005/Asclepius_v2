using Asclepius.User;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asclepius.AppPages.Models
{
    public class WorkoutPageModel : INotifyPropertyChanged
    {
        User.AppUser _selectedUser;
        public ObservableCollection<User.Record> listRecords = new ObservableCollection<Record>();

        public ObservableCollection<User.Record> ListRecords
        {
            get { return listRecords; }
        }

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
                SelectedDay = 0;
            }
        }

        public bool IsPreviousAvailable()
        {
            return SelectedUser.FindRecord(DateTime.Now - TimeSpan.FromDays(SelectedDay + 1), true) != null;
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

        private void UpdateData()
        {
            listRecords.Clear();
            DateTime _date = DateTime.Now - TimeSpan.FromDays(_day);
            DailyRecord _rec = SelectedUser.FindRecord(_date, true);
            if (_rec != null)
            {
                foreach (Record r in _rec.Records)
                {
                    if (r.ActivityType != 0) listRecords.Add(r);
                }
            }
            listRecords.UpdateCollection();
            OnPropertyChanged(null);
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
        
        public bool IsRecordsEmpty
        {
            get
            {
                return (ListRecords.Count == 0);
            }
        }
    }
}
