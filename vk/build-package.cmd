@echo off
dotnet build -c Release src\Vk.Generator\Vk.Generator.csproj
dotnet build -c Release src\Vk.Rewrite\Vk.Rewrite.csproj
dotnet pack -c Release src\Vk\Vk.csproj
