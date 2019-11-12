using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TIMESUM
{
    class Program
    {
        static void Main(string[] args)
        {;
            int dist = 250;
            double time = 0;
            long vertex = 0;
            string dir = @"C:\Users\Dima\source\repos\data_nav\data\Washington\"+ dist + "\\TIME";
            //string dir = @"C:\Users\Dima\source\repos\data_nav\data\Washington\FULL\TIME";
            /* foreach (string file in Directory.EnumerateFiles(dir, "*.txt"))
            {
                string contents = File.ReadAllText(file);
                time += Convert.ToDouble(contents);
            }
            using (StreamWriter wr = new StreamWriter(dir + "\\ALL_TIME.txt"))
            {
                wr.Write(time);
            }
            */
            dir += @"\vertex";
            //string dir = @"C:\Users\Dima\source\repos\data_nav\data\Washington\FULL\TIME";
            foreach (string file in Directory.EnumerateFiles(dir, "*.txt"))
            {
                string contents = File.ReadAllText(file);
                vertex += Convert.ToInt64(contents);
            }
            using (StreamWriter wr = new StreamWriter(dir + "\\ALL_VERTEX.txt"))
            {
                wr.Write(vertex);
            }
        }
    }
}
