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
    class PublicTransportPath
    {
        public long transport_id_dest;
        public long parent_tp;
        public long dest_tp;
        public double transport_cost;
        public double koef_time;
        public double koef;
        public double koef_transfer;
        public double heuristic;
        public PublicTransportPath(PublicTransport parent_p, PublicTransport dest_p)
        {
            koef = 1;
            koef_time = 5;
            koef_transfer = 1;
            transport_id_dest = dest_p.transport_id;
            parent_tp = parent_p.point_id;
            dest_tp = dest_p.point_id;
            if (dest_p.transport_id == parent_p.transport_id)
                koef_time = Math.Abs(parent_p.avarage_time - dest_p.avarage_time);
        }
        public PublicTransportPath(PublicTransport last_point)
        {
            koef = 999;
            koef_time = 999;
            koef_transfer = 99;
            transport_id_dest = last_point.transport_id;
            parent_tp = last_point.point_id;
            dest_tp = last_point.point_id;
        }
        public PublicTransportPath(long parent_id, PublicTransport dest_p)
        {
            koef = 1;
            koef_time = 5;
            koef_transfer = 1;
            transport_id_dest = dest_p.transport_id;
            parent_tp = parent_id;
            dest_tp = dest_p.point_id;
        }
        public void SetHeuristic(List<long> t_c)
        {
            if (!t_c.Contains(transport_id_dest))
                koef_transfer = 2;
            heuristic = koef_time * koef_transfer * koef;
        }
        public double GetHeuristic()
        {
            return heuristic;
        }
    }
}
