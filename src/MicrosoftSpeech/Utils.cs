using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicrosoftSpeech
{
    static class Extensions
    {

        internal static void AppendLine(this System.Windows.Forms.TextBox tb, string msg)
        {
            if (tb.InvokeRequired)
            {
                tb.Invoke((Action)(() => { AppendLine(tb, msg); }));
            }
            else
            {
                tb.AppendText(msg + "\r\n");
            }
        }
    }
}
