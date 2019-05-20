using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainAnalysis.Model
{
    public class RelatedFileModel
    {
        public string TopicName { get; set; }

        public string RelatedFileName { get; set; }

        public string RelatedFilePath { get; set; }

        public string Similarity { get; set; }
    }
}
