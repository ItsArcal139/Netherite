using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Linq;
using Netherite.Generator.Enums;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Netherite.Generator
{
    class Program
    {
        static void Main(string[] args)
        {
            GenerateBlockStates();
        }

        public static void GenerateBlockStates()
        {
            string content = @"
namespace Netherite.Protocol.v754
{
    public partial class Registry
    {
        public static List<(int, string)> IdState = new List<(int, string)>();

        static Registry()
        {";

            var json = JObject.Parse(File.ReadAllText("blocks.json"));
            foreach(var (name, token) in json)
            {
                JArray states = (JArray)token.SelectToken("states");
                foreach(var state in states)
                {
                    var str = name;
                    var propToken = state.SelectToken("properties");
                    if(propToken != null)
                    {
                        var pStr = "";
                        JObject prop = (JObject)propToken;
                        foreach(var (pName, pVal) in prop)
                        {
                            pStr += "," + pName + "=" + ((JValue)pVal).Value.ToString();
                        }
                        str += "[" + pStr[1..] + "]";
                    }

                    var defToken = state.SelectToken("default");
                    bool isDefaultState = defToken != null;

                    var id = ((JValue)state.SelectToken("id")).Value;
                    content += @$"
            IdState.Add(({id}, ""{str}""));";
                }
            }

            content += @"
        }
    }
}";
            File.WriteAllText("Registry.BlockState.cs", content);
        }
    }
}
