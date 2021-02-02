using Netherite.Texts;
using Netherite.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace Netherite.Net.Protocols
{
    public static class ProtocolLoader
    {
        private static AssemblyLoadContext context = new AssemblyLoadContext(nameof(ProtocolLoader), true);
        private static SemaphoreSlim contextLock = new SemaphoreSlim(1, 1);
        private static List<Type> protocols = new List<Type>();

        public static void LoadProtocols()
        {
            Logger.Info(
                LiteralText.Of("Loading protocols...")
                );

            foreach (Type t in protocols)
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

            List<Task> loadTasks = new List<Task>();

            SemaphoreSlim loadLock = new SemaphoreSlim(1, 1);

            string[] files = Directory.GetFiles("Protocols");
            foreach (string file in files)
            {
                if (file.ToLower().EndsWith(".dll"))
                {
                    loadTasks.Add(Task.Run(() =>
                    {
                        string path = Directory.GetCurrentDirectory() + "\\" + file;
                        Assembly asm = context.LoadFromAssemblyPath(path);
                        foreach (Type t in asm.GetTypes())
                        {
                            if (typeof(Protocol).IsAssignableFrom(t))
                            {
                                try
                                {
                                    RuntimeHelpers.RunClassConstructor(t.TypeHandle);
                                    protocols.Add(t);
                                }
                                catch (TypeInitializationException ex)
                                {
                                    loadLock.Wait();
                                    Logger.Error(
                                        TranslateText.Of("Error loading %s")
                                        .AddWith(LiteralText.Of(file).SetColor(TextColor.Gold))
                                        );
                                    Logger.Error(ex.InnerException.GetType().Name + ": " + ex.InnerException.Message);

                                    string[] lines = ex.InnerException.StackTrace.Split('\n');
                                    foreach (string line in lines)
                                    {
                                        Logger.Error(line);
                                    }
                                    loadLock.Release();
                                }
                                catch (Exception ex)
                                {
                                    loadLock.Wait();
                                    Logger.Error(
                                        TranslateText.Of("Error loading %s")
                                        .AddWith(LiteralText.Of(file).SetColor(TextColor.Gold))
                                        );
                                    Logger.Error(ex.GetType().Name + ": " + ex.Message);

                                    string[] lines = ex.StackTrace.Split('\n');
                                    foreach(string line in lines)
                                    {
                                        Logger.Error(line);
                                    }
                                    loadLock.Release();
                                }
                            }
                        }
                    }));
                }
            }

            Task.WaitAll(loadTasks.ToArray());
            Logger.Info(LiteralText.Of(protocols.Count + " protocols has been loaded."));
        }

        public async static Task LoadProtocolsAsync()
        {
            await Task.Run(async () =>
            {
                await contextLock.WaitAsync();
                try
                {
                    LoadProtocols();
                }
                finally
                {
                    contextLock.Release();
                }
            });
        }
    }
}
