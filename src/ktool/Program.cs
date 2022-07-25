// See https://aka.ms/new-console-template for more information
using KustoExplorer.Core;
using KustoExplorer.Core.QueryBuilder;

Console.WriteLine("Hello, World!");


var r = new QueryDefintion { Body = "RawEvebts | !#n1#! |  !#n2#! " };
var s = QueryBuilder.BuildQuery(r);


var x = new QuerySnippet("n", "czxczx");

Snippets.SaveToFile(x, "/Users/gregm/text.json");

var y = Snippets.DeserializeFromFile("/Users/gregm/text.json");

Console.WriteLine(y.Name);

var q = new Query("asd");
q.Snippets = new QuerySnippet[2]
{
    new QuerySnippet("aaa", "aaaaaa"),
    new QuerySnippet("bbb", "bbbbbbb"),
};

JsonHelpers.SaveToFile(q, "/Users/gregm/text.json");