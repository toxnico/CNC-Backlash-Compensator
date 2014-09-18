using BacklashCompensator.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            updateOutput();

        }

        private void updateOutput()
        {
            String[] lines = txtInput.Lines;

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

            txtOutput.Lines = lines;
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
    }
}
