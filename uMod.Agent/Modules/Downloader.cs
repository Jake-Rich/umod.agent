using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

using uMod.Agent.UI;
using uMod.Agent.Commands;

namespace uMod.Agent.Modules
{
    /// <summary>
    /// Contains methods responsible for fetching remote resources
    /// </summary>
    public sealed class Downloader : IModule, ICommandHandler
    {
        /// <summary>
        /// Gets the name of this module
        /// </summary>
        public string Name => "Downloader";

        /// <summary>
        /// Gets the version of this module
        /// </summary>
        public string Version => "0.0.1";

        private IDictionary<string, CommandHandler> commands;

        public Downloader()
        {
            commands = new Dictionary<string, CommandHandler>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "download", cmd_fetch },
                { "fetch", cmd_fetch }
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

        private bool cmd_fetch(CommandContext ctx, Command cmd, IOutputDevice outputDevice)
        {
            // Identify URL
            var url = cmd.GetNamedArg("url");
            var nextBasicArg = 0;
            if (string.IsNullOrEmpty(url))
            {
                if (cmd.SimpleArgs.Length <= nextBasicArg)
                {
                    outputDevice.WriteStaticLine("$redURL of resource must be specified.");
                    ctx.ErrorFlag = true;
                    return true;
                }
                url = cmd.SimpleArgs[nextBasicArg++];
            }

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                outputDevice.WriteStaticLine("$redURL is not well formed.");
                ctx.ErrorFlag = true;
                return true;
            }

            // Identify output file
            var outPath = cmd.GetNamedArg("out");
            if (string.IsNullOrEmpty(outPath))
                outPath = cmd.SimpleArgs.Length <= nextBasicArg ? Path.GetFileName(url) : cmd.SimpleArgs[nextBasicArg++];
            outPath = Path.GetFullPath(Path.Combine(ctx.WorkingDirectory, outPath));

            // Security check the output path
            if (!ModuleRegistry.GetModule<FileSystem>().SecurityCheck(outPath))
            {
                outputDevice.WriteStaticLine("$redDestination path blocked due to security reasons.");
                ctx.ErrorFlag = true;
                return true;
            }

            ILabel sizeLabel = null;
            try
            {
                // Let UI know we're starting to do something
                var label = outputDevice.WriteLabel($"Requesting {url}...");

                // Create request, acquire response
                var req = WebRequest.Create(url);
                var response = req.GetResponse();

                // Let UI know so far so good
                label.Text = $"Downloading {url}...";
                sizeLabel = outputDevice.WriteLabel("");

                // Did the server give us a content size? If so, add a progress bar
                var progBar = response.ContentLength > 0 ? outputDevice.WriteProgressBar() : null;
                var totalSize = response.ContentLength > 0 ? FormatContentLength(response.ContentLength) : "unknown";

                // Write to file
                using (var outStream = File.OpenWrite(outPath))
                {
                    // Get the response stream and allocate a buffer
                    var stream = response.GetResponseStream();
                    var buffer = new byte[1024];

                    // Iterate for as long as we can
                    long totalRead = 0;
                    while (stream != null && stream.CanRead)
                    {
                        // Read into the buffer, write to file, check eos (end of stream)
                        var read = stream.Read(buffer, 0, 1024);
                        outStream.Write(buffer, 0, read);
                        if (read == 0) break;
                        totalRead += read;

                        // Update UI
                        if (progBar != null) progBar.Progress = (totalRead / (float)response.ContentLength);
                        sizeLabel.Text = $"{FormatContentLength(totalRead)} / {totalSize}";
                    }
                }

            }
            catch (Exception ex)
            {
                outputDevice.WriteStaticLine("Unknown error when fetching resource:");
                outputDevice.WriteStaticLine(ex.ToString());
                if (sizeLabel != null) sizeLabel.Text = "$redCancelled";
                ctx.ErrorFlag = true;
                return true;
            }

            ctx.ErrorFlag = false;
            return true;
        }

        /// <summary>
        /// Formats the specified length in bytes to KiB, MiB or GiB
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string FormatContentLength(long length)
        {
            if (length < 1 << 10)
            {
                return $"{length} B";
            }
            if (length < 1 << 20)
            {
                return $"{length >> 10}{(length & 0x3FF) / 1024.0f:.00} KiB";
            }
            if (length < 1 << 30)
            {
                return $"{length >> 10}{((length & 0xFFFFF) >> 10) / 1024.0f:.00} MiB";
            }
            return $"{length >> 10}{((length & 0x3FFFFFFF) >> 10) / 1024.0f:.00} GiB";
        }

        #endregion
    }
}
