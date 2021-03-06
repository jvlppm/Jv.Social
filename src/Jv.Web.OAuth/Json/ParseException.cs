﻿//
// ParseException.cs
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

using System;

namespace Jv.Web.OAuth.Json
{
    public class ParseException : FormatException
    {
        public ParseException(string message)
            : base(message)
        {
        }
    }

    public class LexicalException : ParseException
    {
        public LexicalException(string message)
            : base(message)
        {
        }
    }

    public class SemanticException : ParseException
    {
        public override string Message
        {
            get
            {
                if (Expected.Length == 1)
                    return string.Format("Expected {0} but got {1}.", Expected[0], Got);
                return string.Format("Unexpected {0}", Got);
            }
        }

        public SemanticException(string expected, string got)
            : this(new[] { expected }, got)
        {
        }

        public SemanticException(string[] expected, string got)
            : base("Semantic Exception")
        {
            Expected = expected;
            Got = got;
        }

        public string[] Expected { get; private set; }
        public string Got { get; private set; }
    }
}