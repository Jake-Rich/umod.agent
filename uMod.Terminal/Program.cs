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
        private static Command defaultCommand = new Command(". run:\"scan select:true;patch;launch\"");

        /// <summary>
        /// Entry point
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            // Setup output device
            ConsoleOutputDevice outputDevice = new ConsoleOutputDevice();

            // Setup engine
            CommandEngine engine = new CommandEngine(outputDevice);
            engine.Context = new CommandContext
            {
                WorkingDirectory = Path.GetFullPath("."),
                Terminate = false
            };

            // Register all modules
            foreach (IModule module in ModuleRegistry.Modules)
            {
                module.RegisterCommands(engine);
                module.PrintInfo(outputDevice, true);
            }

            // Parse args
            Command cmdLine = new Command(Environment.CommandLine);

            // If the command is empty, fallback to our default one
            if (cmdLine.NamedArgCount == 0 && cmdLine.SimpleArgs.Length == 0)
            {
                cmdLine = defaultCommand;
            }

            // Run commands?
            string runCmd = cmdLine.GetNamedArg("run");
            if (!string.IsNullOrEmpty(runCmd))
            {
                string[] cmdList = runCmd.Split(';');
                for (int i = 0; i < cmdList.Length; i++)
                {
                    // Write
                    outputDevice.WriteStaticLine($"$cyan${Path.GetFileName(engine.Context.WorkingDirectory)}/$white: {cmdList[i]}");

                    // Handle command
                    if (!engine.HandleCommand(cmdList[i]))
                    {
                        outputDevice.WriteStaticLine("$redUnknown command!");
                    }
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
                    Console.Write($"${Path.GetFileName(engine.Context.WorkingDirectory)}/");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(": ");
                    string line = Console.ReadLine();

                    // Write
                    outputDevice.WriteStaticLine($"$cyan${Path.GetFileName(engine.Context.WorkingDirectory)}/$white: {line}");

                    // Handle command
                    if (!engine.HandleCommand(line))
                    {
                        outputDevice.WriteStaticLine("$redUnknown command!");
                    }

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
