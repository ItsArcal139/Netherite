using Netherite.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite
{
    public class Sound
    {
        public Identifier Id { get; set; }
    }

    public class Note
    {
        public class NoteTone
        {
            public static readonly NoteTone G = new NoteTone('G', 0x1, true);
            public static readonly NoteTone A = new NoteTone('A', 0x3, true);
            public static readonly NoteTone B = new NoteTone('B', 0x5, false);
            public static readonly NoteTone C = new NoteTone('C', 0x6, true);
            public static readonly NoteTone D = new NoteTone('D', 0x8, true);
            public static readonly NoteTone E = new NoteTone('E', 0xA, false);
            public static readonly NoteTone F = new NoteTone('F', 0xB, true);

            [Obsolete]
            public byte Id { get; private set; }

            public bool Sharpable { get; private set; }

            private static int counter = 0;

            public int Ordinal { get; } = counter++;

            public static NoteTone[] Values => new[] { 
                G, A, B, C, D, E, F
            };

            private char name;

            private static Dictionary<byte, NoteTone> map = new Dictionary<byte, NoteTone>();

#pragma warning disable 0612
            private NoteTone(char name, byte id, bool sharpable)
            {
                this.name = name;

                map.Add(id, this);
                if(sharpable)
                {
                    map.Add((byte)(id + 1), this);
                }

                Id = id;
                Sharpable = sharpable;
            }
#pragma warning restore

            [Obsolete]
            public static NoteTone GetById(byte id)
            {
                return map[id];
            }

            [Obsolete]
            public byte GetID(bool sharped = false)
            {
                byte id = (byte)(sharped && Sharpable ? Id + 1 : Id);
                return (byte)(id % 12);
            }

            [Obsolete]
            public bool IsSharped(byte id)
            {
                if (id == GetID()) return false;
                if (id == GetID(true)) return true;
                throw new ArgumentException("Not matching NoteTone");
            }

            public override string ToString()
            {
                return name + "";
            }
        }

        private byte note;

        public Note(int note)
        {
            this.note = (byte)note;
        }

#pragma warning disable 0612
        public Note(int octave, NoteTone tone, bool sharped)
        {
            if(sharped && tone.Sharpable)
            {
                tone = NoteTone.Values[tone.Ordinal + 1];
                sharped = false;
            }

            if(octave < 0 || octave > 2 || (octave == 2 && !(tone == NoteTone.F && sharped)))
            {
                throw new ArgumentException("Tone and octave have to be between F#0 and F#2");
            }

            this.note = (byte)(octave * 12 + tone.GetID(sharped));
        }
#pragma warning restore

        public static Note Flat(int octave, NoteTone tone)
        {
            if(octave == 2) throw new ArgumentException("Octave cannot be 2 for flats");
            tone = tone == NoteTone.G ? NoteTone.F : NoteTone.Values[tone.Ordinal - 1];
            return new Note(octave, tone, tone.Sharpable);
        }

        public static Note Sharp(int octave, NoteTone tone)
        {
            return new Note(octave, tone, true);
        }

        public static Note Natural(int octave, NoteTone tone)
        {
            if (octave == 2) throw new ArgumentException("Octave cannot be 2 for naturals");
            return new Note(octave, tone, false);
        }

        public Note Sharped
        {
            get
            {
                if (note == 24) throw new InvalidOperationException("This note cannot be sharped because it is the highest known note!");
                return new Note(note + 1);
            }
        }

        public Note Flattened
        {
            get
            {
                if (note == 0) throw new InvalidOperationException("This note cannot be flattened because it is the lowest known note!");
                return new Note(note - 1);
            }
        }
        
        [Obsolete]
        public byte Id => note;

        public int Octave => note / 12;

        private byte ToneByte => (byte)(note % 12);

#pragma warning disable 0612
        public NoteTone GetTone() => NoteTone.GetById(ToneByte);

        public bool IsSharped => NoteTone.GetById(ToneByte).IsSharped(ToneByte);
#pragma warning restore

        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;
            result = prime * result + note;
            return result;
        }

        public override string ToString()
        {
            return GetTone().ToString() + (IsSharped ? "#" : "") + Octave;
        }
    }
}
