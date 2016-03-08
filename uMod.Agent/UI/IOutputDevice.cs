using System;

namespace uMod.Agent.UI
{
    /// <summary>
    /// Represents a device capable of delivering output to the user
    /// </summary>
    public interface IOutputDevice
    {
        /// <summary>
        /// Writes a non-changing line of text to the device
        /// </summary>
        /// <param name="message"></param>
        void WriteStaticLine(string message);

        /// <summary>
        /// Writes a mutable line of text to the device
        /// </summary>
        /// <param name="initialText"></param>
        ILabel WriteLabel(string initialText);

        /// <summary>
        /// Writes a progress bar to the device
        /// </summary>
        /// <param name="initialText"></param>
        IProgressBar WriteProgressBar();
    }
}
