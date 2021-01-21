using Netherite.Nbt.Serializations;

namespace Netherite.Worlds
{
    /// <summary>
    /// Represents the game version of the world.
    /// </summary>
    public class GameVersion
    {
        /// <summary>
        /// Whether this is a snapshot version.
        /// </summary>
        [NbtProperty("Snapshot")]
        public bool IsSnapshot { get; set; }

        /// <summary>
        /// The data version internally used by the game.
        /// </summary>
        [NbtProperty("Id")]
        public int DataVersion { get; set; }

        /// <summary>
        /// The user-friendly name of the version.
        /// </summary>
        public string Name { get; set; }
    }
}
