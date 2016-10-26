using uMod.Agent.UI;

namespace uMod.Terminal.UI
{
    /// <summary>
    /// Represents a generic console UI component
    /// </summary>
    public abstract class UIComponent : IUIComponent
    {
        /// <summary>
        /// Gets the height of this UI component in text rows
        /// </summary>
        public abstract int Height { get; }

        // The console output device
        private ConsoleOutputDevice console;

        // Change flags
        private bool dirty, immediateMode;

        /// <summary>
        /// Gets or sets if changes to this component should cause a render immediately
        /// </summary>
        public bool ImmediateMode
        {
            get
            {
                return immediateMode;
            }
            set
            {
                immediateMode = value;
                if (value && dirty) dirty = false;
            }
        }

        /// <summary>
        /// Initialises a new instance of the UIComponent class
        /// </summary>
        /// <param name="console"></param>
        protected UIComponent(ConsoleOutputDevice console)
        {
            this.console = console;
            immediateMode = true;
        }

        /// <summary>
        /// Renders this UI component at the specified location
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public abstract void Render(int x, int y);

        /// <summary>
        /// Requests a render
        /// </summary>
        protected void MakeDirty()
        {
            if (immediateMode)
            {
                console.Render();
                dirty = false;
            }
            else
            {
                dirty = true;
            }
        }

        /// <summary>
        /// Renders if this component has changed
        /// </summary>
        public void Update()
        {
            if (dirty)
            {
                console.Render();
                dirty = false;
            }
        }
    }
}
