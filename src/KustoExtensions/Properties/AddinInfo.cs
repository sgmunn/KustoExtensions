using System.Runtime.Versioning;
using Mono.Addins;
using Mono.Addins.Description;

[assembly: Addin(
    "KustoExtensions",
    Namespace = "VisualStudio",
    Version = "0.0.1"
)]

[assembly: AddinName("VisualStudio Kusto Extensions")]
[assembly: AddinCategory("IDE extensions")]
[assembly: AddinDescription("Run your Kusto Queries from the IDE")]
[assembly: AddinAuthor("Greg Munn")]