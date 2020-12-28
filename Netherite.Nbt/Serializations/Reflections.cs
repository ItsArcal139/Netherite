using Netherite.Nbt.Serializations.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Netherite.Nbt.Serializations
{
    internal static class Reflections
    {
        internal static string[] GetProperties<T>() => GetProperties(typeof(T));

        internal static string[] GetProperties(Type t)
        {
            List<string> results = new List<string>();
            PropertyInfo[] props = t.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                results.Add(prop.Name);
            }
            return results.ToArray();
        }

        internal static Dictionary<string, object> ToDictionary(object obj)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            Type t = obj.GetType();
            foreach (string prop in GetProperties(t))
            {
                PropertyInfo p = t.GetProperty(prop);
                object op = p.GetValue(obj);
                if (op != null)
                {
                    var attr = p.GetCustomAttribute<NbtConverterAttribute>();
                    if (attr != null)
                    {
                        NbtConverter c = (NbtConverter)attr.ConverterType.GetConstructor(new Type[0]).Invoke(new object[0]);
                        op = c.ToNbt(op);
                    }
                    else
                    {
                        op = ToSerializable(op);
                    }

                    if (p.GetCustomAttribute<NbtIgnoreAttribute>() == null)
                    {
                        var pAttr = p.GetCustomAttribute<NbtPropertyAttribute>();
                        if (pAttr != null)
                        {
                            result.Add(pAttr.Name, op);
                        }
                        else
                        {
                            result.Add(prop, op);
                        }
                    }
                }
            }
            return result;
        }

        internal static object ToSerializable(object o)
        {
            if (o is Dictionary<string, object>) return o;

            if (o is NbtTag) return o;

            Type t = o.GetType();
            if (t.IsPrimitive) return o;

            if (o is string s) return s;

            if (IsInstanceOfGenericType(typeof(IDictionary<,>), o))
            {
                IDictionary<object, object> d = o as IDictionary<object, object>;
                Type[] args = t.GenericTypeArguments;
                if (args[0] != typeof(string))
                {
                    Dictionary<string, object> inner = new Dictionary<string, object>();
                    foreach (KeyValuePair<object, object> pair in d)
                    {
                        inner.Add(pair.Key.ToString(), ToSerializable(pair.Value));
                    }
                    return inner;
                }
                return o;
            }

            if (o is ICollection || IsInstanceOfGenericType(typeof(ICollection<>), o))
            {
                return o;
            }

            return ToDictionary(o);
        }

        internal static bool IsInstanceOfGenericType(Type genericType, object instance)
        {
            return IsTypeOfGenericType(genericType, instance.GetType());
        }

        internal static bool IsTypeOfGenericType(Type genericType, Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
                type = type.BaseType;
            }
            return false;
        }
    }
}
