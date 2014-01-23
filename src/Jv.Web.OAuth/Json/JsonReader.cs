//
// JsonReader.cs
//
// Author:
//   João Vitor Pietsiaki Moraes <jvlppm@gmail.com>
//
// Copyright (c) 2010 João Vitor Pietsiaki Moraes
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Jv.Web.OAuth.Json
{
    class JsonToken
    {
        public JsonToken(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public TokenType Type { get; private set; }
        public string Value { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}: \"{1}\"", Type, Value);
        }
    }

    class JsonReader
    {
        TextReader _textReader;
        Stack<int> _readChars;


        Stack<JsonToken> BackToken { get; set; }

        public JsonReader(string json)
        {
            _textReader = new StringReader(json);
            BackToken = new Stack<JsonToken>();
        }

        public JsonToken ReadToken()
        {
            if (BackToken.Count > 0)
                return BackToken.Pop();

            TokenType tokenType = TokenType.Unidentified;
            string token = string.Empty;
            bool endToken = false;
            while(!endToken)
            {
                int read = _textReader.Read();

                if (read == -1)
                    break;

                switch ((char)read)
                {
                    case '\t':
                    case '\r':
                    case '\n':
                    case ' ':
                        if (token != string.Empty)
                            endToken = true;
                        break;

                    case ',':
                    case ':':
                    case '{':
                    case '}':
                    case '[':
                    case ']':
                        var nextToken = new JsonToken(TokenType.SpecialChar, ((char)read).ToString());
                        if (token != string.Empty)
                        {
                            endToken = true;
                            BackToken.Push(nextToken);
                            break;
                        }
                        return nextToken;

                    case '\"':
                        if (token != string.Empty)
                            throw new LexicalException("Unexpected char '\"' while reading \"" + token + "\"");

                        while(true)
                        {
                            int tokenChar = _textReader.Read();
                            if (tokenChar == -1)
                                break;

                            if (tokenChar != '\"')
                            {
                                if (tokenChar != '\\')
                                    token += (char)tokenChar;
                                else
                                {
                                    tokenChar = _textReader.Read();
                                    if (tokenChar == -1)
                                        break;

                                    switch (tokenChar)
                                    {
                                        case 'b': token += "\b"; break;
                                        case 'f': token += "\f"; break;
                                        case 'n': token += "\n"; break;
                                        case 'r': token += "\r"; break;
                                        case 't': token += "\t"; break;
                                        case 'v': token += "\v"; break;
                                        case '\"': token += "\""; break;
                                        case '\\': token += "\\"; break;
                                        case '/': token += "/"; break;

                                        case 'u':
                                            var code = new StringBuilder();
                                            for(int i = 0; i < 4; i++) {
                                                tokenChar = _textReader.Read();
                                                if (tokenChar == -1)
                                                    break;
                                                code.Append(tokenChar);
                                            }
                                            if(code.Length < 4)
                                                break;

                                            token += (char)int.Parse(code.ToString(), NumberStyles.HexNumber);
                                            break;

                                        default:
                                            throw new LexicalException("Bad escape sequence: \'\\" + tokenChar + "\'");
                                    }
                                }
                            }
                            else
                            {
                                return new JsonToken(TokenType.String, token);
                            }
                        }
                        throw new LexicalException("End quote not found");

                    default:
                        if (token == string.Empty)
                        {
                            if (char.IsNumber((char)read) || read == '+' || read == '-')
                                tokenType = TokenType.Number;
                        }
                        else if (tokenType == TokenType.Number && !char.IsNumber((char)read) && (read != '.' || token.Contains(".")))
                            tokenType = TokenType.String;
                        token += (char)read;
                        break;
                }
            }

            if (tokenType == TokenType.Unidentified)
            {
                switch (token)
                {
                    case "true":
                    case "false":
                    case "null":
                        tokenType = TokenType.KeyWord;
                        break;
                    default:
                        tokenType = TokenType.String;
                        break;
                }
            }

            return new JsonToken(tokenType, token);
        }

        public void PutBack(JsonToken token)
        {
            BackToken.Push(token);
        }
    }
}
