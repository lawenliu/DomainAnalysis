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
using System.Xml;

namespace DomainAnalysis
{
    public partial class ImportManualDataWindow : Form
    {
        public ImportManualDataWindow()
        {
            InitializeComponent();
        }

        public string RawFileDir
        {
            get { return tbRawFileFolder.Text; }
        }

        public string ModelFilePath
        {
            get { return tbModelFilePath.Text; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Configures.SaveManualRawMaterialFileDir(tbRawFileFolder.Text);
            Configures.SaveManualModelFilePath(tbModelFilePath.Text);
            Configures.SaveManualSearchTermPath(tbSearchTermPath.Text);
        }

        private void btnSelectRawFileFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select the raw files directory";
            folderBrowserDialog.ShowNewFolderButton = false;
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            folderBrowserDialog.SelectedPath = Configures.GetManualRawMaterialFileDir();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                tbRawFileFolder.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btnSelectModelFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location;
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                tbModelFilePath.Text = openFileDialog.FileName;
            }
        }

        private void ImportManualDataWindow_Load(object sender, EventArgs e)
        {
            tbRawFileFolder.Text = Configures.GetManualRawMaterialFileDir();
            tbModelFilePath.Text = Configures.GetManualModelFilePath();
            tbSearchTermPath.Text = Configures.GetManualSearchTermPath();
        }

        private void searchTermPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location;
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            tbSearchTermPath.Text = Configures.GetManualSearchTermPath();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.tbSearchTermPath.Text = openFileDialog.FileName;
            }
        }
    }
}
