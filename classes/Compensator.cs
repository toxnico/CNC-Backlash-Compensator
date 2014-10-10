using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BacklashCompensator.classes
{
    public class Compensator : AbstractPostProcessor
    {
        private String _axisName { get; set; }
        private Decimal _compensationAmount { get; set; }

        private Decimal _currentPosition = 0;
        private int _currentDirection = 0;

        public Compensator(String axisName, Decimal compensationAmount)
        {
            this._axisName = axisName;
            this._compensationAmount = compensationAmount;
        }

        override public String[] GetOutput()
        {
            if (_compensationAmount == 0)
                return lines_arr;

            List<String> lst = new List<String>();

            foreach (String line in lines_arr)
            {
                String tmpLine = cleanupLine(line);
                
                //check si c'est une commande G0 ou G1
                if (tmpLine.ToUpper().StartsWith("G0") || tmpLine.ToUpper().StartsWith("G1"))
                {
                    if (tmpLine.IndexOf(_axisName) > -1)
                    {
                        Decimal destination = getDestinationForAxis(tmpLine, _axisName);

                        Decimal difference = destination - _currentPosition;

                        int lastDirection = _currentDirection;
                        _currentDirection = difference < 0 ? -1 : 1;

                        if (lastDirection + _currentDirection == 0) //direction opposée?
                        {
                            addCompensationInstructions(lst, destination);
                        }

                        _currentPosition = destination;
                    }
                }
                
                lst.Add(line);
            }


            return lst.ToArray();
        }

        /// <summary>
        /// Ajoute les commandes de compensation
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="axisDestination"></param>
        private void addCompensationInstructions(List<String> lst, Decimal axisDestination)
        {
            Decimal compensation = _compensationAmount * _currentDirection;

            String str = String.Format(";{0} compensation : {1} to {2}", _axisName, _currentPosition, axisDestination);
            lst.Add(str);

            //bascule en mode relatif:
            lst.Add("G91;relative mode");

            //ajoute la compensation
            lst.Add("G1 " + _axisName + compensation + ";compensation");

            //revient en mode absolu
            lst.Add("G90;back to absolute mode");

            //redéfinit la position courante de l'axe
            lst.Add("G92 " + _axisName + _currentPosition + ";reset current " + _axisName + " position");

            lst.Add(";End of " + _axisName + " compensation");
        }

        /// <summary>
        /// retire les commentaires
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private string cleanupLine(string line)
        {
            line = line.ToUpper();
            int pos = line.IndexOf(";");
            if (pos == -1)
                return line;

            return line.Substring(0, pos);
        }

        private decimal getDestinationForAxis(String line, String axisName)
        {
            try
            {
                String[] arr = line.Split(' ');
                foreach (String part in arr)
                {
                    if (!part.StartsWith(axisName))
                        continue;

                    return Decimal.Parse(part.Substring(axisName.Length));
                }

                return Decimal.MinValue;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
