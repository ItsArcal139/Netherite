using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Utils
{
    /// <summary>
    /// Represents an array whose values are 4-bit nibbles.
    /// </summary>
    public class NibbleArray : IEnumerable<byte>
    {
        public byte[] Data { get; private set; }

        /// <summary>
        /// Creates a <see cref="NibbleArray"/> with an amount of nibbles.
        /// </summary>
        /// <param name="size"></param>
        public NibbleArray(int size)
        {
            if (!(size > 0 && size % 2 == 0))
            {
                throw new ArgumentException($"{nameof(size)} must be a positive and even number, not {size}.");
            }

            Data = new byte[size / 2];
        }

        /// <summary>
        /// Creates a <see cref="NibbleArray"/> with a raw byte array.
        /// </summary>
        /// <param name="data"></param>
        public NibbleArray(byte[] data)
        {
            Data = data;
        }

        /// <summary>
        /// The length of the <see cref="NibbleArray"/>.
        /// </summary>
        public int Length => Data.Length * 2;

        /// <summary>
        /// The size of this <see cref="NibbleArray"/> in bytes.
        /// </summary>
        public int SizeInBytes => Data.Length;

        /// <summary>
        /// Get the nibble at the specified index.
        /// </summary>
        /// <param name="index">The index of the nibble.</param>
        /// <returns>The 4-bit nibble value in <see cref="byte"/>.</returns>
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

        /// <summary>
        /// Fill this <see cref="NibbleArray"/> with a specified nibble value.
        /// </summary>
        /// <param name="value">A 4-bit nibble value to be filled.</param>
        public void Fill(byte value)
        {
            value &= 0xf;
            Array.Fill(Data, (byte)(value << 4 | value));
        }

        /// <summary>
        /// Set the raw data of this <see cref="NibbleArray"/> from a byte array.
        /// </summary>
        /// <param name="data"></param>
        public void SetRawData(byte[] data)
        {
            Array.Copy(data, Data, Data.Length);
        }

        /// <summary>
        /// Clone this <see cref="NibbleArray"/>.
        /// </summary>
        /// <returns>A clone of this <see cref="NibbleArray"/>.</returns>
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
