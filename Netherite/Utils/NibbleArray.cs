using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Utils
{
    public class NibbleArray : IEnumerable<byte>
    {
        public byte[] Data { get; private set; }

        public NibbleArray(int size)
        {
            if (!(size > 0 && size % 2 == 0))
            {
                throw new ArgumentException($"{nameof(size)} must be a positive and even number, not {size}.");
            }

            Data = new byte[size / 2];
        }

        public NibbleArray(byte[] data)
        {
            Data = data;
        }

        public int Length => Data.Length * 2;

        public int SizeInBytes => Data.Length;

        public byte this[int index]
        {
            get
            {
                byte val = Data[index / 2];
                if (index % 2 == 0)
                {
                    return (byte)(val & 0xf);
                }
                else
                {
                    return (byte)((val & 0xf0) >> 4);
                }
            }

            set
            {
                value &= 0xf;
                int half = index / 2;
                byte previous = Data[half];

                if (index % 2 == 0)
                {
                    Data[half] = (byte)(previous & 0xf0 | value);
                }
                else
                {
                    Data[half] = (byte)(previous & 0xf | value << 4);
                }
            }
        }

        public void Fill(byte value)
        {
            value &= 0xf;
            Array.Fill(Data, (byte)(value << 4 | value));
        }

        public void SetRawData(byte[] data)
        {
            Array.Copy(data, Data, Data.Length);
        }

        public NibbleArray Clone()
        {
            byte[] clone = new byte[Data.Length];
            Data.CopyTo(clone, 0);
            return new NibbleArray(clone);
        }

        public IEnumerator<byte> GetEnumerator()
        {
            for (int i = 0; i < Length; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < Length; i++)
            {
                yield return this[i];
            }
        }
    }
}
