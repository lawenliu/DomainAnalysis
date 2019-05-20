using Microsoft.Msagl.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = Microsoft.Msagl.Drawing.Color;
using Edge = Microsoft.Msagl.Drawing.Edge;
using Node = Microsoft.Msagl.Drawing.Node;
using Point = Microsoft.Msagl.Core.Geometry.Point;
using Rectangle = Microsoft.Msagl.Core.Geometry.Rectangle;
using GeomEdge = Microsoft.Msagl.Core.Layout.Edge;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Geometry;
using System.IO;
using Microsoft.Msagl.Core.Layout;
using DomainAnalysis.Model;
using MaterialSkin.Controls;
using MaterialSkin;
using DomainAnalysis.Resources.Dialogs;
using DomainAnalysis.HighlightPDF;
using DomainAnalysis.Highlight.HighlightOffice;
using DomainAnalysis.Utils;
using System.Diagnostics;
using DomainAnalysis.RenderPrepare;
using DomainAnalysis.Models;
using DomainAnalysis.GenerateDataMg;
using DomainAnalysis.DataPrepare;
using DomainAnalysis.ToolMg;
using System.Threading;
using Microsoft.Office.Interop.Word;
using System.Text.RegularExpressions;

namespace DomainAnalysis
{
    public partial class MainWindow : MaterialForm
    {
        private readonly ToolTip mToolTip = new ToolTip();
        private MaterialSkinManager mMaterialSkinManager;
        private object mSelectedObjectAttr;
        private object mSelectedObject;

        private List<NodeModel> mNodeList = new List<NodeModel>();
        private List<double> mSortedNodeWeightList = new List<double>();
        private double mMinNodeWeight = double.MaxValue;
        private List<List<double>> mEdgeList = new List<List<double>>();
        private Dictionary<string, List<RelatedFileModel>> mSimilarityDictionary = new Dictionary<string, List<RelatedFileModel>>();
        private Dictionary<string, string> mCompSummDictionary = new Dictionary<string, string>();
        private int mNodeNumberThreshold = -Constants.RenderNodeNumberThreshold;
        private double mEdgeWeightThreshold = Constants.RenderEdgeWeightThreshold;
        private RenderManualDataMg mRenderManualDataMg = new RenderManualDataMg();
        private string mTopicTermFilePath = string.Empty;

        private TreeElement mTreeRoot = new TreeElement();
        private BiNode mBiTreeRoot = null;

        private BackgroundWorker backgroundWorker;

        private Dictionary<int, string> pageResult = new Dictionary<int,string>();//sort the highlighted content and the related pages
        private string hilightedFileName;
        //private int previousPageNo = 0; //previous pdf page no.

        public MainWindow()
        {
            InitializeComponent();

            InitializeToolTip();
            InitializeUI();
            InitializeTheme();
            InitializeFolder();
        }

        private void InitializeFolder()
        {
            FileMg.InitDataFolder();
        }

        private void InitializeToolTip()
        {
            mToolTip.Active = true;
            mToolTip.AutoPopDelay = 5000;
            mToolTip.InitialDelay = 1000;
            mToolTip.ReshowDelay = 500;
        }

        private void InitializeUI() 
        {
            InitListView();

            tabControl.Appearance = TabAppearance.FlatButtons;
            tabControl.ItemSize = new Size(0, 1);
            tabControl.SizeMode = TabSizeMode.Fixed;

            scMainPanel.Panel2Collapsed = true;
        }

        private void InitListView()
        {
            int columnNumber = 3;
            ColumnHeader columnAuto = new ColumnHeader();
            columnAuto.Text = "File Name";
            columnAuto.TextAlign = HorizontalAlignment.Center;
            columnAuto.Width = lvRelatedAutoFile.Width / columnNumber;
            lvRelatedAutoFile.Columns.Add(columnAuto);

            //columnAuto = new ColumnHeader();
            //columnAuto.Text = "Similarity";
            //columnAuto.TextAlign = HorizontalAlignment.Center;
            //columnAuto.Width = lvRelatedAutoFile.Width / columnNumber;
            //lvRelatedAutoFile.Columns.Add(columnAuto);

            columnAuto = new ColumnHeader();
            columnAuto.Text = "File Path";
            columnAuto.TextAlign = HorizontalAlignment.Center;
            columnAuto.Width = lvRelatedAutoFile.Width / columnNumber;
            lvRelatedAutoFile.Columns.Add(columnAuto);

            columnAuto = new ColumnHeader();
            columnAuto.Text = "Topic Name";
            columnAuto.TextAlign = HorizontalAlignment.Center;
            columnAuto.Width = lvRelatedAutoFile.Width / columnNumber;
            lvRelatedAutoFile.Columns.Add(columnAuto);

            ColumnHeader columnManual = new ColumnHeader();
            columnManual.Text = "File Name";
            columnManual.TextAlign = HorizontalAlignment.Left;
            columnManual.Width = lvRelatedManualFile.Width / columnNumber;
            lvRelatedManualFile.Columns.Add(columnManual);

            //columnManual = new ColumnHeader();
            //columnManual.Text = "Similarity";
            //columnManual.TextAlign = HorizontalAlignment.Left;
            //columnManual.Width = lvRelatedManualFile.Width / columnNumber;
            //lvRelatedManualFile.Columns.Add(columnManual);

            columnManual = new ColumnHeader();
            columnManual.Text = "File Path";
            columnManual.TextAlign = HorizontalAlignment.Left;
            columnManual.Width = lvRelatedManualFile.Width / columnNumber;
            lvRelatedManualFile.Columns.Add(columnManual);

            columnManual = new ColumnHeader();
            columnManual.Text = "Component Name";
            columnManual.TextAlign = HorizontalAlignment.Left;
            columnManual.Width = lvRelatedManualFile.Width / columnNumber;
            lvRelatedManualFile.Columns.Add(columnManual);


        }

