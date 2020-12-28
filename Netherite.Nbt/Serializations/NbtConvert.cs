using Netherite.Nbt.Serializations.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Netherite.Nbt.Serializations
{
    public static class NbtConvert
    {
        public static byte[] SerializeToBuffer<T>(T obj)
        {
            BufferSerializer serializer = new BufferSerializer();
            object o = Reflections.ToSerializable(obj);
            NbtTag tag = ToNbt(o);
            serializer.Write(tag);
            return serializer.ToBuffer();
        }

        public static NbtTag ToNbt<T>(T obj)
        {
            if (obj is NbtTag n) return n;
            if (obj is bool m) return new NbtByte((byte)(m ? 1 : 0));
            if (obj is byte a) return new NbtByte(a);
            if (obj is short b) return new NbtShort(b);
            if (obj is int c) return new NbtInt(c);
            if (obj is long d) return new NbtLong(d);
            if (obj is float e) return new NbtFloat(e);
            if (obj is double f) return new NbtDouble(f);
            if (obj is byte[] g) return new NbtByteArray(g);
            if (obj is string h) return new NbtString(h);
            if (obj is int[] k) return new NbtIntArray(k);
            if (obj is long[] l) return new NbtLongArray(l);

            if (obj is Dictionary<string, object> j)
            {
                NbtCompound result = new NbtCompound();
                foreach (KeyValuePair<string, object> pair in j)
                {
                    object o = pair.Value;
                    result.Add(pair.Key, ToNbt(Reflections.ToSerializable(o)));
                }
                return result;
            }

            if (obj is ICollection i)
            {
                if (i.Count == 0)
                {
                    return new NbtList(NbtTag.TagType.Byte);
                }

                Type t = i.GetType();
                NbtTag temp = ToNbt(t.GetProperty("Item").GetValue(i, new object[] { 0 }));
                NbtList result = new NbtList((NbtTag.TagType)temp.RawType);
                result.Add(temp);

                for (int z = 1; z < i.Count; z++)
                {
                    object x = t.GetProperty("Item").GetValue(i, new object[] { z });
                    if (x != null) result.Add(ToNbt(x));
                }
                return result;
            }

            if (Reflections.IsInstanceOfGenericType(typeof(Nullable<>), obj))
            {
                Type t = obj.GetType();
                bool hasValue = (bool)t.GetProperty("HasValue").GetValue(obj);
                if(hasValue)
                {
                    return ToNbt(t.GetProperty("Value").GetValue(obj));
                }
            }

            return ToNbt(Reflections.ToSerializable(obj));
        }

        public static string SerializeToString<T>(T obj)
        {
            StringSerializer serializer = new StringSerializer();
            object o = Reflections.ToSerializable(obj);
            NbtTag tag = ToNbt(o);
            serializer.Write(tag);
            return serializer.ToString();
        }

        internal static object InternalDeserialize(Type t, byte[] arr, bool named = false)
        {
            return InternalDeserialize(t, arr, 0, arr.Length, named);
        }

        internal static object InternalDeserialize(Type t, NbtTag tag)
        {
            if (t == null) return null;

            if (typeof(byte) == t)
            {
                if (!(tag is NbtByte)) throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into byte");
                return ((NbtByte)tag).Value;
            }

            if (typeof(bool) == t)
            {
                if (!(tag is NbtByte)) throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into bool");
                return ((NbtByte)tag).Value == 1;
            }

            if (typeof(short) == t)
            {
                if (!(tag is NbtShort)) throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into short");
                return ((NbtShort)tag).Value;
            }

            if (typeof(int) == t)
            {
                if (!(tag is NbtInt)) throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into int");
                return ((NbtInt)tag).Value;
            }

            if (typeof(long) == t)
            {
                if (!(tag is NbtLong)) throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into long");
                return ((NbtLong)tag).Value;
            }

            if (typeof(float) == t)
            {
                if (!(tag is NbtFloat)) throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into float");
                return ((NbtFloat)tag).Value;
            }

            if (typeof(double) == t)
            {
                if (!(tag is NbtDouble)) throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into double");
                return ((NbtDouble)tag).Value;
            }

            if (typeof(string) == t)
            {
                if (!(tag is NbtString)) throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into float");
                return ((NbtString)tag).Value;
            }

            if (typeof(long[]) == t)
            {
                if (!(tag is NbtLongArray)) throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into long[]");
                return ((NbtLongArray)tag).Value;
            }

            if (typeof(byte[]) == t)
            {
                if (!(tag is NbtByteArray)) throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into byte[]");
                return ((NbtByteArray)tag).Value;
            }

            if (Reflections.IsTypeOfGenericType(typeof(Nullable<>), t))
            {
                return InternalDeserialize(t.GetGenericArguments()[0], tag);
            }

            if (Reflections.IsTypeOfGenericType(typeof(Array), t))
            {
                if (!(tag is NbtList)) throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into float");

                List<object> list = new List<object>();
                foreach (NbtTag child in (NbtList)tag)
                {
                    Type related = t.GetGenericArguments()[0];
                    list.Add(InternalDeserialize(related, child));
                }
                return Convert.ChangeType(list, t);
            }

            if (Reflections.IsTypeOfGenericType(typeof(ICollection<>), t))
            {
                if (!(tag is NbtList)) throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into list");

                NbtList nl = (NbtList)tag;
                Type related = FromNbtType(nl.ContentType);

                if (related == null) return null;

                Type listType = typeof(List<>).MakeGenericType(new Type[] { related });
                object list = listType.GetConstructor(new Type[0]).Invoke(new object[0]);
                foreach (NbtTag child in nl)
                {
                    listType.GetMethod("Add").Invoke(list, new object[] { InternalDeserialize(related, child) });
                }
                return list;
            }

            Dictionary<string, object> result = new Dictionary<string, object>();
            NbtCompound c = (NbtCompound)tag;
            foreach(KeyValuePair<string, NbtTag> pair in c)
            {
                Type related = FromNbtType((NbtTag.TagType)pair.Value.RawType);
                result.Add(pair.Key, InternalDeserialize(related, pair.Value));
            }
            return result;
        }

        internal static Type FromNbtType(NbtTag.TagType type)
        {
            switch (type)
            {
                case NbtTag.TagType.Byte:
                    return typeof(byte);
                case NbtTag.TagType.Short:
                    return typeof(short);
                case NbtTag.TagType.Int:
                    return typeof(int);
                case NbtTag.TagType.Long:
                    return typeof(long);
                case NbtTag.TagType.Float:
                    return typeof(float);
                case NbtTag.TagType.Double:
                    return typeof(double);
                case NbtTag.TagType.ByteArray:
                    return typeof(byte[]);
                case NbtTag.TagType.String:
                    return typeof(string);
                case NbtTag.TagType.List:
                    return typeof(ICollection<object>);
                case NbtTag.TagType.Compound:
                    return typeof(Dictionary<string, object>);
                case NbtTag.TagType.IntArray:
                    return typeof(int[]);
                case NbtTag.TagType.LongArray:
                    return typeof(long[]);
            }
            return null;
        }

        internal static object InternalDeserialize(Type t, byte[] arr, int index, int length, bool named = false)
        {
            int _i = index;
            NbtTag tag = NbtTag.Deserialize(arr, ref _i, named);
            return InternalDeserialize(t, tag);
        }

        public static T Deserialize<T>(byte[] buffer, bool named = false)
        {
            object result = InternalDeserialize(typeof(T), buffer, named);
            if (result.GetType() == typeof(T)) return (T)result;

            if(result is Dictionary<string, object>)
            {
               return (T) CastFromDictionary(typeof(T), (Dictionary<string, object>)result);
            }

            return default(T);
        }

        private static NbtTag FromObject(object obj)
        {
            int index = 0;
            return NbtTag.Deserialize(SerializeToBuffer(obj), ref index);
        }

        private static object CastFromDictionary(Type t, Dictionary<string, object> dict)
        {
            object result = Activator.CreateInstance(t);
            var props = t.GetProperties();
            foreach(var prop in props)
            {
                var attr = prop.GetCustomAttribute<NbtConverterAttribute>();
                if (attr != null)
                {
                    var converter = (NbtConverter)Activator.CreateInstance(attr.ConverterType);
                    var val = dict[prop.Name];
                    prop.SetValue(result, converter.FromNbt(FromObject(val)));
                }
                else
                {

                    if (dict.ContainsKey(prop.Name))
                    {
                        var val = dict[prop.Name];
                        if (val is Dictionary<string, object>)
                        {
                            val = CastFromDictionary(prop.PropertyType, (Dictionary<string, object>)val);
                        }
                        prop.SetValue(result, val);
                    }
                }
            }
            return result;
        }
    }
}
