using System;

namespace uMod.Agent.UI
{
    /// <summary>
    /// Represents a mutable label
    /// </summary>
    public interface ILabel : IUIComponent
    {
        /// <summary>
        /// Gets or sets the text stored within this label
        /// </summary>
        string Text { get; set; }
    }
}
