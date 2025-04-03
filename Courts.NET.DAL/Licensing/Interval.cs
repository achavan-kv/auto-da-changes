using System;
using System.Collections.Generic;
using System.Text;

namespace STL.DAL.Licensing
{
    public class Interval
    {
        public Interval() { }
        public Interval(int hours, int minutes, int seconds)
        {
            this.Hours = hours;
            this.Minutes = minutes;
            this.Seconds = seconds;

            CalculateMilliseconds();
        }

        public Interval(int milliseconds)
        {
            this.MilliSeconds = milliseconds;
        }

        public Interval(string time)
        {
            string[] times = time.Split(':');
            this.Hours = int.Parse(times[0]);
            this.Minutes = int.Parse(times[1]);
            this.Seconds = int.Parse(times[2]);

            CalculateMilliseconds();
        }

        public int Hours
        { get; private set; }
        public int Minutes
        { get; private set; }
        public int Seconds
        { get; private set; }
        public int MilliSeconds
        { get; private set; }

        void CalculateMilliseconds()
        {
            MilliSeconds += Seconds * 1000;
            MilliSeconds += Minutes * 60 * 1000;
            MilliSeconds += Hours * 60 * 60 * 1000;
        }
    }
}
