using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mermaid.NetStandard.Tests
{
    internal static class Utility
    {
        public static async Task<T> IsDiagramType<T>(this string src) where T:MermaidDiagram
        {
            return Assert.IsType<T>(await MermaidParser.Parse(src));
        }

        public static Task<InvalidDiagramException> IsInvalidDiagram(this string src)
        {
            return Assert.ThrowsAsync<InvalidDiagramException>(() => MermaidParser.Parse(src));
        }

        public static Task<T> IsInvalidDiagram<T>(this string src) where T:Exception
        {
            return Assert.ThrowsAsync<T>(() => MermaidParser.Parse(src));
        }

        public static async Task IsInvalidDiagram<T>(this string src, string message) where T : Exception
        {
            var exc = await Assert.ThrowsAsync<T>(() => MermaidParser.Parse(src));
            Assert.Equal(message, exc.Message);
        }
    }
}
