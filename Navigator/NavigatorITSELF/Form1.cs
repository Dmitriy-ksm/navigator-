using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using NavigationDLL;
using System.Diagnostics;
using MapRenderDLL;
using NavigationDLLver2;
using System.Globalization;

namespace Navigator
{
    public partial class Form1 : Form
    {
        #region A* 
        //Граф в памяти
        NavigationDLL.Graf tree;
        //Колекции точек что принадлежат выбраному пути
        List<long> way, way_2;
        //Колекции с точками что используются в процессе поиска пути
        List<SavedData> Look, Not_Look, Look_2, Not_Look_2;
        //Начать двупоточный поиск пути
        private void StartSearch(object sender, EventArgs e)
        {
            if (start != 0 && end != 0)
            {
                ButtonOff(button_start_search, button_first_point, button_last_point);
                ButtonOn(button_stop_search, button_update_status_search);
                textBox_info.Text = "Начат быстрый поиск пути";
                map.Drop();
                way = new List<long>();
                way_2 = new List<long>();
                Look_2 = new List<SavedData>();
                Look = new List<SavedData>();
                Not_Look_2 = new List<SavedData>();
                Not_Look = new List<SavedData>();
                if (backgroundWorker1.IsBusy != true)
                    backgroundWorker1.RunWorkerAsync(1);
            }
            else
            {
                textBox_info.Text = "Не заданы точки: ";
                if (start == 0)
                    textBox_info.Text += "Начала ";
                if (end == 0)
                    textBox_info.Text += "Конца";
            }
        }
        //Загрузка карты
        private void OpenNavData(object sender, EventArgs e)
        {
            pictureBox_navmap.Width = this.Width - 230;
            pictureBox_navmap.Height = this.Height - 20;
            ButtonOff(button_load_navdata, button_start_search, button_first_point,
                button_prev_map, button_next_map, button_last_point,
                button_update_status_search, button_stop_search);
            if (backgroundWorker1.IsBusy)
                backgroundWorker1.CancelAsync();
            if (backgroundWorker2.IsBusy)
                backgroundWorker2.CancelAsync();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog1.Filter = "my format (*.GNF.xml)|*.GNF.xml";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                stopwatch.Restart();
                textBox_info.Text = "Строим граф";
                file_name = openFileDialog1.FileName;
                if (backgroundWorker1.IsBusy != true)
                    backgroundWorker1.RunWorkerAsync(0);
                start = 0;
                end = 0;
                PTS_point = 0;
            }
            else
            {
                if (map != null)
                {
                    if (map.left_up.Length == 1)
                    {
                        ButtonOff(button_prev_map, button_next_map);
                    }
                    else
                    {
                        ButtonOn(button_prev_map, button_next_map);
                    }
                    ButtonOn(button_start_search, button_first_point, button_last_point);
                    ButtonOff(button_update_status_search, button_stop_search);
                }
                ButtonOn(button_load_navdata);
            }
        }
        //BackgroundWorker Однопоточный поиск пути 
        private void OneWaySearchBackgroundThread(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-UA");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-UA");
            tree.Refresh_Search();
            dist = 0;
            way = new List<long>();
            way_2 = new List<long>();
            Look_2 = new List<SavedData>();
            Look = new List<SavedData>();
            Not_Look_2 = new List<SavedData>();
            Not_Look = new List<SavedData>();
            bonus_thread = new Thread(() =>
            {
                way = tree.getWay_A(start, end, out dist, ref Not_Look, ref Look);
            });
            stopwatch.Restart();
            bonus_thread.Start();
            while (bonus_thread.IsAlive)
            {
                if (backgroundWorker2.CancellationPending)
                {
                    bonus_thread.Abort();
                    stopwatch.Stop();
                    break;
                }
            }
            //bonus_thread.Join();
            stopwatch.Stop();
        }
        //Остановка поиска
        private void StopSearch(object sender, EventArgs e)
        {
            map.Drop();
            ButtonOn(button_start_search, button_first_point, button_last_point);
            ButtonOff(button_update_status_search, button_stop_search);
            if (backgroundWorker1.IsBusy)
                backgroundWorker1.CancelAsync();
            if (backgroundWorker2.IsBusy)
                backgroundWorker2.CancelAsync();
        }
        //Обновление на карте результатов поиска 
        private void UpdateSearchStatus(object sender, EventArgs e)
        {
            stopwatch.Stop();
            tree.blocker = true;
            while (!tree.block_suc)
            {
                Thread.Sleep(10);
                break;
            }
            if (!tree.block_suc)
            {
                textBox_info.Text += Environment.NewLine + "Не удалось заблокировать поток,подождите и повторите запрос";
                return;
            }
            map.Drop();
            if (Look.Count > 0)
                map.getWay(tree, Look, Not_Look, map_index);
            if (Look_2.Count > 0)
                map.getWay(tree, Look_2, Not_Look_2, map_index);
            pictureBox_navmap.Image = map.temp_map;
            tree.block_suc = false;
            tree.blocker = false;
            stopwatch.Start();
        }
        //BackgroundWorker двупоточного поиска пути и открытия файла
        private void OpenFileOrTwoWaySearchBackgroubdThread(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-UA");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-UA");
            int count_proc = (int)e.Argument;
            switch (count_proc)
            {
                //Поиск пути 
                case 1:
                    {
                        map.Drop();
                        pictureBox_navmap.Image = map.temp_map;
                        tree.Refresh_Search();
                        dist = 0;
                        dist_2 = 0;
                        bonus_thread = new Thread(() =>
                        {
                            way = tree.getWay_A(start, end, out dist, ref Not_Look, ref Look);
                        });
                        bonus_thread_2 = new Thread(() =>
                        {
                            way_2 = tree.getWay_A(end, start, out dist_2, ref Not_Look_2, ref Look_2, true);
                        });
                        stopwatch.Restart();
                        bonus_thread.Start();
                        bonus_thread_2.Start();
                        while (bonus_thread.IsAlive)
                        {
                            //Остановка поиска пути
                            if (backgroundWorker1.CancellationPending)
                            {
                                bonus_thread.Abort();
                                if (bonus_thread_2.IsAlive)
                                    bonus_thread_2.Abort();
                                stopwatch.Stop();
                                break;
                            }
                        }
                        //bonus_thread.Join();
                        //bonus_thread_2.Join();
                        stopwatch.Stop();
                        backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(TwoWaySearchCompleted);
                        break;
                    }
                //Загрузка карты
                default:
                    {
                        using (StreamReader myStream = new StreamReader(file_name))
                        {
                            tree = new NavigationDLL.Graf(myStream.ReadLine().Split(' ')[1].Split('"')[1]);
                            while (!myStream.EndOfStream)
                            {
                                string data = myStream.ReadLine().Split(' ')[1].Split('"')[1];
                                tree.AddNode(new GrafNode(data, '/', ';'));
                            }
                        }
                        double min_lat = double.MaxValue;
                        double min_lon = double.MaxValue;
                        double max_lat = double.MinValue;
                        double max_lon = double.MinValue;
                        foreach (var item in tree.tree)
                        {
                            if (min_lat > item.Value.parametrs.coords.latitude)
                                min_lat = item.Value.parametrs.coords.latitude;
                            if (min_lon > item.Value.parametrs.coords.longitude)
                                min_lon = item.Value.parametrs.coords.longitude;
                            if (max_lat < item.Value.parametrs.coords.latitude)
                                max_lat = item.Value.parametrs.coords.latitude;
                            if (max_lon < item.Value.parametrs.coords.longitude)
                                max_lon = item.Value.parametrs.coords.longitude;
                        }
                        NavigationDLL.Coords l_u = new NavigationDLL.Coords(max_lat, min_lon);
                        NavigationDLL.Coords r_d = new NavigationDLL.Coords(min_lat, max_lon);
                        //stopwatch.Stop();
                        //ShowMyDialogBox(l_u, r_d);
                        //stopwatch.Start();
                        map = new NavMap(pictureBox_navmap.Width, pictureBox_navmap.Height, l_u, r_d);
                        map_index = 0;
                        map.SetAllPoint(tree, map_index);
                        map.getLine(tree, map_index);
                        map.setBackColor(map_index, Color.Olive);
                        map.Save();
                        backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(FileOpenCompleted);
                        break;
                    }
            }
        }
        //Вывод результатов поиска пути (однопоточный режим)
        private void OneWaySearchCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            map.Drop();
            TimeView();
            if (Look.Count > 0)
                map.getWay(tree, Look, Not_Look, map_index);
            if (way.Count > 1)
                map.getWay(tree, way, map_index, Brushes.Red);
            pictureBox_navmap.Image = map.temp_map;
            //textBox3.Text = dist + " m";
            ButtonOn(button_start_search, button_first_point, button_last_point);
            ButtonOff(button_stop_search, button_update_status_search);
        }
        //Вывод результатов поиска пути (многопоточный режим)
        private void TwoWaySearchCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            double some_dist = dist + dist_2;
            if (Look.Count > 0)
                map.getWay(tree, Look, Not_Look, map_index);
            if (Look_2.Count > 0)
                map.getWay(tree, Look_2, Not_Look_2, map_index);
            if (way.Count > 1)
                map.getWay(tree, way, map_index, Brushes.Red);
            if (way_2.Count > 1)
                map.getWay(tree, way_2, map_index, Brushes.Red);
            pictureBox_navmap.Image = map.temp_map;
            //textBox3.Text = dist + dist_2 + " m";
            TimeView();
            backgroundWorker1.RunWorkerCompleted -= TwoWaySearchCompleted;
            ShowMyDialogBox();
        }
        //Отображение загруженной карты
        private void FileOpenCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TimeView();
            pictureBox_navmap.Image = map.temp_map;
            textBox_info.Text += "Кол-во карт " + map.left_up.Length + Environment.NewLine;
            textBox_info.Text += "Левый верхний угол" + map.left_up[0].latitude.ToString() + map.left_up[0].longitude.ToString() + Environment.NewLine;
            textBox_info.Text += "Правый Нижний угол" + map.right_down[0].latitude.ToString() + map.right_down[0].longitude.ToString() + Environment.NewLine;
            if (map.left_up.Length == 1)
            {
                ButtonOff(button_prev_map, button_next_map);
            }
            else
            {
                ButtonOn(button_prev_map, button_next_map);
            }
            ButtonOn(button_start_search, button_first_point, button_last_point, button_load_navdata);
            backgroundWorker1.RunWorkerCompleted -= FileOpenCompleted;
        }
        //Однопоточный поиск пути
        public void ShowMyDialogBox()
        {
            DialogResult res_2 = MessageBox.Show("Start slow search?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (res_2 == DialogResult.OK)
            {
                ButtonOff(button_start_search, button_first_point, button_last_point);
                ButtonOn(button_update_status_search, button_stop_search);
                backgroundWorker2.RunWorkerAsync();
            }
            if (res_2 == DialogResult.Cancel)
            {
                ButtonOff(button_update_status_search, button_stop_search);
                ButtonOn(button_start_search, button_first_point, button_last_point);
            }
        }
        #endregion
        #region ALT
        //ALT
        ALT.ALT testClass = new ALT.ALT();
        //Загрузка маяков и данных препроцессинга
        private void button_alt_load_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog1.Filter = "ALT data (*.txt)|*.txt";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ButtonOff(button_ALT_Search);
                testClass.CopyGraf(tree);
                // ways = new Dictionary<long, List<long>>();
                file_name = openFileDialog1.FileName;
                testClass.LoadLandmarks(file_name);
                //testClass.LoadALTData(file_name);
            }
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            FBD.RootFolder = Environment.SpecialFolder.Desktop;
            if (FBD.ShowDialog() == DialogResult.OK)
                backgroundWorker3.RunWorkerAsync(FBD.SelectedPath);
        }
        //Построение пути по данным препроцессинга
        private void button_ALT_Search_Click(object sender, EventArgs e)
        {
            map.Drop();
            stopwatch.Restart();
            bool flag;
            List<long> path = testClass.GetWay(end, start, out flag);
            TimeView();
            textBox_info.Text += "ALT: " + flag.ToString();
            map.getWay(tree, path, map_index, Brushes.Red);
            pictureBox_navmap.Image = map.temp_map;
            /* if(ways.ContainsKey(end))
             {
                 map.Drop();
                 map.getWay(tree, ways[end], map_index, Brushes.Red);
                 pictureBox_navmap.Image = map.temp_map;
             }*/
        }
        //Загрузка путей после препроцессинга 
        private void LoadALTPaths(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-UA");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-UA");
            string dir = (string)e.Argument;
            stopwatch.Restart();
            foreach (string file in Directory.EnumerateFiles(dir, "*.txt"))
            {
                testClass.LoadALTData(file);
            }    
        }
        private void LoadALTPathsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TimeView();
            ButtonOn(button_ALT_Search);
        }
        #endregion
        #region Map
        //Карта на форме
        NavMap map;
        //индекс куска карты и индекс указатель
        //(1-точка начала пути, 2- точка конца пути, 3- точка остановки)
        int map_index, texboxindex;
        //Сами id точек соответственно
        long start, end, PTS_point;
        //Разбитие карты на подкарты 
        public void ShowMyDialogBox(NavigationDLL.Coords l_u, NavigationDLL.Coords r_d)
        {
            DialogResult res = MessageBox.Show("Map on one panel?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            stopwatch.Start();
            if (res == DialogResult.OK)
                map = new NavMap(pictureBox_navmap.Width, pictureBox_navmap.Height, l_u, r_d);
            if (res == DialogResult.Cancel)
                map = new NavMap(l_u, r_d, 0.0001);
        }
        //Получение индекса ближайшей точки на карте
        private void GetIndexFromMap(object sender, MouseEventArgs e)
        {
            Point start_road;
            Point end_road;
            //Поиск ближайшей точки 
            map.SearchRoad(e.X, e.Y, out start_road, out end_road);
            //Первая ближайшая точка 
            if (map.points_on_map.ContainsKey(start_road))
            {
                if (texboxindex == 1)
                    start = map.points_on_map[start_road][0];
                if (texboxindex == 2)
                    end = map.points_on_map[start_road][0];
                if (texboxindex == 3)
                    PTS_point = map.points_on_map[start_road][0];

            }
            //Вторая ближайшая точка
            if (map.points_on_map.ContainsKey(end_road))
            {
                if (texboxindex == 1)
                    start = map.points_on_map[end_road][0];
                if (texboxindex == 2)
                    end = map.points_on_map[end_road][0];
                if (texboxindex == 3)
                    PTS_point = map.points_on_map[end_road][0];
            }
            pictureBox_navmap.Image = map.temp_map;
            pictureBox_navmap.Enabled = false;
            if (start != 0)
                textBox_info.Text = "Начало пути выбрано";
            if (end != 0)
                textBox_info.Text = "Конец пути выбран";
            if (start != 0 && end != 0)
                textBox_info.Text = "Точки пути выбраны";
            if (PTS_point != 0)
                textBox_info.Text = "Точка остановки выбрана";
        }
        //Переход на предыдущий кусок карты
        private void PrevMap(object sender, EventArgs e)
        {
            stopwatch.Restart();
            if (map_index > 0)
            {
                map.FullClear();
                map_index--;
                map.points_on_map = new Dictionary<Point, List<long>>();
                map.SetAllPoint(tree, map_index);
                map.getLine(tree, map_index);
                map.setBackColor(map_index, Color.Olive);
                map.Save();
            }
            pictureBox_navmap.Image = map.temp_map;
            TimeView();
            textBox_info.Text += "Кол-во карт " + map.left_up.Length;
        }
        //Переход на следующий кусок карты
        private void NextMap(object sender, EventArgs e)
        {
            stopwatch.Restart();
            if (map_index < map.left_up.Length - 1)
            {
                map.FullClear();
                map_index++;
                map.points_on_map = new Dictionary<Point, List<long>>();
                map.SetAllPoint(tree, map_index);
                map.getLine(tree, map_index);
                map.setBackColor(map_index, Color.Olive);
                map.Save();
            }
            pictureBox_navmap.Image = map.temp_map;
            TimeView();
            textBox_info.Text += "Кол-во карт " + map.left_up.Length;
        }
        //Выбор точки начала пути
        private void FirstPoint(object sender, EventArgs e)
        {
            pictureBox_navmap.Enabled = true;
            texboxindex = 1;
        }
        //Выбор точки конца поиска пути
        private void LastPoint(object sender, EventArgs e)
        {
            pictureBox_navmap.Enabled = true;
            texboxindex = 2;
        }
        #endregion
        #region Работа с общественным транспортом 
        //Колекция данных об общественном транспорте
        Dictionary<long, List<PublicTransportPath>> path;
        //Строка с данными о точках остановки
        string PTS_data;
        //Загрузка формы для создания точки остановки
        private void button_form_save_pts_Click(object sender, EventArgs e)
        {
            PublicTransportStopManager.PTS(PTS_point, this);
        }
        //Запись из статического класса данных об остановке
        private void button_save_pts_Click(object sender, EventArgs e)
        {
            PublicTransportStopManager.Res(ref PTS_data);
            textBox_info.Text = PTS_data;
        }
        //Сохранение данных об остановках в файл
        private void button_create_ptsfile_Click(object sender, EventArgs e)
        {
            if (saveFileDialog_PTS.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = saveFileDialog_PTS.FileName;
            System.IO.File.WriteAllText(filename, PTS_data);
        }
        private void PTSSearch_Click(object sender, EventArgs e)
        {
            //Путь к остановкам
            map.Drop();
            double min = double.MaxValue;
            long closestLandmark = -1;
            foreach (var landmarks in testClass.Landmarks.Keys)
            {
                double dist_h = tree[start].calculationCost(tree[landmarks].parametrs);
                if (dist_h < min)
                {
                    closestLandmark = landmarks;
                    min = dist_h;
                }
            }
            bool altused;
            List<long> path_1 = testClass.GetWay(closestLandmark, start, out altused);
            min = double.MaxValue;
            closestLandmark = -1;
            foreach (var landmarks in testClass.Landmarks.Keys)
            {
                double dist_h = tree[end].calculationCost(tree[landmarks].parametrs);
                if (dist_h < min)
                {
                    closestLandmark = landmarks;
                    min = dist_h;
                }
            }
            bool altused_2;
            List<long> path_2 = testClass.GetWay(closestLandmark, end, out altused_2);
            map.getWay(tree, path_1, map_index, Brushes.Red);
            map.getWay(tree, path_2, map_index, Brushes.Red);
            //Выбор транспорта
            //Не сделаль
            //КАКАЯТОФУНКЦИЯ(path_1[0],path_2[0]);
        }
        //Загрузка остановок
        private void PTSLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog1.Filter = "PTS data (*.xml.PTS)|*.xml.PTS";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ButtonOff(PTSSearch);
                using (StreamReader sr = new StreamReader(openFileDialog1.FileName))
                    while (!sr.EndOfStream)
                    {
                        Dictionary<long, List<PublicTransport>> PTS = new Dictionary<long, List<PublicTransport>>();
                        string[] data_all = sr.ReadLine().Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var data in data_all)
                            if (data.Contains("avarage_time"))
                            {
                                long transport_number = Convert.ToInt64(data.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                                if (!PTS.ContainsKey(transport_number))
                                    PTS.Add(transport_number, new List<PublicTransport>());
                                PTS[transport_number].Add(new PublicTransport(data));
                            }
                        //тест
                        Color[] colors = new Color[PTS.Keys.Count];
                        int size = 12;
                        for (int i=0;i<colors.Length;i++)
                        {
                            colors[i] = Color.FromArgb(255, new Random().Next(0, 255),
                                new Random().Next(0, 255), new Random().Next(0, 255));
                            size--;
                            List<long> points = new List<long>();
                            foreach (var pts in PTS[PTS.Keys.ToArray()[i]])
                                points.Add(pts.point_id);
                            map.FlagThePoint(colors[i], size, points.ToArray());
                            pictureBox_navmap.Image = map.temp_map;
                        }
                        //тест окончен
                        foreach (var pts_transp in PTS)
                        {
                            PublicTransport[] pts = new PublicTransport[pts_transp.Value.Count];
                            foreach (var item in pts_transp.Value)
                            {
                                pts[item.count_stop - 1] = item;
                            }

                            for (int i = 0; i < pts.Length - 1; i++)
                            {
                                long id = pts[i].point_id;
                                if (!path.ContainsKey(id))
                                    path.Add(id, new List<PublicTransportPath>());
                                else
                                {
                                    PublicTransportPath[] temp = new PublicTransportPath[path[id].Count];
                                    path[id].CopyTo(temp);
                                    foreach (var item in temp)
                                    {
                                        path[id].Add(new PublicTransportPath(item.parent_tp, pts[i]));
                                    }
                                }
                                path[id].Add(new PublicTransportPath(pts[i], pts[i + 1]));
                            }
                            long id_ = pts[pts.Length - 1].point_id;
                            if (!path.ContainsKey(id_))
                                path.Add(id_, new List<PublicTransportPath>());
                            else
                            {
                                List<PublicTransportPath> temp = new List<PublicTransportPath>();
                                foreach (var item in path[id_])
                                    temp.Add(item);
                                foreach( var item in temp)
                                    path[id_].Add(new PublicTransportPath(item.parent_tp, pts[pts.Length - 1]));
                            }
                            path[id_].Add(new PublicTransportPath(pts[pts.Length - 1]));
                        }
                    }
                ButtonOn(PTSSearch);
            }
        }
        //Выбор точки с остановкой
        private void button_pts_add_Click(object sender, EventArgs e)
        {
            pictureBox_navmap.Enabled = true;
            texboxindex = 3;
        }
        #endregion
        //Потоки для BackgroundWorkera
        Thread bonus_thread, bonus_thread_2;
        //Счетчик для оценки производительности
        Stopwatch stopwatch;
        //Растоянния между точкой начала и конца
        double dist, dist_2;
        
        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LandmarkFlag_Click(object sender, EventArgs e)
        {
            SearchLandmark();
        }

        //Имя файла
        string file_name;
        private void ButtonOff(params Button[] buttons)
        {
            foreach (var button in buttons)
                button.Enabled = false;
        }
        private void ButtonOn(params Button[] buttons)
        {
            foreach (var button in buttons)
                button.Enabled = true;
        }
        private void TimeView()
        {
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            textBox_info.Text = "RunTime " + elapsedTime + Environment.NewLine;
        }
        public Form1()
        {

            CultureInfo culture = new CultureInfo("ru-UA");

            InitializeComponent();
            //pictureBox_navmap.PreferredSize = new Size(0, 0);
            //pictureBox_navmap.Enabled = true;
            //pictureBox_navmap.Enabled = false;
            path = new Dictionary<long, List<PublicTransportPath>>();
            stopwatch = new Stopwatch();
        }
        private void StopSelectTextBoxes(object sender, MouseEventArgs e)
        {
            ActiveControl = button_load_navdata;
        }

        private void SearchLandmark()
        {
            map.FlagThePoint(Color.Blue,5,testClass.Landmarks.Keys.ToArray());
            pictureBox_navmap.Image = map.temp_map;
        }
    }
}
