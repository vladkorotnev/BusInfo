using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusInfo
{
    public class Time
    {
        public int hour, minute;
        public Time(int h, int m)
        {
            hour = h;
            minute = m;
        }

        public override string ToString()
        {
            return this.ToString(':');
        }

        public string ToString(char joiner = ':')
        {
            if (joiner == 0)
            {
                return String.Format("{0}{1}",
                                hour.ToString(),
                                minute >= 10 ? minute.ToString() : "0" + minute.ToString());
            }
            return String.Format("{0}{1}{2}",
                                hour.ToString(),
                                joiner,
                                minute >= 10 ? minute.ToString() : "0" + minute.ToString());
        }

        public static Time FromString(string time, char splitter = ':')
        {
            string[] components;

            if (splitter != 0)
            {
                components = time.Split(splitter);
            }
            else if (time.Length == 4)
            {
                components = new string[] {
                        time.Substring(0,2),
                        time.Substring(2,2)
                };
            }
            else return null;

            if (components.Count() != 2)
                return null;

            Time result = new Time(
                                Convert.ToInt32(components[0]),
                                Convert.ToInt32(components[1])
                              );
            return result;
        }

        public bool Passed()
        {
            DateTime now = DateTime.Now;
            if (now.Hour > this.hour ||
                (now.Hour == this.hour && now.Minute > this.minute)) return true;
            return false;
        }

        public int MinutesUntil()
        {
            DateTime now = DateTime.Now;
            int remain = (this.hour - now.Hour) * 60 +
                            (this.minute - now.Minute);
            return remain;
        }

        public int MinutesAfter()
        {
            return -1 * MinutesUntil();
        }

        static public Time Now()
        {
            DateTime now = DateTime.Now;
            return new Time(now.Hour, now.Minute);
        }
    }
}
