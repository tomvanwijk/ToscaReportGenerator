using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BKR.Test.ToscaReportGenerator
{
    public static class Helper
    {
        private const string FILENAME = @"D:\toscatest.txt";
        public static void Clear()
        {
        
            File.WriteAllText(FILENAME,"");
        }

        public static void Schrijf (string t1, string t2, int inspring = 0)
        {
            String tekst = new string(' ', inspring);
            tekst += String.Format("Name = {0}  | Type = {1} ", t1 , t2);
            tekst += Environment.NewLine;
            Console.Write(tekst);
            File.AppendAllText(FILENAME, tekst);
        }
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

    }



}
