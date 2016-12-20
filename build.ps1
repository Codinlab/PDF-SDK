$rootFolder = Split-Path -parent $script:MyInvocation.MyCommand.Definition
$nupkgsFolder = Join-Path $rootFolder "nupkgs"

dotnet restore
dotnet build

dotnet test test\DocumentFormat.Pdf.Tests

if($?) {
	dotnet pack src/DocumentFormat.Pdf --output $nupkgsFolder
}
else {
	exit 1
}