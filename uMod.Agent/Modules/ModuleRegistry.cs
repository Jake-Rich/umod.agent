using System;
using System.Collections.Generic;

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
            new GameScanner(), // Game scanner module
            new Downloader() // Resource downloader module
        };

    }
}
