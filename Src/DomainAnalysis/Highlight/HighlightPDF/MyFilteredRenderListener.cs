using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace DomainAnalysis.HighlightPDF
{
    public class MyFilteredTextRenderListener : FilteredRenderListener, ITextExtractionStrategy
    {

        /** The deleg that will receive the text render operation if the filters all pass */
        private ITextExtractionStrategy deleg;

        /**
            * Construction
            * @param deleg the deleg {@link RenderListener} that will receive filtered text operations
            * @param filters the Filter(s) to apply
            */
        public MyFilteredTextRenderListener(ITextExtractionStrategy deleg, RenderFilter[] filters)
            : base(deleg, filters)
        {
            this.deleg = deleg;
        }

        /**
            * This class delegates this call
            * @see com.itextpdf.text.pdf.parser.TextExtractionStrategy#getResultantText()
            */
        public String GetResultantText()
        {
            return deleg.GetResultantText();
        }
    }
}

