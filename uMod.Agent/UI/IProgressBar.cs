namespace uMod.Agent.UI
{
    /// <summary>
    /// Represents a mutable progress bar
    /// </summary>
    public interface IProgressBar : IUIComponent
    {
        /// <summary>
        /// Gets or sets the progress of this bar (0 = 0%, 1 = 100%)
        /// </summary>
        float Progress { get; set; }

        /// <summary>
        /// Gets or sets the width of this progress bar. May have a default setting.
        /// </summary>
        int Width { get; set; }
    }
}
