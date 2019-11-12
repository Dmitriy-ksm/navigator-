using System;
using System.Collections.Generic;
using System.Text;

namespace NavigationDLLver2
{
    public class Path_nav
    {
        public double temp_path_crossed = 0;
        public char flag;// Если flag = 'f' - вершину нашли со старта, если flag = 's' - вершину нашли с конца
        public GrafNodeParam_nav param;
        public Dictionary<long, double> path = new Dictionary<long, double>();
        /// <summary>
        /// Конструктор для создания путей
        /// </summary>
        /// <param name="data">Строка данных</param>
        /// <param name="data_separator">Разделитель данных</param>
        /// <param name="massive_element_separator">Разделитель элементов в массиве</param>
        public Path_nav(string data, char data_separator = '/', char massive_element_separator = ';')
        {
            string[] pars_data = data.Split(data_separator);
            string[] nodes = pars_data[1].Split(massive_element_separator);
            string[] nodes_distans = pars_data[2].Split(massive_element_separator);
            for (int i = 0; i < nodes.Length; i++)
            {
                long id = Convert.ToInt64(nodes[i]);
                double dist = Convert.ToDouble(nodes_distans[i]);
                if (!path.ContainsKey(id))
                    path.Add(id, dist);
                else
                {
                    if (dist < path[id])
                        path[id] = dist;
                }
            }
            string[] pars_params = pars_data[pars_data.Length - 1].Split(massive_element_separator);
            param = new GrafNodeParam_nav(Convert.ToDouble(pars_params[0]), Convert.ToDouble(pars_params[1]), Convert.ToDouble(pars_params[2]));
        }
        public double this[long index]
        {
            get
            {
                return path[index];
            }
        }
        /// <summary>
        /// Расчет растояния между родительской точкой и точкой назначения 
        /// </summary>
        /// <param name="dest_node">Координаты родителя</param>
        /// <param name="dest_node">Координаты назначения</param>
        /// <returns>Возвращает длину в метрах</returns>
        public static double calculationCost(GrafNodeParam_nav source_node, GrafNodeParam_nav dest_node)
        {
            double return_value;
            double sin_delt_Longitude = Math.Sin(dest_node.coord_Longitude_radian - source_node.coord_Longitude_radian);
            double cos_delt_Longitude = Math.Cos(dest_node.coord_Longitude_radian - source_node.coord_Longitude_radian);
            double y = Math.Sqrt(Math.Pow(dest_node.cos_coord_Latitude_radian * sin_delt_Longitude, 2) + Math.Pow(source_node.cos_coord_Latitude_radian * dest_node.sin_coord_Latitude_radian - source_node.sin_coord_Latitude_radian * dest_node.cos_coord_Latitude_radian * cos_delt_Longitude, 2));
            double x = source_node.sin_coord_Latitude_radian * dest_node.sin_coord_Latitude_radian + source_node.cos_coord_Latitude_radian * dest_node.cos_coord_Latitude_radian * cos_delt_Longitude;
            return_value = Math.Atan2(y, x) * 6372795;
            return return_value;
        }
    }
    public struct GrafNodeParam_nav
    {
        public Coords_nav coords;
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
        public GrafNodeParam_nav(double latitude, double longitude, double koef)
        {
            coords = new Coords_nav(latitude, longitude);
            sin_coord_Latitude_radian = Math.Sin(latitude * Math.PI / 180);
            cos_coord_Latitude_radian = Math.Cos(latitude * Math.PI / 180);
            coord_Longitude_radian = longitude * Math.PI / 180;
            this.koef = koef;
        }
    }
    public struct Coords_nav
    {
        public double latitude;
        public double longitude;
        public Coords_nav(double latitude, double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }
    }
}
