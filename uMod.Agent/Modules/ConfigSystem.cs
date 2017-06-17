using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using uMod.Agent.UI;
using uMod.Agent.Commands;

namespace uMod.Agent.Modules
{
    /// <summary>
    /// Contains methods responsible for general configuration
    /// </summary>
    public sealed class ConfigSystem : IModule, ICommandHandler
    {
        /// <summary>
        /// Gets the name of this module
        /// </summary>
        public string Name => "Config";

        /// <summary>
        /// Gets the version of this module
        /// </summary>
        public string Version => "0.0.1";

        private IDictionary<string, CommandHandler> commands = new Dictionary<string, CommandHandler>(StringComparer.InvariantCultureIgnoreCase)
        {
        };

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

        #endregion

        #region API

        /// <summary>
        /// Gets the configuration for the specified name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetConfig<T>(CommandContext context, string name) where T : class
        {
            var path = Path.Combine(context.WorkingDirectory, $"uMod.{name}.json");
            if (!ModuleRegistry.GetModule<FileSystem>().SecurityCheck(path)) return null;
            return !File.Exists(path) ? null : JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }

        #endregion
    }
}
