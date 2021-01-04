using Netherite.Data;
using Netherite.Nbt.Serializations;
using Netherite.Net.Packets.Play.Serverbound;
using Netherite.Physics;
using Netherite.Texts;
using Netherite.Worlds;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Entities
{
    public abstract class Entity
    {
        public Vector3 Position { get; set; }

        public float Yaw { get; set; }

        public float Pitch { get; set; }

        public Guid Guid { get; set; }

        private static int counter = 0;

        public int Handle { get; private set; }

        public Vector3 Velocity { get; set; }

        public World World { get; set; }

        protected Entity(World world)
        {
            Handle = counter++;
            map.Add(Handle, this);

            World = world;
        }

        private static Dictionary<int, Entity> map = new Dictionary<int, Entity>();

        public EntityMetadata Metadata { get; set; } = new EntityMetadata();

        public static Entity GetById(int id)
        {
            return map[id];
        }

        public virtual void Tick()
        {

        }
    }

    public class DummyEntity : Entity
    {
        public new int Handle { get; set; }

        public DummyEntity(int handle) : base(null)
        {
            Handle = handle;
        }
    }

    public class EntityMetadata
    {
        public enum PoseType
        {
            Standing
        }

        public bool IsOnFire { get; set; }
        public bool IsCrouching { get; set; }
        public bool IsSprinting { get; set; }
        public bool IsSwimming { get; set; }
        public bool IsInvisible { get; set; }
        public bool IsGlowing { get; set; }
        public bool FlyingWithElytra { get; set; }

        public int AirTicks { get; set; }
        public Text CustomName { get; set; }
        public bool CustomNameVisible { get; set; }
        public bool Silent { get; set; }
        public bool NoGravity { get; set; }
        public PoseType Pose { get; set; }
    }

    public class LivingEntityMetadata : EntityMetadata
    {
        public bool IsHandActive { get; set; }
        public byte ActiveHand { get; set; }
        public bool IsInRiptideAttack { get; set; }
        public float Health { get; set; }
        public Color PotionEffectColor { get; set; }
        public bool IsPotionEffectAmbient { get; set; }
        public int ArrowCount { get; set; }
        public int Absorption { get; set; }
        public Vector3? BedPosition { get; set; }
    }

    public class PlayerMetadata : LivingEntityMetadata
    {
        public float AdditionalHearts { get; set; }
        public int Score { get; set; }
        public SkinPart EnabledParts { get; set; }
        public byte MainHand { get; set; }
        public Entity LeftShoulderEntity { get; set; }
        public Entity RightShoulderEntity { get; set; }
    }

    public abstract class LivingEntity : Entity
    {
        public LivingEntity(World w) : base(w) { }
        public new LivingEntityMetadata Metadata { get; set; } = new LivingEntityMetadata();
    }
}
