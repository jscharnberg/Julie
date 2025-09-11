using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Julie.Core.Services
{
    public class UpdateService
    {
        private readonly string _repoOwner;
        private readonly string _repoName;

        public UpdateService(string repoOwner, string repoName)
        {
            _repoOwner = repoOwner;
            _repoName = repoName;
        }

        public async Task<string?> GetLatestVersionAsync()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Julie-App");

            var url = $"https://api.github.com/repos/{_repoOwner}/{_repoName}/releases/latest";
            try
            {
                var json = await client.GetStringAsync(url);
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("tag_name", out var tag))
                {
                    return tag.GetString();
                }
            }
            catch
            {
                // Fehler ignorieren, z.B. kein Internet
            }

            return null;
        }

        public string GetDownloadUrl(string version)
        {
            return $"https://github.com/{_repoOwner}/{_repoName}/releases/download/{version}/Julie.zip";
        }
    }
}
