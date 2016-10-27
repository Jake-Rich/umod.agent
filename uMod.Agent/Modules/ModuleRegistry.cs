using System.Collections.Generic;
using System.Linq;

namespace uMod.Agent.Modules
{
    /// <summary>
    /// The module registry
    /// </summary>
    public static class ModuleRegistry
    {
        public static readonly IEnumerable<IModule> Modules = new IModule[]
        {
            new Agent(), // Core agent module
            new ConfigSystem(), // Config system module
            new Downloader(), // Resource downloader module
            new FileSystem(), // File system module
            new GameLauncher(), // Game launcher module
            new GamePatcher(), // Game patcher module
            new GameScanner(), // Game scanner module
            new SteamUpdater() // Steam updater module
        };

        public static T GetModule<T>() where T : class => Modules.SingleOrDefault(m => m is T) as T;
    }
}
