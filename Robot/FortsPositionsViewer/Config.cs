using System;
using System.IO;

namespace FortsPositionsViewer
{
    public class Config
    {
        public static string GetFortsOpenPositionsDataFilePath(DeriativeNames name)
        {
            return String.Format("C:\\MyProjects\\FortsData\\Data\\{0}.txt", name);
        }

        public static string GetFortsOpenPositionsRawDataFolderPath()
        {
            return String.Format("C:\\MyProjects\\FortsData\\RawData");
        }

        public static string GetFortsOpenPositionsRawDataFilePath(DateTime date)
        {
            return Path.Combine(GetFortsOpenPositionsRawDataFolderPath(),   
                                String.Format("{0}.txt", date.ToString("yy-MM-dd")));
        }

        public static  string GetFortsOpenPositionsUrl(DateTime date)
        {
            return String.Format("http://moex.com/ru/derivatives/open-positions-csv.aspx?d={0}&t=2", date.ToString("yyyyMMdd")); 
        }

        public static DateTime GetSyncDefaultStartDate()
        {
            return new DateTime(2016, 1, 1);
        }

        
    }
}
