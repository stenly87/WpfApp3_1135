using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp3
{
    public class QRCode
    {
        public int id { get; set; }
        public string name { get; set; }
        public int age { get; set; }
        public long dateend { get; set; } = DateTime.Now.ToBinary();
        public DateTime date 
        { 
            get => DateTime.FromBinary(dateend);
            set => dateend = value.ToBinary();
        }
    }
}
