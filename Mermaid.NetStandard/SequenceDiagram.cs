using System;
using System.Collections.Generic;
using System.Text;

namespace Mermaid.NetStandard
{
    public class SequenceDiagram:MermaidDiagram
    {
        public const string MermaidType = "sequenceDiagram";
        public override string Type => MermaidType;
        public bool AutoNumber { get; set; }
    }
}
