using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Data.Entities
{
    public struct Identifier
    {
        private string _namespace;
        public string Namespace
        {
            get => _namespace;
            set
            {
                ValidateNamespace(value);
                _namespace = value;
            }
        }

        private string _key;
        public string Key
        {
            get => _key;
            set
            {
                ValidateKey(value);
                _key = value;
            }
        }

        public static Identifier Minecraft(string key)
        {
            return new Identifier(key);
        }

        public Identifier(string key, string @namespace = "minecraft")
        {
            _key = key;
            _namespace = @namespace;
        }

        private void ValidateNamespace(string name)
        {
            foreach (char c in name)
            {
                bool valid = (
                    c >= '0' && c <= '9'
                    || c >= 'a' && c <= 'z'
                    || c == '-' || c == '_'
                );

                if (!valid)
                {
                    throw new ArgumentException($"{name} is not a valid namespace.");
                }
            }
        }

        private void ValidateKey(string name)
        {
            foreach (char c in name)
            {
                bool valid = (
                    c >= '0' && c <= '9'
                    || c >= 'a' && c <= 'z'
                    || c == '-' || c == '_'
                    || c == '.' || c == '/'
                );

                if (!valid)
                {
                    throw new ArgumentException($"{name} is not a valid key.");
                }
            }
        }

        public override string ToString()
        {
            return $"{Namespace}:{Key}";
        }
    }
}
