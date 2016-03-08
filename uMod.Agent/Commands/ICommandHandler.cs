using System;

using uMod.Agent.UI;

namespace uMod.Agent.Commands
{
    /// <summary>
    /// A delegate that can handle a command
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="cmd"></param>
    /// <param name="outputDevice"></param>
    /// <returns></returns>
    public delegate bool CommandHandler(CommandContext ctx, Command cmd, IOutputDevice outputDevice);

    /// <summary>
    /// Represents an object capable of handling user commands
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Handles the specified command
        /// </summary>
        /// <param name="cmd">The command to handle</param>
        /// <param name="outputDevice">The device to write output to</param>
        /// <returns>True if handled, false if not</returns>
        bool Handle(CommandContext ctx, Command cmd, IOutputDevice outputDevice);
    }
}
