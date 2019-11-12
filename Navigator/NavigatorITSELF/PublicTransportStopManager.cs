using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Navigator
{
    /// <summary>
    /// Статический класс для получения работы с формой PTSM
    /// </summary>
    static class PublicTransportStopManager
    {
       static string result;
       static PTSM form;
       static public void PTS(long point_id,Form owner)
        {
            form = new PTSM();
            form.setData(point_id);
            form.Owner = owner;
            form.FormClosed += (object s, FormClosedEventArgs args) =>
            {
                result = form.getData();
            };
            form.Show();
        }
        static public void Res(ref string output)
        {
            if(!output.Contains(result))
                output += result;
        }
    }
}
