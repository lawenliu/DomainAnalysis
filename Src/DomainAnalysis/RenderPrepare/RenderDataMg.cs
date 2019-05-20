using DomainAnalysis.DataPrepare;
using DomainAnalysis.Model;
using DomainAnalysis.Models;
using DomainAnalysis.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainAnalysis.RenderPrepare
{
    class RenderDataMg
    {
        private List<NodeModel> mNodeList = new List<NodeModel>();
        private List<double> mSortedNodeWeightList = new List<double>();
        private double mMinNodeWeight = double.MaxValue;
        private List<List<double>> mEdgeList = new List<List<double>>();
        private Dictionary<string, List<RelatedFileModel>> mSimilarityDictionary = new Dictionary<string, List<RelatedFileModel>>();
        private int mNodeNumberThreshold = 10;
        private double mEdgeWeightThreshold = 4.0;
        private string mTopicTermFilePath = FileMg.AutoTopicLabelFileDir + Constants.TopicTermFileName;
        private BiNode mBiTreeRoot = null;

        public RenderDataMg()
        { }

        public bool PrepareRender()
        {
            LoadTopicLabelFile();
            LoadToicSimilarity();
            TryToLoadSimilarityFile();
            TryToLoadBiTree();

            if (mNodeList == null || mNodeList.Count == 0
                || mEdgeList == null)
            {
                return false;
            }

            return true;
        }

        public List<NodeModel> NodeList
        {
            get { return mNodeList; }
        }

        public List<double> SortedNodeWeightList
        {
            get { return mSortedNodeWeightList; }
        }

        public List<List<double>> EdgeList
        {
            get { return mEdgeList; }
        }

        public Dictionary<string, List<RelatedFileModel>> SimilarityDictionary
        {
            get { return mSimilarityDictionary; }
        }

        public double MinNodeWeight
        {
            get { return mMinNodeWeight; }
        }

        public int NodeNumberThreshold
        {
            get { return mNodeNumberThreshold; }
        }

        public double EdgeWeightThreshold
        {
            get { return mEdgeWeightThreshold; }
        }

        public string TopicTermFilePath
        {
            get { return mTopicTermFilePath; }
        }

        public BiNode BiTreeRoot
        {
            get { return mBiTreeRoot; }
        }

        private void LoadTopicLabelFile()
        {
            if (File.Exists(FileMg.AutoTopicLabelFileDir + Constants.TopicLabelFileName))
            {
                try
                {
                    StreamReader sr = new StreamReader(FileMg.AutoTopicLabelFileDir + Constants.TopicLabelFileName);
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();

                        string[] segs = line.Split(':');
                        if (segs.Length == 2)
                        {
                            NodeModel node = new NodeModel();
                            node.Name = segs[0];
                            node.Weight = double.Parse(segs[1]);
                            mNodeList.Add(node);
                            InsertNodeWeight(node.Weight);

                            if (node.Weight < mMinNodeWeight)
                            {
                                mMinNodeWeight = node.Weight;
                            }
                        }
                    }

                    sr.Close();
                }
                catch
                {
                    //MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        public void InsertNodeWeight(double weight)
        {
            int index = 0;
            for (index = 0; index < mSortedNodeWeightList.Count; index++)
            {
                if (weight > mSortedNodeWeightList[index])
                {
                    mSortedNodeWeightList.Insert(index, weight);
                    break;
                }
            }

            if (index >= mSortedNodeWeightList.Count)
            {
                mSortedNodeWeightList.Add(weight);
            }
        }

        private void LoadToicSimilarity()
        {
            if (File.Exists(FileMg.AutoTopicLabelFileDir + Constants.TopicSimilarityFileName))
            {
                try
                {                    
                    StreamReader sr = new StreamReader(FileMg.AutoTopicLabelFileDir + Constants.TopicSimilarityFileName);
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();

                        string[] attrs = line.Split(',');
                        if (attrs.Length == mNodeList.Count)
                        {
                            List<double> arrary = new List<double>();
                            for (int index = 0; index < attrs.Length; index++)
                            {
                                arrary.Add(double.Parse(attrs[index]));
                            }

                            mEdgeList.Add(arrary);
                        }
                        else
                        {
                            break;
                        }
                    }

                    sr.Close();
                }
                catch
                {
                    //MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void TryToLoadSimilarityFile()
        {
            string similarityFilePath = FileMg.AutoTopicLabelFileDir + Constants.TopicRelatedFileName;
            if (File.Exists(similarityFilePath))
            {
                StreamReader sr = new StreamReader(similarityFilePath);
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    string[] attrs = line.Split('\t');
                    if (attrs.Length == 3)
                    {
                        string rawFileName = attrs[1].Trim();
                        string rawFilePath = FileMg.AutoSourceFileDir + rawFileName.Remove(rawFileName.Length - 4);

                        int startIndex = rawFileName.LastIndexOf('\\');
                        if (startIndex > -1)
                        {
                            rawFileName = rawFileName.Substring(startIndex + 1);
                        }

                        rawFileName = rawFileName.Remove(rawFileName.Length - 4);

                        RelatedFileModel model = new RelatedFileModel();
                        model.TopicName = attrs[0];
                        model.Similarity = attrs[2];
                        model.RelatedFileName = rawFileName;
                        model.RelatedFilePath = rawFilePath;

                        if (mSimilarityDictionary.ContainsKey(model.TopicName))
                        {
                            List<RelatedFileModel> similarityList = mSimilarityDictionary[model.TopicName];
                            similarityList.Add(model);
                            mSimilarityDictionary[model.TopicName] = similarityList;
                        }
                        else
                        {
                            List<RelatedFileModel> similarityList = new List<RelatedFileModel>();
                            similarityList.Add(model);
                            mSimilarityDictionary.Add(model.TopicName, similarityList);
                        }
                    }
                }

                sr.Close();
            }
        }

        private void TryToLoadBiTree()
        {
            List<BiNode> biNodeList = new List<BiNode>();
            string biTreeFilePath = FileMg.AutoRDataFileDir + Constants.ROutputFileName;
            if (File.Exists(biTreeFilePath))
            {
                StreamReader sr = new StreamReader(biTreeFilePath);
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine().Trim();

                    string[] attrs = line.Split(' ');
                    if (attrs.Length >= 2)
                    {
                        BiNode leftNode = new BiNode();
                        BiNode rightNode = new BiNode();
                        BiNode curNode = new BiNode();

                        string leftValue = attrs[0];
                        string rightValue = attrs[attrs.Length - 1];

                        if (leftValue.StartsWith("-"))
                        {
                            leftNode.Left = null;
                            leftNode.Right = null;
                            leftNode.Value = Int32.Parse(leftValue.Substring(1));
                        }
                        else
                        {
                            int pos = Int32.Parse(leftValue);
                            leftNode = biNodeList[pos - 1];
                        }

                        if (rightValue.StartsWith("-"))
                        {
                            rightNode.Left = null;
                            rightNode.Right = null;
                            rightNode.Value = Int32.Parse(rightValue.Substring(1));
                        }
                        else
                        {
                            int pos = Int32.Parse(rightValue);
                            rightNode = biNodeList[pos - 1];
                        }

                        curNode.Left = leftNode;
                        curNode.Right = rightNode;
                        curNode.Value = -1;

                        biNodeList.Add(curNode);
                    }
                }

                sr.Close();
            }

            if(biNodeList.Count > 0)
            {
                mBiTreeRoot = biNodeList[biNodeList.Count - 1];
            }            
        }
    }
}
