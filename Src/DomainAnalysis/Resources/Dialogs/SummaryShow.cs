using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DomainAnalysis.Resources.Dialogs
{
    public partial class SummaryShow : Form
    {
        public SummaryShow()
        {
            InitializeComponent();
        }

        private void SummaryShow_Load(object sender, EventArgs e)
        {

        }

        public void SetQualityComp(string comp, string quality, string summary)
        {
            this.component.Text = comp;
            this.quality.Text = quality;
            this.summaryBox.Text = summary;
        }
    }
}
