using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using uMod.Agent.UI;
using uMod.Agent.Commands;

namespace uMod.Agent.Modules
{
    /// <summary>
    /// Contains file-system manipulation commands
    /// </summary>
    public sealed class FileSystem : IModule, ICommandHandler
    {
        /// <summary>
        /// Gets the name of this module
        /// </summary>
        public string Name => "FileSystem";

        /// <summary>
        /// Gets the version of this module
        /// </summary>
        public string Version => "0.0.1";

        private IDictionary<string, CommandHandler> commands;

        public FileSystem()
        {
            commands = new Dictionary<string, CommandHandler>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "cd", cmd_cd },
                { "dir", cmd_ls },
                { "ls", cmd_ls }
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

        private bool cmd_ls(CommandContext ctx, Command cmd, IOutputDevice outputDevice)
        {
            // Find directories
            foreach (var dir in Directory.EnumerateDirectories(ctx.WorkingDirectory).OrderBy(x => x))
                outputDevice.WriteStaticLine($"$graydir $white{Path.GetFileName(dir)}");

            // Find files
            foreach (var file in Directory.EnumerateFiles(ctx.WorkingDirectory).OrderBy(x => x))
                outputDevice.WriteStaticLine($"$grayfile $white{Path.GetFileName(file)}");

            // Done
            return true;
        }

        private bool cmd_cd(CommandContext ctx, Command cmd, IOutputDevice outputDevice)
        {
            if (cmd.SimpleArgs.Length == 0)
            {
                ctx.WorkingDirectory = Path.GetFullPath(".");
            }
            else
            {
                var newPath = Path.Combine(ctx.WorkingDirectory, string.Join(" ", cmd.SimpleArgs));
                if (File.Exists(newPath))
                {
                    outputDevice.WriteStaticLine($"$red{Path.GetFileName(newPath)} is a file!");
                }
                else if (!Directory.Exists(newPath))
                {
                    outputDevice.WriteStaticLine($"$red{Path.GetFileName(newPath)} does not exist!");
                }
                else
                {
                    newPath = Path.GetFullPath(newPath);
                    var lastC = newPath[newPath.Length - 1];
                    if (lastC == '\\' || lastC == '/') newPath = newPath.Substring(0, newPath.Length - 1);
                    ctx.WorkingDirectory = newPath;
                }
            }

            // Done
            return true;
        }

        #endregion

        #region API

        /// <summary>
        /// Checks if the specified path is save to write to.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool SecurityCheck(string path)
        {
            var exeLoc = Path.GetFullPath(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location));
            if (path.Length < exeLoc.Length || !string.Equals(path.Substring(0, exeLoc.Length), exeLoc, StringComparison.OrdinalIgnoreCase))
            {
                exeLoc = Environment.CurrentDirectory;
                if (path.Length < exeLoc.Length || !string.Equals(path.Substring(0, exeLoc.Length), exeLoc, StringComparison.OrdinalIgnoreCase))
                    return false;
            }
            return true;
        }

        #endregion
    }
}
