# Simulates an external sensor: POSTs the raw contents of a JSON file to the Agent's
# HTTP intake. Raw file bytes on purpose — corrupt the file's JSON syntax to test the
# malformed-body 400 path too. Written for Windows PowerShell 5.1.
param(
    [string]$File = "$PSScriptRoot\sample-events\valid-event.json",
    [string]$Url = "http://localhost:5100/events/sensor"
)

$body = [System.IO.File]::ReadAllText($File)

try {
    $response = Invoke-WebRequest -Uri $Url -Method Post -ContentType "application/json" -Body $body -UseBasicParsing
    Write-Host "HTTP $([int]$response.StatusCode)"
    Write-Host $response.Content
} catch [System.Net.WebException] {
    $res = $_.Exception.Response
    if ($null -eq $res) { throw }
    $reader = New-Object System.IO.StreamReader($res.GetResponseStream())
    Write-Host "HTTP $([int]$res.StatusCode)"
    Write-Host $reader.ReadToEnd()
}
