$fileNames = Get-ChildItem -Path $scriptPath

foreach ($file in $fileNames)
{
    if ($file.Name.EndsWith("vert") -Or $file.Name.EndsWith("frag") -Or $file.Name.EndsWith("comp"))
    {
        Write-Host "Compiling $file"
        glslangvalidator -V $file -o $file".spv"
    }
}

..\..\..\Veldrid.SPIRV.VariantCompiler\bin\Release\netcoreapp3.1\Veldrid.SPIRV.VariantCompiler.exe --search-path .\FlverShader --output-path .\FlverShader --set .\FlverShader\FlverShader.json