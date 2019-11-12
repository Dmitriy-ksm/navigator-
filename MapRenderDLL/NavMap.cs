using System;
using System.Collections.Generic;
using System.Drawing;
using NavigationDLL;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapRenderDLL
{
    public class NavMap : IDisposable
    {
        bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                clear_map.Dispose();
                temp_map.Dispose();
                // Free any other managed objects here.
                //
            }
            points_on_map.Clear();
            points_on_map = null;
            left_up = null;
            right_down = null;
            // Free any unmanaged objects here.
            //
            disposed = true;
        }
        //Сохранненная карта
        Bitmap clear_map;
        //Текущая карта
        public Bitmap temp_map;
        //Колекция точек с их координатами
        public Dictionary<Point, List<long>> points_on_map = new Dictionary<Point, List<long>>();
        //КОординаты левого верхнего угла и правого нижнего
        public Coords[] left_up, right_down;
        //Коэфициент масштаба карты
        Coords decByPixel;
        /// <summary>
        /// Разбитие карты на куски
        /// </summary>
        /// <param name="left_up">Левый верхний</param>
        /// <param name="right_down">Правый нижний</param>
        /// <param name="decByPixel">Масштаб</param>
        public NavMap(Coords left_up, Coords right_down, double decByPixel)
        {
            //this.left_up = left_up;
            //this.right_down = right_down;
            this.decByPixel.latitude = -decByPixel;
            this.decByPixel.longitude = decByPixel;
            int WidthMax = 650;
            int HeightMax = 450;
            int TotalHeight = (int)((right_down.latitude - left_up.latitude) / this.decByPixel.latitude) + 1;
            int TotalWidth = (int)((right_down.longitude - left_up.longitude) / this.decByPixel.longitude) + 1;
            double latForMap = HeightMax * this.decByPixel.latitude;
            double longForMap = WidthMax * this.decByPixel.longitude;
            double w = (double)TotalWidth / WidthMax;
            double h = (double)TotalHeight / HeightMax;
            double widthCount = w > Math.Floor(w) ? Math.Floor(w) + 1 : Math.Floor(w);
            double heightCount = w > Math.Floor(h) ? Math.Floor(h) + 1 : Math.Floor(h);
            int countOfMap = (int)widthCount * (int)heightCount;
            clear_map = new Bitmap(650, 450);
            temp_map = new Bitmap(clear_map);
            this.left_up = new Coords[countOfMap];
            this.right_down = new Coords[countOfMap];
            Coords left_position_first = left_up;
            Coords left_position_second;
            Coords right_position;
            int count = 0;
            for (int i = 0; i < (int)widthCount; i++)
            {
                this.left_up[count] = left_position_first;
                left_position_second = left_position_first;
                right_position = new Coords(left_position_first.latitude + latForMap, left_position_first.longitude + longForMap);
                this.right_down[count] = right_position;
                count++;
                for (int j = 1; j < (int)heightCount; j++)
                {
                    left_position_second.latitude += latForMap;
                    right_position = new Coords(left_position_second.latitude + latForMap, left_position_second.longitude + longForMap);
                    this.left_up[count] = left_position_second;
                    this.right_down[count] = right_position;
                    count++;
                }
                left_position_first.longitude += longForMap;
            }
        }
        /// <summary>
        /// Создание карты заданого размера
        /// </summary>
        /// <param name="Width">ширина</param>
        /// <param name="Height">высота</param>
        /// <param name="left_up">левый верхний</param>
        /// <param name="right_down">правый нижний</param>
        public NavMap(int Width, int Height, Coords left_up, Coords right_down)
        {
            clear_map = new Bitmap(Width, Height);
            temp_map = new Bitmap(clear_map);
            this.left_up = new Coords[1] { left_up };
            this.right_down = new Coords[1] { right_down };
            decByPixel.latitude = (right_down.latitude - left_up.latitude) / clear_map.Height;
            decByPixel.longitude = (right_down.longitude - left_up.longitude) / clear_map.Width;
        }
        /// <summary>
        /// Получение точки по координатам
        /// </summary>
        /// <param name="point">Координаты</param>
        /// <param name="index">Индекс карты</param>
        /// <returns>Координаты точки</returns>
        public Point getPoint(Coords point, int index)
        {
            Point return_value = new Point();
            return_value.X = (int)((point.longitude - left_up[index].longitude) / decByPixel.longitude);
            return_value.Y = (int)((point.latitude - left_up[index].latitude) / decByPixel.latitude);
            return return_value;
        }
        /// <summary>
        /// Добавление точки на карту
        /// </summary>
        /// <param name="set_point">Координаты точки</param>
        /// <param name="index_map">Индекс куска карты</param>
        /// <param name="index">Индекс точки</param>
        public void SetPointOnMap(Coords set_point, int index_map, long index)
        {
            if (set_point.latitude < left_up[index_map].latitude && set_point.latitude > right_down[index_map].latitude)
                if (set_point.longitude > left_up[index_map].longitude && set_point.longitude < right_down[index_map].longitude)
                {
                    int x = (int)((set_point.longitude - left_up[index_map].longitude) / decByPixel.longitude);
                    int y = (int)((set_point.latitude - left_up[index_map].latitude) / decByPixel.latitude);
                    Point p = new Point(x, y);
                    temp_map.SetPixel(x, y, Color.Black);
                    if (points_on_map.ContainsKey(p))
                        points_on_map[p].Add(index);
                    else
                        points_on_map.Add(p, new List<long> { index });
                }
        }
        /// <summary>
        /// Добавление точки на карту
        /// </summary>
        /// <param name="p">Точка которую нужно добавить</param>
        /// <param name="color">Цвет точки</param>
        public void SetPointOnMap(Point p, Color color)
        {
            temp_map.SetPixel(p.X, p.Y, color);
        }
        /// <summary>
        /// Восстановление сохранненной карты
        /// </summary>
        public void Drop()
        {
            temp_map = new Bitmap(clear_map);
        }
        /// <summary>
        /// Очистка сохранненной карты
        /// </summary>
        public void FullClear()
        {
            clear_map = new Bitmap(650, 450);
            temp_map = clear_map;
        }
        /// <summary>
        /// Сохраненние карты
        /// </summary>
        public void Save()
        {
            clear_map = new Bitmap(temp_map);
        }
        /// <summary>
        /// Соединение точек линиями 
        /// </summary>
        /// <param name="tree">Граф</param>
        /// <param name="index">Индекс карты</param>
        public void getLine(Graf tree, int index)
        {
            foreach (var item in tree.tree)
            {
                using (var g = Graphics.FromImage(temp_map))
                {
                    foreach (var ind_broth in item.Value.broth_node)
                        g.DrawLine(new Pen(Brushes.Black, 1), getPoint(item.Value.parametrs.coords, index), getPoint(tree[ind_broth].parametrs.coords, index));
                }
            }
        }
        /// <summary>
        /// Соединение точек линиями
        /// </summary>
        /// <param name="tree">Граф</param>
        /// <param name="indexes">Индексы соседей</param>
        /// <param name="index">Индекс карты</param>
        /// <param name="color">Цвет линии</param>
        public void getWay(Graf tree, List<long> indexes, int index, Brush color)
        {
            using (var g = Graphics.FromImage(temp_map))
            {
                Point first_point = getPoint(tree[indexes[0]].parametrs.coords, index);
                Point second_point;
                for (int i = 1; i < indexes.Count; i++)
                {
                    second_point = getPoint(tree[indexes[i]].parametrs.coords, index);
                    g.DrawLine(new Pen(color, 1), first_point, second_point);
                    first_point = second_point;
                }
            }
        }
        /// <summary>
        /// Соединение точек линиями
        /// </summary>
        /// <param name="tree">Граф</param>
        /// <param name="Look">Массив рассмотренных точек</param>
        /// <param name="Not_Look">Массив потенциальных точек</param>
        /// <param name="index">Индекс карты</param>
        public void getWay(Graf tree, List<SavedData> Look, List<SavedData> Not_Look, int index)
        {
            using (var g = Graphics.FromImage(temp_map))
            {
                int Look_counter = Look.Count;
                int Not_Look_counter = Not_Look.Count - 1;
                for (int i = 0; i < Not_Look_counter; i++)
                {
                    Point first_point = getPoint(tree[Not_Look[i].index_root].parametrs.coords, index);
                    Point second_point = getPoint(tree[Not_Look[i].index].parametrs.coords, index);
                    g.DrawLine(new Pen(Brushes.Blue, 1), first_point, second_point);
                }
                for (int i = 0; i < Look_counter; i++)
                {
                    Point first_point = getPoint(tree[Look[i].index_root].parametrs.coords, index);
                    Point second_point = getPoint(tree[Look[i].index].parametrs.coords, index);
                    g.DrawLine(new Pen(Brushes.Green, 1), first_point, second_point);
                }
            }
        }
        /// <summary>
        /// Установка точек 
        /// </summary>
        /// <param name="tree">Граф</param>
        /// <param name="map_index">Индекс карты</param>
        public void SetAllPoint(Graf tree, int map_index)
        {
            foreach (var item in tree.tree)
            {
                double lat = item.Value.parametrs.coords.latitude;
                double lon = item.Value.parametrs.coords.longitude;
                SetPointOnMap(new Coords(lat, lon), map_index, item.Key);

            }
        }
        /// <summary>
        /// Установка фона карты
        /// </summary>
        /// <param name="map_index">Индекс карты</param>
        /// <param name="color">Цвет фона</param>
        public void setBackColor(int map_index, Color color)
        {
            for (int i = 0; i < temp_map.Width; i++)
            {
                for (int j = 0; j < temp_map.Height; j++)
                {
                    if (temp_map.GetPixel(i, j).Name != "ff000000")
                        temp_map.SetPixel(i, j, color);
                }
            }
        }
        /// <summary>
        /// Нахождение концов линии
        /// </summary>
        /// <param name="p">Точка на дороге</param>
        /// <param name="second">Точка конца дороги</param>
        /// <returns></returns>
        Point GetNodeOnRoad(Point p, out Point second)
        {
            Point return_value = new Point();
            second = new Point();
            Point temp_point = p;
            for (int i = temp_point.X; i < temp_map.Width; i++)
            {
                temp_point = new Point(i, temp_point.Y);
                //Правая часть карты
                if (temp_map.GetPixel(i, temp_point.Y).Name == "ff000000")
                {
                    SetPointOnMap(temp_point, Color.Purple);
                    if (points_on_map.ContainsKey(temp_point))
                    {
                        return_value = temp_point;
                        break;
                    }
                }
                else
                {
                    //Нижняя часть
                    temp_point.Y += 1;
                    if (temp_point.Y > temp_map.Height)
                        temp_point.Y = temp_map.Height;
                    if (temp_map.GetPixel(i, temp_point.Y).Name == "ff000000")
                    {
                        SetPointOnMap(temp_point, Color.Purple);
                        if (points_on_map.ContainsKey(temp_point))
                        {
                            return_value = temp_point;
                            break;
                        }
                    }
                    else
                    {
                        //Верхняя часть
                        temp_point.Y -= 2;
                        if (temp_point.Y < 0)
                            temp_point.Y = 0;
                        if (temp_map.GetPixel(i, temp_point.Y).Name == "ff000000")
                        {
                            SetPointOnMap(temp_point, Color.Purple);
                            if (points_on_map.ContainsKey(temp_point))
                            {
                                return_value = temp_point;
                                break;
                            }
                        }
                    }
                }
            }
            temp_point = new Point(p.X, p.Y);
            SetPointOnMap(temp_point, Color.Black);
            for (int i = p.X; i > 0; i--)
            {
                temp_point = new Point(i, temp_point.Y);
                //Левая часть карты
                if (temp_map.GetPixel(i, temp_point.Y).Name == "ff000000")
                {
                    SetPointOnMap(temp_point, Color.Purple);
                    if (points_on_map.ContainsKey(temp_point))
                    {
                        second = temp_point;
                        break;
                    }
                }
                else
                {
                    //Нижняя часть
                    temp_point.Y += 1;
                    if (temp_point.Y > temp_map.Height)
                        temp_point.Y = temp_map.Height;
                    if (temp_map.GetPixel(i, temp_point.Y).Name == "ff000000")
                    {
                        SetPointOnMap(temp_point, Color.Purple);
                        if (points_on_map.ContainsKey(temp_point))
                        {
                            second = temp_point;
                            break;
                        }
                    }
                    else
                    {
                        //Верхняя часть
                        temp_point.Y -= 2;
                        if (temp_point.Y < 0)
                            temp_point.Y = 0;
                        if (temp_map.GetPixel(i, temp_point.Y).Name == "ff000000")
                        {
                            SetPointOnMap(temp_point, Color.Purple);
                            if (points_on_map.ContainsKey(temp_point))
                            {
                                second = temp_point;
                                break;
                            }
                        }
                    }
                }
            }
            return return_value;
        }
        /// <summary>
        /// Поиск ближайшей линии на карте
        /// </summary>
        /// <param name="x">Координата Х</param>
        /// <param name="y">Координата У</param>
        /// <param name="start_road">Первая точка на дороге</param>
        /// <param name="end_road">Вторая точка на дороге</param>
        /// <returns></returns>
        public bool SearchRoad(int x, int y, out Point start_road, out Point end_road)
        {
            start_road = new Point(x, y);
            end_road = new Point(x, y);
            Point p = new Point(x, y);
            Drop();
            bool flag = false;
            Point start = new Point(x, y);
            int left_x, right_x, up_y, down_y;
            if (temp_map.GetPixel(p.X, p.Y) != Color.Black)
            {
                SetPointOnMap(p, Color.YellowGreen);
                int count = 1;
                while (temp_map.GetPixel(p.X, p.Y).Name != "ff000000")
                {
                    if (count > 649)
                    {
                        return false;
                    }
                    p = start;
                    if (x + count < temp_map.Width)
                        right_x = count;
                    else
                        right_x = temp_map.Width - x - 1;
                    p.X = x + right_x;
                    p.Y = y;
                    if (temp_map.GetPixel(p.X, p.Y).Name == "ff000000")
                        break;
                    SetPointOnMap(p, Color.YellowGreen);
                    if (y + count < temp_map.Height)
                        down_y = count;
                    else
                        down_y = temp_map.Height - y - 1;
                    for (int j = 1; j <= down_y; j++)
                    {
                        p.Y = y + j;
                        if (temp_map.GetPixel(p.X, p.Y).Name == "ff000000")
                        {
                            flag = true;
                            break;
                        }
                        SetPointOnMap(p, Color.YellowGreen);
                    }
                    if (flag)
                        break;
                    if (y - count > 0)
                        up_y = count;
                    else
                        up_y = y;
                    for (int j = 1; j <= up_y; j++)
                    {
                        p.Y = y - j;
                        if (temp_map.GetPixel(p.X, p.Y).Name == "ff000000")
                        {
                            flag = true;
                            break;
                        }
                        SetPointOnMap(p, Color.YellowGreen);
                    }
                    if (flag)
                        break;
                    if (x - count > 0)
                        left_x = count;
                    else
                        left_x = x;
                    p.Y = y;
                    p.X = x - left_x;
                    if (temp_map.GetPixel(p.X, p.Y).Name == "ff000000")
                        break;
                    SetPointOnMap(p, Color.YellowGreen);
                    if (y + count < temp_map.Height)
                        down_y = count;
                    else
                        down_y = temp_map.Height - y - 1;
                    for (int j = 1; j <= down_y; j++)
                    {
                        p.Y = y + j;
                        if (temp_map.GetPixel(p.X, p.Y).Name == "ff000000")
                        {
                            flag = true;
                            break;
                        }
                        SetPointOnMap(p, Color.YellowGreen);
                    }
                    if (flag)
                        break;
                    if (y - count > 0)
                        up_y = count;
                    else
                        up_y = y;
                    for (int j = 1; j <= up_y; j++)
                    {
                        p.Y = y - j;
                        if (temp_map.GetPixel(p.X, p.Y).Name == "ff000000")
                        {
                            flag = true;
                            break;
                        }
                        SetPointOnMap(p, Color.YellowGreen);
                    }
                    if (flag)
                        break;
                    if (y + count < temp_map.Height)
                        down_y = count;
                    else
                        down_y = temp_map.Height - y - 1;
                    p.X = x;
                    p.Y = y + down_y;
                    if (x + count < temp_map.Width)
                        right_x = count;
                    else
                        right_x = temp_map.Width - x - 1;
                    for (int i = 0; i <= right_x; i++)
                    {
                        p.X = x + i;
                        if (temp_map.GetPixel(p.X, p.Y).Name == "ff000000")
                        {
                            flag = true;
                            break;
                        }
                        SetPointOnMap(p, Color.YellowGreen);
                    }
                    if (flag)
                        break;
                    if (x - count > 0)
                        left_x = count;
                    else
                        left_x = x;
                    for (int i = 0; i <= left_x; i++)
                    {
                        p.X = x - i;
                        if (temp_map.GetPixel(p.X, p.Y).Name == "ff000000")
                        {
                            flag = true;
                            break;
                        }
                        SetPointOnMap(p, Color.YellowGreen);
                    }
                    if (flag)
                        break;
                    if (y - count > 0)
                        up_y = count;
                    else
                        up_y = y;
                    p.X = x;
                    p.Y = y - up_y;
                    if (temp_map.GetPixel(p.X, p.Y).Name == "ff000000")
                        break;
                    SetPointOnMap(p, Color.YellowGreen);
                    if (x + count < temp_map.Width)
                        right_x = count;
                    else
                        right_x = temp_map.Width - x - 1;
                    for (int i = 0; i <= right_x; i++)
                    {
                        p.X = x + i;
                        if (temp_map.GetPixel(p.X, p.Y).Name == "ff000000")
                        {
                            flag = true;
                            break;
                        }
                        SetPointOnMap(p, Color.YellowGreen);
                    }
                    if (flag)
                        break;
                    if (x - count > 0)
                        left_x = count;
                    else
                        left_x = x;
                    for (int i = 0; i <= left_x; i++)
                    {
                        p.X = x - i;
                        if (temp_map.GetPixel(p.X, p.Y).Name == "ff000000")
                        {
                            flag = true;
                            break;
                        }
                        SetPointOnMap(p, Color.YellowGreen);
                    }
                    if (flag)
                        break;
                    count++;
                }
            }
            if (points_on_map.ContainsKey(p))
            {
                start_road = p;
                end_road = p;
            }
            else
            {
                start_road = GetNodeOnRoad(p, out end_road);
                if (end_road.X == 0 && end_road.Y == 0)
                    end_road = start_road;
            }
            return true;
        }

        #region New
        /// <summary>
        /// Поиск точки по индексу
        /// </summary>
        /// <param name="index">индекс точки</param>
        /// <returns></returns>
        public Point SearchForPoints(long index)
        {
            foreach (var item in points_on_map)
                foreach (var indexes in item.Value)
                    if (index == indexes)
                        return item.Key;
            throw new Exception("Точка не найдена");
        }
        /// <summary>
        /// Зарисовка области около точек
        /// </summary>
        /// <param name="indexes">коллекция точек</param>
        public void FlagThePoint(Color color, int size,params long[] indexes)
        {
            foreach(var index in indexes)
            {
                Point point_to_flaged = SearchForPoints(index);
                for(int x=-size; x< size+1; x++)
                {
                    for(int y= size; y>-size-1; y--)
                    {
                        temp_map.SetPixel(point_to_flaged.X+x, point_to_flaged.Y+y, color);
                    }
                }
                
            }
        }
        /// <summary>
        /// Соединение точек линиями
        /// </summary>
        /// <param name="tree">Граф</param>
        /// <param name="Look">Массив рассмотренных точек</param>
        /// <param name="Not_Look">Массив потенциальных точек</param>
        /// <param name="index">Индекс карты</param>
        public void getWay(Graf tree, Dictionary<long,List<long>> Look, Dictionary<long, List<long>> Not_Look, int index)
        {
            using (var g = Graphics.FromImage(temp_map))
            {
                foreach(var item in Not_Look)
                {
                    Point first_point = getPoint(tree[item.Key].parametrs.coords, index);
                    foreach(var item_val in item.Value)
                    { 
                        Point second_point = getPoint(tree[item_val].parametrs.coords, index);
                        g.DrawLine(new Pen(Brushes.Blue, 1), first_point, second_point);
                    }
                }
                foreach (var item in Look)
                {
                    Point first_point = getPoint(tree[item.Key].parametrs.coords, index);
                    foreach (var item_val in item.Value)
                    {
                        Point second_point = getPoint(tree[item_val].parametrs.coords, index);
                        g.DrawLine(new Pen(Brushes.Blue, 1), first_point, second_point);
                    }
                }
            }
        }
        #endregion
    }
}
