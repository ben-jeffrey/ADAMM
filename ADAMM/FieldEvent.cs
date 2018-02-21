using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAMM {
    class FieldEvent : Event {
        public FieldEvent(int number, int ptr, char gender, Division div, char stat, char cat, char unit) : base(number, ptr, gender, 0, div, stat, cat, 0, unit) {
            
        }

        public override string ToString() {
            string gender = EventGender == 'M' ? "Mens" : "Womens";
            string type = CategoryString();
            string div = EventDivision.DivisionName;
            return String.Format("{0} {1} {2}", gender, type, div);
        }
    }
}
