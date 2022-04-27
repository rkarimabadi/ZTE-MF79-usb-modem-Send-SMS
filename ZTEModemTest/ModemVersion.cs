using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTEModemTest
{
    public class ModemVersion
    {
        public string Language { get; set; }
        public string cr_version { get; set; }
        public string wa_inner_version { get; set; }
        public string GroupRD => wa_inner_version + cr_version;
    }
}
