using System;
using NavigationDLL;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace ALT
{
    /// <summary>
    /// Укороченая структура путей, без учета евристики  
    /// </summary>
    public struct Dict_data : IComparable<Dict_data>
    {
        public long index_root;
        public double real_cost;
        public Dict_data(long ind_root, double r_c)
        {
            index_root = ind_root;
            real_cost = r_c;
        }
        public int CompareTo(Dict_data obj)
        {
            if (this.real_cost > obj.real_cost)
                return 1;
            if (this.real_cost < obj.real_cost)
                return -1;
            else
                return 0;
        }
        public static int GetMin(List<Dict_data> data)
        {
            double min = Double.MaxValue;
            int ret_val = -1;
            for(int i=0;i<data.Count;i++)
            {
                if(data[i].real_cost<min)
                {
                    ret_val = i;
                }
            }
            return ret_val;
        }
    }
    public class ALT
    {
        /// <summary>
        /// Поиск пути через ALT
        /// </summary>
        /// <param name="destination">точка назначения</param>
        /// <param name="start">точка старта</param>
        /// <param name="altresult">использовался ли АЛТ</param>
        /// <returns></returns>
        public List<long> GetWay(long destination, long start, out bool altresult)
        {
            altresult = true;
            List<long> path = new List<long>();
            path.Add(destination);
            foreach (var landmark in landmarks)
            {
                if (!Landmarks[landmark].ContainsKey(destination) || !Landmarks[landmark].ContainsKey(start))
                    break;
                Dictionary<long, long> destination_to_alt_path = new Dictionary<long, long>();
                long target = destination;
                while (target!=landmark)
                { 
                    destination_to_alt_path.Add(target, Landmarks[landmark][target]);
                    target = destination_to_alt_path[target];
                }
                if (destination_to_alt_path.ContainsKey(start))
                {
                    while(!path.Contains(start))
                        path.Add(destination_to_alt_path[path.Last()]);
                    return path;
                }
                Dictionary<long, long> start_to_alt_path = new Dictionary<long, long>();
                target = start;
                while (target != landmark)
                {
                    start_to_alt_path.Add(target, Landmarks[landmark][target]);
                    target = start_to_alt_path[target];
                    if(destination_to_alt_path.ContainsKey(target))
                        break;
                }
                while (!path.Contains(target))
                    path.Add(destination_to_alt_path[path.Last()]);
                List<long> path_2 = new List<long>();
                path_2.Add(start);
                while (!path_2.Contains(target))
                    path_2.Add(start_to_alt_path[path_2.Last()]);
                path.AddRange(path_2);
                return path;
            }
            altresult = false;
            double dist;
            List<SavedData> N_L= new List<SavedData>(), L = new List<SavedData>();
            path = tree.getWay_A(start, destination, out dist, ref N_L, ref L);
            return path;
        }
        // Коллекция маршрутов маяков
        public Dictionary<long, Dictionary<long, long>> Landmarks;
        /// <summary>
        /// Инициализация маяков
        /// </summary>
        /// <param name="landmarks">Коллекция айдишников маяков</param>
        public void GetLandmarksCollection(List<long> landmarks)
        {
            Landmarks=new Dictionary<long, Dictionary<long, long>>();
            foreach(var landmark in landmarks)
            {
                Landmarks.Add(landmark, new Dictionary<long, long>());
            }
        }
        /// <summary>
        /// Выгрузка путей из строки
        /// </summary>
        /// <param name="landmark">ID маяка</param>
        /// <param name="path">строка данных</param>
        public void GetLandmarksPath(long landmark, string path)
        {
            // 0 - destination point
            // 1 - parent point
            string[] components = path.Split(new char[] { '/' },StringSplitOptions.RemoveEmptyEntries);
            Landmarks[landmark].Add(Convert.ToInt64(components[0]), Convert.ToInt64(components[1]));
        }
        /// <summary>
        /// Установка пройденного пути для вершины
        /// </summary>
        /// <param name="index">Индекс вершины</param>
        /// <param name="passWay">Пройденный путь</param>
        private void SetCost(long index, double passWay)
        {
            tree[index].calculatedPassedWay(passWay);
        }
        /// <summary>
        /// Получение минимального пути в очереди не просмотренных путей
        /// </summary>
        /// <param name="array">Очередь не просмотренных путей</param>
        /// <returns>Самый короткий путь</returns>
        private static SavedData GetMin(List<SavedData> array)
        {
            SavedData return_value = new SavedData();
            double min = 40076001;
            foreach (var item in array)
            {
                if (item.cost < min)
                {
                    min = item.cost;
                    return_value = item;
                }
            }
            return return_value;
        }
        /// <summary>
        /// Заполнение очереди не просмотренных путей
        /// </summary>
        /// <param name="out_data">Очередь не просмотренных путей</param>
        /// <param name="index">Текущая вершина</param>
        /// <param name="root_index">Вершина из которой был проложен путь к текущей</param>
        /// <param name="check_data">Очередь просмотренных путей</param>
        private void AddOutData(ref List<SavedData> out_data, long index, long root_index, ref Dictionary<long, double> check_data)
        {
            for (int i = 0; i < tree[index].count_nodes; i++)
            {
                if (tree[index].broth_node[i] != root_index)
                {
                    if (check_data.Count > 0)
                    {
                        if (!check_data.ContainsKey(tree[index].broth_node[i]))
                        {
                            out_data.Add(new SavedData(index, tree[index].broth_node[i], tree[index].broth_node_range_way_passed[i] + tree[index].broth_cost_h[i], tree[index].broth_node_range_way_passed[i]));
                            check_data.Add(tree[index].broth_node[i], tree[index].broth_node_range_way_passed[i]);
                        }
                        else
                        {
                            if (check_data[tree[index].broth_node[i]] > tree[index].broth_node_range_way_passed[i])
                            {
                                out_data.Add(new SavedData(index, tree[index].broth_node[i], tree[index].broth_node_range_way_passed[i] + tree[index].broth_cost_h[i], tree[index].broth_node_range_way_passed[i]));
                                check_data[tree[index].broth_node[i]] = tree[index].broth_node_range_way_passed[i];
                            }
                        }
                    }
                    else
                    {
                        out_data.Add(new SavedData(index, tree[index].broth_node[i], tree[index].broth_node_range_way_passed[i] + tree[index].broth_cost_h[i], tree[index].broth_node_range_way_passed[i]));
                        check_data.Add(tree[index].broth_node[i], tree[index].broth_node_range_way_passed[i]);
                    }
                }
            }
        }
        //Дорожный граф
        Graf tree;
        public void CopyGraf(Graf tree)
        {
            this.tree = tree;
        }
        //Колекция маяков
        List<long> landmarks;
        /// <summary>
        /// Построенние дорожного графа 
        /// </summary>
        /// <param name="file_name">Файл с нав. датой</param>
        public void LoadGraf(string file_name)
        {
            using (StreamReader myStream = new StreamReader(file_name))
            {
                tree = new Graf(myStream.ReadLine().Split(' ')[1].Split('"')[1]);
                while (!myStream.EndOfStream)
                {
                    string data = myStream.ReadLine().Split(' ')[1].Split('"')[1];
                    tree.AddNode(new GrafNode(data, '/', ';'));
                }
            }
        }
        /// <summary>
        /// Добавление "Маяков" 
        /// </summary>
        /// <param name="file_name">Файл с маяками</param>
        public void LoadLandmarks(string file_name)
        {
            using (StreamReader myStream = new StreamReader(file_name))
            {
                landmarks = new List<long>();
                while (!myStream.EndOfStream)
                {
                    string[] data = myStream.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < data.Length; i++)
                        landmarks.Add(Convert.ToInt64(data[i]));
                }
            }
            GetLandmarksCollection(landmarks);
        }
        /// <summary>
        /// Расчет маршрутов для маячка 
        /// </summary>
        /// <param name="landmark_index">Маяк</param>
        /// <param name="file_name">Файл для маршрутов из маяка</param>
        /// <param name="distans">Максимальное расстояние от маяка</param>
        /// <returns>Кол-во просмотренных вершин</returns>
        public long FullInfo(long landmark_index, string file_name, double distans = -1)
        {
            long count = 0;
            FileStream fs = new FileStream(file_name + "_" + landmark_index + ".txt", FileMode.OpenOrCreate, FileAccess.Write);
            using (StreamWriter writer = new StreamWriter(fs))
            {
                string value = "< landmar=" + landmark_index + ">" + Environment.NewLine;
                writer.Write(value);
                List<SavedData> Not_Look = new List<SavedData>();
                //List<SavedData> Look = new List<SavedData>();
                Dictionary<long, List<Dict_data>> Look_D = new Dictionary<long, List<Dict_data>>();
                Dictionary<long, double> check = new Dictionary<long, double>();
                AddOutData(ref Not_Look, landmark_index, -1, ref check);
                SavedData temp = GetMin(Not_Look);
                SetCost(temp.index, temp.real_cost);
                //double distans = 0;
                long last_ind = -1;
                long last_ind_root = -1;
                //Look.Add(temp);
                Look_D.Add(temp.index, new List<Dict_data>());
                Look_D[temp.index].Add(new Dict_data(temp.index_root, temp.real_cost));
                last_ind = temp.index;
                last_ind_root = temp.index_root;
                Not_Look.Remove(temp);
                while (Not_Look.Count > 0)
                {
                    AddOutData(ref Not_Look, last_ind, last_ind_root, ref check);
                    temp = GetMin(Not_Look);
                    if (distans > -1)
                        if (temp.cost > distans)
                        {
                            Not_Look.Remove(temp);
                            break;
                        }
                    SetCost(temp.index, temp.real_cost);
                    if (Look_D.ContainsKey(temp.index))
                        Look_D[temp.index].Add(new Dict_data(temp.index_root, temp.real_cost));
                    else
                    {
                        Look_D.Add(temp.index, new List<Dict_data>());
                        Look_D[temp.index].Add(new Dict_data(temp.index_root, temp.real_cost));
                    }
                    last_ind = temp.index;
                    last_ind_root = temp.index_root;
                    Not_Look.Remove(temp);
                    
                }
                long[] index = new long[Look_D.Keys.Count];
                Look_D.Keys.CopyTo(index, 0);
                string path_id = landmark_index + "_";
                //long id = 0;
                Dictionary<long, string> paths = new Dictionary<long, string>();
                for (int i = 0; i < index.Length; i++)
                {
                    // List<Dict_data> somelist = Look_D[index[index.Length - 1]];
                    if (index[i] != landmark_index)
                    {
                        count++;
                        value = "\t< " + index[i] + " / " + Look_D[index[i]][Dict_data.GetMin(Look_D[index[i]])].index_root
                             + " >" + Environment.NewLine;
                        //Устарело
                        /*paths.Add(index[i], path_id + id.ToString());
                        value = "\t< " + path_id + id.ToString() + " " + index[i] + " way=\"";
                        id++;
                        List<long> way = new List<long>();
                        way.Add(index[i]);
                        while (way.Last() != landmark_index)
                        {
                            long next_index = Look_D[way.Last()][Dict_data.GetMin(Look_D[way.Last()])].index_root;
                            if (paths.ContainsKey(next_index))
                            {
                                value += paths[next_index] + ",";
                                break;
                            }
                            else
                                way.Add(next_index);
                        }
                        way.Reverse();
                        foreach (long ind in way)
                            value += ind + ",";
                        value.TrimEnd(',');
                        value += "\" >" + Environment.NewLine;
                         */
                        writer.Write(value);
                    }
                }
                //writer.Write(Environment.NewLine+"Кол-во вершин"+count);
            }
            fs.Dispose();
            return count;
        }
        /// <summary>
        /// Расчет маршрутов для маяков
        /// </summary>
        /// <param name="file_name">Файл для маршрутов из маяков</param>
        /// <param name="distans">Максимальное расстояние от маяка</param>
        /// <param name="timefile_fullpatch_name">Файл для сохранения логов</param>
        public void SetLandmarksData(string file_name, double distans = -1, string timefile_fullpatch_name = "")
        {
            foreach (long l_i in landmarks)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                long count = FullInfo(l_i, file_name, distans);
                stopwatch.Stop();
                TimeSpan ts = stopwatch.Elapsed;
                if (timefile_fullpatch_name != "")
                {
                    FileStream fs = new FileStream(timefile_fullpatch_name + "_TIME_" + l_i + ".txt", FileMode.OpenOrCreate, FileAccess.Write);
                    using (StreamWriter writer = new StreamWriter(fs))
                        writer.Write(ts.TotalMilliseconds.ToString());
                    fs = new FileStream(timefile_fullpatch_name + "_VERTEX_COUNT_" + l_i + ".txt", FileMode.OpenOrCreate, FileAccess.Write);
                    using (StreamWriter writer = new StreamWriter(fs))
                        writer.Write(count.ToString());
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file_name"></param>
        public void LoadALTData(string file_name)
        {
            using (StreamReader sr = new StreamReader(file_name))
            {
                long index = -1;
                while (!sr.EndOfStream)
                {
                    string[] data_line = sr.ReadLine().Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach(var data in data_line)
                    {
                        if (data.Contains("landmar"))
                            index = Convert.ToInt64(data.Split('=')[1]);
                        else
                        {
                            if (index == -1)
                                throw new Exception("Landmark not initiate" + data);
                            if(data.Contains('/'))
                                GetLandmarksPath(index, data);
                        }
                        /*
                        for (int i = 0; i < data.Length; i++)
                            landmarks.Add(Convert.ToInt64(data[i]));
                            */
                    }
                }
            }
        }
    }
}
