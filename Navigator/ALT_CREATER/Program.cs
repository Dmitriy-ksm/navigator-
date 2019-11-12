using System;
using System.Collections.Generic;
using System.IO;


namespace ALT_CREATER
{
    class Program
    {
        static void Main(string[] args)
        {
            ALT.ALT alt = new ALT.ALT();
            
            string dir = @"C:\Users\Dima\source\repos\data_nav\data\";
            string sity = "Washington";
            string papka = dir + sity + "\\";
            string file_routinegraf_name = sity+".GNF.XML";
            string file_Landmarks_name = "Landmarks"+sity+".txt";
            string file_ALT_name = "FULL\\ALT_"+sity;
            string filetime_ALT_name = papka + "FULL\\TIME\\ALT_" + sity;
            Directory.CreateDirectory(papka + "\\FULL");
            Directory.CreateDirectory(papka + "\\FULL\\TIME");
            alt.LoadGraf(papka+ file_routinegraf_name);
            alt.LoadLandmarks(papka + file_Landmarks_name);
            alt.SetLandmarksData(file_name:papka + file_ALT_name, timefile_fullpatch_name:filetime_ALT_name);
            /*
            double dist = 7500;
            string dir = @"C:\Users\Dima\source\repos\data_nav\data\";
            string sity = "Washington";
            string papka = dir + sity + "\\";
            string file_routinegraf_name = sity + ".GNF.XML";
            string file_Landmarks_name = "Landmarks" + sity + ".txt";
            string file_ALT_name = dist + "\\ALT_" + sity;
            string filetime_ALT_name = papka + dist + "\\TIME\\ALT_" + sity;
            Directory.CreateDirectory(papka + "\\" + dist);
            Directory.CreateDirectory(papka + "\\" + dist + "\\TIME");
            alt.LoadGraf(papka + file_routinegraf_name);
            alt.LoadLandmarks(papka + file_Landmarks_name);
            alt.SetLandmarksData(file_name:papka + file_ALT_name, distans: dist, timefile_fullpatch_name:filetime_ALT_name);
            */
        }
    }
}
