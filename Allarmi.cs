using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progetto_Main
{
    class Allarmi
    {
        public string Message { get; set; }
        public bool IsActive { get; set; }
        public bool IsWarning { get; set; }

        // TODO: Settare ad ogni evento i suoi bit di allarme o controllarli ad uno ad uno?
    }
}
