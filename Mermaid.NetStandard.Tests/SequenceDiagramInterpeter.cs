using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var diagram = await src.IsDiagramType<SequenceDiagram>();
            Assert.Equal("participant", Assert.Single(diagram.Participants).Key);
        }

        [Fact]
        public async Task ExplicitParticipantRequiresSpace()
        {
            var src = @"sequenceDiagram
participant->BC";
            var diagram = await src.IsDiagramType<SequenceDiagram>();
            Assert.Equal("participant",diagram.Participants.First().Key);
        }

        [Fact]
        public async Task AddsExplicitParticipant()
        {
            var src = @"sequenceDiagram
participant BC";
            var diagram = await src.IsDiagramType<SequenceDiagram>();
            var participant = diagram.Participants.Single();
            Assert.Equal("BC", participant.Key);
            Assert.Equal("BC", participant.Value);
        }

        [Fact]
        public async Task NoAsAfterParticipantName()
        {
            var src = @"sequenceDiagram
participant BC alias";
            await src.IsInvalidDiagram<InvalidDiagramException>("Expected 'as' after participant id");
        }

        [Fact]
        public async Task NoAliasAfterAsParticipantAs()
        {
            var src = @"sequenceDiagram
participant BC as ";
            await src.IsInvalidDiagram<InvalidDiagramException>("Expected alias after 'as'");
        }

        [Fact]
        public async Task AddsExplicitParticipantAlias()
        {
            var src = @"sequenceDiagram
participant BC as Background";
            var diagram = await src.IsDiagramType<SequenceDiagram>();
            var participant = diagram.Participants.Single();
            Assert.Equal("BC", participant.Key);
            Assert.Equal("Background", participant.Value);
        }

        [Theory]
        [InlineData("A->B", "A", ArrowEnding.None, ArrowLine.Solid, "B")]
        [InlineData("AC->>BD", "AC", ArrowEnding.Arrowhead, ArrowLine.Solid, "BD")]
        [InlineData("AC-XBD", "AC", ArrowEnding.Cross, ArrowLine.Solid, "BD")]
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
            Assert.Equal(originator, msg.Originator);
            Assert.Equal(recipient, msg.Recipient);
        }
    }
}
