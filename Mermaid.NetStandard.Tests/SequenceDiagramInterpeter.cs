using System.Drawing;
using Mermaid.NetStandard.SequenceDiagrams;

namespace Mermaid.NetStandard.Tests
{
    public class SequenceDiagramInterpeter
    {
        [Fact]
        public async Task DiagramSupported()
        {
            await "sequenceDiagram".IsDiagramType<SequenceDiagram>();
        }

        [Fact]
        public async Task AutoNumberSupported()
        {
            var src = @"

sequenceDiagram
autoNumber   
";
            var diagram = await src.IsDiagramType<SequenceDiagram>();
            Assert.True(diagram.AutoNumber);
        }

        [Fact]
        public async Task NoActorThrowsException()
        {
            var src = @"
sequenceDiagram
participant
-->";
            await src.IsInvalidDiagram<InvalidDiagramException>("No actor found");
        }

        [Fact]
        public async Task ExplicitParticipantRequiresSpace()
        {
            var src = @"sequenceDiagram
participant->BC";
            var diagram = await src.IsDiagramType<SequenceDiagram>();
            Assert.Equal("participant", diagram.Participants.First().Key);
        }

        [Theory]
        [InlineData("participant BC Thingy", "BC Thingy", "BC Thingy")]
        [InlineData("participant BC", "BC", "BC")]
        [InlineData("participant BC as", "BC as", "BC as")]
        [InlineData("participant A as B", "A", "B")]
        [InlineData("actor as as as", "as", "as", ParticipantType.Actor)]
        [InlineData("participant as as as", "as", "as", ParticipantType.Participant)]
        [InlineData("participant as as as as", "as", "as as")]
        [InlineData("participant thing one as thing two", "thing one", "thing two")]
        [InlineData("actor thing one as thing two", "thing one", "thing two", ParticipantType.Actor)]
        public async Task ParseParticipant(string line, string id, string label, ParticipantType type = ParticipantType.Participant)
        {
            var src = @$"sequenceDiagram
{line}";
            var diagram = await src.IsDiagramType<SequenceDiagram>();
            var part = Assert.Single(diagram.Participants);
            Assert.Equal(id, part.Key);
            Assert.Equal(label, part.Value.Name);
            Assert.Equal(type, part.Value.Type);
        }

        [Theory]
        [InlineData("A->B", "A", ArrowEnding.None, ArrowLine.Solid, "B")]
        [InlineData("Thing One->Thing Two", "Thing One", ArrowEnding.None, ArrowLine.Solid, "Thing Two")]
        [InlineData("AC->>BD", "AC", ArrowEnding.Arrowhead, ArrowLine.Solid, "BD")]
        [InlineData("AC-XBD", "AC", ArrowEnding.Cross, ArrowLine.Solid, "BD")]
        [InlineData("AC--XBD", "AC", ArrowEnding.Cross, ArrowLine.Dotted, "BD")]
        public async Task ParseMessage(string message, string originator, ArrowEnding ending, ArrowLine line,
            string recipient)
        {
            var src = @$"sequenceDiagram
{message}";
            var diagram = await src.IsDiagramType<SequenceDiagram>();
            Assert.Equal(2, diagram.Participants.Count);
            var msg = Assert.Single(diagram.Messages);
            AssertMessage(msg, originator, ending, line, recipient);
        }

        private void AssertMessage(Message msg, string originator, ArrowEnding ending, ArrowLine line, string recipient)
        {
            Assert.Equal(ending, msg.Ending);
            Assert.Equal(line, msg.Line);
            Assert.Equal(originator, msg.Originator!.Name);
            Assert.Equal(recipient, msg.Recipient!.Name);
        }

        [Fact]
        public async Task BoxParsingRequiresEnd()
        {
            var src = @"sequenceDiagram
box transparent test box";
            await src.IsInvalidDiagram<InvalidDiagramException>("Box without end");
        }

        [Fact]
        public async Task BoxMessagesAreAttachedToBox()
        {
            var src = @"sequenceDiagram
box transparent test box
A-->>B
end";
            var drg = await src.IsDiagramType<SequenceDiagram>();
            var box = Assert.IsType<Box>(drg.Containers.First());
            var msg = Assert.Single(box.Messages);
            AssertMessage(msg, "A", ArrowEnding.Arrowhead, ArrowLine.Dotted, "B");
        }

        public static IEnumerable<object?[]> BoxData => new List<object?[]>
        {
            new object?[] { string.Empty, Color.Transparent, null },
            new object?[] { "transparent label", Color.Transparent, "label" },
            new object[] { "purple green", Color.Purple, "green" },
            new object[] { "AntiqueWHite stuffy thing", Color.AntiqueWhite, "stuffy thing" },
            new object[] { "totally not green", Color.Transparent, "totally not green" },
        };

        [Theory]
        [MemberData(nameof(BoxData))]
        public async Task ValidBoxAppearsInDiagram(string boxText, Color color, string? label)
        {
            var src = $@"sequenceDiagram
box {boxText}
end";
            var drg = await src.IsDiagramType<SequenceDiagram>();
            var container = Assert.Single(drg.Containers);
            var box = Assert.IsType<Box>(container);
            Assert.Equal(color, box.Color);
            Assert.Equal(label, box.Label);
        }
    }
}
