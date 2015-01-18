$fileContent = Get-Content 'src\Autofac.Integration.Mef\Properties\AssemblyInfo.cs'
$regex = new-object System.Text.RegularExpressions.Regex ('AssemblyInformationalVersion\("(.+?)"\)', [System.Text.RegularExpressions.RegexOptions]::MultiLine)
$match = $regex.Match($fileContent)
$productVersion = $match.Groups[1]

Write-Host "Setting %PackageVersion% environment variable to $productVersion"

[Environment]::SetEnvironmentVariable("PackageVersion", $productVersion)