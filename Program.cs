using System;
using System.Collections.Generic;
using System.IO;

namespace SubCycles
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            inout io = new inout();

            List<string> filePathList = new List<string>();
            filePathList = io.getFilePathsFromUser();

            Console.WriteLine("proceed? [y/n]");
            char proceedResponse = Console.ReadKey().KeyChar;
            if (proceedResponse != 'y')
                System.Environment.Exit(0);

            foreach (var filepath in filePathList)
            {
                Console.WriteLine("\nProcessing -> " + filepath);

                // settings
                string inputFilePath = filepath; //@"/Users/yngve/Documents/11_400.log";

                string outHtmlFile = Path.GetFullPath(inputFilePath).Substring(0, Path.GetFullPath(inputFilePath).Length - 4) + ".html";

                // gather file data

                var list = io.readFile(inputFilePath, true);

                string info = io.createInfoString(filepath);
                Console.WriteLine (info);

                DateTime globalStartTimestamp = io.getZeroHourTimestamp("FillCart");
                float globalStartSeconds = io.getZeroHourSeconds("FillCart");

                List<SubSequence> seqList = new List<SubSequence>();
         
                // build sequence objects
                foreach (var item in list)
                {
                    SubSequence ss = new SubSequence(item, globalStartTimestamp, globalStartSeconds);
                    seqList.Add(ss);
                }
                

                // calculate
                calculations calc = new calculations(seqList);
                io.writeStats(calc.getStatsDataList(), outHtmlFile.Substring(0, outHtmlFile.Length - 5) + ".txt");

                List<SubSequence> strippedSeqList = new List<SubSequence>();

                foreach (var item in seqList)
                {
                    if (!item._name.Contains(":"))
                    {
                        strippedSeqList.Add(item);
                    }
                }
                
                // write to html file
                HtmlGenerator htmlLines = new HtmlGenerator(info, strippedSeqList, outHtmlFile.Substring(0, outHtmlFile.Length - 5));
                io.writeHtml(htmlLines.generatedHtmlLineList, outHtmlFile);    

                Console.WriteLine("                   -> " + outHtmlFile.Substring(0, outHtmlFile.Length - 5) + ".txt");
                Console.WriteLine("                   -> " + outHtmlFile.Substring(0, outHtmlFile.Length - 5) + ".html");

            }


        }
    }
}
