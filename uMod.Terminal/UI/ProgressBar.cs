using System;
using uMod.Agent.UI;

namespace uMod.Terminal.UI
{
    public class ProgressBar : UIComponent, IProgressBar
    {
        private float progress;
        private int width;

        /// <summary>
        /// Gets or sets the progress of this bar (0 = 0%, 1 = 100%)
        /// </summary>
        public float Progress
        {
            get
            {
                return progress;
            }
            set
            {
                progress = value;
                MakeDirty();
            }
        }

        /// <summary>
        /// Gets or sets the width of this progress bar. May have a default setting.
        /// </summary>
        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
                MakeDirty();
            }
        }

        /// <summary>
        /// Gets the height of this progress bar in text rows
        /// </summary>
        public override int Height => 1;

        /// <summary>
        /// Initialises a new instance of the ProgressBar class
        /// </summary>
        /// <param name="console"></param>
        public ProgressBar(ConsoleOutputDevice console) : base(console)
        {
            progress = 0.0f;
            width = ConsoleOutputDevice.Columns - 10;
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

            // Render
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write('[');
            var toDraw = (int)Math.Floor(progress * width + 0.5f);
            for (var i = 0; i < toDraw; i++) Console.Write('=');
            for (var i = 0; i < width - toDraw; i++) Console.Write(' ');
            Console.Write(']');
            Console.Write($" {Math.Floor(progress * 100.0f + 0.5f)}%");
        }
    }
}
