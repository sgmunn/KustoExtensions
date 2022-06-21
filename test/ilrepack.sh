#!/bin/sh

nuget install packages.config -OutputDirectory packages

mono ./packages/ILRepack.MSBuild.Task.2.0.13/tools/ilrepack.exe \
  /lib:../lib \
  /out:../lib/merged/Kusto.Merged.dll \
  ../lib/Azure.Core.dll \
  ../lib/Azure.Identity.dll \
  ../lib/Kusto.Cloud.Platform.Aad.dll \
  ../lib/Kusto.Cloud.Platform.dll \
  ../lib/Kusto.Data.dll \
  ../lib/Newtonsoft.Json.dll \
  ../lib/Microsoft.Bcl.AsyncInterfaces.dll \
  ../lib/Microsoft.Identity.Client.dll \
  ../lib/System.Text.Json.dll
  