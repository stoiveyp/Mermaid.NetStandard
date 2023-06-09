﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Mermaid.NetStandard.SequenceDiagrams;

namespace Mermaid.NetStandard
{
    public class MermaidParser
    {
        public string CurrentLine { get; set; }
        public TextReader Reader { get; }
        public int LineNumber { get; private set; }
        public int CurrentPosition { get; private set; }
        public char Current => CurrentLine[CurrentPosition];
        public char? Peek() => EndOfLine ? null : CurrentLine[CurrentPosition + 1];
        public bool EndOfLine => CurrentPosition >= CurrentLine.Length - 1;

        private MermaidParser(TextReader reader)
        {
            Reader = reader;
        }

        public async Task<bool> NextInterpeterLine()
        {
            while (await NextLine())
            {
                if (!string.IsNullOrWhiteSpace(CurrentLine))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> NextLine()
        {
            var line = await Reader.ReadLineAsync();
            if (line != null)
            {
                LineNumber++;
                CurrentLine = line;
                CurrentPosition = 0;
                return true;
            }

            return false;
        }

        public string? NextWord()
        {
            if (EndOfLine)
            {
                return char.IsLetterOrDigit(Current) ? Current.ToString() : null;
            }

            while (!EndOfLine && char.IsWhiteSpace(Current))
            {
                Next();
            }

            if (EndOfLine && !char.IsLetterOrDigit(Current)) return null;

            var currentPosition = CurrentPosition;
            var broken = false;

            while (Peek().HasValue)
            {
                Next();
                broken = true;
                if (!char.IsLetterOrDigit(Current))
                {
                    break;
                }
            }

            if (!broken)
            {
                Next();
            }

            if (EndOfLine && !char.IsWhiteSpace(Current))
            {
                CurrentPosition++;
            }

            if (currentPosition == CurrentPosition)
            {
                return null;
            }

            return CurrentLine[currentPosition..CurrentPosition];
        }

        public bool Next()
        {
            if (EndOfLine)
            {
                return false;
            }

            CurrentPosition++;
            return true;
        }

        public static Dictionary<string, Func<MermaidParser, Task<SequenceDiagram>>> DiagramTypes = new()
        {
            { SequenceDiagram.MermaidType, SequenceDiagramInterpreter.Interpret}
        };

        public static Task<MermaidDiagram> Parse(string text) => Parse(new StringReader(text));

        public static async Task<MermaidDiagram> Parse(TextReader reader)
        {
            var parser = new MermaidParser(reader);
            if (!await parser.NextInterpeterLine())
            {
                throw new InvalidOperationException("No diagram found");
            }

            if (!DiagramTypes.ContainsKey(parser.CurrentLine))
            {
                throw new InvalidOperationException($"Diagram type {parser.CurrentLine} not supported");
            }

            return await DiagramTypes[parser.CurrentLine](parser);
        }

        public string RestOfLine()
        {
            return CurrentLine[CurrentPosition..].Trim();
        }

        public void MoveForward(int number)
        {
            CurrentPosition += number;
        }
    }
}
