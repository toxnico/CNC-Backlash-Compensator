using BacklashCompensator.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace BacklashCompensator
{
    public partial class frmBacklashCompensator : Form
    {
        public frmBacklashCompensator()
        {
            InitializeComponent();

            nudX.Value = Settings.Default.XCompensation;
            nudY.Value = Settings.Default.YCompensation;
            nudZ.Value = Settings.Default.ZCompensation;
            chkRemoveZMovements.Checked = Settings.Default.removeZMovements;

            String[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
                loadFile(Environment.GetCommandLineArgs()[1]);

            if (args.Length > 2)
                File.WriteAllLines(args[2], txtOutput.Lines);


            
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            updateOutput();
        }

        private void updateOutput()
        {
            String[] lines  = txtInput.Lines;

            if(chkRemoveZMovements.Checked)
                lines = removeLines(lines, "G(0|1).*Z.*");
            
            lines = doCompensation(lines);

            txtOutput.Lines = lines;
        }

        /// <summary>
        /// Supprime les mouvements des axes à ignorer
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private string[] removeLines(String[] lines, String regExp)
        {
            List<String> lst = new List<String>();
                        
            Regex exp = new Regex(regExp);

            foreach(String line in lines)
            {
                Match m = exp.Match(line);
                if (m.Success)
                    continue;

                lst.Add(line);
            }

            return lst.ToArray();
        }

        private string[] doCompensation(String[] lines)
        {
            Compensator cx = new Compensator();
            cx.AxisName = "X";
            cx.CompensationAmount = nudX.Value;

            Compensator cy = new Compensator();
            cy.AxisName = "Y";
            cy.CompensationAmount = nudY.Value;

            Compensator cz = new Compensator();
            cz.AxisName = "Z";
            cz.CompensationAmount = nudZ.Value;

            cx.lines_arr = lines;

            lines = cx.injectCompensation();

            cy.lines_arr = lines;

            lines = cy.injectCompensation();

            cz.lines_arr = lines;

            lines = cz.injectCompensation();

            return lines;
        }

        private void nudX_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.XCompensation = nudX.Value;
            Settings.Default.Save();

            updateOutput();
        }

        private void nudY_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.YCompensation = nudY.Value;
            Settings.Default.Save();

            updateOutput();
        }

        private void nudZ_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.ZCompensation = nudZ.Value;
            Settings.Default.Save();
            updateOutput();
        }

        private void chkRemoveZMovements_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.removeZMovements = chkRemoveZMovements.Checked;
            Settings.Default.Save();
            updateOutput();
        }

        private void btnCopyClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtOutput.Text);
        }

        private void txtInput_DragDrop(object sender, DragEventArgs e)
        {
            String[] file_arr = (String[])e.Data.GetData(DataFormats.FileDrop);

            if (file_arr.Length == 0)
                return;

            loadFile(file_arr[0]);

        }

        private void loadFile(String path)
        {
            String[] lines = File.ReadAllLines(path);

            txtInput.Lines = lines;
        }

        private void txtInput_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) 
                e.Effect = DragDropEffects.Copy;
        }
    }
}
