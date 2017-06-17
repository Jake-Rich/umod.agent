using System;
using System.Collections.Generic;
using System.Diagnostics;
using uMod.Agent.UI;
using uMod.Agent.Commands;

namespace uMod.Agent.Modules
{
    /// <summary>
    /// Contains methods responsible for launching moddable games
    /// </summary>
    public sealed class GameLauncher : IModule, ICommandHandler
    {
        /// <summary>
        /// Gets the name of this module
        /// </summary>
        public string Name => "GameLauncher";

        /// <summary>
        /// Gets the version of this module
        /// </summary>
        public string Version => "0.0.1";

        private IDictionary<string, CommandHandler> commands;

        public GameLauncher()
        {
            commands = new Dictionary<string, CommandHandler>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "launch", cmd_launch }
            };
        }

        /// <summary>
        /// Prints this module's info to the specified output device
        /// </summary>
        /// <param name="outputDevice">The device to write to</param>
        /// <param name="init">Is the session just starting?</param>
        public void PrintInfo(IOutputDevice outputDevice, bool init)
        {
            if (!init) outputDevice.WriteStaticLine($"$whiteModule $green{Name} $whiteversion $yellow{Version}");
        }

        /// <summary>
        /// Registers all commands
        /// </summary>
        /// <param name="commandEngine"></param>
        public void RegisterCommands(CommandEngine commandEngine)
        {
            foreach (var key in commands.Keys) commandEngine.RegisterHandler(key, this);
        }

        /// <summary>
        /// Handles the specified command
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="cmd">The command to handle</param>
        /// <param name="outputDevice">The device to write output to</param>
        /// <returns>True if handled, false if not</returns>
        public bool Handle(CommandContext ctx, Command cmd, IOutputDevice outputDevice)
        {
            CommandHandler handler;
            return commands.TryGetValue(cmd.Verb, out handler) && handler(ctx, cmd, outputDevice);
        }

        #region Commands

        private bool cmd_launch(CommandContext ctx, Command cmd, IOutputDevice outputDevice)
        {
            if (!ctx.Engine.ExecuteCommand("scan") || ctx.LocatedGame == null) return false;

            outputDevice.WriteStaticLine($"$whiteLaunching game $green{ctx.LocatedGame.Name}$white...");
            Process.Start(ctx.LocatedGame.ScanData.KeyFiles[0].Path, ctx.LocatedGame.LaunchArguments);

            return true;
        }

        #endregion
    }
}
