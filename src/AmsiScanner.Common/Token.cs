using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace AmsiScanner.Common {
    /*
    Attribute 	9 	

Attributes
Command 	1 	

Command
CommandArgument 	3 	

Command Argument
CommandParameter 	2 	

Command Parameter
Comment 	15 	

Comment
GroupEnd 	13 	

Group Ender
GroupStart 	12 	

Group Starter
Keyword 	14 	

Keyword
LineContinuation 	18 	

Line continuation
LoopLabel 	8 	

Loop label
Member 	7 	

Property name or method name
NewLine 	17 	

Number 	4 	

Number
Operator 	11 	

Operators
Position 	19 	

Position token
StatementSeparator 	16 	

Statement separator. This is ';'
String 	5 	

String
Type 	10 	

Types
Unknown 	0 	

Unknown token
Variable 	6 	

Variable
    */
    public enum TokenType {
        GroupStart = PSTokenType.GroupStart,
        GroupEnd = PSTokenType.GroupEnd,
        Operator = PSTokenType.Operator,
        NewLine = PSTokenType.NewLine,
        Unknown = PSTokenType.Unknown,
        Variable = PSTokenType.Variable,
        Attribute = PSTokenType.Attribute,
        Command = PSTokenType.Command,
        CommandArgument = PSTokenType.CommandArgument,
        CommandParameter = PSTokenType.CommandParameter,
        Member = PSTokenType.Member,
        Number = PSTokenType.Number,
        Comment = PSTokenType.Comment,
        Keyword = PSTokenType.Keyword,
        LineContinuation = PSTokenType.LineContinuation,
        LoopLabel = PSTokenType.LoopLabel,
        Position = PSTokenType.Position,
        StatementSeparator = PSTokenType.StatementSeparator,
        String = PSTokenType.String,
        Type = PSTokenType.Type,
        Whitespace
    }

    public class Token {
        public int Length {
            get;
        }

        public int Start {
            get;
        }

        public int StartRow {
            get;
        }

        public int StartColumn {
            get;
        }

        public int EndRow {
            get;
        }

        public int EndColumn {
            get;
        }

        public string Contents {
            get;
        }

        public TokenType TokenType {
            get;
        }

        public Token(PSToken token) {
            this.Start = token.Start;
            this.Length = token.Length;
            this.StartRow = token.StartLine;
            this.StartColumn = token.StartColumn;
            this.EndRow = token.EndLine;
            this.EndColumn = token.EndColumn;
            this.Contents = token.Content;
            this.TokenType = (TokenType)token.Type;
        }

        public Token(int start, int length, int startRow, int startColumn, int endRow, int endColumn, TokenType tokenType, string contents) {
            this.Start = start;
            this.Length = length;
            this.StartRow = startRow;
            this.StartColumn = startColumn;
            this.EndRow = endRow;
            this.EndColumn = endColumn;
            this.TokenType = tokenType;
            this.Contents = contents;
        }

        public static Token[] Tokenize(string script) {
            List<Token> results = new List<Token>();
            Collection<PSParseError> errors = null;
            PSToken[] tokens = PSParser.Tokenize(script, out errors).ToArray();
            PSToken previous = tokens[0];
            for (int i = 0; i < tokens.Length; i++) {
                PSToken current = tokens[i];

                if (current.Start > previous.Start + previous.Length) {
                    int start = previous.Start + previous.Length;
                    int length = current.Start - start;
                    results.Add(new Token(start, length, previous.EndLine, previous.EndColumn, current.StartLine, current.StartColumn, TokenType.Whitespace, script.Substring(previous.Start, length)));
                }

                results.Add(new Token(current));

                previous = current;
            }

            return results.ToArray();
        }
    }
}
