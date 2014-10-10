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
using BacklashCompensator.classes;

namespace BacklashCompensator
{
    public partial class frmBacklashCompensator : Form
    {
        private List<AbstractPostProcessor> _lstPostProcessors = new List<AbstractPostProcessor>();

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

        private void initPostProcessors()
        {
            _lstPostProcessors.Clear();

            if (chkRemoveZMovements.Checked)
                _lstPostProcessors.Add(new LineRemover("G(0|1).*Z.*"));

            _lstPostProcessors.Add(new Compensator("X", nudX.Value));
            _lstPostProcessors.Add(new Compensator("Y", nudY.Value));
            _lstPostProcessors.Add(new Compensator("Z", nudZ.Value));

        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            process();
        }

        private void process()
        {
            initPostProcessors();

            String[] lines  = txtInput.Lines;

            foreach (AbstractPostProcessor pp in _lstPostProcessors)
            {
                pp.LoadGCodeLines(lines);
                lines = pp.GetOutput();
            }
            
            txtOutput.Lines = lines;
        }

        #region Evenements de changements de paramètres
        private void nudX_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.XCompensation = nudX.Value;
            Settings.Default.Save();

            process();
        }

        private void nudY_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.YCompensation = nudY.Value;
            Settings.Default.Save();

            process();
        }

        private void nudZ_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.ZCompensation = nudZ.Value;
            Settings.Default.Save();
            process();
        }

        private void chkRemoveZMovements_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.removeZMovements = chkRemoveZMovements.Checked;
            Settings.Default.Save();
            process();
        }
        #endregion

        /// <summary>
        /// Copie la sortie dans le presse papier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCopyClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtOutput.Text);
        }

        /// <summary>
        /// Quand on lache un fichier sur la zone input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtInput_DragDrop(object sender, DragEventArgs e)
        {
            String[] file_arr = (String[])e.Data.GetData(DataFormats.FileDrop);

            if (file_arr.Length == 0)
                return;

            loadFile(file_arr[0]);

        }

        /// <summary>
        /// charge un fichier
        /// </summary>
        /// <param name="path"></param>
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
