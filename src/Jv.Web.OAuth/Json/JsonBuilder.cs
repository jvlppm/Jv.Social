//
// JsonBuilder.cs
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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Collections;
#if !NET35
using System.Dynamic;
#endif

namespace Jv.Web.OAuth.Json
{
    public static class JsonBuilder
    {
#if NET35
        public static object Build(string json)
#else
        public static dynamic Build(string json)
#endif
        {
            return Build(new JsonReader(json));
        }

#if NET35
        static object Build(JsonReader reader)
#else
        static dynamic Build(JsonReader reader)
#endif
        {
            var token = reader.ReadToken();
            switch (token.Type)
            {
                case TokenType.SpecialChar:
                    reader.PutBack(token);

                    if (token.Value == "{")
                        return BuildObject(reader);

                    else if (token.Value == "[")
                        return BuildList(reader);

                    throw new SemanticException(new[] { "'{'", "'['" }, token.Value);

                case TokenType.Number:
                    if (token.Value.Contains(".") || token.Value.Length > 18)
                        return decimal.Parse(token.Value, System.Globalization.CultureInfo.InvariantCulture);
                    if (token.Value.Length >= 10)
                    {
                        int smallResult;
                        if (int.TryParse(token.Value, out smallResult))
                            return smallResult;

                        return Int64.Parse(token.Value);
                    }
                    return int.Parse(token.Value);

                case TokenType.KeyWord:
                    switch (token.Value)
                    {
                        case "null":
                            return null;

                        case "false":
                        case "true":
                            return token.Value == "true";
                    }

                    throw new SemanticException(new[] { "Boolean", "Number", "String", "Array", "Object" }, token.Value);

                default:
                    return token.Value;
            }
        }

#if NET35
        static object BuildObject(JsonReader reader)
#else
        static dynamic BuildObject(JsonReader reader)
#endif
        {
#if NET35
            var obj = new Dictionary<string, object>();
#else
            var obj = new ExpandoObject() as IDictionary<string, object>;
#endif
            var nextToken = reader.ReadToken();

            if (nextToken.Type != TokenType.SpecialChar || nextToken.Value != "{")
                throw new SemanticException("{", nextToken.Value);

            nextToken = reader.ReadToken();

            if (nextToken.Type != TokenType.SpecialChar || nextToken.Value != "}")
            {
                reader.PutBack(nextToken);

                do
                {
                    var property = reader.ReadToken();
                    if (property.Type != TokenType.String)
                        throw new SemanticException("String", property.Type.ToString());

                    nextToken = reader.ReadToken();
                    if (nextToken.Type != TokenType.SpecialChar || nextToken.Value != ":")
                        throw new SemanticException("':'", nextToken.Value);

                    var value = JsonBuilder.Build(reader);
                    obj.Add(property.Value, value);

                    nextToken = reader.ReadToken();

                } while (nextToken.Type == TokenType.SpecialChar && nextToken.Value == ",");

                if (nextToken.Type != TokenType.SpecialChar || nextToken.Value != "}")
                    throw new SemanticException(new[] { "'}'", "','" }, nextToken.Value);
            }

            return obj;
        }

#if !NET35
        static IList<dynamic> BuildList(JsonReader reader)
#else
        static IList<object> BuildList(JsonReader reader)
#endif
        {
            var values = new List<dynamic>();

            var nextToken = reader.ReadToken();
            if (nextToken.Type != TokenType.SpecialChar || nextToken.Value != "[")
                throw new SemanticException("'['", nextToken.Value);

            nextToken = reader.ReadToken();

            if (nextToken.Type != TokenType.SpecialChar || nextToken.Value != "]")
            {
                reader.PutBack(nextToken);

                do
                {
                    values.Add(Build(reader));
                    nextToken = reader.ReadToken();
                } while (nextToken.Type == TokenType.SpecialChar && nextToken.Value == ",");

                if (nextToken.Type != TokenType.SpecialChar || nextToken.Value != "]")
                    throw new SemanticException("']'", nextToken.Value);
            }

            return values;
        }

        public static string Extract(object obj)
        {
            return Extract(obj, false, 0).Trim('\r', '\n');
        }

        public static string Extract(object obj, bool ident)
        {
            return Extract(obj, ident, 0).Trim('\r', '\n');
        }

        static string Extract(object obj, bool ident, int currentIdentation)
        {
            if ((object)obj == null)
                return "null";

            if (obj is bool)
                return ((bool)obj).ToString().ToLower();

            if (obj is int || obj is Int64 || obj is decimal || obj is double || obj is float)
                return Convert.ToString(obj, CultureInfo.InvariantCulture);

            if (obj is string || obj is char)
                return "\"" + EncodeString(obj.ToString()) + "\"";

            IEnumerable<string> dictKeys = null;
            Func<string, object> getValue = null;

            if (obj is IDictionary && ((IDictionary)obj).Keys.OfType<object>().All(k => k is string))
            {
                dictKeys = ((IDictionary)obj).Keys.OfType<string>();
                getValue = s => ((IDictionary)obj)[s];
            }
            else if (obj is IDictionary<string, object>)
            {
                dictKeys = ((IDictionary<string, object>)obj).Keys;
                getValue = s => ((IDictionary<string, object>)obj)[s];
            }

            if (dictKeys != null)
            {
                StringBuilder json = new StringBuilder();
                if (ident)
                    json.AppendLine();
                json.Append(new string('\t', currentIdentation));
                json.Append("{");

                bool first = true;

                if (ident)
                    currentIdentation++;

                foreach (var key in dictKeys)
                {
                    object value = getValue(key);

                    if (value is Delegate)
                        continue;

                    if (first) first = false;
                    else json.Append(',');

                    if (ident)
                    {
                        json.AppendLine();
                        json.Append(new string('\t', currentIdentation));
                    }

                    json.AppendFormat(ident ? "\"{0}\": {1}" : "\"{0}\":{1}", key, Extract(value, ident, currentIdentation));
                }

                if (ident)
                {
                    currentIdentation--;
                    json.AppendLine();
                    json.Append(new string('\t', currentIdentation));
                }

                json.Append("}");

                return json.ToString();
            }

            else if (obj is IEnumerable)
            {
                StringBuilder json = new StringBuilder();
                if (ident)
                    json.AppendLine();
                json.Append(new string('\t', currentIdentation));
                json.Append("[");

                bool first = true;

                if (ident)
                    currentIdentation++;

                foreach (var value in (obj as IEnumerable))
                {
                    if (first) first = false;
                    else json.Append(',');

                    if (ident)
                    {
                        json.AppendLine();
                        json.Append(new string('\t', currentIdentation));
                    }

                    json.Append(Extract(value, ident, currentIdentation));
                }

                if (ident)
                {
                    currentIdentation--;
                    json.AppendLine();
                    json.Append(new string('\t', currentIdentation));
                }

                json.Append("]");

                return json.ToString();
            }

            var extractedInfo = new Dictionary<string, object>();
            foreach (PropertyInfo prop in obj.GetType().GetTypeInfo().DeclaredProperties)
                extractedInfo.Add(prop.Name, prop.GetValue(obj, null));
            return Extract(extractedInfo, ident, currentIdentation);
        }

        static string EncodeString(string original)
        {
            StringBuilder final = new StringBuilder();
            foreach (char ch in original)
            {
                if ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-+_.,~^ ()[]{}%@/!?#&*:".IndexOf(ch) < 0)
                    final.Append("\\u" + ((int)ch).ToString("X4"));
                else final.Append(ch);
            }

            return final.ToString();
        }
    }
}
