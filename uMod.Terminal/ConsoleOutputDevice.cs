using System;
using System.Collections.Generic;

using uMod.Agent.UI;
using uMod.Terminal.UI;

namespace uMod.Terminal
{
    /// <summary>
    /// An output device that renders to the console
    /// </summary>
    public class ConsoleOutputDevice : IOutputDevice
    {
        /// <summary>
        /// The number of columns in the console (in characters)
        /// </summary>
        public static readonly int Columns = Console.BufferWidth;

        // All components in the UI
        private IList<UIComponent> uilist;

        // The total height of all components
        private int totalHeight;

        /// <summary>
        /// Gets the total number of renders that have occured
        /// </summary>
        public int TotalRenders { get; private set; }

        /// <summary>
        /// Initialises a new instance of the ConsoleOutputDevice class
        /// </summary>
        public ConsoleOutputDevice()
        {
            uilist = new List<UIComponent>();
            totalHeight = 0;
        }

        /// <summary>
        /// Writes a non-changing line of text to the device
        /// </summary>
        /// <param name="message"></param>
        public void WriteStaticLine(string message)
        {
            WriteLabel(message);
        }

        /// <summary>
        /// Writes a mutable label to the device
        /// </summary>
        /// <param name="label"></param>
        public ILabel WriteLabel(string initialText)
        {
            Label lbl = new Label(this);
            lbl.ImmediateMode = false;
            lbl.Text = initialText;
            AddUIComponent(lbl);
            lbl.ImmediateMode = true;
            return lbl;
        }

        /// <summary>
        /// Adds a new component to this device
        /// </summary>
        /// <param name="component"></param>
        private void AddUIComponent(UIComponent component)
        {
            uilist.Add(component);
            totalHeight += component.Height;
            Render();
        }

        /// <summary>
        /// Re-renders the console
        /// </summary>
        public void Render()
        {
            // Clear
            Console.Clear();

            // Find y offset
            int yOffset = Console.BufferHeight - totalHeight;

            // Find elements to cut off
            int curY = yOffset;
            int i;
            for (i = 0; i < uilist.Count; i++)
            {
                UIComponent component = uilist[i];
                int bottom = curY + component.Height;
                if (bottom > 0) break;
            }
            int cnt = i;
            for (i = 0; i < cnt; i++)
            {
                UIComponent component = uilist[i];
                uilist.RemoveAt(i);
                totalHeight -= component.Height;
            }

            // Update y offset
            yOffset = Console.BufferHeight - totalHeight;
            if (yOffset > 0) yOffset = 0;

            // Render
            curY = yOffset;
            for (i = 0; i < uilist.Count; i++)
            {
                UIComponent component = uilist[i];
                component.Render(0, curY);
                curY += component.Height;
            }

            // Done
            TotalRenders++;
        }
    }
}
