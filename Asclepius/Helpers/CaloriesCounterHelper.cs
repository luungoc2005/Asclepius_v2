using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asclepius.Helpers
{
    class CaloriesCounterHelper
    {
        User.AppUser _user;

        public CaloriesCounterHelper(User.AppUser user)
        {
            this._user = user;
        }
        
        //per record
        public int CalcBMR()
        {
            if (_user == null)
            {
                return 0;
            }
            else
            {
                return RawBMR(_user.UserGender, _user.Weight, _user.Height, _user.Age);
            }
        }

        public int CalcHourlyBMR(User.Record record)
        {
            if (_user == null)
            {
                return 0;
            }
            else
            {
                if (_user.Snapshots.Count == 0)
                {
                    return 0;
                }
                else
                {
                    User.Snapshot snapshot = User.Snapshot.FindNearestSnapshot(record, _user);
                    return (int)((double)RawBMR(_user.UserGender, snapshot.Weight, snapshot.Height, Common.CommonMethods.CalcAge(record.StartDate, _user.Birthdate)) / 24);
                }
            }
        }

        //fallback function
        public int CalcHourlyBMR(DateTime date)
        {
            if (_user == null)
            {
                return 0;
            }
            else
            {
                if (_user.Snapshots.Count == 0)
                {
                    return 0;
                }
                else
                {
                    User.Record record = new User.Record();
                    record.StartDate = date;

                    User.Snapshot snapshot = User.Snapshot.FindNearestSnapshot(record, _user);
                    return (int)((double)RawBMR(_user.UserGender, snapshot.Weight, snapshot.Height, Common.CommonMethods.CalcAge(record.StartDate, _user.Birthdate)) / 24);
                        //* GetHBMultiplier(_user.UserActivityLevel));
                }
            }
        }

        public int CalcBMR(User.Record record) 
        {
            return (int)(CalcHourlyBMR(record) * (record.EndDate - record.StartDate).TotalHours);
        }

        public int RawBMR(User.AppUser.Gender gender, double weight, double height, int age)
        {
            //revised Harris–Benedict equation
            /*
            Men	BMR = 88.362 + (13.397 x weight in kg) + (4.799 x height in cm) - (5.677 x age in years)
            Women	BMR = 447.593 + (9.247 x weight in kg) + (3.098 x height in cm) - (4.330 x age in years)
            */
            User.AppUser.Gender _gender;
            switch (gender)
            {
                case Asclepius.User.AppUser.Gender.Other:
                    _gender = Asclepius.User.AppUser.Gender.Male;
                    break;
                case Asclepius.User.AppUser.Gender.Male:
                    _gender = Asclepius.User.AppUser.Gender.Male;
                    break;
                case Asclepius.User.AppUser.Gender.Female:
                    _gender = Asclepius.User.AppUser.Gender.Female;
                    break;
                default:
                    _gender = Asclepius.User.AppUser.Gender.Female;
                    break;
            }

            return (int)((_gender == Asclepius.User.AppUser.Gender.Male) ?
            88.362 + (13.397 * weight) + (4.799 * height) - (5.677 * age) :
            447.593 + (9.247 * weight) + (3.098 * height) - (4.330 * age));
        }

        public int AdjustedBMR(User.Record record)
        {
            return (int)(CalcBMR(record)); //* GetHBMultiplier(record.ActivityLevel));
        }

        public double GetHBMultiplier(User.AppUser.ActivityLevel level)
        {
            switch (level)
            {
                case Asclepius.User.AppUser.ActivityLevel.Little:
                    return 1.2;
                case Asclepius.User.AppUser.ActivityLevel.Light:
                    return 1.375;
                case Asclepius.User.AppUser.ActivityLevel.Moderate:
                    return 1.55;
                case Asclepius.User.AppUser.ActivityLevel.Heavy:
                    return 1.725;
                case Asclepius.User.AppUser.ActivityLevel.VeryHeavy:
                    return 1.9;
                default:
                    return 1;
            }
        }

        public int GetWalkingCalorieBurn(User.Record record)
        {
            User.Snapshot snapshot = User.Snapshot.FindNearestSnapshot(record, _user);
            double Time = ((double)(record.WalkTime + record.RunTime) / 3600);
            if (Time <= 0.0013888888888889) return 0; //5 seconds
            double KPH = Common.CommonMethods.CalcDistance((uint)(record.WalkingStepCount+record.RunningStepCount), _user.StepLength) / Time;
            return (int)Math.Round((0.0215 * KPH * KPH * KPH - 0.1765 * KPH * KPH + 0.8710 * KPH + 1.4577) * snapshot.Weight * Time);
        }

        public int GetDailyCalorieGoal()
        {
            return (int)((double)RawBMR(_user.UserGender, _user.Weight, _user.Height, _user.Age) * GetHBMultiplier(_user.UserActivityLevel));
        }

        //per day
        public int AdjustedBMR(User.DailyRecord record)
        {
            return record.Records.Sum(r => r.ActivityType == 0 ? AdjustedBMR(r) : 0);
        }

        public int GetWalkingCalorieBurn(User.DailyRecord record)
        {
            return record.Records.Sum(r => GetWalkingCalorieBurn(r));
        }

        public int TotalCalories(User.DailyRecord record)
        {
            return AdjustedBMR(record) + GetWalkingCalorieBurn(record);
        }
    }
}
