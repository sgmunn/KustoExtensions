using System.Text;
using Kusto.Cloud.Platform.Utils;
using Newtonsoft.Json;

namespace KustoExplorer.Core.QueryBuilder
{
    public class QuerySnippet
    {
        public QuerySnippet()
        {
        }

        public QuerySnippet(string name)
        {
            this.Name = name;
        }

        public QuerySnippet(string name, string text) : this(name)
        {
            this.Text = text;
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class Query
    {
        public Query()
        {
        }

        public Query(string name)
        {
            this.Name = name;
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("snippets")]
        public QuerySnippet[] Snippets { get; set; }
    }

    public class QueryDefintion
    {
        public QueryDefintion()
        {
        }

        public QueryDefintion(string name)
        {
            this.Name = name;
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("snippets")]
        public string[] Snippets { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }
    }

    public static class QueryBuilder
    {
        public static string BuildQuery(QueryDefintion queryDefintion)
        {
            if (string.IsNullOrEmpty(queryDefintion?.Body))
            {
                return string.Empty;
            }

            var root = "/Users/gregm/Projects/QuerySnippets";
            var snippets = new Snippets();

            snippets.LoadSnippets(root);

            // todo: much smarter replacer, tokenizer etc
            var sb = new StringBuilder();
            int loc = 0;
            var currentSnippetName = string.Empty;
            var scanSnippetName = false;

            while (loc < queryDefintion.Body.Length)
            {
                var token = PeekToken(queryDefintion.Body, loc, out char currentChar);
                switch (token)
                {
                    case Token.SnippetStart:
                        scanSnippetName = true;
                        currentSnippetName = string.Empty;
                        loc++;
                        break;
                    case Token.SnippetEnd:
                        scanSnippetName = false;
                        sb.Append(snippets.GetSnippet(currentSnippetName));
                        loc++;
                        break;
                    default:
                        if (scanSnippetName)
                        {
                            currentSnippetName += currentChar;
                        }
                        else
                        {
                            sb.Append(currentChar);
                        }

                        break;
                }

                loc++;
            }

            return sb.ToString();

            static char PeekChar(string text, int loc)
            {
                if (loc >= text.Length)
                {
                    return char.MinValue;
                }

                return text[loc];
            }

            static Token PeekToken(string text, int loc, out char currentChar)
            {
                currentChar = PeekChar(text, loc);
                if (currentChar == char.MinValue)
                {
                    return Token.Null;
                }

                var nextChar = PeekChar(text, loc + 1);
                switch (currentChar)
                {
                    case '!':
                        switch (nextChar)
                        {
                            case '#':
                                return Token.SnippetStart;
                            default:
                                //sb.Append(currentChar);
                                return Token.Text;
                        }

                    case '#':
                        switch (nextChar)
                        {
                            case '!':
                                return Token.SnippetEnd;
                            default:
                                //sb.Append(currentChar);
                                return Token.Text;
                        }

                    default:
                        //sb.Append(currentChar);
                        return Token.Text;
                }
            }
        }

        enum Token
        {
            Null,
            Text,
            SnippetStart,
            SnippetEnd,
        }
    }
}