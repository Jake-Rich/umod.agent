using System;
using System.IO;
using System.Linq;

using uMod.Agent;
using uMod.Agent.Modules;
using uMod.Agent.Commands;

namespace uMod.Terminal
{
    /// <summary>
    /// The core program class
    /// </summary>
    public static class Program
    {
        private static Command defaultCommand = new Command(". run:\"scan;patch;launch\"");

        /// <summary>
        /// Entry point
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            // Setup output device
            var outputDevice = new ConsoleOutputDevice();

            // Setup engine
            var engine = new CommandEngine(outputDevice)
            {
                Context = new CommandContext { WorkingDirectory = Path.GetFullPath("."), Terminate = false }
            };

            // Register all modules
            foreach (var module in ModuleRegistry.Modules)
            {
                module.RegisterCommands(engine);
                module.PrintInfo(outputDevice, true);
            }

            // Parse args
            var cmdLine = new Command(Environment.CommandLine);

            // If the command is empty, fallback to our default one
            if (cmdLine.NamedArgCount == 0 && cmdLine.SimpleArgs.Length == 0) cmdLine = defaultCommand;

            // Run commands?
            var runCmd = cmdLine.GetNamedArg("run");
            if (!string.IsNullOrEmpty(runCmd))
            {
                var cmdList = runCmd.Split(';');
                for (var i = 0; i < cmdList.Length; i++)
                {
                    // Write
                    outputDevice.WriteStaticLine($"$cyan{Path.GetFileName(engine.Context.WorkingDirectory)}/$white: {cmdList[i]}");

                    // Handle command
                    if (!engine.ExecuteCommand(cmdList[i])) outputDevice.WriteStaticLine("$redUnknown command");
                }
            }

            // Terminal mode?
            if (cmdLine.SimpleArgs.Any(a => a == "t" || a == "terminal"))
            {
                // Core input loop
                while (!engine.Context.Terminate)
                {
                    // Input prompt
                    Console.SetCursorPosition(0, Console.CursorTop + 1);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"{Path.GetFileName(engine.Context.WorkingDirectory)}/");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(": ");
                    var line = Console.ReadLine();

                    // Write
                    outputDevice.WriteStaticLine($"$cyan{Path.GetFileName(engine.Context.WorkingDirectory)}/$white: {line}");

                    // Handle command
                    engine.Context.ErrorFlag = false;
                    if (!engine.ExecuteCommand(line)) outputDevice.WriteStaticLine("$redUnknown command");
                }
            }

            // Exit check
            if (!cmdLine.SimpleArgs.Any(a => a == "ce" || a == "cleanexit"))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
    }
}
