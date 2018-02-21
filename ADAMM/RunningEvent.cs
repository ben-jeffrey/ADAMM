using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAMM {
    class RunningEvent : Event {
        public RunningEvent(int number, int ptr, char gender, int posCount, Division div, char stat, char cat, int dist, char unit) : base(number, ptr, gender, posCount, div, stat, cat, dist, unit) {

        }
    }
}
