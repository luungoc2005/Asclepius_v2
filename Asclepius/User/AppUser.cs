using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace Asclepius.User
{
    public class AppUser
    {
        [XmlIgnore]
        public BitmapImage UserAvatar
        {
            get
            {
                if (UserAvatarSerialized != null)
                {
                    using (MemoryStream ms = new MemoryStream(UserAvatarSerialized))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.SetSource(ms);
                        return bitmap;
                    }
                }
                else
                {
                    return new BitmapImage(new Uri("/Resources/images/defaultAvatar.jpg", UriKind.Relative));
                }
            }
            set
            {
                if (value != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        WriteableBitmap wbi = new WriteableBitmap(value);
                        wbi.SaveJpeg(ms, Math.Min(128, wbi.PixelWidth), Math.Min(128, wbi.PixelHeight), 0, 85); //cut to 128x128
                        UserAvatarSerialized = ms.ToArray();
                    }
                };
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement("UserAvatar")]
        public byte[] UserAvatarSerialized { get; set; }

        public string Username { get; set; }

        private string fileName = "";
        public string FileName
        {
            get
            {
                if (fileName == "")
                {
                    //var random = new Random();
                    //byte[] temp = new byte[16];
                    //random.NextBytes(temp);

                    //fileName = AsciiToString(temp);
                    fileName = System.IO.Path.GetRandomFileName();
                }
                return fileName;
            }
            set
            {
                this.fileName = value;
            }
        }

        public string EmailAddr { get; set; }

        public static string AsciiToString(byte[] bytes)
        {
            return string.Concat(bytes.Select(b => b <= 0x7f ? (char)b : '?'));
        }

        public byte[] Password { get; set; }

        public void setPassword(string pass)
        {
            Password = hashPassword(pass);
        }

        public bool comparePassword(string pass)
        {
            if (Password == null) return false;

            byte[] hash = hashPassword(pass);
            for (int i = 0; i < Math.Min(hash.Length, Password.Length); i++)
            {
                if (hash[i] != Password[i]) return false;
            }
            return true;
        }

        private byte[] hashPassword(string pass)
        {
            var hash = new SHA256Managed();
            byte[] password = Encoding.Unicode.GetBytes(pass);
            return hash.ComputeHash(password);
        }

        public AppUser()
        {
            Birthdate = DateTime.Now;
            Username = "New User";
        }

        #region "Basic info"

        public enum Gender : byte { Other, Male, Female };
        public Gender UserGender { get; set; }

        public enum ActivityLevel : byte { Little, Light, Moderate, Heavy, VeryHeavy };
        public ActivityLevel UserActivityLevel { get; set; }

        public DateTime Birthdate { get; set; }

        public List<Snapshot> Snapshots = new List<Snapshot>();

        public double Weight //user weight in kg
        {
            get
            {
                if (Snapshots.Count == 0)
                {
                    return 0;
                }
                else
                {
                    int index = Snapshots.FindIndex(snap => snap.Weight != 0); //first instance of non-zero weight
                    return (index == -1 ? 0 : Snapshots[index].Weight);
                }
            }
        }

        public double Height //user height in centimeters
        {
            get
            {
                if (Snapshots.Count == 0)
                {
                    return 0;
                }
                else
                {
                    int index = Snapshots.FindIndex(snap => snap.Height != 0); //first instance of non-zero height
                    return (index == -1 ? 0 : Snapshots[index].Height);
                }
            }
        }

        public double BMI
        {
            get
            {
                return Math.Round(Weight * 10000 / (Height * Height), 1);
            }
        }

        public int Age
        {
            get
            {
                return Common.CommonMethods.CalcAge(DateTime.Now, Birthdate);
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public double OverrideStepLength { get; set; }

        [XmlIgnore]
        public double StepLength
        {
            get
            {
                if (OverrideStepLength > 0)
                {
                    return OverrideStepLength;
                }
                else
                {
                    switch (UserGender)
                    {
                        case Gender.Other:
                            return Height * 0.415;
                        case Gender.Male:
                            return Height * 0.415;
                        case Gender.Female:
                            return Height * 0.413;
                        default:
                            return Height * 0.413;
                    }
                }
            }
            set
            {
                OverrideStepLength = value;
            }
        }

        #endregion

        #region "Records"

        public List<DailyRecord> DailyRecords = new List<DailyRecord>();

        public DailyRecord FindRecord(DateTime date, bool nullIfNotFound=false)
        {
            DateTime refDate=new DateTime(date.Year, date.Month, date.Day);
            int index = DailyRecords.FindIndex(x => (x.Date == refDate));
            if (index == -1)
            {
                if (nullIfNotFound) return null;
                var ret = new DailyRecord();
                ret.Date = new DateTime(date.Year, date.Month, date.Day);
                DailyRecords.Add(ret);
                return ret;
            }
            else
            {
                return DailyRecords[index];
            }
        }

        public DailyRecord Today
        {
            get { return FindRecord(DateTime.Now); }
        }

        #endregion

        #region "Heart rate"
        public double HeartRate //user heart rate
        {
            get
            {
                if (Snapshots.Count == 0)
                {
                    return 0;
                }
                else
                {
                    int index = Snapshots.FindIndex(snap => snap.HeartRate != 0); //first instance of non-zero heart rate
                    return (index == -1 ? 0 : Snapshots[index].HeartRate);
                }
            }
        }
        #endregion

        #region "Social"

        public List<string> Friends;
        public string Status { get; set; }

        #endregion

    }
}
