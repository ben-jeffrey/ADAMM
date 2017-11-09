using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAMM {
    class Division {
        public int DivisionNumber {get; set;}
        public String DivisionName { get; set; }
        public int DivisionAgeLow { get; set; }
        public int DivisionAgeHigh { get; set; }

        public Division(int num, String name, int l, int h) {
            DivisionNumber = num;
            DivisionName = name;
            DivisionAgeLow = l;
            DivisionAgeHigh = h;
        }

    }
}
