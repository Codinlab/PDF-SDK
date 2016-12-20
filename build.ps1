$rootFolder = Split-Path -parent $script:MyInvocation.MyCommand.Definition
$nupkgsFolder = Join-Path $rootFolder "nupkgs"

dotnet restore
dotnet build
dotnet pack src/DocumentFormat.Pdf --output $nupkgsFolder