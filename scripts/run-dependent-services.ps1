Get-ChildItem -Recurse | `
Where-Object {
    $_.Name -eq "run-dependent-services.ps1" -and $_.DirectoryName -ne $PSScriptRoot
} | `
ForEach-Object {
    & $_
}