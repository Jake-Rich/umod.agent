using System;
using System.Collections.Generic;

using uMod.Agent.Commands;
using uMod.Agent.UI;

namespace uMod.Agent
{
    /// <summary>
    /// The core command engine
    /// </summary>
    public sealed class CommandEngine
    {
        // The output device
        private IOutputDevice outputDevice;

        // All registered command handlers
        private IDictionary<string, ICommandHandler> handlers;

        /// <summary>
        /// Gets or sets the current command context
        /// </summary>
        public CommandContext Context { get; set; }

        /// <summary>
        /// Initialises a new instance of the CommandEngine class
        /// </summary>
        /// <param name="outputDevice"></param>
        public CommandEngine(IOutputDevice outputDevice)
        {
            this.outputDevice = outputDevice;
            handlers = new Dictionary<string, ICommandHandler>();
        }

        /// <summary>
        /// Registers the specified handler
        /// </summary>
        /// <param name="verb"></param>
        /// <param name="handler"></param>
        public void RegisterHandler(string verb, ICommandHandler handler)
        {
            handlers.Add(verb, handler);
        }

        /// <summary>
        /// Handles the specified command
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool HandleCommand(string str)
        {
            return HandleCommand(new Command(str));
        }

        /// <summary>
        /// Handles the specified command
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public bool HandleCommand(Command cmd)
        {
            ICommandHandler handler;
            if (handlers.TryGetValue(cmd.Verb, out handler))
                return handler.Handle(Context, cmd, outputDevice);
            else
                return false;
        }
    }
}
