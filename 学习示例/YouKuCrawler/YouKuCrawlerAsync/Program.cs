using System;

namespace YouKuCrawlerAsync
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            DBMission.InsertTypes();
            DBMission.InsertContent();
            Console.WriteLine("Ok");
            Console.Read();
        }
    }
}