        private void InitializeTheme()
        {
            // Initialize MaterialSkinManager
            mMaterialSkinManager = MaterialSkinManager.Instance;
            mMaterialSkinManager.AddFormToManage(this);
            mMaterialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            //mMaterialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
            //mMaterialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo500, Primary.Indigo700, Primary.Indigo100, Accent.Pink200, TextShade.WHITE);
            //mMaterialSkinManager.ColorScheme = new ColorScheme(Primary.Green600, Primary.Green700, Primary.Green200, Accent.Red100, TextShade.WHITE);
        }

        private void RecalculateListView()
        {
            foreach (ColumnHeader column in lvRelatedAutoFile.Columns)
            {
                column.Width = lvRelatedAutoFile.Width / lvRelatedAutoFile.Columns.Count;
            }

            foreach (ColumnHeader column in lvRelatedManualFile.Columns)
            {
                column.Width = lvRelatedManualFile.Width / lvRelatedManualFile.Columns.Count;
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            gViewer.ObjectUnderMouseCursorChanged += new EventHandler<ObjectUnderMouseCursorChangedEventArgs>(gViewer_ObjectUnderMouseCursorChanged);
            CreateGraph();
        }

        private void gViewer_ObjectUnderMouseCursorChanged(object sender, ObjectUnderMouseCursorChangedEventArgs e)
        {
            mSelectedObject = e.OldObject != null ? e.OldObject.DrawingObject : null;

            if (mSelectedObject != null)
            {
                if (mSelectedObject is Edge)
                    (mSelectedObject as Edge).Attr = mSelectedObjectAttr as EdgeAttr;
                else if (mSelectedObject is Node)
                    (mSelectedObject as Node).Attr = mSelectedObjectAttr as NodeAttr;

                mSelectedObject = null;
            }

            if (gViewer.SelectedObject == null)
            {
                labelSelected.Text = "";
                this.gViewer.SetToolTip(mToolTip, "");
            }
            else
            {
                mSelectedObject = gViewer.SelectedObject;
                Edge edge = mSelectedObject as Edge;
                if (edge != null)
                {
                    mSelectedObjectAttr = edge.Attr.Clone();
                    edge.Attr.Color = Microsoft.Msagl.Drawing.Color.Magenta;

                    //here we can use e.Attr.Id or e.UserData to get back to the user data
                    this.gViewer.SetToolTip(mToolTip, String.Format("edge from {0} {1}", edge.Source, edge.Target));

                }
                else if (mSelectedObject is Node)
                {

                    mSelectedObjectAttr = (gViewer.SelectedObject as Node).Attr.Clone();
                    (mSelectedObject as Node).Attr.Color = Microsoft.Msagl.Drawing.Color.Magenta;
                    //here you can use e.Attr.Id to get back to your data
                    this.gViewer.SetToolTip(mToolTip, String.Format("node {0}", (mSelectedObject as Node).Attr.Id));
                }

                labelSelected.Text = mSelectedObject.ToString();
            }

            gViewer.Invalidate();
        }

        private void InsertNodeIntoGraph(Rectangle rectangle)
        {
            Node node = new Node("testNode");
            node.Attr.FillColor = Color.Red;
            node.Attr.Shape = Microsoft.Msagl.Drawing.Shape.DrawFromGeometry;
            node.Label = null;
            var geomNode =
                node.GeometryNode = GeometryGraphCreator.CreateGeometryNode(gViewer.Graph, gViewer.Graph.GeometryGraph, node, ConnectionToGraph.Disconnected);
            var center = (rectangle.LeftBottom + rectangle.RightTop) / 2;
            geomNode.BoundaryCurve = CurveFactory.CreateRectangle(rectangle.Width, rectangle.Height, center);
            node.GeometryNode = geomNode;
            var dNode = gViewer.CreateIViewerNode(node);
            gViewer.AddNode(dNode, true);
        }

        private Point GetRandomCenter(double nodeHeight, double nodWidth, Random random)
        {
            double x = random.NextDouble();
            double y = random.NextDouble();
            x = gViewer.Graph.Left + nodWidth / 2 + (gViewer.Graph.Width - nodWidth) * x;
            y = gViewer.Graph.Bottom + nodeHeight / 2 + (gViewer.Graph.Height - nodeHeight) * y;
            return new Point(x, y);
        }

        IEnumerable<RectangleNode<object>> GetRectangleNodesFromGraph()
        {
            var graph = gViewer.Graph.GeometryGraph;
            foreach (var node in graph.Nodes)
                yield return new RectangleNode<object>(node, node.BoundingBox);
            foreach (var edge in graph.Edges)
            {
                foreach (var edgeRectNode in EdgeRectNodes(edge))
                    yield return edgeRectNode;

            }
        }

        IEnumerable<RectangleNode<object>> EdgeRectNodes(GeomEdge edge)
        {
            const int parts = 64; //divide each edge into 64 segments
            var curve = edge.Curve;
            double delta = (curve.ParEnd - curve.ParStart) / parts;
            Point p0 = curve.Start;
            for (int i = 1; i <= parts; i++)
                yield return new RectangleNode<object>(edge, new Rectangle(p0, p0 = curve[curve.ParStart + i * delta]));

            if (edge.ArrowheadAtSource)
                yield return new RectangleNode<object>(edge, new Rectangle(edge.EdgeGeometry.SourceArrowhead.TipPosition, curve.Start));

            if (edge.ArrowheadAtTarget)
                yield return new RectangleNode<object>(edge, new Rectangle(edge.EdgeGeometry.TargetArrowhead.TipPosition, curve.End));
        }

        private bool IsNeedShow(double weight)
        {
            if (mSortedNodeWeightList.Count > mNodeNumberThreshold && 
                weight > mSortedNodeWeightList[mNodeNumberThreshold])
            {
                return true;
            }

            return false;
        }

        private void CreateGraph()
        {
            Graph graph = new Graph();

            if (mNodeList.Count == 0 || mEdgeList.Count == 0)
            {
                return;
            }

            for (int index = 0; index < mNodeList.Count; index++)
            {
                if (IsNeedShow(mNodeList[index].Weight))
                {
                    Node node = new Node(mNodeList[index].Name);
                    node.Attr.LabelMargin = (int)(mNodeList[index].Weight / mMinNodeWeight * 2);//node size
                    node.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Circle;
                    graph.AddNode(node);
                }
            }

            double maxWeight = 0;
            Edge maxWeightEdge = null;
            for (int indexX = 0; indexX < mNodeList.Count - 1; indexX++)
            {
                for (int indexY = indexX + 1; indexY < mNodeList.Count; indexY++)
                {
                    if (IsNeedShow(mNodeList[indexX].Weight) && IsNeedShow(mNodeList[indexY].Weight))
                    {
                        double edgeWeight = mEdgeList[indexX][indexY];
                        //if (edgeWeight > mEdgeWeightThreshold)
                        {
                            Edge edge = graph.AddEdge(mNodeList[indexX].Name, mNodeList[indexY].Name) as Edge;
                            edge.Attr.ArrowheadLength = 1;
                            if (edgeWeight > maxWeight)
                            {
                                maxWeight = edgeWeight;
                                maxWeightEdge = edge;
                            }
                        }
                    }
                }
            }

            if(maxWeightEdge != null)
            {
                maxWeightEdge.Attr.LineWidth = 3;
            }

            gViewer.Graph = graph;
        }

        private void CreateTreeGraph()
        {
            Graph graph = new Graph();

            if (mBiTreeRoot == null || mNodeList == null || mNodeList.Count == 0)
            {
                return;
            }

            Node node = graph.AddNode("0");
            node.LabelText = "";
            node.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Octagon;
            node.Attr.FillColor = Color.LightGray;

            AddTreeNode(graph, mBiTreeRoot, "0");

            gViewer.Graph = graph;
        }

        private void AddTreeNode(Graph graph, BiNode biNode, string nodeName)
        {
            string sourceName = null;

            if(biNode.Value != -1)
            {
                sourceName = mNodeList[biNode.Value - 1].Name;
            }
            else
            {
                sourceName = nodeName;
            }

            if (biNode.Left != null)
            {
                string targetName = null;
                if(biNode.Left.Value != -1)
                {
                    targetName = mNodeList[biNode.Left.Value - 1].Name;
                    Edge edge = graph.AddEdge(sourceName, targetName);
                    edge.TargetNode.Attr.LabelMargin = 5;
                }
                else
                {
                    targetName = nodeName + "0";
                    Edge edge = graph.AddEdge(sourceName, targetName);
                    edge.TargetNode.LabelText = "";
                    edge.TargetNode.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Circle;
                    edge.TargetNode.Attr.FillColor = Color.LightGray;
                }
                
                AddTreeNode(graph, biNode.Left, targetName);
            }

            if (biNode.Right != null)
            {
                string targetName = null;
                if (biNode.Right.Value != -1)
                {
                    targetName = mNodeList[biNode.Right.Value - 1].Name;
                    Edge edge = graph.AddEdge(sourceName, targetName);
                    edge.TargetNode.Attr.LabelMargin = 5;
                }
                else
                {
                    targetName = nodeName + "1";
                    Edge edge = graph.AddEdge(sourceName, targetName);
                    edge.TargetNode.LabelText = "";
                    edge.TargetNode.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Circle;
                    edge.TargetNode.Attr.FillColor = Color.LightGray;
                }
                
                AddTreeNode(graph, biNode.Right, targetName);
            }
        }

        private static void CreateSourceNode(Node a)
        {
            a.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Box;
            a.Attr.XRadius = 4;
            a.Attr.YRadius = 4;
            a.Attr.FillColor = Microsoft.Msagl.Drawing.Color.Green;
            a.Attr.LineWidth = 10;
        }

        private void CreateTargetNode(Node a)
        {
            a.Attr.Shape = Microsoft.Msagl.Drawing.Shape.DoubleCircle;
            a.Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightGray;

            a.Attr.LabelMargin = -4;
            a.UserData = this;
        }

        private void RecalcluateGraph()
        {
            //CreateGraph();
            CreateTreeGraph();
        }

        private void recalcluateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RecalcluateGraph();
        }

