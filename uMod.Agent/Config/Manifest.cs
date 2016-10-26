namespace uMod.Agent.Config
{
    /// <summary>
    /// Contains details used to scan for moddable games
    /// </summary>
    public sealed class Manifest
    {
        /// <summary>
        /// All potential games
        /// </summary>
        public GameInfo[] Games;
    }

    /// <summary>
    /// Contains details to identify an individual game
    /// </summary>
    public sealed class GameInfo
    {
        /// <summary>
        /// Name of the game
        /// </summary>
        public string Name;

        /// <summary>
        /// Scanning data for the game
        /// </summary>
        public ScanInfo ScanData;

        /// <summary>
        /// Command-line arguments used to launch
        /// </summary>
        public string LaunchArguments;
    }

    /// <summary>
    /// Contains details of how to scan for the presense of a particular game
    /// </summary>
    public sealed class ScanInfo
    {
        public ScanFileInfo[] KeyFiles;
    }

    /// <summary>
    /// Contains details of an individual file
    /// </summary>
    public sealed class ScanFileInfo
    {
        public string Path;
        public string Hash;
    }
}
