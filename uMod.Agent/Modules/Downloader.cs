using System;
using System.Collections.Generic;
using System.Net;

using uMod.Agent.UI;
using uMod.Agent.Commands;

namespace uMod.Agent.Modules
{
    /// <summary>
    /// Contains methods responsible for fetching remote resources.
    /// </summary>
    public sealed class Downloader : IModule, ICommandHandler
    {
        /// <summary>
        /// Gets the name of this module
        /// </summary>
        public string Name { get { return "Downloader"; } }

        /// <summary>
        /// Gets the version of this module
        /// </summary>
        public string Version { get { return "dev 0.0.1"; } }

        private static IDictionary<string, CommandHandler> commands = new Dictionary<string, CommandHandler>(StringComparer.InvariantCultureIgnoreCase)
        {
            {  "fetch", cmd_fetch }
        };

        /// <summary>
        /// Prints this module's info to the specified output device
        /// </summary>
        /// <param name="outputDevice">The device to write to</param>
        /// <param name="init">Is the session just starting?</param>
        public void PrintInfo(IOutputDevice outputDevice, bool init)
        {
            if (!init)
            {
                outputDevice.WriteStaticLine($"$whiteModule $green{Name} $whiteversion $yellow{Version}");
            }
        }

        /// <summary>
        /// Registers all commands
        /// </summary>
        /// <param name="commandEngine"></param>
        public void RegisterCommands(CommandEngine commandEngine)
        {
            foreach (var key in commands.Keys)
                commandEngine.RegisterHandler(key, this);
        }

        /// <summary>
        /// Handles the specified command
        /// </summary>
        /// <param name="cmd">The command to handle</param>
        /// <param name="outputDevice">The device to write output to</param>
        /// <returns>True if handled, false if not</returns>
        public bool Handle(CommandContext ctx, Command cmd, IOutputDevice outputDevice)
        {
            CommandHandler handler;
            if (commands.TryGetValue(cmd.Verb, out handler))
                return handler(ctx, cmd, outputDevice);
            else
                return false;
        }

        #region Commands

        private static bool cmd_fetch(CommandContext ctx, Command cmd, IOutputDevice outputDevice)
        {
            // Identify URL
            string url = cmd.GetNamedArg("url");
            if (string.IsNullOrEmpty(url))
            {
                if (cmd.SimpleArgs.Length == 0)
                {
                    outputDevice.WriteStaticLine("$redURL of resource must be specified.");
                    return true;
                }
            }

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                outputDevice.WriteStaticLine("$redURL is not well formed.");
                return true;
            }

            // Done
            return true;
        }

        #endregion
    }
}
