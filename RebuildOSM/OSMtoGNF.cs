using System;
using System.IO;
using System.Collections.Generic;

namespace OSMRework
{
    public static class OSMtoGNF
    {
        private struct GrafNodeParam
        {
            public double latittude;
            public double longitude;
            public double sin_coord_Latitude_radian; //Синус долготы
            public double cos_coord_Latitude_radian; //Косинус долготы
            public double coord_Longitude_radian;// Широта
            public double koef;// Доп. параметры
            /// <summary>
            /// Установка вершин
            /// </summary>
            /// <param name="latitude">Долгота(градусы)</param>
            /// <param name="longitude">Широта(градусы)</param>
            /// <param name="koef">Доп. Параметр</param>
            public GrafNodeParam(double latittude, double longitude, double koef)
            {
                this.latittude = latittude;
                this.longitude = longitude;
                sin_coord_Latitude_radian = Math.Sin(latittude * Math.PI / 180);
                cos_coord_Latitude_radian = Math.Cos(latittude * Math.PI / 180);
                coord_Longitude_radian = longitude * Math.PI / 180;
                this.koef = koef;
            }
        }
        /// <summary>
        /// Расчет растояния между данной и точкой назначения 
        /// </summary>
        /// <param name="dest_node">Координаты назначения</param>
        /// <param name="sour_node">Координаты источника</param>
        /// <returns>Возвращает длину в метрах</returns>
        static private double calculationCost(GrafNodeParam dest_node,GrafNodeParam sour_node)
        {
            double return_value;
            double sin_delt_Longitude = Math.Sin(dest_node.coord_Longitude_radian - sour_node.coord_Longitude_radian);
            double cos_delt_Longitude = Math.Cos(dest_node.coord_Longitude_radian - sour_node.coord_Longitude_radian);
            double y = Math.Sqrt(Math.Pow(dest_node.cos_coord_Latitude_radian * sin_delt_Longitude, 2) + Math.Pow(sour_node.cos_coord_Latitude_radian * dest_node.sin_coord_Latitude_radian - sour_node.sin_coord_Latitude_radian * dest_node.cos_coord_Latitude_radian * cos_delt_Longitude, 2));
            double x = sour_node.sin_coord_Latitude_radian * dest_node.sin_coord_Latitude_radian + sour_node.cos_coord_Latitude_radian * dest_node.cos_coord_Latitude_radian * cos_delt_Longitude;
            return_value = Math.Atan2(y, x) * 6372795;
            return return_value;
        }
        /// <summary>
        /// Отделяет точки от других объектов
        /// </summary>
        /// <param name="file_input">Карта</param>
        /// <param name="file_output_name">Файл вывода</param>
        /// <param name="file_output_format">Формат файла</param>
        /// <param name="count_node_in_one_file">Количество точек на один выходной файл</param>
        /// <param name="append">Записывать в конец файла или перезаписать файл</param>
        public static void GetNodeFromOSMByPatch(string file_input, string file_output_name, string file_output_format = ".xml", long count_node_in_one_file = 0, bool append = false)
        {
            StreamReader reader = new StreamReader(file_input);
            string file_output = file_output_name + file_output_format;
            StreamWriter writer = new StreamWriter(file_output, append);
            string temp_str = "-1";
            long count_file = 0;
            long count = 0;
            Console.WriteLine("Просмотр " + file_input);
            while (!reader.EndOfStream)
            {
                temp_str = reader.ReadLine();
                if (temp_str.Contains("<node"))
                {
                    count++;
                    if (!temp_str.Contains("/>"))
                        temp_str = temp_str.Replace('>', '/') + ">";
                    writer.WriteLine(temp_str);
                    if (count_node_in_one_file != 0 && count % count_node_in_one_file == 0)
                    {
                        count_file++;
                        writer.Close();
                        file_output = file_output_name + "_" + count_file + file_output_format;
                        writer = new StreamWriter(file_output, append);
                        Console.WriteLine("Записан {0} файл", count_file);
                    }
                }
            }
            writer.Close();
            reader.Close();
            Console.WriteLine("Файл просмотрен " + file_input);
        }
        /// <summary>
        /// Отделяет линии от других объектов
        /// </summary>
        /// <param name="file_input">Карта</param>
        /// <param name="file_output_name">Файл вывода</param>
        /// <param name="file_output_format">>Формат файла</param>
        /// <param name="count_node_in_one_file">Количество линий на один выходной файл</param>
        /// <param name="append">Записывать в конец файла или перезаписать файл</param>
        public static void GetWayFromOSMByPatch(string file_input, string file_output_name, string file_output_format = ".xml", long count_node_in_one_file = 0, bool append = false)
        {
            StreamReader reader = new StreamReader(file_input);
            string file_output = file_output_name + file_output_format;
            StreamWriter writer = new StreamWriter(file_output, append);
            string temp_str = "-1";
            long count_file = 0;
            long count = 0;
            Console.WriteLine("Просмотр " + file_input);
            while (!reader.EndOfStream)
            {
                temp_str = reader.ReadLine();
                if (temp_str.Contains("<way"))
                {
                    count++;
                    while (!temp_str.Contains("</way>"))
                    {
                        writer.WriteLine(temp_str);
                        temp_str = reader.ReadLine();
                    }
                    writer.WriteLine(temp_str);
                    if (count_node_in_one_file != 0 && count % count_node_in_one_file == 0)
                    {
                        count_file++;
                        writer.Close();
                        file_output = file_output_name + "_" + count_file + file_output_format;
                        writer = new StreamWriter(file_output, append);
                        Console.WriteLine("Записан {0} файл", count_file);
                    }
                }
            }
            reader.Close();
            writer.Close();
            Console.WriteLine("Файл просмотрен " + file_input);
        }
        /// <summary>
        /// Отделяет highway линии
        /// </summary>
        /// <param name="file_input">Карта</param>
        /// <param name="file_output_name">Файл вывода</param>
        /// <param name="file_output_format">Формат файла</param>
        /// <param name="count_node_in_one_file">Количество highway линий на один выходной файл</param>
        /// <param name="append">Записывать в конец файла или перезаписать файл</param>
        public static void GetRoadFromOSMByPatch(string file_input, string file_output_name, string file_output_format = ".xml", long count_node_in_one_file = 0, bool append = false, params string[] filter)
        {
            StreamReader reader = new StreamReader(file_input);
            string file_output = file_output_name + file_output_format;
            StreamWriter writer = new StreamWriter(file_output, append);
            string temp_str = "-1";
            long count_file = 0;
            string temp_str_2;
            long count = 0;
            Console.WriteLine("Просмотр " + file_input);
            while (!reader.EndOfStream)
            {
                temp_str_2 = "";
                temp_str = reader.ReadLine();
                if (temp_str.Contains("<way"))
                {
                    while (!temp_str.Contains("</way>"))
                    {
                        temp_str_2 += temp_str + "\n";
                        temp_str = reader.ReadLine();
                    }
                    temp_str_2 += temp_str + "\n";
                    if (temp_str_2.Contains("k=\"highway\""))
                    {
                        bool flag = true;
                        if(filter != null)
                        {
                            foreach(var item in filter)
                            {
                                if (temp_str_2.Contains("v=\"" + item + "\""))
                                {
                                    flag = false;
                                    break;
                                }
                            }
                        }
                        if (flag)
                        { 
                            count++;
                            writer.Write(temp_str_2);
                            if (count_node_in_one_file != 0 && count % count_node_in_one_file == 0)
                            {
                                count_file++;
                                writer.Close();
                                file_output = file_output_name + "_" + count_file + file_output_format;
                                writer = new StreamWriter(file_output, append);
                                Console.WriteLine("Записан {0} файл", count_file);
                            }
                        }
                    }
                }
            }
            reader.Close();
            writer.Close();
            Console.WriteLine("Файл просмотрен " + file_input);
        }
        /// <summary>
        /// Заполняет лист индексов, что относятся к линиям
        /// </summary>
        /// <param name="file_road">Файл с линиями highway</param>
        /// <param name="index_node">Коллекция индексов</param>
        public static void GetListNodeIndexFromWay(string file_road, ref List<long> index_node)
        {
            StreamReader reader_road = new StreamReader(file_road);
            string temp;
            double count = 0;
            Console.WriteLine("Просмотр " + file_road);
            while (!reader_road.EndOfStream)
            {
                temp = reader_road.ReadLine();
                if (temp.Contains("<way"))
                {
                    count++;
                    while (!temp.Contains("</way>"))
                    {
                        string[] temp_container = temp.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (temp_container[0].Contains("<nd"))
                        {
                            for (int i = 0; i < temp_container.Length; i++)
                            {
                                if (temp_container[i].Contains("ref=\""))
                                {
                                    index_node.Add(Convert.ToInt64(temp_container[i].Split('"')[1]));
                                    break;
                                }
                            }
                        }
                        temp = reader_road.ReadLine();
                    }
                    if (count % 100000 == 0)
                        Console.WriteLine("Прочитано {0} строк", count);
                }
            }
            reader_road.Close();
            Console.WriteLine("Файл просмотрен " + file_road);
            Console.WriteLine("Количество точек " + index_node.Count);
        }
        /// <summary>
        /// Обновить лист индексов, что относятся к линиям
        /// </summary>
        /// <param name="file_node">Файл с уже найдеными точками</param>
        /// <param name="index_node_from_road">Коллекция индексов</param>
        public static void UpdateListNode(string file_node, ref List<long> index_node_from_road)
        {
            StreamReader reader_node = new StreamReader(file_node);
            string temp;
            Console.WriteLine("Просмотр " + file_node);
            while (!reader_node.EndOfStream && index_node_from_road.Count > 0)
            {
                temp = reader_node.ReadLine();
                if (temp.Contains("<node"))
                {
                    string[] temp_container = temp.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 1; i < temp_container.Length; i++)
                    {
                        if (temp_container[i].Contains("id="))
                        {
                            long check_id = Convert.ToInt64(temp_container[i].Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[1].Trim(new char[] { '"' }));
                            index_node_from_road.Remove(check_id);
                            break;
                        }
                    }
                }
            }
            reader_node.Close();
            Console.WriteLine("Файл просмотрен " + file_node);
            Console.WriteLine("Требуется добавить {0} строк", index_node_from_road.Count);
        }
        /// <summary>
        /// Заполнение файла точек,что относятся к линиям
        /// </summary>
        /// <param name="index_node_from_road">Коллекция индексов</param>
        /// <param name="file_node">Файл с точками</param>
        /// <param name="file_output_name">Файл вывода</param>
        /// <param name="file_output_format">Формат файла</param>
        /// <param name="count_node_in_one_file">Количество точек на один файл</param>
        /// <param name="append">Записывать в конец файла или перезаписать файл</param>
        public static void NodeAndRoadByPatch(ref List<long> index_node_from_road, string file_node, string file_output_name, string file_output_format = ".xml", long count_node_in_one_file = 0, bool append = false)
        {
            Console.WriteLine("Проверка " + file_node);
            StreamReader reader_node = new StreamReader(file_node);
            string file_output = file_output_name + file_output_format;
            StreamWriter writer = new StreamWriter(file_output, append);
            long count_need = 0;
            string temp;
            string temp_write;
            while (!reader_node.EndOfStream && index_node_from_road.Count > 0)
            {
                temp = reader_node.ReadLine();
                if (temp.Contains("<node"))
                {
                    string[] temp_container = temp.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    temp_write = "<node";
                    long count_file = 0;
                    int count_container = 0;
                    int index = -1;
                    long check_id = Convert.ToInt64(temp_container[1].Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[1].Trim(new char[] { '"' }));
                    index = index_node_from_road.IndexOf(check_id);
                    count_container++;
                    temp_write += " " + temp_container[1];
                    if(index != -1)
                    { 
                        for (int i = 2; i < temp_container.Length; i++)
                        {
                            if (temp_container[i].Contains("lat=") || temp_container[i].Contains("lon="))
                            {
                                count_container++;
                                temp_write += " " + temp_container[i];
                            }
                            if (count_container == 3)
                            {
                                temp_write += "/>";
                                writer.WriteLine(temp_write);
                                index_node_from_road.RemoveAt(index);
                                count_need++;
                                if (count_node_in_one_file != 0 && count_need % count_node_in_one_file == 0)
                                {
                                    count_file++;
                                    writer.Close();
                                    file_output = file_output_name + "_" + count_file + file_output_format;
                                    writer = new StreamWriter(file_output, append);
                                    Console.WriteLine("Записан {0} файл", count_file);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            reader_node.Close();
            writer.Close();
            Console.WriteLine("Конец работы с " + file_node);
        }
        /// <summary>
        /// Получить список точек из файла
        /// </summary>
        /// <param name="file">Файл с точками</param>
        /// <param name="node">Коллекция точек</param>
        public static void getListNode(string file, ref List<string> node)
        {
            using (StreamReader read_node = new StreamReader(file))
            {
                while (!read_node.EndOfStream)
                {
                    string temp_road = read_node.ReadLine();
                    if (temp_road.Contains("<node "))
                        node.Add(temp_road);
                }
            }
        }
        /// <summary>
        /// Получить список дорог из файла
        /// </summary>
        /// <param name="file">Файл с дорогами</param>
        /// <param name="roads">Коллекция коллекций индексов точек для дорог</param>
        public static void getListRoad(string file, ref List<List<long>> roads)
        {
            using (StreamReader read_road = new StreamReader(file))
            {
                while (!read_road.EndOfStream)
                {
                    string temp_road = read_road.ReadLine();
                    if (temp_road.Contains("<way"))
                    {
                        List<long> indexes = new List<long>();
                        while (!temp_road.Contains("</way>"))
                        {
                            temp_road = read_road.ReadLine();
                            if (temp_road.Contains("<nd"))
                                indexes.Add(Convert.ToInt64(temp_road.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[] { '"' }, StringSplitOptions.RemoveEmptyEntries)[1]));
                        }
                        roads.Add(indexes);
                    }
                }
            }
        }
        /// <summary>
        /// Создание временного файла с точками,их координатами и индексами смежных для них точек
        /// </summary>
        /// <param name="file">Выходной файл</param>
        /// <param name="roads">Коллекция коллекций индексов точек для дорог</param>
        /// <param name="node_data">Коллекция точек</param>
        public static void setFileMyNode(string file, List<List<long>> roads, List<string> node_data)
        {
            using (StreamWriter write_my_node = new StreamWriter(file))
            {
                int count = 0;
                int f = (int)(node_data.Count * 0.05);
                int ff = (int)(node_data.Count * 0.15);
                int fff = (int)(node_data.Count * 0.30);
                int ffff = (int)(node_data.Count * 0.50);
                int fffff = (int)(node_data.Count * 0.75);
                int ffffff = (int)(node_data.Count * 0.90);
                int fffffff = (int)(node_data.Count * 0.95);
                foreach (string node in node_data)
                {
                    string[] node_container = node.Split(' ');
                    write_my_node.WriteLine("<node>");
                    long index = Convert.ToInt64(node_container[1].Split('"')[1]);
                    write_my_node.WriteLine("<index>" + index + "</index>");
                    write_my_node.Write("<coord>");
                    write_my_node.Write(node_container[2].Split('"')[1] + ";" + node_container[3].Split('"')[1]);
                    write_my_node.WriteLine("</coord>");
                    write_my_node.Write("<neighbors>");
                    foreach (List<long> road_container in roads)
                    {
                        long prew_indexes = -1;
                        bool flag = false;
                        foreach (long road_indexes in road_container)
                        {
                            if (flag)
                            {
                                flag = false;
                                write_my_node.Write(road_indexes + ";");
                                if (prew_indexes != -1)
                                    write_my_node.Write(prew_indexes + ";");
                            }
                            else
                            {
                                if (road_indexes == index)
                                    flag = true;
                                else
                                    prew_indexes = road_indexes;
                            }
                        }
                        if (flag)
                        {
                            //write_my_node.Write(road_container[road_container.Count-1] + ";");
                            if (prew_indexes != -1)
                                write_my_node.Write(prew_indexes + ";");
                            //break;
                        }
                    }
                    write_my_node.WriteLine("</neighbors>");
                    //write_my_node.WriteLine("<neighborsdistans></neighborsdistans>");
                    write_my_node.WriteLine("</node>");
                    count++;
                    if (count == f)
                        Console.WriteLine("5 % завершенно");
                    if (count == ff)
                        Console.WriteLine("15 % завершенно");
                    if (count == fff)
                        Console.WriteLine("30 % завершенно");
                    if (count == ffff)
                        Console.WriteLine("50 % завершенно");
                    if (count == fffff)
                        Console.WriteLine("75 % завершенно");
                    if (count == ffffff)
                        Console.WriteLine("90 % завершенно");
                    if (count == fffffff)
                        Console.WriteLine("95 % завершенно");
                }
            }
        }
        /// <summary>
        /// Получить или сохранить список не заполеннных точек относящихся к дорогам
        /// </summary>
        /// <param name="index_list">Коллекция индексов точек</param>
        /// <param name="file_output">Файл с индексами</param>
        /// <param name="write">Записать или считать с файла</param>
        public static void ListIndexFromFile(ref List<long> index_list, string file_output, bool write = false)
        {
            if (write)
            {
                using (var list_index_file = new StreamWriter(file_output))
                {
                    foreach (long index in index_list)
                        list_index_file.WriteLine(index);
                }
                Console.WriteLine("Список сохранен");
            }
            else
            {
                using (var list_index_file = new StreamReader(file_output))
                {
                    while (!list_index_file.EndOfStream)
                        index_list.Add(Convert.ToInt64(list_index_file.ReadLine()));
                }
                Console.WriteLine("Список загружен");
            }
        }
        /// <summary>
        /// Фильтрация из карты в файл вершин(входящих в дороги) и файл дорог за раз
        /// </summary>
        /// <param name="folder">Папка с данными</param>
        /// <param name="osm_file_name">Карта</param>
        /// <param name="road">Файл дорог</param>
        /// <returns>Файл вершин</returns>
        public static string DataFilter(string folder, string osm_file_name, out string road,params string[] filter)
        {
            Console.WriteLine("Обрабатываем " + folder);
            GetWayFromOSMByPatch(folder + osm_file_name, folder + "way");
            GetRoadFromOSMByPatch(folder + "way.xml", folder + "road",filter:filter);
            road = folder + "road.xml";
            GetNodeFromOSMByPatch(folder + osm_file_name, folder + "node");
            List<long> road_node_index = new List<long>();
            GetListNodeIndexFromWay(folder + "road.xml", ref road_node_index);
            NodeAndRoadByPatch(ref road_node_index, folder + "node.xml", folder + "nodeandroad");
            Console.WriteLine("Фильтрация " + folder + " завершенна");
            return folder + "nodeandroad.xml";
        }
        /// <summary>
        /// Расчитывает растояния до смежных вершин 
        /// </summary>
        /// <param name="GNFtemp">Файл приближенный к формату GNF</param>
        /// <param name="GNF">Файл вывода</param>
        public static void setDistansInFile(string GNFtemp, string GNF, char dataseparator = '/')
        {
            Dictionary<long, GrafNodeParam> node_param = new Dictionary<long, GrafNodeParam>();
            Dictionary<long, long[]> node_neighbors = new Dictionary<long, long[]>();
            using (StreamReader read = new StreamReader(GNFtemp))
            {
                while (!read.EndOfStream)
                {
                    string temp = read.ReadLine();
                    if (temp.Contains("<node>"))
                    {
                        temp = read.ReadLine();
                        long index = Convert.ToInt64(temp.Split(new char[] { '>' }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[] { '<' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                        temp = read.ReadLine();
                        string[] param_string = temp.Split(new char[] { '>' }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[] { '<' }, StringSplitOptions.RemoveEmptyEntries)[0].Split(';');
                        for (int i = 0; i < param_string.Length; i++)
                        {
                            param_string[i] = param_string[i].Replace('.', ',');
                        }
                        GrafNodeParam param = new GrafNodeParam(Convert.ToDouble(param_string[0]), Convert.ToDouble(param_string[1]), 1);
                        temp = read.ReadLine();
                        string[] param_string_next = temp.Split(new char[] { '>' }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[] { '<' }, StringSplitOptions.RemoveEmptyEntries)[0].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        List<long> temp_neighbors = new List<long>();
                        for(int i=0;i<param_string_next.Length;i++)
                        {
                            long temp_index_neig = Convert.ToInt64(param_string_next[i]);
                            if (!temp_neighbors.Contains(temp_index_neig))
                                temp_neighbors.Add(temp_index_neig);
                        }
                        long[] neighbors = new long[temp_neighbors.Count];
                        temp_neighbors.CopyTo(neighbors);
                        if (!node_param.ContainsKey(index))
                        {
                            node_param.Add(index, param);
                            node_neighbors.Add(index, neighbors);
                        }
                    }
                }
            }
            Console.WriteLine("Вершины загруженны");
            using (StreamWriter write = new StreamWriter(GNF))
            {
                foreach (var item in node_neighbors)
                {
                    string data = "<node v=\"";
                    data += item.Key +""+ dataseparator;
                    for (int i = 0; i < item.Value.Length; i++)
                    {
                        if (node_param.ContainsKey(item.Value[i]))
                            data += item.Value[i] + ";";
                    }
                    data = data.Remove(data.Length - 1) + dataseparator;
                    for (int i = 0; i < item.Value.Length; i++)
                    {
                        if (node_param.ContainsKey(item.Value[i]))
                            data += calculationCost(node_param[item.Key], node_param[item.Value[i]]) + ";";
                    }
                    data = data.Remove(data.Length - 1) + dataseparator;
                    data += node_param[item.Key].latittude + ";" + node_param[item.Key].longitude + ";1\"/>";
                    if (data.Contains(";0;"))
                        data.Replace(";0;", ";0,0000;");
                    if (data.Contains("/0;"))
                        data.Replace("/0;", "/0,0000;");
                    write.WriteLine(data);
                }
            }
            Console.WriteLine("Файл "+GNF+" успешно заполнен");
        }
        /// <summary>
        /// Уменьшает кол-во вершин,убирая вершины буфера
        /// </summary>
        /// <param name="GNF">Входной файл</param>
        /// <param name="GNFupdate">Файл вывода</param>
        /// <param name="count">Сколько вершин удалить за один проход</param>
        /// <returns>True - нету вершин с двумя соседями. False - достигнут count</returns>
        public static bool deleteTwoNeighborsNode(string GNF,string GNFupdate, int count = 0, char dataseparator = '/')
        {
            Dictionary<long,string> nodes = new Dictionary<long, string>();
            using (StreamReader reader = new StreamReader(GNF))
            {
                while (!reader.EndOfStream)
                {
                    string temp = reader.ReadLine();
                    if (temp.Contains("<node"))
                    {
                        string temp_container = temp.Split(' ')[1].Split(new char[] { '"' }, StringSplitOptions.RemoveEmptyEntries)[1];
                        nodes.Add(Convert.ToInt64(temp_container.Split(new char[] { dataseparator }, StringSplitOptions.RemoveEmptyEntries)[0]), temp_container);
                    }
                }
            }
            Console.WriteLine("Вершины загруженны");
            Console.WriteLine("Кол-во вершин " + nodes.Count);
            int count_temp = 0;
            bool flag = false;
            long key = -1;
            while(!flag)
            {
                string first_value = "";
                long[] neighbors = new long[2];
                string second_value = "";
                flag = true;
                foreach (var item in nodes)
                {
                    string[] str_neighbors = item.Value.Split(new char[] { dataseparator }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if (str_neighbors.Length == 2)
                    { 
                        for (int i = 0; i < 2; i++)
                            neighbors[i] = Convert.ToInt64(str_neighbors[i]);
                        string[] str_neigborsdist = item.Value.Split(new char[] { dataseparator }, StringSplitOptions.RemoveEmptyEntries)[2].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        double[] neigborsdist = new double[2];
                        double neigborsdist_all = 0;
                        for (int i = 0; i < 2; i++)
                        {
                            neigborsdist[i] = Convert.ToDouble(str_neigborsdist[i]);
                              neigborsdist_all += neigborsdist[i];
                        }
                        first_value = nodes[neighbors[0]];
                        second_value = nodes[neighbors[1]];
                        first_value =first_value.Replace(item.Key.ToString(), neighbors[1].ToString());
                        first_value = first_value.Replace(neigborsdist[0].ToString(), neigborsdist_all.ToString());
                        second_value = second_value.Replace(item.Key.ToString(), neighbors[0].ToString());
                        second_value = second_value.Replace(neigborsdist[1].ToString(), neigborsdist_all.ToString());
                        flag = false;
                        key = item.Key;
                        break;
                    }
                }
                if(!flag)
                {
                    count_temp++;
                    nodes[neighbors[0]] = first_value;
                    nodes[neighbors[1]] = second_value;
                    nodes.Remove(key);
                }
                if(count!=0)
                    if(count_temp==count)
                    {
                        break;
                    }
            }
            using (StreamWriter writer = new StreamWriter(GNFupdate))
            {
                foreach(var item in nodes)
                {
                    writer.WriteLine("<node v=\"" + item.Value + "\"/>");
                }
            }
            if(count_temp==count)
                return false;
            return true;
        }
    }
}
