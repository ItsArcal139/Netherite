using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Threading;

namespace Netherite.Net.Protocols
{
    public static class ProtocolLoader
    {
        private static AssemblyLoadContext context = new AssemblyLoadContext(nameof(ProtocolLoader), true);
        private static SemaphoreSlim contextLock = new SemaphoreSlim(1, 1);
        private static List<Type> protocols = new List<Type>();

        public async static void LoadDLLs()
        {
            await contextLock.WaitAsync();

            try
            {
                foreach(Type t in protocols)
                {
                    Protocol.Unregister(t);
                }
                protocols.Clear();

                context.Unload();
                context = new AssemblyLoadContext(nameof(ProtocolLoader));

                if (!Directory.Exists("Protocols"))
                {
                    Directory.CreateDirectory("Protocols");
                }

                string[] files = Directory.GetFiles("Protocols");
                foreach (string file in files)
                {
                    if (file.ToLower().EndsWith(".dll"))
                    {
                        string path = Directory.GetCurrentDirectory() + "\\" + file;
                        Assembly asm = context.LoadFromAssemblyPath(path);
                        foreach(Type t in asm.GetTypes())
                        {
                            if(typeof(Protocol).IsAssignableFrom(t))
                            {
                                RuntimeHelpers.RunClassConstructor(t.TypeHandle);
                                protocols.Add(t);
                            }
                        }
                    }
                }
            } finally
            {
                contextLock.Release();
            }
        }
    }
}
