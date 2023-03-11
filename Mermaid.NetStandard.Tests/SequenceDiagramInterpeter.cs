using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mermaid.NetStandard.Tests
{
    public class SequenceDiagramInterpeter
    {
        [Fact]
        public async Task DiagramSupported()
        {
            var diagram = await MermaidParser.Parse("sequenceDiagram");
            Assert.IsType<SequenceDiagram>(diagram);
        }

        [Fact]
        public async Task AutoNumberSupported()
        {
            var src = @"

sequenceDiagram
autoNumber   
";
            var diagram = await MermaidParser.Parse(src);
            var seq = Assert.IsType<SequenceDiagram>(diagram);
            Assert.True(seq.AutoNumber);
        }

        [Fact]
        public async Task NoActorThrowsException()
        {
            var src = @"
sequenceDiagram
-->";
            var diagram = await Assert.ThrowsAsync<InvalidDiagramException>(() => MermaidParser.Parse(src));
            Assert.Equal(3,diagram.LineNumber);
        }
    }
}
