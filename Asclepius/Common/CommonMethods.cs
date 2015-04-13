using Asclepius.AppPages.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Asclepius.Common
{
    public class CommonMethods
    {
        public class ActivityType
        {
            public string description { get; set; }
            public string imagePath { get; set; }
            public double metaEquivalent { get; set; }
            public ActivityType(string _description, double _met, string _path = "")
            {
                this.description = _description;
                this.metaEquivalent = _met;
                this.imagePath = _path;
            }
        }

        public static Dictionary<byte, ActivityType> activityTypes = new Dictionary<byte, ActivityType>() 
        { 
            {0, new ActivityType("Walking",4)},
            {1, new ActivityType("Mild stretching",2)},
            {2, new ActivityType("Billards",2.5)},
            {3, new ActivityType("Darts", 2.5)},
            {4, new ActivityType("Light workout",3)},
            {5, new ActivityType("Miniature Golf",3)},
            {6, new ActivityType("Bowling",3)},
            {7, new ActivityType("Frisbee",3)},
            {8, new ActivityType("Fishing",3)},
            {9, new ActivityType("Marching band", 3.5)},
            {10, new ActivityType("Archery", 3.5)},
            {11, new ActivityType("Horseback riding", 4)},
            {12, new ActivityType("Table tennis", 4)},
            {13, new ActivityType("Volleyball (general)", 4)},
            {14, new ActivityType("Tai Chi", 4)},
            {15, new ActivityType("Badminton (general)", 4.5)},
            {16, new ActivityType("Golf", 4.5)},
            {17, new ActivityType("Ball room dancing", 5)},
            {18, new ActivityType("Baseball", 5)},
            {19, new ActivityType("Skateboarding", 5)},
            {20, new ActivityType("Health club exercise", 5.5)},
            {21, new ActivityType("Hiking", 6)},
            {22, new ActivityType("Weight lifting", 6)},
            {23, new ActivityType("Fencing", 6)},
            {24, new ActivityType("Basketball (general)", 6)},
            {26, new ActivityType("Swimming (leisure)", 6)},
            {27, new ActivityType("Water skiing", 6)},
            {28, new ActivityType("Badminton (competitive)", 7)},
            {29, new ActivityType("Soccer (general)", 7)},
            {30, new ActivityType("Swimming (backstroke)", 7)},
            {31, new ActivityType("Bicycling", 8)},
            {32, new ActivityType("Basketball (competitive)", 8)},
            {33, new ActivityType("Voleyball (competitive)", 8)},
            {34, new ActivityType("Soccer (competitive)", 10)},
            {35, new ActivityType("Judo/karate/kick boxing", 10)},
            {36, new ActivityType("Rope jumping", 10)},
            {37, new ActivityType("Swimming (butterfly)", 11)},
            {38, new ActivityType("Squash (butterfly)", 12)},
            {39, new ActivityType("Boxing", 12)},
        };

        public static void PromptExitApplication()
        {
            if (MessageBox.Show("Are you sure you want to quit?", "Confirmation", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Application.Current.Terminate();
            }
        }

        public static UserPageModel mainModel;

        public static double CalcDistance(uint steps, double stepsLength)
        {
            return Math.Round((stepsLength / 100000) * (steps), 2);
        }

        public static string FormatDate(DateTime date)
        {
            CultureInfo ci = new CultureInfo("en-GB");

            string format = "D";
            return date.ToString(format, ci);
        }

        public static int CalcAge(DateTime reference, DateTime birthDate)
        {
            DateTime now = reference;
            int age = now.Year - birthDate.Year;
            if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day)) age--;
            return age;
        }

        public static float WeightedVariance(float[] _samples)
        {
            float _sumOfSamples = 0;
            float _countOfSamples = 0;
            float _average = 0;
            float _variance = 0;

            for (int i = 0; i < _samples.Length; i++)
            {
                _sumOfSamples += (i + 1) * _samples[i];
                if (_samples[i] != 0) _countOfSamples += (i + 1);
            }
            _average = _sumOfSamples / _countOfSamples;

            _countOfSamples = 0;
            for (int i = 0; i < _samples.Length; i++)
            {
                _variance += (i + 1) * (_samples[i] - _average) * (_samples[i] - _average); //weighted variance
                _countOfSamples += (i + 1);
            }
            _variance = _variance / _countOfSamples;

            return _variance;
        }
    }
}
