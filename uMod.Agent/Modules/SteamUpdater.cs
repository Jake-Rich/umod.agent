using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

using uMod.Agent.UI;
using uMod.Agent.Commands;

namespace uMod.Agent.Modules
{
    /// <summary>
    /// Contains methods responsible for updating Steam games
    /// </summary>
    public sealed class SteamUpdater : IModule, ICommandHandler
    {
        /// <summary>
        /// Gets the name of this module
        /// </summary>
        public string Name => "SteamUpdater";

        /// <summary>
        /// Gets the version of this module
        /// </summary>
        public string Version => "0.0.1";

        private IDictionary<string, CommandHandler> commands;

        public SteamUpdater()
        {
            commands = new Dictionary<string, CommandHandler>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "steam", cmd_steam },
                { "steamcmd", cmd_steam }
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

        private bool cmd_steam(CommandContext ctx, Command cmd, IOutputDevice outputDevice)
        {
            if (!ctx.Engine.ExecuteCommand("scan") || ctx.LocatedGame == null) return false;

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                    if (File.Exists(Path.Combine("Steam", "steamcmd.exe"))) break;

                    ctx.Engine.ExecuteCommand("fetch \"https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip\"");
                    ZipFile.ExtractToDirectory("steamcmd.zip", Path.Combine(ctx.WorkingDirectory, "Steam"));
                    File.Delete("steamcmd.zip");

                    break;

                case PlatformID.Unix:
                    if (File.Exists(Path.Combine("Steam", "steamcmd.exe"))) break;

                    ctx.Engine.ExecuteCommand("fetch \"https://steamcdn-a.akamaihd.net/client/installer/steamcmd_linux.tar.gz\"");
                    // TODO: Extract steamcmd_linux.tar.gz
                    File.Delete("steamcmd_linux.tar.gz");

                    break;
            }

            outputDevice.WriteStaticLine($"$whiteUpdating game $green{ctx.LocatedGame.Name}$white...");
            Process.Start(Path.Combine("Steam", "steamcmd"), $"+login {ctx.LocatedGame.Steam.Login} +force_install_dir ../ +app_update {ctx.LocatedGame.Steam.AppId} {ctx.LocatedGame.Steam.Branch} validate +quit");
            // TODO: Make SteamCMD use existing console window if possible

            return true;
        }

        #endregion
    }
}