        private void insertNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gViewer == null || gViewer.Graph == null)
            {
                MessageBox.Show("Please create graph first.", "Insert Node Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var tree = RectangleNode<object>.CreateRectangleNodeOnEnumeration(GetRectangleNodesFromGraph());
            if (tree == null)
            {
                return;
            }

            var numberOfTries = 10000;
            Random random = new Random(1);
            double rectWidth = 100;
            double rectHeight = 100;
            var delta = new Point(rectWidth / 2, rectHeight / 2);
            Rectangle bestRectangle = Rectangle.CreateAnEmptyBox();

            Point hint = (gViewer.Graph.BoundingBox.LeftBottom + gViewer.Graph.BoundingBox.RightTop) / 2;
            double minDistance = double.PositiveInfinity;
            for (int i = 0; i < numberOfTries; i++)
            {
                Point randomCenter = GetRandomCenter(rectHeight, rectWidth, random);
                Rectangle r = new Rectangle(randomCenter);
                r.Add(randomCenter + delta);
                r.Add(randomCenter - delta);
                if (tree.GetNodeItemsIntersectingRectangle(r).Any()) { }
                else
                {
                    var len = (randomCenter - hint).LengthSquared;
                    if (len < minDistance)
                    {
                        minDistance = len;
                        bestRectangle = r;
                    }
                }
            }

            if (bestRectangle.IsEmpty == false)
                InsertNodeIntoGraph(bestRectangle);
            else
                MessageBox.Show("cannot insert");
        }

        private void gViewer_Click(object sender, EventArgs e)
        {
            if (mSelectedObject != null && mSelectedObject is Node)
            {
                string id = (mSelectedObject as Node).Attr.Id;
                lvRelatedAutoFile.Items.Clear();                
                if (mSimilarityDictionary.ContainsKey(id) && mSimilarityDictionary[id] != null)
                {
                    for (int index = 0; index < mSimilarityDictionary[id].Count; index++)
                    {
                        RelatedFileModel model = mSimilarityDictionary[id][index];
                        double similarity = Double.Parse(model.Similarity);
                        if (similarity > 0.5 && model.RelatedFileName != null)
                        {
                            string rawFileName = model.RelatedFileName;
                            ListViewItem lvt = new ListViewItem(model.RelatedFileName);
                            //lvt.SubItems.Add(model.Similarity);
                            lvt.SubItems.Add(model.RelatedFilePath);
                            lvt.SubItems.Add(model.TopicName);
                            lvRelatedAutoFile.Items.Add(lvt);
                        }
                    }
                }
            }
        }

        private void createFromDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //ImportAutoDataWindow dlg = new ImportAutoDataWindow();
            //DialogResult dr = dlg.ShowDialog();
            //if (dr == DialogResult.OK)
            //{
            //    scMainPanel.Panel2Collapsed = false;
            //    scOutputResult.Panel1Collapsed = true;
            //    scOutputResult.Panel2Collapsed = false;

            //    string rawfiledir = dlg.RawFileDir;

            //    backgroundWorker = new BackgroundWorker();
            //    backgroundWorker.WorkerReportsProgress = true;
            //    backgroundWorker.DoWork += new DoWorkEventHandler(GenerateAutoData_DoWork);
            //    backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(GenerateAutoData_ProgressChanged);
            //    backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GenerateAutoData_Completed);
            //    backgroundWorker.RunWorkerAsync(rawfiledir);
            //}
            //else if (dr == DialogResult.Retry)
            //{
            //    RenderAutoData();
            //}

            GenerateAutoDataWizard dlg = new GenerateAutoDataWizard();
            DialogResult dr = dlg.ShowDialog();
            if (dr == DialogResult.OK)
            {
                RenderAutoData();
                tabControl.SelectedIndex = 1;
            }
        }

