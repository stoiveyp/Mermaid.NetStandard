using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mermaid.NetStandard.Tests
{
    internal static class Utility
    {
        public static TextReader ToReader(this string text)
        {
            return new StringReader(text);
        }
    }
}
