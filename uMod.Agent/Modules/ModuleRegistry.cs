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
            new FileSystem(), // File system module
            new ConfigSystem(), // Config system module
            new GameScanner(), // Game scanner module
            new Downloader() // Resource downloader module
        };

        public static T GetModule<T>() where T : class => Modules.SingleOrDefault(m => m is T) as T;
    }
}