        private void GenerateAutoData_DoWork(object sender, DoWorkEventArgs e)
        {
           e.Result = GenerateAutoDataMg.StartGenerate(backgroundWorker, (string)(e.Argument));
        }

        private void GenerateAutoData_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateOutputResult(e.UserState.ToString());
        }

        private void GenerateAutoData_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!(bool)(e.Result))
            {
                MessageBox.Show("Generating automated model failed! Please check the ouput window to fix the problem and run again.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            RenderAutoData();
        }

        private void RenderAutoData()
        {
            RenderDataMg renderDataMg = new RenderDataMg();
            if (renderDataMg.PrepareRender())
            {
                mNodeList = renderDataMg.NodeList;
                mMinNodeWeight = renderDataMg.MinNodeWeight;
                mEdgeList = renderDataMg.EdgeList;
                mSimilarityDictionary = renderDataMg.SimilarityDictionary;
                mSortedNodeWeightList = renderDataMg.SortedNodeWeightList;
                mNodeNumberThreshold = renderDataMg.NodeNumberThreshold;
                mEdgeWeightThreshold = renderDataMg.EdgeWeightThreshold;
                mTopicTermFilePath = renderDataMg.TopicTermFilePath;
                mBiTreeRoot = renderDataMg.BiTreeRoot;

                RecalcluateGraph();
                tabControl.SelectedIndex = 1;
            }
        }
        
        private void createFromManualDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportManualDataWindow dlg = new ImportManualDataWindow();
            DialogResult dr = dlg.ShowDialog();
            if (dr == DialogResult.OK)
            {
                scMainPanel.Panel2Collapsed = false;
                scOutputResult.Panel1Collapsed = true;
                scOutputResult.Panel2Collapsed = false;

                string[] paths = { dlg.RawFileDir, dlg.ModelFilePath };
                backgroundWorker = new BackgroundWorker();
                backgroundWorker.WorkerReportsProgress = true;
                backgroundWorker.DoWork += new DoWorkEventHandler(GenerateManualData_DoWork);
                backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(GenerateManualData_ProgressChanged);
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GenerateManualData_Completed);
                backgroundWorker.RunWorkerAsync(paths);
            }
            else if (dr == DialogResult.Retry)
            {
                RenderManualData(Configures.GetManualModelFilePath(), Configures.GetManualSearchTermPath());
            }


            //step by step
            //GenerateManualDataWizard dlg = new GenerateManualDataWizard();
            //DialogResult dr = dlg.ShowDialog();
            //if (dr == DialogResult.OK)
            //{
            //    RenderManualData(Configures.GetManualWizardModelFilePath());
            //}
        }
        
        private void GenerateManualData_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] args = (string[])(e.Argument);
            e.Result = GenerateManualDataMg.StartGenerate(backgroundWorker, args[0], args[1]);
        }

        private void GenerateManualData_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateOutputResult(e.UserState.ToString());
        }

        private void GenerateManualData_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!(bool)(e.Result))
            {
                MessageBox.Show("Generating manual model failed! Please check the ouput window to fix the problem and run again.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            RenderManualData(Configures.GetManualModelFilePath(), FileMg.ManualTopicLabelFileDir + Constants.TopicManualTermFileName);
        }

        private void RenderManualData(string modelFilePath, string searchTermFilePath)
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += new DoWorkEventHandler(PrepareManualData_DoWork);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(PrepareManualData_ProgressChanged);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(PrepareManualData_Completed);
            string[] paths = { modelFilePath, searchTermFilePath, null };
            backgroundWorker.RunWorkerAsync(paths);
        }

        private void GenerateManualSummaryData(string searchTermFilePath, string compName)
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += new DoWorkEventHandler(PrepareManualData_DoWork);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(PrepareManualData_ProgressChanged);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(PrepareManualData_Completed);
            string[] paths = { null, searchTermFilePath, compName };
            backgroundWorker.RunWorkerAsync(paths);
        }

        private void PrepareManualData_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] args = (string[])(e.Argument);
            if (!string.IsNullOrEmpty(args[2]))
            {
                mRenderManualDataMg.GenerateCompSummary(backgroundWorker, args[1], FileMg.ManualCleanComponentFileDir, args[2]);
                e.Result = args[2]; // not render tree
            }
            else
            {
                mRenderManualDataMg.PrepareRender(backgroundWorker, args[0], args[1], FileMg.ManualCleanComponentFileDir);
                e.Result = null; // render tree
            }
        }

        private void PrepareManualData_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateOutputResult(e.UserState.ToString());
        }

        private void PrepareManualData_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            string compName = (string)(e.Result);
            mTreeRoot = mRenderManualDataMg.TreeRoot;
            mSimilarityDictionary = mRenderManualDataMg.SimilarityDictionary;
            mCompSummDictionary = mRenderManualDataMg.CompSummDictionary;
            mTopicTermFilePath = mRenderManualDataMg.TopicTermFilePath;
            tbSummary.Text = String.Empty;
            if (!string.IsNullOrEmpty(compName) && mCompSummDictionary.ContainsKey(compName))
            {
                tbSummary.Text = mCompSummDictionary[compName];
                materialPanel3.HeaderText = "Summary - " + compName;
            }
            if (string.IsNullOrEmpty(compName) && mTreeRoot != null && mTreeRoot.SubElements != null)
            {
                treeView.Nodes.Clear();
                TreeNode node = treeView.Nodes.Add(mTreeRoot.Text);
                foreach (TreeElement elem in mTreeRoot.SubElements)
                {
                    UpdateTree(elem, node);
                }
            }

            tabControl.SelectedIndex = 2;
            scMainPanel.Panel2Collapsed = false;
            scOutputResult.Panel1Collapsed = false;
            scOutputResult.Panel2Collapsed = false;
        }

        private void UpdateTree(TreeElement treeElem, TreeNode treeNode)
        {
            TreeNode node = treeNode.Nodes.Add(treeElem.Text);
            if(treeElem.SubElements != null)
            {
                foreach (TreeElement elem in treeElem.SubElements)
                {
                    UpdateTree(elem, node);
                }
            }
        }

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            lvRelatedManualFile.Items.Clear();
            string id = e.Node.Text;
            if (!mCompSummDictionary.ContainsKey(id))
            {
                GenerateManualSummaryData(FileMg.ManualTopicLabelFileDir + Constants.TopicManualTermFileName, id);
            }

            if (mCompSummDictionary.ContainsKey(id))
            {
                tbSummary.Text = mCompSummDictionary[id];
                materialPanel3.HeaderText = "Summary - " + id;
            }

            if (mSimilarityDictionary.ContainsKey(id) && mSimilarityDictionary[id] != null)
            {
                for (int index = 0; index < mSimilarityDictionary[id].Count; index++)
                {
                    RelatedFileModel model = mSimilarityDictionary[id][index];
                    double similarity = Double.Parse(model.Similarity);
                    if (similarity > 0)
                    {
                        ListViewItem lvt = new ListViewItem(model.RelatedFileName);
                        //lvt.SubItems.Add(model.Similarity);
                        lvt.SubItems.Add(model.RelatedFilePath);
                        lvt.SubItems.Add(model.TopicName);
                        lvRelatedManualFile.Items.Add(lvt);
                    }
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutWindow dlg = new AboutWindow();
            dlg.ShowDialog();
        }

        private void configureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureWindow dlg = new ConfigureWindow();
            DialogResult dr = dlg.ShowDialog();
        }

        /// <summary>
        /// we didn't create the summary for the automated topics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvRelatedAutoFile_DoubleClick(object sender, EventArgs e)
        {
            string targetTopic = lvRelatedAutoFile.SelectedItems[0].SubItems[2].Text;
            string sourceFileName = lvRelatedAutoFile.SelectedItems[0].SubItems[0].Text;
            string sourceFilePath = lvRelatedAutoFile.SelectedItems[0].SubItems[1].Text;
            if (!Directory.Exists(FileMg.AutoHighlightFileDir))
            {
                Directory.CreateDirectory(FileMg.AutoHighlightFileDir);
            }


            HighlightFile(targetTopic, sourceFileName, sourceFilePath, FileMg.AutoHighlightFileDir + sourceFileName);
        }

        private void lvRelatedManualFile_Click(object sender, EventArgs e)
        {
            string targetTopic = lvRelatedManualFile.SelectedItems[0].SubItems[2].Text;
            string sourceFileName = lvRelatedManualFile.SelectedItems[0].SubItems[0].Text;
            string sourceFilePath = lvRelatedManualFile.SelectedItems[0].SubItems[1].Text;
            if (!Directory.Exists(FileMg.ManualHighlightFileDir))
            {
                Directory.CreateDirectory(FileMg.ManualHighlightFileDir);
            }

            HighlightFile(targetTopic, sourceFileName, sourceFilePath, FileMg.ManualHighlightFileDir + sourceFileName);
        }

        private void lvRelatedManualFile_DoubleClick(object sender, EventArgs e)
        {
            string targetTopic = lvRelatedManualFile.SelectedItems[0].SubItems[2].Text;
            string sourceFileName = lvRelatedManualFile.SelectedItems[0].SubItems[0].Text;
            string sourceFilePath = lvRelatedManualFile.SelectedItems[0].SubItems[1].Text;
            if (!Directory.Exists(FileMg.ManualHighlightFileDir))
            {
                Directory.CreateDirectory(FileMg.ManualHighlightFileDir);
            }

            HighlightFile(targetTopic, sourceFileName, sourceFilePath, FileMg.ManualHighlightFileDir + sourceFileName);
        }

        private void HighlightFile(string targetTopic, string sourceFileName, string sourceFilePath, string targetFilePath)
        {
            //if (!sourceFileName.EndsWith("pdf"))
            //{
            //    MessageBox.Show("Current document type is not support. We only support PDF now!");
            //    return;
            //} //edit by xlian

            scMainPanel.Panel2Collapsed = false;
            scOutputResult.Panel1Collapsed = true;
            scOutputResult.Panel2Collapsed = false;

            string[] args = { targetTopic, sourceFileName, sourceFilePath, targetFilePath };
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += new DoWorkEventHandler(HighlightFile_DoWork);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(HighlightFile_ProgressChanged);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(HighlightFile_Completed);
            backgroundWorker.RunWorkerAsync(args);            
        }

        //Revised by xlian by adding MS Word highlighting
        //02/15/2016
        private void HighlightFile_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {                
                string[] args = (string[])(e.Argument);
                string originalFilePath = args[2];
                string hilightedPath = args[3];
                Dictionary<int, string> pageSummaryList = new Dictionary<int,string>();
                if (originalFilePath.EndsWith(".pdf"))
                {
                    HighlightPDFProcessor pdfHighlight = new HighlightPDFProcessor(args[2], mTopicTermFilePath, args[0], args[3]);
                    pageSummaryList = pdfHighlight.ExecuteHighlight(backgroundWorker);
                }
                else //word processing
                {
                    HighlightDoc wordHighlight = new HighlightDoc(originalFilePath, mTopicTermFilePath, args[0], hilightedPath);
                    pageSummaryList = wordHighlight.ExecuteHighlight(backgroundWorker);
                }
                object[] results = { args[3], pageSummaryList, true };
                e.Result = results;
            }
            catch (Exception)
            {
               // MessageBox.Show(ex.Message);
                object[] results = { null, null, false };
                e.Result = results;
            }
        }

        private void HighlightFile_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateOutputResult(e.UserState.ToString());
        }

        private void HighlightFile_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            scMainPanel.Panel2Collapsed = false;
            scOutputResult.Panel1Collapsed = false;
            scOutputResult.Panel2Collapsed = false;

            object[] results = (object[])(e.Result);

            if ((bool)(results[2]))
            {
                UpdateOutputResult("Highlight Finished!");
                pageResult = (Dictionary<int, string>)results[1];

                hilightedFileName = (string)results[0];
                pageResult = CleanPageResult(pageResult);

              //  UpdateSummaryResultTree((string)results[0], (Dictionary<int, string>)results[1]); //store this result
                UpdateSummaryResultTree(hilightedFileName, pageResult);
                
                //Process.Start("explorer.exe", "/select," + (string)results[0]);
            }
            else
            {
                UpdateOutputResult("Hightlight failed!! Highlight pdf file failed, please check the original pdf for permission!");
            }            
        }

        /*
         * if the highlighted string contains some special character, remove them
         */
        private Dictionary<int, string> CleanPageResult(Dictionary<int, string> pageResult)
        {
            Dictionary<int, string> result = new Dictionary<int, string>();
            Regex wordRegex = new Regex(@"[\.{2,*} | \{ | \} |\( |\)]");
            foreach (int pageIndex in pageResult.Keys)
            {
                string content = pageResult[pageIndex];
                string tmpContent = wordRegex.Replace(content, " ");
                result.Add(pageIndex, tmpContent);
            }
            return result;
        }

        private void UpdateSummaryResultTree(string fileName, Dictionary<int, string> pageSummaryList)
        {
            tvHighlightingResult.Nodes.Clear();
            TreeNode rootNode = tvHighlightingResult.Nodes.Add("fileName", fileName);
            
            if(pageSummaryList != null)
            {
                for (int index = 0; index < pageSummaryList.Count; index++)
                {
                    string hightContent = pageSummaryList.Values.ElementAt(index);
                    if(string.IsNullOrEmpty(hightContent.Trim()))
                    {
                        continue;
                    }

                    string tag = pageSummaryList.Keys.ElementAt(index).ToString();
                    TreeNode node = rootNode.Nodes.Add("Page " + tag);
                    node.Tag = tag;                    
                    string [] lines = hightContent.Split('\n');
                    if (lines != null && lines.Length > 0)
                    {
                        foreach (string line in lines)
                        {
                            if(!string.IsNullOrEmpty(line.Trim()))
                            {
                                node.Nodes.Add(line).Tag = tag;    
                            }
                        }
                    }
                }
            }
            
        }

        private void tvSummaryResult_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
                object pageNumber = e.Node.Tag;
                string filePath = tvHighlightingResult.Nodes[0].Text;
                string fileName = FileNameParse.GetFileName(filePath);
                Process myProcess = new Process();
                if (filePath.EndsWith(".pdf"))
                {
                    Process[] collectionOfProcess = Process.GetProcessesByName("AcroRd32");
                    foreach (Process p in collectionOfProcess)
                    {
                        string runningFile = p.MainWindowTitle;
                        if (runningFile.Contains("- Adobe Reader"))
                        {
                            int adobeIndex = runningFile.IndexOf("- Adobe Reader");
                            runningFile = runningFile.Substring(0, adobeIndex - 1);
                        }
                        
                        if (runningFile.Equals(fileName))
                        {
                            p.Kill();
                        }
                    }

                    try
                    {
                        myProcess.StartInfo.FileName = "AcroRd32.exe";
                        myProcess.StartInfo.Arguments = string.Format("/A \"page={0}\" \"{1}\"", pageNumber, filePath);
                        myProcess.Start();
                    }
                    
                    catch
                    {
                        MessageBox.Show("Failed to open pdf file. We need adobe reader to open the pdf file. Please make sure you have setup Adobe reader.");
                    }
                }
                else
                {
                    object missing = System.Reflection.Missing.Value;

                    int pageNumValue = Convert.ToInt32(pageNumber);
                    bool isActive = Relocate(filePath, pageNumValue);

                    if (!isActive)
                    {
                        try
                        {
                            Microsoft.Office.Interop.Word.Application app = new Microsoft.Office.Interop.Word.Application();

                            app.Visible = true;
                            object readOnly = false;

                            var doc = app.Documents.Open(filePath, missing, readOnly);
                            object what = WdGoToItem.wdGoToPage;
                            object which = WdGoToDirection.wdGoToAbsolute;
                            Range range = app.Selection.GoTo(what, which, pageNumber, missing);
                            doc.Activate();
                            app.Activate();
                            doc.Save();
                        }
                        catch (Exception)
                        {

                        }
                    }
                    
                }
        }

        private bool Relocate(string hilightedPath, int pageNumber)
        {
            try
            {
                Microsoft.Office.Interop.Word.Application activeApp = (Microsoft.Office.Interop.Word.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Word.Application");
                if (activeApp != null)
                {
                    foreach (Microsoft.Office.Interop.Word.Document d in activeApp.Documents)
                    {
                        if (d.FullName.ToLower() == hilightedPath.ToLower())
                        {
                            object missing = System.Reflection.Missing.Value;
                            object what = WdGoToItem.wdGoToPage;
                            object which = WdGoToDirection.wdGoToAbsolute;
                            Range range = activeApp.Selection.GoTo(what, which, pageNumber, missing);
                            d.Save();
                            return true;
                        }
                    }
                }
                //  activeApp.Quit();
            }
            catch (Exception)
            {
                
            }
            return false;

        }

        private void UpdateOutputResult(string outputText)
        {
            tbExecuteResult.Text += outputText + "\r\n";
            tbExecuteResult.SelectionStart = tbExecuteResult.TextLength;
            tbExecuteResult.ScrollToCaret();
        }

        private void viewHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //IExplorerMg.RunIExplorerTool(FileMg.MapRelToAbsFilePath(Constants.HelpPageFilePath));
            try
            {
                object pageNumber = 1;
                string filePath = FileMg.MapRelToAbsFilePath(Constants.HelpPageFilePath);
                Process myProcess = new Process();
                myProcess.StartInfo.FileName = "AcroRd32.exe";
                myProcess.StartInfo.Arguments = string.Format("/A \"page={0}\" \"{1}\"", pageNumber, filePath);
                myProcess.Start();
            }
            catch
            {
                MessageBox.Show("Failed to open pdf file. We need adobe reader to open the pdf file. Please make sure you have setup Adobe reader.");
            }
        }

        private void MainWindow_SizeChanged(object sender, EventArgs e)
        {
            RecalculateListView();
        }

        private void gViewer_Load(object sender, EventArgs e)
        {

        }

        //select the page order seq
        private void comboBox_pageSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTerm = (string)comboBox_pageSort.SelectedItem;
            if (selectedTerm.Equals("list pages by relevance"))
            {
                Dictionary<int, string> sortedPageContent = new Dictionary<int, string>();
                sortedPageContent = pageResult.OrderByDescending(x => x.Value.Length).ToDictionary(x => x.Key, x => x.Value);
                UpdateSummaryResultTree(hilightedFileName, sortedPageContent);
            }
            else
            {
                UpdateSummaryResultTree(hilightedFileName, pageResult);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Dictionary<int, string> sortedPageContent = new Dictionary<int, string>();
            sortedPageContent = pageResult.OrderByDescending(x => x.Value.Length).ToDictionary(x => x.Key, x => x.Value);
            if (sortedPageContent.Count == 0)
            {
                return;
            }

            int mostRelevantPage = sortedPageContent.Keys.ElementAt(0);

            string filePath = tvHighlightingResult.Nodes[0].Text;
            Process myProcess = new Process();
            if (filePath.EndsWith(".pdf"))
            {
                string fileName = FileNameParse.GetFileName(filePath);
                Process[] collectionOfProcess = Process.GetProcessesByName("AcroRd32");
                foreach (Process p in collectionOfProcess)
                {
                    string runningFile = p.MainWindowTitle;
                    if (runningFile.Contains("- Adobe Reader"))
                    {
                        int adobeIndex = runningFile.IndexOf("- Adobe Reader");
                        runningFile = runningFile.Substring(0, adobeIndex - 1);
                    }

                    if (runningFile.Equals(fileName))
                    {
                        p.Kill();
                    }
                }

                try
                {
                    myProcess.StartInfo.FileName = "AcroRd32.exe";
                    myProcess.StartInfo.Arguments = string.Format("/A \"page={0}\" \"{1}\"", mostRelevantPage, filePath);
                    myProcess.Start();
                }

                catch
                {
                    MessageBox.Show("Failed to open pdf file. We need adobe reader to open the pdf file. Please make sure you have setup Adobe reader.");
                }
            }
            else
            {
                object missing = System.Reflection.Missing.Value;

                int pageNumValue = Convert.ToInt32(mostRelevantPage);
                bool isActive = Relocate(filePath, pageNumValue);

                if (!isActive)
                {
                    try
                    {
                        Microsoft.Office.Interop.Word.Application app = new Microsoft.Office.Interop.Word.Application();

                        app.Visible = true;
                        object readOnly = false;

                        var doc = app.Documents.Open(filePath, missing, readOnly);
                        object what = WdGoToItem.wdGoToPage;
                        object which = WdGoToDirection.wdGoToAbsolute;
                        Range range = app.Selection.GoTo(what, which, mostRelevantPage, missing);
                        doc.Activate();
                        app.Activate();
                        doc.Save();
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
    }
}
