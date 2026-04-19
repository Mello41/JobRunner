using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace JobRunner.Avalonia.Services
{
    public class TagService
    {
        private readonly string _filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "JobRunner", "usertags.json");

        public ConcurrentDictionary<string, string> UserTags { get; set; } = new(); // TagName -> Color

        public async Task LoadAsync()
        {
            if (!File.Exists(_filePath)) return;
            var json = await File.ReadAllTextAsync(_filePath);
            UserTags = JsonSerializer.Deserialize<ConcurrentDictionary<string, string>>(json) ?? new();
        }

        public async Task SaveAsync()
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir!);
            var json = JsonSerializer.Serialize(UserTags);
            await File.WriteAllTextAsync(_filePath, json);
        }

        public void AddTag(string name, string color = "#888888")
        {
            UserTags.TryAdd(name, color);
        }

        public void RemoveTag(string name)
        {
            UserTags.TryRemove(name, out _);
        }
    }
}
