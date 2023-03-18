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
            var diagram = await src.IsDiagramType<SequenceDiagram>();
            Assert.Equal("participant",Assert.Single(diagram.Participants).Key);
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
    }
}
