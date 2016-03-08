using System;

using uMod.Agent.UI;

namespace uMod.Agent.Modules
{
    /// <summary>
    /// Represents a generic module
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Gets the name of this module
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the version of this module
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Prints this module's info to the specified output device
        /// </summary>
        /// <param name="outputDevice">The device to write to</param>
        /// <param name="init">Is the session just starting?</param>
        void PrintInfo(IOutputDevice outputDevice, bool init);

        /// <summary>
        /// Registers all commands
        /// </summary>
        /// <param name="commandEngine"></param>
        void RegisterCommands(CommandEngine commandEngine);
    }
}
