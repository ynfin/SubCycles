using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SubCycles
{
    public class HtmlGenerator
    {

        private List<string> sequenceString = new List<string>();
        public List<string> generatedHtmlLineList { get;}

        public HtmlGenerator(string headerText, List<SubSequence> sequenceList, string rootpath)
        {
            foreach (var sequence in sequenceList)
            {
               
                    string startString = "new Date(" + sequence._startTime.ToString("yyyy,MM,dd,HH,mm,ss,fff") + ")";
                    string endString = "new Date(" + sequence._endTime.ToString("yyyy,MM,dd,HH,mm,ss,fff") + ")";

                    string workString = " [";
                    workString = workString + "'" + sequence._name + "',";
                    workString = workString + "'" + sequence._name + "',";
                    workString = workString + startString + ",";
                    workString = workString + endString;
                    workString = workString + "],";

                    sequenceString.Add(workString);

            }

            // strip last char of last element (the last comma)
            sequenceString[sequenceString.Count - 1] = sequenceString[sequenceString.Count - 1].Substring(0, sequenceString[sequenceString.Count - 1].Length - 1);

            List<string> HtmlTemplate = loadHtmlResourceFile();
            generatedHtmlLineList = injectData(HtmlTemplate, headerText, rootpath, sequenceString);

        }
            

        private List<string> injectData(List<string> htmlDataArray, string header,string outpath, List<string> sequences)
        {
            for (int i = 0; i < htmlDataArray.Count; i++)
            {
                if (htmlDataArray[i].Contains("<!-- logfile_insertion_point -->"))
                {
                    string buildstring = "<a href=\"" + outpath + ".log\">Logfile</a>";
                    htmlDataArray.Insert(i + 1, buildstring);
                    break;
                }
            }

            for (int i = 0; i < htmlDataArray.Count; i++)
            {
                if (htmlDataArray[i].Contains("<!-- stats_insertion_point -->"))
                {
                    string buildstring = "<a href=\"" + outpath + ".txt\">Statistics</a>";
                    htmlDataArray.Insert(i + 1, buildstring);
                    break;
                }
            }

            for (int i = 0; i < htmlDataArray.Count; i++)
            {
                if (htmlDataArray[i].Contains("<!-- header_insertion_point -->"))
                {
                    htmlDataArray.Insert(i + 1, header);
                    break;
                }
            }

            for (int i = 0; i < htmlDataArray.Count; i++)
            {
                if (htmlDataArray[i].Contains("<!-- data_insertion_point -->"))
                {
                    htmlDataArray.InsertRange(i + 1, sequences);
                    break;
                }

                //<a href="11_400.log">Logfile</a>

            }

            foreach (var item in htmlDataArray)
            {
                //Console.WriteLine(item);
            }

            return htmlDataArray;
        }

        private List<string> loadHtmlResourceFile()
        {
            string fs = @"EmptyHtmlFile.html";
            string[] lines = File.ReadAllLines(fs);
           

            int idx = 0;
            foreach (var item in lines)
            {
                //Console.WriteLine(idx + "->" + item);  
                idx++;
            }
                          
            List<string> lst = new List<string>(lines);

            return lst;
        }


    }
        
}

