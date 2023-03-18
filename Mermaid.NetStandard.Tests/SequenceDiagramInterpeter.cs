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
            var diagram = await src.IsInvalidDiagram();
            Assert.Equal(3,diagram.LineNumber);
        }

        [Fact]
        public async Task ExplicitParticipantRequiresSpace()
        {
            var src = @"sequenceDiagram
participant-BC";
            var exc = await src.IsInvalidDiagram();
            Assert.Equal("Name expected after 'participant'",exc.Message);
            Assert.Equal(2,exc.LineNumber);
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
        public async Task AddsExplicitParticipantAlias()
        {
            var src = @"sequenceDiagram
participant BC as Background";
            var diagram = await src.IsDiagramType<SequenceDiagram>();
            var participant = diagram.Participants.Single();
            Assert.Equal("BC", participant.Key);
            Assert.Equal("Background", participant.Value);
        }
    }
}
