using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asclepius.User
{
    public class Snapshot
    {
        public DateTime DateTaken { get; set; }

        public double Weight { get; set; } //user weight in kg
        public double Height { get; set; } //user height in meters

        //Device stats
        public int HeartRate { get; set; }
        public double Temperature { get; set; } //in Celcius

        public Snapshot()
        {
            DateTaken = DateTime.Now;
            var user = AccountsManager.Instance.CurrentUser;
            if (user != null)
            {
                //default values=lastest values
                Weight = user.Weight;
                Height = user.Height;
            }
        }

        public static Snapshot FindNearestSnapshot(Record record, AppUser user)
        {
            if (user == null) return new Snapshot();

            User.Snapshot snapshot = user.Snapshots[0];
            if (user.Snapshots.Count > 0)
            {
                for (int i = 1; i < user.Snapshots.Count; i++)
                {
                    if ((record.StartDate - user.Snapshots[i].DateTaken) <
                    (record.StartDate - snapshot.DateTaken)) snapshot = user.Snapshots[i];
                }
            }

            return snapshot;
        }

        public double WeightInLbs()
        {
            return Weight * 2.20462;
        }

        public double TempInFahrenheit()
        {
            return Temperature * 1.8 + 32;
        }

        public double BMI
        {
            get
            {
                return Math.Round(Weight * 10000 / (Height * Height), 1);
            }
        }
    }
}
