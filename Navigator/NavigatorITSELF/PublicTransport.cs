using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navigator
{
    class PublicTransport
    {
        public long transport_id;
        public int count_stop;
        public long point_id;
        public double avarage_time;
        public PublicTransport(string data)
        {
            string[] component = data.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            transport_id = Convert.ToInt64(component[0]);
            point_id = Convert.ToInt64(component[1].Split(new char[] { '=', '"' }, StringSplitOptions.RemoveEmptyEntries)[1]);
            count_stop = Convert.ToInt32(component[2].Split(new char[] { '=', '"' }, StringSplitOptions.RemoveEmptyEntries)[1]);
            avarage_time = Convert.ToDouble(component[3].Split(new char[] { '=', '"' }, StringSplitOptions.RemoveEmptyEntries)[1]);
        }
    }
    class PublicTransportPath : IEquatable<PublicTransportPath>
    {
        public static double Mode(PublicTransportPath point, int mode)
        {
            double check_value;
            switch (mode)
            {
                case 1:
                    check_value = point.time_for_travel;
                    break;
                case 2:
                    check_value = point.heuristic;
                    break;
                default:
                    check_value = point.transport_cost_dest;
                    break;
            }
            return check_value;
        }
        public static PublicTransportPath next_Point(List<PublicTransportPath> Not_Look, int mode)
        {
            PublicTransportPath temp_path = null;
            //PublicTransportPath PTS_path = null;
            double min = double.MaxValue;
            foreach (var path in Not_Look)
            {
                double check_value = PublicTransportPath.Mode(path, mode);
                //min = double.MaxValue;
                if (min > check_value)
                {
                    temp_path = new PublicTransportPath(path);
                    min = check_value;
                }
            }
            return temp_path;
        }
        /// <summary>
        /// Функция нахождения пути используя общественный транспорт
        /// </summary>
        /// <param name="PTS">Коллекция путей транспорта</param>
        /// <param name="start">Начальная остановка</param>
        /// <param name="end">Конечная остановка</param>
        /// <param name="max_cost">Сумма цен всех возможных транспортов (+1)</param>
        /// <param name="trans_id">Колекция с пересадками и номерами транспорта</param>
        /// <param name="real_cost">Общая цена проезда</param>
        /// <param name="timetravel">Общее время проезда</param>
        /// <param name="mode">Режим работы(0-по цене,1-по времени,2-по евристике)</param>
        /// <returns>Колекция с номерами остановок</returns>
        public static List<long> GetPath(Dictionary<long, List<PublicTransportPath>> PTS,long start,long end, double max_cost, out Dictionary<long,long> trans_id, out double real_cost, out double timetravel, int mode=0)
        {
            long index = start;
            List<PublicTransportPath> Not_Look = new List<PublicTransportPath>();
            List<PublicTransportPath> Look = new List<PublicTransportPath>();
            foreach (var paths in PTS[start])
            {
                if (paths.dest_tp != paths.parent_tp)
                {
                    paths.SetHeuristic(max_cost, -1);
                    Not_Look.Add(new PublicTransportPath(paths));
                }
            }
            PublicTransportPath temp_path = PublicTransportPath.next_Point(Not_Look, mode);
            Look.Add(temp_path);
            Not_Look.Remove(temp_path);
            //double time = temp_path.time_for_travel;
            //double cost = temp_path.transport_cost_dest;
            //List<PublicTransportPath> test = new List<PublicTransportPath>();
            while(temp_path.dest_tp!=end)
            {
                List<PublicTransportPath> collection = PTS[temp_path.dest_tp];
                foreach (var paths in collection)
                {
                    if(paths.dest_tp!= temp_path.parent_tp && paths.transport_id_sour == temp_path.transport_id_dest)
                    {
                        bool flag = false;
                        foreach(var path_look in Look)
                        {
                            if (path_look.parent_tp == paths.dest_tp && paths.transport_id_sour == path_look.transport_id_dest)
                            { 
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                            break;
                        paths.SetHeuristic(max_cost, temp_path.transport_cost_dest,temp_path.time_for_travel);
                        Not_Look.Add(new PublicTransportPath(paths));
                    }
                }
                temp_path = PublicTransportPath.next_Point(Not_Look, mode);
                //if (temp_path.transport_id_dest == 310)
                //    test.Add(temp_path);
                Look.Add(temp_path);
                Not_Look.Remove(temp_path);
            }
            List<long> way = new List<long>();
            way.Add(Look.Last().dest_tp);
            way.Add(Look.Last().parent_tp);
            trans_id = new Dictionary<long, long>();
            real_cost = temp_path.transport_cost_dest;
            timetravel = temp_path.time_for_travel;
            while (!way.Contains(start))
            {
                double min = double.MaxValue;
                long temp_ind = -1;
                foreach(var path in Look)
                {
                    double check_value = PublicTransportPath.Mode(path, mode);
                    if (path.dest_tp==way.Last()&&
                        min>check_value)
                    {
                        min = check_value;
                        temp_ind = path.parent_tp;
                    }
                }
                if (temp_ind == -1)
                    throw new Exception("Can`t find the way");
                way.Add(temp_ind);
            }
            return way;
           
        }
        public bool Equals(PublicTransportPath other)
        {
            if (this.parent_tp==other.parent_tp &&
                this.dest_tp==other.dest_tp &&
                this.transport_id_sour==other.transport_id_sour &&
                this.transport_id_dest==other.transport_id_dest)
                return true;
            return false;
        }
        public long transport_id_sour;
        public long transport_id_dest;
        public long parent_tp;
        public long dest_tp;
        public double transport_cost_dest;
        public double time_for_travel;
        public double koef;
        public double koef_transfer;
        public double heuristic;
        public PublicTransportPath(PublicTransportPath obj)
        {
            transport_id_sour = obj.transport_id_sour;
            transport_id_dest = obj.transport_id_dest;
            parent_tp = obj.parent_tp;
            dest_tp = obj.dest_tp;
            transport_cost_dest = obj.transport_cost_dest;
            time_for_travel = obj.time_for_travel;
            koef = obj.koef;
            koef_transfer = obj.koef_transfer;
            heuristic = obj.heuristic;
        }
        public PublicTransportPath(PublicTransport parent_p, PublicTransport dest_p, double transport_cost_dest = 0)
        {
            koef = 1;
            time_for_travel = 5;
            koef_transfer = 1;
            transport_id_sour = parent_p.transport_id;
            transport_id_dest = dest_p.transport_id;
            parent_tp = parent_p.point_id;
            dest_tp = dest_p.point_id;
            this.transport_cost_dest = transport_cost_dest;
            if (dest_p.transport_id == parent_p.transport_id)
                time_for_travel = Math.Abs(parent_p.avarage_time - dest_p.avarage_time);
        }
        public PublicTransportPath(PublicTransport last_point, double transport_cost_dest = 0)
        {
            koef = 999;
            time_for_travel = 999;
            koef_transfer = 99;
            transport_id_dest = last_point.transport_id;
            transport_id_sour = transport_id_dest;
            parent_tp = last_point.point_id;
            dest_tp = last_point.point_id;
            this.transport_cost_dest = transport_cost_dest;
        }
        public PublicTransportPath(long parent_id, PublicTransport dest_p, long transport_id = 0, double transport_cost_dest = 0)
        {
            koef = 1;
            time_for_travel = 5;
            koef_transfer = 1;
            transport_id_dest = dest_p.transport_id;
            transport_id_sour = transport_id;
            parent_tp = parent_id;
            dest_tp = dest_p.point_id;
            this.transport_cost_dest = transport_cost_dest;
        }
        public PublicTransportPath(PublicTransport parent_p, long dest_p_id, long transport_id = 0, double transport_cost_dest = 0)
        {
            koef = 1;
            time_for_travel = 5;
            koef_transfer = 1;
            transport_id_dest = transport_id;
            transport_id_sour = parent_p.transport_id;
            parent_tp = parent_p.point_id;
            dest_tp = dest_p_id;
            this.transport_cost_dest = transport_cost_dest;
        }
        public double SetHeuristic(double max_cost,double cost = 0, double time = 0)
        {
            double cost_r = getCost(cost);
            transport_cost_dest = cost_r;
            koef_transfer = (cost_r / max_cost);
            time_for_travel += time;
            heuristic = time_for_travel * koef_transfer * koef;
            return cost_r;
        }
        public double SetHeuristic(double max_cost,long start_index)
        {
            double cost_r = transport_cost_dest;
            koef_transfer = 1 - (cost_r / max_cost);
            heuristic = time_for_travel * koef_transfer * koef;
            return cost_r;
        }
        public double getCost(double cost = 0)
        {
            double cost_r = cost;
            if (transport_id_dest != transport_id_sour)
                cost_r += transport_cost_dest;
            return cost_r;
        }
        public void Reset()
        {
            koef = 1;
            time_for_travel = 5;
            koef_transfer = 1;
        }
        /*public double GetHeuristic()
        {
            return heuristic;
        }*/
    }
}
