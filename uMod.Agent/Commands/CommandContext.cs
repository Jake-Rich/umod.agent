namespace uMod.Agent.Commands
{
    /// <summary>
    /// A context in which commands may run
    /// </summary>
    public sealed class CommandContext
    {
        /// <summary>
        /// Gets or sets the command engine that backs this context
        /// </summary>
        public CommandEngine Engine { get; set; }

        /// <summary>
        /// Gets or sets the current working directory
        /// </summary>
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Gets or sets if the session should be terminated
        /// </summary>
        public bool Terminate { get; set; }

        /// <summary>
        /// Gets or sets the error state of the previous command
        /// </summary>
        public bool ErrorFlag { get; set; }
    }
}
