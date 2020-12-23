Get-ChildItem -Path . -Filter *.xml |
ForEach-Object {
    $x = [xml](Get-Content $_.FullName)
    $x.Save($_.FullName)
}