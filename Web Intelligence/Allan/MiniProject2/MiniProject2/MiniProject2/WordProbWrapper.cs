using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniProject2
{
    class WordProbWrapper
    {
        public double dependentPos { get; set; }
        public double dependentNeg { get; set; }

        public double notdependentPos { get { return 1 - dependentPos; } }
        public double notdependentNeg { get { return 1 - dependentNeg; } }

        public WordProbWrapper(double dependentPos, double dependentNeg)
        {
            this.dependentPos = dependentPos;
            this.dependentNeg = dependentNeg;
        }
    }
}
