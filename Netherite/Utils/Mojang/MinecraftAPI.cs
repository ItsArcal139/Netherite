using Netherite.Auth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Netherite.Utils.Mojang
{
    public static class MinecraftAPI
    {
        private static HttpClient http = new HttpClient();

        static MinecraftAPI()
        {
            var userAgent = http.DefaultRequestHeaders.UserAgent;
            userAgent.Clear();
            userAgent.Add(new ProductInfoHeaderValue("Netherite", "0.1"));
            userAgent.Add(new ProductInfoHeaderValue(".NETCore", "3.1"));
        }

        public static async Task<List<GameProfile>> GetUsersAsync(params string[] usernames)
        {
            using (var response = await http.PostAsync("https://api.mojang.com/profiles/minecraft",
                new StringContent(
                    JsonConvert.SerializeObject(usernames),
                    Encoding.UTF8, "application/json")
                ))
            {
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<List<MojangUser>>(await response.Content.ReadAsStringAsync())
                        .ConvertAll(u => (GameProfile)u);
                }
                return null;
            }
        }

        public static async Task<GameProfile> GetUserAndSkinAsync(string uuid)
        {
            using (var response = await http.GetAsync("https://sessionserver.mojang.com/session/minecraft/profile/" + uuid))
            {
                Debugger.Break();
                if (response.IsSuccessStatusCode)
                {
                    return (GameProfile)JsonConvert.DeserializeObject<MojangUser>(await response.Content.ReadAsStringAsync());
                }
            }

            return null;
        }

        public static async Task<GameProfile> GetUserAsync(string username)
        {
            var result = await GetUsersAsync(new[] { username });
            if (result == null || result.Count <= 0) return null;
            return result.FirstOrDefault();
        }

        public static async Task<GameProfile> HasJoined(string username, string serverId)
        {
            using (var response = await http.GetAsync($"https://sessionserver.mojang.com/session/minecraft/hasJoined?username={username}&serverId={serverId}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    return (GameProfile)JsonConvert.DeserializeObject<JoinedResponse>(await response.Content.ReadAsStringAsync());
                }
            }

            return null;
        }
    }
}
