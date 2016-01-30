using System;
using System.Collections.Generic;
using System.Linq;

namespace SubCycles
{
    public class calculations
    {
        List<statsData> _Sortedlist = new List<statsData>();

        public calculations(List<SubSequence> inputList)
        {

            var routineNames = inputList.GroupBy(a => a._name).Reverse();
            foreach (var routine in routineNames)
            {
                statsData tempData = new statsData(routine.Key);

                foreach (SubSequence sequence in routine)
                {
                    tempData.addDurationTime((float)sequence._duration.TotalSeconds);
                }

                _Sortedlist.Add(tempData);
            }
        }


        public List<statsData> getStatsDataList()
        {
            return _Sortedlist;    
        }

    }

    public class statsData
    {
        public string _name;
        public List<float> _durations = new List<float>();

        public statsData(string name)
        {
            _name = name;
        }

        public void addDurationTime(float indur)
        {
            _durations.Add(indur);    
        }

        public float getAverage()
        {
            return _durations.Average();
        }

        public float getMax()
        {
            return _durations.Max();
        }

        public float getMin()
        {
            return _durations.Min();
        }

    }

}

