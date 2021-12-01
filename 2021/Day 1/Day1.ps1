# Import lines as ints
[int[]]$Scans = Get-Content .\input.txt

$LargerMeasurementsCount = 0

# Start comparing from second row, since the first one doesn't have a previous line to compare with
for ($i = 1; $i -lt $Scans.Count; $i++) {
    if ($Scans[$i] -gt $Scans[$i - 1]) {
        $LargerMeasurementsCount++
    }
}

Write-Host "Part 1: $LargerMeasurementsCount"

$LargerWindowMeasurementsCount = 0

# Start from fourth row and compare three steps back
for ($i = 4; $i -lt $Scans.Count; $i++) {
    $PreviousWindowSum = $Scans[($i - 1)..($i - 3)] | Measure-Object -Sum | Select-Object -ExpandProperty Sum
    $CurrentWindowSum = $Scans[$i..($i - 2)] | Measure-Object -Sum | Select-Object -ExpandProperty Sum
    if ($CurrentWindowSum -gt $PreviousWindowSum) {
        $LargerWindowMeasurementsCount++
    }
}

Write-Host "Part 2: $LargerWindowMeasurementsCount"