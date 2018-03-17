using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CDS.Devices.Client
{
    public class HwProductKey
    {
        public string email {get; private set; }
        public string password { get; private set; }

        public static HwProductKey CreateHwProductKey(string email, string password)
        {
            HwProductKey hwProductKey = new HwProductKey();
            hwProductKey.email = email;
            hwProductKey.password = password;

            return hwProductKey;
        }
    }
}
