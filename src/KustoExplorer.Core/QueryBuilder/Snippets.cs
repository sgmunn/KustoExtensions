using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace KustoExplorer.Core.QueryBuilder
{
    public sealed class Snippets
    {
        private readonly Dictionary<string, QuerySnippet> snippets;

        public Snippets()
        {
            this.snippets = new Dictionary<string, QuerySnippet>();
        }

        public void LoadSnippets(string dir)
        {
            foreach (var file in Directory.EnumerateFiles(dir, "*.snippet"))
            {
                var s = JsonHelpers.DeserializeFromFile<QuerySnippet>(file);
                this.snippets.Add(s.Name, s);
            }
        }

        public void ClearLoadedSnippets()
        {
            this.snippets.Clear();
        }





        public static QuerySnippet Deserialize(string s)
        {
            var crashReport = JsonConvert.DeserializeObject<QuerySnippet>(s);
            return crashReport;
        }

        public static QuerySnippet DeserializeFromFile(string filename)
        {
            using (var file = File.OpenText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();
                var snippet = (QuerySnippet)serializer.Deserialize(file, typeof(QuerySnippet));
                return snippet;
            }
        }

        public static void Save(QuerySnippet report, string filename)
        {
            var snippet = JsonConvert.SerializeObject(report);
            File.WriteAllText(filename, snippet); ;
        }

        public static void SaveToFile(QuerySnippet snippet, string filename)
        {
            using (StreamWriter file = File.CreateText(filename))
            {
                JsonSerializer serializer = new JsonSerializer() { Formatting = Formatting.Indented };
                serializer.Serialize(file, snippet);
            }
        }

        public string GetSnippet(string name)
        {
            if (this.snippets.ContainsKey(name)) {
                return this.snippets[name].Text;
            }

            return "string.Empty";
        }
    }
}

