using System;
using System.Collections.Generic;
using System.Text;

namespace Mermaid.NetStandard
{
    public class InvalidDiagramException:ApplicationException
    {
        public InvalidDiagramException(string message, int lineNumber) : this(message)
        {
            LineNumber = lineNumber;
        }

        public int? LineNumber { get; set; }

        public InvalidDiagramException(string message) : base(message)
        {

        }

        public InvalidDiagramException(string message, Exception innerException):base(message, innerException){}
    }
}
