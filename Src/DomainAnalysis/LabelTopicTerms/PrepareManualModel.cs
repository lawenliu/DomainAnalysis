using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using DomainAnalysis.Utils;
using System.Xml;

namespace DomainAnalysis.PrepareFile
{
    class PrepareManualModel
    {
        string origModelPath = "";
        string fixedFormatPath = "";
        //format: component: term1, term2
        Dictionary<string, List<string>> componentLevel = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> subComponentLevel = new Dictionary<string, List<string>>();

        XmlDocument doc;

        public PrepareManualModel(string manualModelFilePath, string outputTermsFilePath)
        {
            origModelPath = manualModelFilePath;
            fixedFormatPath = outputTermsFilePath;
        }

        public void ParseManualModel()
        {
            doc = new XmlDocument();
            doc.Load(origModelPath);

            //delete the existing file
            if (File.Exists(fixedFormatPath))
            {
                File.Delete(fixedFormatPath);
            }

            //select root node
            XmlNode rootNode = doc.SelectSingleNode("/map");
            XmlNodeList nodeList = rootNode.ChildNodes;


            ParseComponentLevel(nodeList);
            ParseSubcomponentLevel(nodeList);
            WriteDomainTerms(componentLevel);//0114
            WriteDomainTerms(subComponentLevel);
        }

        //componentLevel means office segment, locomotive segment...
        private void ParseComponentLevel(XmlNodeList nodeNode)
        {
            foreach (XmlNode component in nodeNode)
            {
                if (component.HasChildNodes)//PTC
                {
                    XmlNodeList components = component.ChildNodes;
                    foreach (XmlNode subComponent in components)
                    {
                        if (subComponent.Name.Equals("node"))
                        {
                            string subComponentName = subComponent.Attributes["TEXT"].Value;
                            List<string> terms;
                            XmlNodeList elems = subComponent.SelectNodes(".//node");
                          //  string termContent = subComponentName + ",";
                            string termContent = "";
                            foreach (XmlNode elem in elems)
                            {
                                string elemName = elem.Attributes["TEXT"].Value.Trim();
                                termContent += elemName + ",";
                            }
                            if (termContent.EndsWith(","))
                            {
                                termContent = termContent.Remove(termContent.Length - 1);
                            }
                            
                            terms = new List<string>(termContent.Split(','));
                            List<string> processedTerms = ProcessReplicateTerm(terms);
                            componentLevel.Add(subComponentName, processedTerms);
                        }

                    }
                }
            }
        }

        //parse the subComponent Level
        private void ParseSubcomponentLevel(XmlNodeList nodeList)
        {
            foreach (XmlNode system in nodeList)
            {
                if (system.HasChildNodes)
                {
                    XmlNodeList components = system.ChildNodes;
                    foreach (XmlNode component in components)
                    {
                        if (component.HasChildNodes)
                        {
                            XmlNodeList subcomponents = component.SelectNodes("./node"); ;
                            foreach (XmlNode subComponent in subcomponents)
                            {
                                string subComponentName = subComponent.Attributes["TEXT"].Value.Trim();
                                Console.WriteLine(subComponentName);
                                List<string> terms;
                                XmlNodeList elems = subComponent.SelectNodes(".//node");
                              //  string termContent = subComponentName + ",";
                                string termContent = "";
                                foreach (XmlNode elem in elems)
                                {
                                    string elemName = elem.Attributes["TEXT"].Value.Trim();
                                    termContent += elemName + ",";
                                }
                                if (termContent.EndsWith(","))
                                {
                                    termContent = termContent.Remove(termContent.Length - 1);
                                }
                                if (termContent.Length == 0)
                                {
                                    continue;
                                }
                                terms = new List<string>(termContent.Split(','));
                                List<string> processedTerms = ProcessReplicateTerm(terms);
                                if (subComponentLevel.ContainsKey(subComponentName))
                                {
                                    List<string> existingTerms = subComponentLevel[subComponentName];
                                    foreach (string existingTerm in existingTerms)
                                    {
                                        if (!processedTerms.Contains(existingTerm))
                                        {
                                            processedTerms.Add(existingTerm);
                                        }
                                    }
                                    subComponentLevel[subComponentName] = processedTerms;
                                }

                                else
                                {
                                    subComponentLevel.Add(subComponentName, processedTerms);
                                } 
                            }
                        }
                    }
                }
            }
        }

        private void WriteDomainTerms(Dictionary<string, List<string>> componentTerms)
        {
            string domainTermContent = "";
            //write component level info
            foreach (string component in componentTerms.Keys)
            {
                List<string> terms = componentTerms[component];
                if (terms.Count == 0)
                {
                    continue;
                }
                //float termCount = terms.Count;
                //float prop = 1/termCount;
                domainTermContent += component + ":";
                if (component.Contains(","))
                {
                    string[] componentNames = component.Split(',');
                    foreach (string name in componentNames)
                    {
                        domainTermContent += name + "," + 1.0 + ";";
                    }
                }
                else
                {
                    domainTermContent += component + "," + 1.0 + ";";
                }

                foreach (string term in terms)
                {
                    domainTermContent += term + "," + 0.2 + ";";
                }
                if (domainTermContent.EndsWith(";"))
                {
                    domainTermContent = domainTermContent.Remove(domainTermContent.Length - 1).ToLower() + "\r\n";
                }
                else
                {
                    domainTermContent += "\r\n";
                }
            }

            if (domainTermContent.EndsWith("\r\n"))
            {
                domainTermContent = domainTermContent.Remove(domainTermContent.Length - 2).ToLower();
            }

            FileOperators.FileAppend(fixedFormatPath, domainTermContent);
        }

        private List<string> ProcessReplicateTerm(List<string> origiTerms)
        {
            List<string> processedTerms = new List<string>();
            foreach(string origTerm in origiTerms)
            {
                if (origTerm.StartsWith(" ") && origTerm.EndsWith(" "))
                {
                    if (!processedTerms.Contains(origTerm))
                    {
                        processedTerms.Add(origTerm);
                    }
                    string temp = "[" + origTerm.Trim() + "]";
                    if (!processedTerms.Contains(temp))
                    {
                        processedTerms.Add(temp);
                    }
                }
                else 
                {
                    string temp = origTerm.Trim();
                    if (processedTerms.Contains(temp))
                    {
                        continue;
                    }
                    else
                    {
                        processedTerms.Add(temp);
                    }

               }
                    
            }
            return processedTerms;
        }
    }
}
