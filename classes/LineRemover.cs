using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BacklashCompensator.classes
{
    public class LineRemover : AbstractPostProcessor
    {
        private String _regexp;
        public LineRemover(String regularExpression)
        {
            _regexp = regularExpression;
        }

        override public string[] GetOutput()
        {
            List<String> lst = new List<String>();

            Regex exp = new Regex(_regexp);

            foreach (String line in lines_arr)
            {
                Match m = exp.Match(line);
                if (m.Success)
                    continue;

                lst.Add(line);
            }

            return lst.ToArray();
        }
    }
}
