using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asclepius.User
{
    public class DailyRecord
    {
        public List<Record> Records = new List<Record>();

        public DateTime Date { get; set; }
        
        public Record GetHourlyRecord(int _hour, bool nullIfNotFound = false)
        {
            DateTime _start = new DateTime(Date.Year, Date.Month, Date.Day, _hour, 0, 0);
            DateTime _end = _start.AddHours(1);

            int index = Records.FindIndex((Record r) => { return (r.ActivityType == 0 && r.StartDate == _start && r.EndDate == _end); });

            if (index == -1)
            {
                if (!nullIfNotFound)
                {
                    Record _ret = new Record(_start, _end);
                    Records.Insert(0, _ret);
                    return _ret;
                }
                else return null;
            }
            else
            {
                return Records[index];
            }
        }

        public void SortLists()
        {
            Records.Sort(delegate(Record x, Record y) { return x.CompareTo(y); });
        }

        public Record SummarizeRecord()
        {
            Record ret=new Record();
            foreach (Record r in Records)
            {
                ret += r;
            }
            return ret;
        }
    }
}
