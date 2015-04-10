using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Asclepius.User
{
    public class Record
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        //Pedometer stats

        public double StepLength { get; set; } //in centimeters
        public int SurfaceGrade { get; set; }
        public int WalkingStepCount { get; set; }
        public int RunningStepCount { get; set; }
        public uint RunTime { get; set; }
        public uint WalkTime { get; set; }
        public int ActivityType { get; set; } //placeholder, default 0 is walking
        public User.AppUser.ActivityLevel ActivityLevel { get; set; }

        //Constructor
        public Record()
        {
            AppUser user = User.AccountsManager.Instance.CurrentUser;
            if (user != null)
            {
                StepLength = user.StepLength;
                ActivityLevel = user.UserActivityLevel;
            }
        }

        public Record(DateTime _start, DateTime _end, double _length = 0, int _type = 0)
        {
            this.StartDate = _start;
            this.EndDate = _end;
            this.ActivityType = _type;
            this.StepLength = _length;
        }

        //Methods
        public void StartRecord()
        {
            StartDate = DateTime.Now;
        }

        public void StopRecord()
        {
            EndDate = DateTime.Now;
        }

        //Operators
        public static Record operator+(Record r1, Record r2)
        {
            Record ret=new Record();
            ret.WalkTime = r1.WalkTime + r2.WalkTime;
            ret.RunTime = r1.RunTime + r2.RunTime;
            ret.WalkingStepCount = r1.WalkingStepCount + r2.WalkingStepCount;
            ret.RunningStepCount = r1.RunningStepCount + r2.RunningStepCount;
            return ret;
        }

        //Converters

        public double Distance() //do not use
        {
            return (WalkingStepCount + RunningStepCount) * StepLength;
        }

        public double TimeInMinutes()
        {
            return (EndDate - StartDate).TotalMinutes;
        }

        public double TimeInHours()
        {
            return TimeInMinutes() / 60;
        }

        public double DistanceInMeters()
        {
            return Distance() / 100;
        }

        public double DistanceInKilometers()
        {
            return Distance() / 1000;
        }

        public double DistanceInMiles()
        {
            return DistanceInKilometers() * 0.621371;
        }

        public int CompareTo(Record target)
        {
            return StartDate.CompareTo(target.StartDate);
        }
    }
}
