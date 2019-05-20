using DomainAnalysis.Model;
using DomainAnalysis.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DomainAnalysis
{
    public partial class ImportAutoDataWindow : Form
    {
        public ImportAutoDataWindow()
        {
            InitializeComponent();
        }

        public string RawFileDir {
            get { return tbRawFileFolder.Text; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Configures.SaveAutoRawMaterialFileDir(tbRawFileFolder.Text);
        }

        private void ImportAutoDataWindow_Load(object sender, EventArgs e)
        {
            tbRawFileFolder.Text = Configures.GetAutoRawMaterialFileDir();
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select the raw files directory";
            folderBrowserDialog.ShowNewFolderButton = false;
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            folderBrowserDialog.SelectedPath = Configures.GetAutoRawMaterialFileDir();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                tbRawFileFolder.Text = folderBrowserDialog.SelectedPath;
            }
        }
    }
}
