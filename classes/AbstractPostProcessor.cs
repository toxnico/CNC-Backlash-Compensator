using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BacklashCompensator.classes
{
    public abstract class AbstractPostProcessor
    {
        protected String[] lines_arr { get; set; }

        public void LoadGCodeLines(String[] lines)
        {
            lines_arr = lines;
        }

        public abstract String[] GetOutput();

    }
}
