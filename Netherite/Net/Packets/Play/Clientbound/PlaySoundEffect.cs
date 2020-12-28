using Netherite.Entities;

namespace Netherite.Net.Packets.Play.Clientbound
{
    public class PlaySoundEffect : Packet
    {
        public string Sound { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        /// <summary>
        /// 音量值。數值可以超過 100%。
        /// </summary>
        public float Volume { get; set; }

        /// <summary>
        /// 音高倍率。兩倍為一個八度。
        /// </summary>
        public float Pitch { get; set; }
    }

    public class SpawnGlobalEntity : Packet
    {
        public Entity Entity { get; set; }

        public byte Type { get; set; }

        public double X { get; set; }
        
        public double Y { get; set; }

        public double Z { get; set; }
    }
}
