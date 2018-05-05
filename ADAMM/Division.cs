using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAMM {
    public class Division {

        public int DivisionNumber {get; set;}
        public String DivisionName { get; set; }
        public int DivisionAgeLow { get; set; }
        public int DivisionAgeHigh { get; set; }
        // Template is of the form (Place : Score)
        // Each division may score things differently
        public Dictionary<int, int> DivisionScoreTemplate { get; set; }

        public Division(int num, String name, int l, int h) {
            DivisionNumber = num;
            DivisionName = name;
            DivisionAgeLow = l;
            DivisionAgeHigh = h;
            DivisionScoreTemplate = new Dictionary<int, int>();
        }

        public override string ToString() {
            return DivisionName;
        }

    }
}
