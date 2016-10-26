using System;
using System.Collections.Generic;
using System.Text;

namespace uMod.Agent
{
    /// <summary>
    /// Represents a parsed command
    /// </summary>
    public sealed class Command
    {
        /// <summary>
        /// Gets the verb
        /// </summary>
        public string Verb { get; private set; }

        /// <summary>
        /// Gets all "simple" arguments
        /// </summary>
        public string[] SimpleArgs { get; private set; }

        // All named args in this command
        private IDictionary<string, string> namedArgs;

        /// <summary>
        /// Gets the number of named args
        /// </summary>
        public int NamedArgCount => namedArgs.Count;

        /// <summary>
        /// Gets all named arg keys
        /// </summary>
        public IEnumerable<string> NamedArgKeys => namedArgs.Keys;

        /// <summary>
        /// Initialises a new instance of the Command class
        /// </summary>
        /// <param name="src"></param>
        public Command(string src)
        {
            // Initialise
            namedArgs = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

            // Parse it
            Parse(src);
        }

        #region Parse

        private enum ParseState { None, EscapedBasicArg, EscapedBasicArgEsc, TaronArg, Identifier, IdentifierEsc, IdentifierPeek, IdentifierArg }

        /// <summary>
        /// Parses the specified string into this command
        /// </summary>
        /// <param name="src"></param>
        private void Parse(string src)
        {
            // Iterate until we hit a whitespace
            char c;
            var i = 0;
            while (i < src.Length && !char.IsWhiteSpace(c = src[i])) i++;

            // Found the verb
            Verb = src.Substring(0, i).ToLowerInvariant();

            // Setup temporary objects
            var basicArgs = new List<string>();

            // Crude FSM
            var curState = ParseState.None;
            StringBuilder sb = null;
            var nestState = 0;
            string namedArgKey = null;
            while (i <= src.Length)
            {
                if (i == src.Length)
                    c = ' ';
                else
                    c = src[i];
                i++;
                switch (curState)
                {
                    case ParseState.None:

                        if (!char.IsWhiteSpace(c))
                        {
                            switch (c)
                            {
                                case '"':
                                    curState = ParseState.EscapedBasicArg;
                                    sb = new StringBuilder();
                                    nestState = 1;
                                    break;
                                case '{':
                                    curState = ParseState.TaronArg;
                                    sb = new StringBuilder();
                                    nestState = 1;
                                    break;
                                default:
                                    curState = ParseState.Identifier;
                                    sb = new StringBuilder();
                                    sb.Append(c);
                                    break;
                            }
                        }

                        break;

                    case ParseState.EscapedBasicArg:

                        if (c == '"')
                        {
                            nestState--;
                            curState = ParseState.None;
                            if (namedArgKey != null)
                            {
                                namedArgs[namedArgKey] = sb.ToString();
                                namedArgKey = null;
                            }
                            else
                            {
                                basicArgs.Add(sb.ToString());
                            }
                            sb = null;
                        }
                        else if (c == '\\')
                        {
                            curState = ParseState.EscapedBasicArgEsc;
                        }
                        else
                        {
                            sb.Append(c);
                        }

                        break;
                    case ParseState.EscapedBasicArgEsc:

                        sb.Append(c);
                        curState = ParseState.EscapedBasicArg;

                        break;

                    case ParseState.Identifier:

                        if (c == ':' && namedArgKey == null)
                        {
                            namedArgKey = sb.ToString();
                            sb = new StringBuilder();
                            curState = ParseState.IdentifierPeek;
                        }
                        else if (c == '\\')
                        {
                            curState = ParseState.IdentifierEsc;
                        }
                        else if (char.IsWhiteSpace(c))
                        {
                            curState = ParseState.None;
                            if (namedArgKey != null)
                            {
                                namedArgs[namedArgKey] = sb.ToString();
                                namedArgKey = null;
                            }
                            else
                            {
                                basicArgs.Add(sb.ToString());
                            }
                            sb = null;
                        }
                        else
                        {
                            sb.Append(c);
                        }

                        break;

                    case ParseState.IdentifierEsc:

                        sb.Append(c);
                        curState = ParseState.Identifier;

                        break;

                    case ParseState.IdentifierPeek:

                        if (c == '"')
                        {
                            curState = ParseState.EscapedBasicArg;
                        }
                        else
                        {
                            curState = ParseState.Identifier;
                            sb.Append(c);
                        }

                        break;

                    default:

                        throw new NotImplementedException();
                }
            }

            // Check state
            if (curState != ParseState.None) throw new Exception($"Unexpected end of string (was in state '{curState}')");

            // Store basic args
            SimpleArgs = basicArgs.ToArray();
        }

        #endregion

        /// <summary>
        /// Gets the specified case-insensitive named argument
        /// </summary>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public string GetNamedArg(string key, string def = null)
        {
            string result;
            return namedArgs.TryGetValue(key, out result) ? result : def;
        }
    }
}
