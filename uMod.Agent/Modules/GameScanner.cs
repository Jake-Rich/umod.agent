using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

using uMod.Agent.UI;
using uMod.Agent.Commands;
using uMod.Agent.Config;

namespace uMod.Agent.Modules
{
    /// <summary>
    /// Contains methods responsible for scanning and identifying moddable games.
    /// </summary>
    public sealed class GameScanner : IModule, ICommandHandler
    {
        /// <summary>
        /// Gets the name of this module
        /// </summary>
        public string Name { get { return "GameScanner"; } }

        /// <summary>
        /// Gets the version of this module
        /// </summary>
        public string Version { get { return "dev 0.0.1"; } }

        private IDictionary<string, CommandHandler> commands;

        public GameScanner()
        {
            commands = new Dictionary<string, CommandHandler>(StringComparer.InvariantCultureIgnoreCase)
            {
                {  "scan", cmd_scan }
            };
        }

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

        private bool cmd_scan(CommandContext ctx, Command cmd, IOutputDevice outputDevice)
        {
            // Grab the manifest
            Manifest manifest = ModuleRegistry.GetModule<ConfigSystem>().GetConfig<Manifest>(ctx, "Manifest");
            if (manifest == null)
            {
                outputDevice.WriteStaticLine("$redFailed to load the manifest.");
                ctx.ErrorFlag = true;
                return true;
            }

            // Iterate each game
            GameInfo locatedGame = null;
            foreach (var game in manifest.Games)
            {
                if (ScanForGame(ctx, game))
                {
                    locatedGame = game;
                    break;
                }
            }
            if (locatedGame == null)
            {
                outputDevice.WriteStaticLine("$redNo recognised games found in current directory.");
                ctx.ErrorFlag = true;
                return true;
            }

            outputDevice.WriteStaticLine($"$whiteIdentified game $green{locatedGame.Name}$white.");

            // Done
            return true;
        }

        

        #endregion

        private bool ScanForGame(CommandContext ctx, GameInfo gameInfo)
        {
            // Sanity check
            var scanData = gameInfo.ScanData;
            if (scanData == null) return false;

            // Check that all files exist
            foreach (var file in scanData.KeyFiles)
            {
                string path = Path.Combine(ctx.WorkingDirectory, file.Path);
                if (!File.Exists(path)) { return false; }
            }

            return true;
        }
    }
}
