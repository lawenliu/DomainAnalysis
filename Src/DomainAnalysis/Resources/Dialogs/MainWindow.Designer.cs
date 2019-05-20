namespace DomainAnalysis
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.labelSelected = new System.Windows.Forms.Label();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createFromManualDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createFromDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.scMainPanel = new System.Windows.Forms.SplitContainer();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.scAutoResult = new System.Windows.Forms.SplitContainer();
            this.gViewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            this.lvRelatedAutoFile = new System.Windows.Forms.ListView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.scManualResult = new System.Windows.Forms.SplitContainer();
            this.treeView = new System.Windows.Forms.TreeView();
            this.lvRelatedManualFile = new System.Windows.Forms.ListView();
            this.scOutputResult = new System.Windows.Forms.SplitContainer();
            this.scSummary = new System.Windows.Forms.SplitContainer();
            this.materialPanel3 = new MaterialSkin.Controls.MaterialPanel();
            this.tbSummary = new System.Windows.Forms.RichTextBox();
            this.materialPanel1 = new MaterialSkin.Controls.MaterialPanel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.comboBox_pageSort = new System.Windows.Forms.ComboBox();
            this.tvHighlightingResult = new System.Windows.Forms.TreeView();
            this.materialPanel2 = new MaterialSkin.Controls.MaterialPanel();
            this.tbExecuteResult = new System.Windows.Forms.RichTextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.menuStrip.SuspendLayout();
            this.panelStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scMainPanel)).BeginInit();
            this.scMainPanel.Panel1.SuspendLayout();
            this.scMainPanel.Panel2.SuspendLayout();
            this.scMainPanel.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scAutoResult)).BeginInit();
            this.scAutoResult.Panel1.SuspendLayout();
            this.scAutoResult.Panel2.SuspendLayout();
            this.scAutoResult.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scManualResult)).BeginInit();
            this.scManualResult.Panel1.SuspendLayout();
            this.scManualResult.Panel2.SuspendLayout();
            this.scManualResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scOutputResult)).BeginInit();
            this.scOutputResult.Panel1.SuspendLayout();
            this.scOutputResult.Panel2.SuspendLayout();
            this.scOutputResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scSummary)).BeginInit();
            this.scSummary.Panel1.SuspendLayout();
            this.scSummary.Panel2.SuspendLayout();
            this.scSummary.SuspendLayout();
            this.materialPanel3.SuspendLayout();
            this.materialPanel1.SuspendLayout();
            this.materialPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // labelSelected
            // 
            this.labelSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelSelected.AutoSize = true;
            this.labelSelected.BackColor = System.Drawing.Color.Transparent;
            this.labelSelected.Location = new System.Drawing.Point(5, 6);
            this.labelSelected.Name = "labelSelected";
            this.labelSelected.Size = new System.Drawing.Size(0, 12);
            this.labelSelected.TabIndex = 3;
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(10, 42);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(162, 26);
            this.menuStrip.TabIndex = 5;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createFromManualDataToolStripMenuItem,
            this.createFromDataToolStripMenuItem});
            this.fileToolStripMenuItem.Font = new System.Drawing.Font("Century", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileToolStripMenuItem.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.fileToolStripMenuItem.Text = "Operations";
            // 
            // createFromManualDataToolStripMenuItem
            // 
            this.createFromManualDataToolStripMenuItem.Name = "createFromManualDataToolStripMenuItem";
            this.createFromManualDataToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
            this.createFromManualDataToolStripMenuItem.Text = "Manual Model-based";
            this.createFromManualDataToolStripMenuItem.Click += new System.EventHandler(this.createFromManualDataToolStripMenuItem_Click);
            // 
            // createFromDataToolStripMenuItem
            // 
            this.createFromDataToolStripMenuItem.Name = "createFromDataToolStripMenuItem";
            this.createFromDataToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
            this.createFromDataToolStripMenuItem.Text = "Automated Model-based";
            this.createFromDataToolStripMenuItem.Click += new System.EventHandler(this.createFromDataToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewHelpToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Font = new System.Drawing.Font("Century", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpToolStripMenuItem.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(55, 22);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // viewHelpToolStripMenuItem
            // 
            this.viewHelpToolStripMenuItem.Name = "viewHelpToolStripMenuItem";
            this.viewHelpToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.viewHelpToolStripMenuItem.Text = "View Help";
            this.viewHelpToolStripMenuItem.Click += new System.EventHandler(this.viewHelpToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // panelStatus
            // 
            this.panelStatus.BackColor = System.Drawing.Color.Gainsboro;
            this.panelStatus.Controls.Add(this.labelSelected);
            this.panelStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelStatus.Location = new System.Drawing.Point(0, 621);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(1158, 24);
            this.panelStatus.TabIndex = 6;
            // 
            // scMainPanel
            // 
            this.scMainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scMainPanel.Location = new System.Drawing.Point(0, 67);
            this.scMainPanel.Name = "scMainPanel";
            this.scMainPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scMainPanel.Panel1
            // 
            this.scMainPanel.Panel1.Controls.Add(this.tabControl);
            // 
            // scMainPanel.Panel2
            // 
            this.scMainPanel.Panel2.Controls.Add(this.scOutputResult);
            this.scMainPanel.Size = new System.Drawing.Size(1158, 556);
            this.scMainPanel.SplitterDistance = 317;
            this.scMainPanel.TabIndex = 7;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Controls.Add(this.tabPage3);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.Padding = new System.Drawing.Point(0, 0);
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1158, 317);
            this.tabControl.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.pictureBox1);
            this.tabPage1.Controls.Add(this.pictureBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(1150, 291);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.scAutoResult);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(1150, 291);
            this.tabPage2.TabIndex = 0;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // scAutoResult
            // 
            this.scAutoResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scAutoResult.Location = new System.Drawing.Point(0, 0);
            this.scAutoResult.Name = "scAutoResult";
            // 
            // scAutoResult.Panel1
            // 
            this.scAutoResult.Panel1.Controls.Add(this.gViewer);
            // 
            // scAutoResult.Panel2
            // 
            this.scAutoResult.Panel2.Controls.Add(this.lvRelatedAutoFile);
            this.scAutoResult.Size = new System.Drawing.Size(1150, 291);
            this.scAutoResult.SplitterDistance = 604;
            this.scAutoResult.TabIndex = 0;
            // 
            // gViewer
            // 
            this.gViewer.ArrowheadLength = 10D;
            this.gViewer.AsyncLayout = false;
            this.gViewer.AutoScroll = true;
            this.gViewer.BackColor = System.Drawing.Color.LightGray;
            this.gViewer.BackwardEnabled = false;
            this.gViewer.BuildHitTree = true;
            this.gViewer.CurrentLayoutMethod = Microsoft.Msagl.GraphViewerGdi.LayoutMethod.UseSettingsOfTheGraph;
            this.gViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gViewer.EdgeInsertButtonVisible = true;
            this.gViewer.FileName = "";
            this.gViewer.ForwardEnabled = false;
            this.gViewer.Graph = null;
            this.gViewer.InsertingEdge = false;
            this.gViewer.LayoutAlgorithmSettingsButtonVisible = true;
            this.gViewer.LayoutEditingEnabled = true;
            this.gViewer.Location = new System.Drawing.Point(0, 0);
            this.gViewer.LooseOffsetForRouting = 0.25D;
            this.gViewer.MouseHitDistance = 0.05D;
            this.gViewer.Name = "gViewer";
            this.gViewer.NavigationVisible = true;
            this.gViewer.NeedToCalculateLayout = true;
            this.gViewer.OffsetForRelaxingInRouting = 0.6D;
            this.gViewer.PaddingForEdgeRouting = 8D;
            this.gViewer.PanButtonPressed = false;
            this.gViewer.SaveAsImageEnabled = true;
            this.gViewer.SaveAsMsaglEnabled = true;
            this.gViewer.SaveButtonVisible = true;
            this.gViewer.SaveGraphButtonVisible = true;
            this.gViewer.SaveInVectorFormatEnabled = true;
            this.gViewer.Size = new System.Drawing.Size(604, 291);
            this.gViewer.TabIndex = 13;
            this.gViewer.TightOffsetForRouting = 0.125D;
            this.gViewer.ToolBarIsVisible = true;
            this.gViewer.Transform = ((Microsoft.Msagl.Core.Geometry.Curves.PlaneTransformation)(resources.GetObject("gViewer.Transform")));
            this.gViewer.UndoRedoButtonsVisible = true;
            this.gViewer.WindowZoomButtonPressed = false;
            this.gViewer.ZoomF = 1D;
            this.gViewer.ZoomFraction = 0.5D;
            this.gViewer.ZoomWhenMouseWheelScroll = true;
            this.gViewer.ZoomWindowThreshold = 0.05D;
            this.gViewer.Load += new System.EventHandler(this.gViewer_Load);
            this.gViewer.Click += new System.EventHandler(this.gViewer_Click);
            // 
            // lvRelatedAutoFile
            // 
            this.lvRelatedAutoFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvRelatedAutoFile.GridLines = true;
            this.lvRelatedAutoFile.Location = new System.Drawing.Point(0, 0);
            this.lvRelatedAutoFile.Name = "lvRelatedAutoFile";
            this.lvRelatedAutoFile.Size = new System.Drawing.Size(542, 291);
            this.lvRelatedAutoFile.TabIndex = 13;
            this.lvRelatedAutoFile.UseCompatibleStateImageBehavior = false;
            this.lvRelatedAutoFile.View = System.Windows.Forms.View.Details;
            this.lvRelatedAutoFile.DoubleClick += new System.EventHandler(this.lvRelatedAutoFile_DoubleClick);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.scManualResult);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1150, 291);
            this.tabPage3.TabIndex = 1;
            this.tabPage3.Text = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // scManualResult
            // 
            this.scManualResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scManualResult.Location = new System.Drawing.Point(0, 0);
            this.scManualResult.Name = "scManualResult";
            // 
            // scManualResult.Panel1
            // 
            this.scManualResult.Panel1.Controls.Add(this.treeView);
            // 
            // scManualResult.Panel2
            // 
            this.scManualResult.Panel2.Controls.Add(this.lvRelatedManualFile);
            this.scManualResult.Size = new System.Drawing.Size(1150, 291);
            this.scManualResult.SplitterDistance = 445;
            this.scManualResult.TabIndex = 0;
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(445, 291);
            this.treeView.TabIndex = 10;
            this.treeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseClick);
            // 
            // lvRelatedManualFile
            // 
            this.lvRelatedManualFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvRelatedManualFile.GridLines = true;
            this.lvRelatedManualFile.Location = new System.Drawing.Point(0, 0);
            this.lvRelatedManualFile.Name = "lvRelatedManualFile";
            this.lvRelatedManualFile.Size = new System.Drawing.Size(701, 291);
            this.lvRelatedManualFile.TabIndex = 11;
            this.lvRelatedManualFile.UseCompatibleStateImageBehavior = false;
            this.lvRelatedManualFile.View = System.Windows.Forms.View.Details;
            this.lvRelatedManualFile.Click += new System.EventHandler(this.lvRelatedManualFile_Click);
            this.lvRelatedManualFile.DoubleClick += new System.EventHandler(this.lvRelatedManualFile_DoubleClick);
            // 
            // scOutputResult
            // 
            this.scOutputResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scOutputResult.Location = new System.Drawing.Point(0, 0);
            this.scOutputResult.Name = "scOutputResult";
            this.scOutputResult.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scOutputResult.Panel1
            // 
            this.scOutputResult.Panel1.Controls.Add(this.scSummary);
            // 
            // scOutputResult.Panel2
            // 
            this.scOutputResult.Panel2.Controls.Add(this.materialPanel2);
            this.scOutputResult.Size = new System.Drawing.Size(1158, 235);
            this.scOutputResult.SplitterDistance = 143;
            this.scOutputResult.TabIndex = 0;
            // 
            // scSummary
            // 
            this.scSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scSummary.Location = new System.Drawing.Point(0, 0);
            this.scSummary.Name = "scSummary";
            this.scSummary.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scSummary.Panel1
            // 
            this.scSummary.Panel1.Controls.Add(this.materialPanel3);
            // 
            // scSummary.Panel2
            // 
            this.scSummary.Panel2.Controls.Add(this.materialPanel1);
            this.scSummary.Size = new System.Drawing.Size(1158, 143);
            this.scSummary.SplitterDistance = 69;
            this.scSummary.TabIndex = 0;
            // 
            // materialPanel3
            // 
            this.materialPanel3.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.materialPanel3.Controls.Add(this.tbSummary);
            this.materialPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.materialPanel3.HeaderColor1 = System.Drawing.Color.DarkGray;
            this.materialPanel3.HeaderColor2 = System.Drawing.Color.Silver;
            this.materialPanel3.HeaderText = "Summary";
            this.materialPanel3.Icon = null;
            this.materialPanel3.IconTransparentColor = System.Drawing.Color.White;
            this.materialPanel3.Location = new System.Drawing.Point(0, 0);
            this.materialPanel3.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.materialPanel3.Name = "materialPanel3";
            this.materialPanel3.Padding = new System.Windows.Forms.Padding(5, 27, 5, 4);
            this.materialPanel3.Size = new System.Drawing.Size(1158, 69);
            this.materialPanel3.TabIndex = 5;
            // 
            // tbSummary
            // 
            this.tbSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbSummary.Location = new System.Drawing.Point(5, 27);
            this.tbSummary.Name = "tbSummary";
            this.tbSummary.ReadOnly = true;
            this.tbSummary.Size = new System.Drawing.Size(1148, 38);
            this.tbSummary.TabIndex = 0;
            this.tbSummary.Text = "";
            // 
            // materialPanel1
            // 
            this.materialPanel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.materialPanel1.Controls.Add(this.linkLabel1);
            this.materialPanel1.Controls.Add(this.comboBox_pageSort);
            this.materialPanel1.Controls.Add(this.tvHighlightingResult);
            this.materialPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.materialPanel1.HeaderColor1 = System.Drawing.Color.DarkGray;
            this.materialPanel1.HeaderColor2 = System.Drawing.Color.Silver;
            this.materialPanel1.HeaderText = "Document Highlight Result";
            this.materialPanel1.Icon = null;
            this.materialPanel1.IconTransparentColor = System.Drawing.Color.White;
            this.materialPanel1.Location = new System.Drawing.Point(0, 0);
            this.materialPanel1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.materialPanel1.Name = "materialPanel1";
            this.materialPanel1.Padding = new System.Windows.Forms.Padding(5, 27, 5, 4);
            this.materialPanel1.Size = new System.Drawing.Size(1158, 70);
            this.materialPanel1.TabIndex = 5;
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel1.Location = new System.Drawing.Point(842, 6);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(77, 12);
            this.linkLabel1.TabIndex = 5;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "I feel lucky";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // comboBox_pageSort
            // 
            this.comboBox_pageSort.AllowDrop = true;
            this.comboBox_pageSort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_pageSort.BackColor = System.Drawing.Color.Silver;
            this.comboBox_pageSort.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBox_pageSort.Font = new System.Drawing.Font("Century", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox_pageSort.FormattingEnabled = true;
            this.comboBox_pageSort.Items.AddRange(new object[] {
            "list pages by natural order",
            "list pages by relevance"});
            this.comboBox_pageSort.Location = new System.Drawing.Point(919, 1);
            this.comboBox_pageSort.Name = "comboBox_pageSort";
            this.comboBox_pageSort.Size = new System.Drawing.Size(236, 24);
            this.comboBox_pageSort.TabIndex = 4;
            this.comboBox_pageSort.Text = "list pages by natural order";
            this.comboBox_pageSort.SelectedIndexChanged += new System.EventHandler(this.comboBox_pageSort_SelectedIndexChanged);
            // 
            // tvHighlightingResult
            // 
            this.tvHighlightingResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvHighlightingResult.Location = new System.Drawing.Point(5, 27);
            this.tvHighlightingResult.Name = "tvHighlightingResult";
            this.tvHighlightingResult.Size = new System.Drawing.Size(1148, 39);
            this.tvHighlightingResult.TabIndex = 2;
            this.tvHighlightingResult.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvSummaryResult_NodeMouseDoubleClick);
            // 
            // materialPanel2
            // 
            this.materialPanel2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.materialPanel2.Controls.Add(this.tbExecuteResult);
            this.materialPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.materialPanel2.HeaderColor1 = System.Drawing.Color.DarkGray;
            this.materialPanel2.HeaderColor2 = System.Drawing.Color.Silver;
            this.materialPanel2.HeaderText = "Output";
            this.materialPanel2.Icon = null;
            this.materialPanel2.IconTransparentColor = System.Drawing.Color.White;
            this.materialPanel2.Location = new System.Drawing.Point(0, 0);
            this.materialPanel2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.materialPanel2.Name = "materialPanel2";
            this.materialPanel2.Padding = new System.Windows.Forms.Padding(5, 27, 5, 4);
            this.materialPanel2.Size = new System.Drawing.Size(1158, 88);
            this.materialPanel2.TabIndex = 4;
            // 
            // tbExecuteResult
            // 
            this.tbExecuteResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbExecuteResult.Location = new System.Drawing.Point(5, 27);
            this.tbExecuteResult.Name = "tbExecuteResult";
            this.tbExecuteResult.ReadOnly = true;
            this.tbExecuteResult.Size = new System.Drawing.Size(1148, 57);
            this.tbExecuteResult.TabIndex = 0;
            this.tbExecuteResult.Text = "";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::DomainAnalysis.Properties.Resources.owl;
            this.pictureBox1.Location = new System.Drawing.Point(117, 128);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(200, 174);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.Image = global::DomainAnalysis.Properties.Resources.welcome0;
            this.pictureBox2.Location = new System.Drawing.Point(323, 155);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(715, 112);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 3;
            this.pictureBox2.TabStop = false;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1158, 645);
            this.Controls.Add(this.scMainPanel);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.panelStatus);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MaRK:Requirement Knowledge Mining";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.SizeChanged += new System.EventHandler(this.MainWindow_SizeChanged);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.panelStatus.ResumeLayout(false);
            this.panelStatus.PerformLayout();
            this.scMainPanel.Panel1.ResumeLayout(false);
            this.scMainPanel.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scMainPanel)).EndInit();
            this.scMainPanel.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.scAutoResult.Panel1.ResumeLayout(false);
            this.scAutoResult.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scAutoResult)).EndInit();
            this.scAutoResult.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.scManualResult.Panel1.ResumeLayout(false);
            this.scManualResult.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scManualResult)).EndInit();
            this.scManualResult.ResumeLayout(false);
            this.scOutputResult.Panel1.ResumeLayout(false);
            this.scOutputResult.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scOutputResult)).EndInit();
            this.scOutputResult.ResumeLayout(false);
            this.scSummary.Panel1.ResumeLayout(false);
            this.scSummary.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scSummary)).EndInit();
            this.scSummary.ResumeLayout(false);
            this.materialPanel3.ResumeLayout(false);
            this.materialPanel1.ResumeLayout(false);
            this.materialPanel1.PerformLayout();
            this.materialPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelSelected;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createFromDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewHelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.ToolStripMenuItem createFromManualDataToolStripMenuItem;
        private System.Windows.Forms.SplitContainer scMainPanel;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.SplitContainer scAutoResult;
        private Microsoft.Msagl.GraphViewerGdi.GViewer gViewer;
        private System.Windows.Forms.ListView lvRelatedAutoFile;
        private System.Windows.Forms.SplitContainer scManualResult;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ListView lvRelatedManualFile;
        private System.Windows.Forms.SplitContainer scOutputResult;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.SplitContainer scSummary;
        private MaterialSkin.Controls.MaterialPanel materialPanel3;
        private System.Windows.Forms.RichTextBox tbSummary;
        private MaterialSkin.Controls.MaterialPanel materialPanel1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.ComboBox comboBox_pageSort;
        private System.Windows.Forms.TreeView tvHighlightingResult;
        private MaterialSkin.Controls.MaterialPanel materialPanel2;
        private System.Windows.Forms.RichTextBox tbExecuteResult;
    }
}

