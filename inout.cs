using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

namespace SubCycles
{
    public class inout
    {

        public List<string> sequenceStringList;

        public inout()
        {
            
        }



        public List<string> getFilePathsFromUser()
        {
            Console.WriteLine("Give Directory or file to analyze: ");
            string input = Console.ReadLine().Trim();

            List<string> fileList = new List<string>();

            if (File.Exists(input))
            {
                if (Path.GetExtension(input).Equals(".log"))
                {
                    Console.WriteLine("Will process:\n--------------------------------------------------------------------------------------");
                    Console.WriteLine("\t"+input);
                    fileList.Add(input);
                    Console.WriteLine("--------------------------------------------------------------------------------------");
                    return fileList;
                }
            }
            else if (Directory.Exists(input))
            {
                Console.WriteLine("Will process:\n--------------------------------------------------------------------------------------");
                foreach (string file in Directory.GetFiles(input))
                {
                    if (Path.GetExtension(file).Equals(".log"))
                    {
                        Console.WriteLine("\t"+file);
                        fileList.Add(file);
                    }
                }
                Console.WriteLine("--------------------------------------------------------------------------------------");
                return fileList;
            }
            else
            {
                Console.WriteLine("Not file or Directory, EXITING!!!");
                System.Environment.Exit(0);

            }

            return fileList;
        }


        public void writeStats(List<statsData> inputList,string outputPath)
        {
            TextWriter tw = new StreamWriter(outputPath);

            tw.WriteLine("----------- Stats overview -------------");

            foreach (statsData s in inputList)
                if (s._name.Contains(":"))
                    tw.WriteLine("\n\tName: \t" + s._name +"\n\tAvg: \t"+ s.getAverage() +"\n\tMin: \t"+ s.getMin() +"\n\tMax: \t" + s.getMax());
                else
                    tw.WriteLine("\nName: \t" + s._name +"\nAvg: \t"+ s.getAverage() +"\nMin: \t"+ s.getMin() +"\nMax: \t" + s.getMax());    
                
            tw.Close();
        }


        public void writeHtml(List<string> inputLines, string outputPath)
        {
            TextWriter tw = new StreamWriter(outputPath);

            foreach (string s in inputLines)
                tw.WriteLine(s);

            tw.Close();
        }

        public string createInfoString(string path)
        {
            var reader = new StreamReader(File.OpenRead(path));
            string outputstring = Path.GetFileNameWithoutExtension(path);

            bool PsFound = false;
            bool AccuFound = false;

            while (!reader.EndOfStream)
            {
                // read line
                var line = reader.ReadLine();

                if (line.Contains(" Paint PS: ") && !PsFound)
                {
                    outputstring = outputstring + ("     Supply PS: " + line.Substring(line.IndexOf("Paint PS: ") + "Paint PS: ".Length, line.Length - (line.IndexOf("Paint PS: ") + "Paint PS: ".Length)));
                    PsFound = true;
                }
                   
                if (line.Contains(" Accu: ") && !AccuFound)
                {
                    string number = line.Substring(line.IndexOf("Accu: ") + "Accu: ".Length, line.Length - (line.IndexOf("Accu: ") + "Accu: ".Length));
                   
                    var newi = (int)Math.Round(double.Parse(number) / 100d)*100;
                    string tempstring = ("     Fill setpoint " + newi);
                    outputstring = outputstring + tempstring;
                    AccuFound = true;
                }

                if (PsFound && AccuFound)
                {
                    return outputstring;
                }
                    
            }

            return outputstring;

        }




        public DateTime getZeroHourTimestamp(string startSequenceName)
        {
            foreach (var line in sequenceStringList)
            {
                if (line.Contains("Sequence: " + startSequenceName) && line.Contains("Total: ") )
                {
                    DateTime zhTimestamp = findTimestamp(line);
                    DateTime zeroHourTime = zhTimestamp - findDuration(line);

                    return zeroHourTime;
                }
            }
            return new DateTime();
        }
            
        public float getZeroHourSeconds(string startSequenceName)
        {
            foreach (var line in sequenceStringList)
            {
                if (line.Contains("Sequence: " + startSequenceName) && line.Contains("Total: ") )
                {
                    DateTime zhTimestamp = findTimestamp(line);

                    int idxEndTimeStart = line.IndexOf(", Total: ") + ", Total: ".Length;
                    float floatGlobalSec = float.Parse(line.Substring(idxEndTimeStart),CultureInfo.InvariantCulture.NumberFormat);

                    floatGlobalSec = floatGlobalSec - (float)findDuration(line).TotalSeconds;

                    return floatGlobalSec;
                }
            }

            throw new Exception("No starttime could be found, check integrity of log and startsequence name");
        }


        public List<string> readFile(string path, bool includeSubRoutines)
        {
            var reader = new StreamReader(File.OpenRead(path));
            List<string> sequenceList = new List<string>();

            while (!reader.EndOfStream)
            {
                // read line
                var line = reader.ReadLine();

                if (includeSubRoutines)
                    if (line.Contains("Sequence: "))
                        sequenceList.Add(line);
                
                if (!includeSubRoutines)
                    if (line.Contains("Sequence: ") && line.Contains("Total:"))
                        sequenceList.Add(line);
            }

            sequenceStringList = sequenceList;
            return sequenceList;
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
            
        private DateTime findTimestamp(string rawString)
        {
            int idxNameEnd = rawString.IndexOf(" Sequence:");
            string timestampString = rawString.Substring(0,idxNameEnd);
            return Convert.ToDateTime(timestampString);
        }

    }
}

