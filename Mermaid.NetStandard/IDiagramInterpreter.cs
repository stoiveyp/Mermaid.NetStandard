using System.Threading.Tasks;

namespace Mermaid.NetStandard;

public interface IDiagramInterpreter
{
    Task<SequenceDiagram> Interpret(MermaidParser reader);
}