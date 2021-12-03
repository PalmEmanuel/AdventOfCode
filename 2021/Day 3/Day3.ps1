[string[]]$Diagnostics = Get-Content .\input.txt

[string]$GammaRate = ''

$PositionHash = @{}
# 0..11
0..($Diagnostics[0].Length - 1) | ForEach-Object {
    $PositionHash["$_-0"] = 0
    $PositionHash["$_-1"] = 0
    foreach ($DiagnosticEntry in $Diagnostics) {
        $PositionHash["$_-$($DiagnosticEntry[$_])"]++
    }
    if ($PositionHash["$_-1"] -gt $PositionHash["$_-0"]) {
        $GammaRate += 1
    }
    else {
        $GammaRate += 0
    }
}

# Reverse gamma rate by swapping 1s and 0s
$EpsilonRate = $GammaRate -replace '1', '2' -replace '0', '1' -replace '2', '0'

$PowerConsumption = Invoke-Expression "0b$EpsilonRate * 0b$GammaRate"

Write-Host "Part 1: $PowerConsumption"

# Initialize lists to filter
$Oxygen = $Diagnostics
$CO2 = $Diagnostics
for ($i = 0; $i -lt $Diagnostics[0].Length; $i++) {
    # Oxygen
    if ($Oxygen.Count -gt 1) {
        if ($Oxygen.Where{ $_[$i] -eq '1' }.Count -ge ($Oxygen.Count / 2)) {
            $Oxygen = $Oxygen | Where-Object { $_[$i] -eq '1' }
        }
        else {
            $Oxygen = $Oxygen | Where-Object { $_[$i] -eq '0' }
        }
    }

    # CO2
    if ($CO2.Count -gt 1) {
        if ($CO2.Where{ $_[$i] -eq '1' }.Count -ge ($CO2.Count / 2)) {
            $CO2 = $CO2 | Where-Object { $_[$i] -eq '0' }
        }
        else {
            $CO2 = $CO2 | Where-Object { $_[$i] -eq '1' }
        }
    }
}
$LifeSupportRating = Invoke-Expression "0b$Oxygen * 0b$CO2"
Write-Host "Part 2: $LifeSupportRating"
