using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FromPTStoLandmark
{
    class Program
    {
        static void Main(string[] args)
        {
            string dir = @"C:\Users\Dima\source\repos\data_nav\data\";
            string sity = @"Washington\";
            string file = "Washington.xml.PTS";
            string outputfile = "LandmarksWashington.txt";
            List<string> xmlobjects = new List<string>();
            List<string> landmarks = new List<string>();
            using (var sr = new StreamReader(dir+sity+file))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    xmlobjects.AddRange(line.Split(new char[] { '<', '>' }));
                }
            }
            foreach(var xmlobject in xmlobjects)
            {
                string[] xmlpath = xmlobject.Split(' ');
                foreach(var attribute in xmlpath)
                {
                    if (attribute.Contains("point="))
                    {
                        string point = attribute.Split('"')[1];
                        if (!landmarks.Contains(point))
                            landmarks.Add(point);
                    }
                }
            }
            using (StreamWriter sw = new StreamWriter(dir + sity + outputfile))
            {
                foreach(var item in landmarks)
                    sw.Write(item + ',');
            }
        }
    }
}
