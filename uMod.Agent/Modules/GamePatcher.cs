using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using uMod.Agent.UI;
using uMod.Agent.Commands;

namespace uMod.Agent.Modules
{
    /// <summary>
    /// Contains methods responsible for patching moddable games
    /// </summary>
    public sealed class GamePatcher : IModule, ICommandHandler
    {
        /// <summary>
        /// Gets the name of this module
        /// </summary>
        public string Name => "GamePatcher";

        /// <summary>
        /// Gets the version of this module
        /// </summary>
        public string Version => "0.0.1";

        private IDictionary<string, CommandHandler> commands;

        public GamePatcher()
        {
            commands = new Dictionary<string, CommandHandler>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "patch", cmd_patch }
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

        private bool cmd_patch(CommandContext ctx, Command cmd, IOutputDevice outputDevice)
        {
            if (ctx.LocatedGame == null) ctx.Engine.ExecuteCommand("scan");

            var gameType = ctx.LocatedGame.ScanData.KeyFiles.Any(d => d.Path.Contains("_Data") || d.Path.Contains("Assembly-CSharp.dll")) ? "Unity" : "Other";
            var patchFile = $"{ctx.LocatedGame.Name.Replace(" ", "")}.opj";

            // Download the patcher, if needed
            if (!File.Exists("OxidePatcher.exe"))
                ctx.Engine.ExecuteCommand("fetch \"https://github.com/OxideMod/Snapshots/raw/master/OxidePatcher.exe\"");

            // Download the latest patch file for the located game
            ctx.Engine.ExecuteCommand($"fetch \"https://github.com/OxideMod/Oxide/raw/develop/Games/{gameType}/Oxide.Game.{ctx.LocatedGame.Name.Replace(" ", "")}/{patchFile}\"");

            outputDevice.WriteStaticLine($"$whitePatching game $green{ctx.LocatedGame.Name}$white...");
            Process.Start("OxidePatcher.exe", $"-c -p {patchFile}");

            // Done
            return true;
        }

        #endregion
    }
}
