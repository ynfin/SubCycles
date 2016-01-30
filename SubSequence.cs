using System;
using System.Globalization;

namespace SubCycles
{
    public class SubSequence
    {
        public string _name { get;}
        public DateTime _timeStamp { get;}
        public TimeSpan _duration { get;}

        public DateTime _startTime { get;}
        public DateTime _endTime {get; }

        public string _infoSupplyPS { get;} 
        public string _infoFillSetPoint { get;} 

        //2016-01-25 13:58:13 Sequence: FillCart:DumpDcu, Time: 3.001s

        public SubSequence(string rawString, DateTime globalStartTime, float globalStartTimeSec)
        {
            this._name = findName(rawString);
            this._timeStamp = findTimestamp(rawString);
            this._duration = findDuration(rawString);
            this._endTime = findEndTime(rawString,globalStartTime,globalStartTimeSec);
            this._startTime = findStartTime(this._endTime, this._duration);
        }
    
        private string findName(string rawString)
        {
            int idxNameStart = rawString.IndexOf("Sequence: ") + "Sequence: ".Length;
            int idxNameEnd = rawString.IndexOf(", Time:");
            int lengthName = idxNameEnd - idxNameStart;
            return rawString.Substring(idxNameStart, lengthName);
        }

        private DateTime findTimestamp(string rawString)
        {
            int idxNameEnd = rawString.IndexOf(" Sequence:");
            string timestampString = rawString.Substring(0,idxNameEnd);
            return Convert.ToDateTime(timestampString);
        }
    
        private TimeSpan findDuration(string rawString)
        {
            int idxDurationStart = rawString.IndexOf(", Time: ") + ", Time: ".Length;
            int idxDurationEnd;
            if (rawString.Contains(", Total:"))
            {
                idxDurationEnd = rawString.IndexOf(", Total:") -1;
            }
            else
            {
                idxDurationEnd = rawString.Length - 1;
            }

            int lengthDuration = idxDurationEnd - idxDurationStart;
            string durationString = rawString.Substring(idxDurationStart, lengthDuration);

            float durationFloat = float.Parse(durationString, CultureInfo.InvariantCulture.NumberFormat);
            return TimeSpan.FromSeconds(durationFloat);
        }

        private DateTime findEndTime(string rawString, DateTime firstTimeStamp, float globalStartTimeSec)
        {
            DateTime endDateTime;
            if (rawString.Contains("Total:"))
            {
                int idxEndTimeStart = rawString.IndexOf(", Total: ") + ", Total: ".Length;
                float floatGlobalSec = float.Parse(rawString.Substring(idxEndTimeStart),CultureInfo.InvariantCulture.NumberFormat);
                endDateTime = firstTimeStamp.Add(TimeSpan.FromSeconds(floatGlobalSec-globalStartTimeSec));
            }
            else
                endDateTime = findTimestamp(rawString);
                
            return endDateTime;
        }

        private DateTime findStartTime(DateTime endTime, TimeSpan durationTime)
        {
            return endTime.Subtract(durationTime);
        }
              
    }
}

