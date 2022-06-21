#!/bin/sh

nuget install packages.config -OutputDirectory packages

mono ./packages/ILRepack.MSBuild.Task.2.0.13/tools/ilrepack.exe \
  /lib:../lib \
  /out:../lib/merged/Kusto.Data.Merged.dll \
  ../lib/Kusto.Data.dll \
  ../lib/Microsoft.Identity.Client.Extensions.Msal.dll \
  ../lib/Microsoft.Identity.Client.dll
  