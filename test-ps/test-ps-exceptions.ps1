function Do-Something
{
    throw "Bad thing happened"
}

function TrapTest {
    trap {"Error found."}
    nonsenseString
}

try
{
    Do-Something
}
catch
{
    Write-Output "Something threw an exception"
}

try
{
    Do-Something -ErrorAction Stop
}
catch
{
    Write-Output "Something threw an exception or used Write-Error"
}

try {
    Get-Item "e:" -ErrorAction SilentlyContinue -ErrorVariable ec
    # TrapTest
}
catch {
     Write-Output "!"
}