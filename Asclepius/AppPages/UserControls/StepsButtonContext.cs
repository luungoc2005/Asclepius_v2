using Asclepius.User;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asclepius.AppPages.UserControls
{
    class StepsButtonContext: INotifyPropertyChanged
    {
        User.AppUser _selectedUser;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public StepsButtonContext(User.AppUser user)
        {
            this._selectedUser = user;
            Helpers.StepCounterHelper.Instance.OnStepCounterReport += Instance_OnStepCounterReport;
        }

        void Instance_OnStepCounterReport(Record record)
        {
            if (_selectedUser != null && _selectedUser == User.AccountsManager.Instance.CurrentUser)
            {
                OnPropertyChanged(null);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int TotalSteps
        {
            get
            {
                List<Record> tempList = _selectedUser.Today.Records;
                return tempList.Sum((Record r) => { return r.WalkingStepCount + r.RunningStepCount; });
            }
        }
    }
}
