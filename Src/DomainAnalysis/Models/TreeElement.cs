using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainAnalysis.Model
{
    public class TreeElement
    {
        public string Text { get; set; }

        public List<TreeElement> SubElements { get; set; }
    }
}
