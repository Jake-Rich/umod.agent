using System;
using System.Collections.Generic;
using uMod.Agent.UI;

namespace uMod.Terminal.UI
{
    /// <summary>
    /// Represents a console label
    /// </summary>
    public class Label : UIComponent, ILabel
    {
        // The text in this label
        private string text;

        /// <summary>
        /// Gets the height of this label in text rows
        /// </summary>
        public override int Height => 1 + ((text.Length-1) / ConsoleOutputDevice.Columns);

        /// <summary>
        /// Gets or sets the text stored within this label
        /// </summary>
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                MakeDirty();
            }
        }

        private static IDictionary<string, Action> mutatorDict = new Dictionary<string, Action>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "black", () => Console.ForegroundColor = ConsoleColor.Black },
            { "blue", () => Console.ForegroundColor = ConsoleColor.Blue },
            { "cyan", () => Console.ForegroundColor = ConsoleColor.Cyan },
            { "darkblue", () => Console.ForegroundColor = ConsoleColor.DarkBlue },
            { "darkcyan", () => Console.ForegroundColor = ConsoleColor.DarkCyan },
            { "darkgreen", () => Console.ForegroundColor = ConsoleColor.DarkGreen },
            { "darkmagenta", () => Console.ForegroundColor = ConsoleColor.DarkMagenta },
            { "darkred", () => Console.ForegroundColor = ConsoleColor.DarkRed },
            { "darkyellow", () => Console.ForegroundColor = ConsoleColor.DarkYellow },
            { "gray", () => Console.ForegroundColor = ConsoleColor.Gray },
            { "green", () => Console.ForegroundColor = ConsoleColor.Green },
            { "magenta", () => Console.ForegroundColor = ConsoleColor.Magenta },
            { "red", () => Console.ForegroundColor = ConsoleColor.Red },
            { "white", () => Console.ForegroundColor = ConsoleColor.White },
            { "yellow", () => Console.ForegroundColor = ConsoleColor.Yellow }
        };

        /// <summary>
        /// Initialises a new instance of the Label class
        /// </summary>
        /// <param name="console"></param>
        public Label(ConsoleOutputDevice console)
            : base(console)
        {
            text = "";
        }

        /// <summary>
        /// Renders this UI component at the specified location
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void Render(int x, int y)
        {
            // Setup state
            Console.ResetColor();
            Console.SetCursorPosition(x, y);

            // Iterate each character
            var text = Text;
            var state = 0;
            var renderPos = 0;
            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];
                switch (state)
                {
                    case 0:
                        if (c == '$')
                        {
                            Console.Write(text.Substring(renderPos, i - renderPos));
                            renderPos = i;
                            state = 1;
                        }
                        break;
                    case 1:
                        if (c == '$')
                        {
                            Console.Write(text.Substring(renderPos, i - renderPos));
                            renderPos = i;
                            state = 1;
                        }
                        else
                        {
                            var seg = text.Substring(renderPos + 1, i - renderPos);
                            Action mutator;
                            if (mutatorDict.TryGetValue(seg, out mutator))
                            {
                                mutator();
                                renderPos = i + 1;
                                state = 0;
                            }
                        }
                        break;
                }
            }
            Console.Write(text.Substring(renderPos, text.Length-renderPos));
        }
    }
}
