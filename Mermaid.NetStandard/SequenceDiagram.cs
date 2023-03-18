using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Mermaid.NetStandard.SequenceDiagrams;

namespace Mermaid.NetStandard
{
    public class SequenceDiagram:MermaidDiagram
    {
        public const string MermaidType = "sequenceDiagram";
        public override string Type => MermaidType;
        public bool AutoNumber { get; set; }
        public Dictionary<string, string> Participants { get; set; } = new();
        public List<Message> Messages { get; set; } = new();
    }
}
