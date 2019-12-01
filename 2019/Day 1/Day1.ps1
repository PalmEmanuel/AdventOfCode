$PuzzleInput = Get-Content .\input.txt

# Part one
$TotalFuel = ($PuzzleInput | ForEach-Object { [math]::Floor(([int]$_)/3)-2 } | Measure-Object -Sum).Sum

# 3302760
"Part one: $TotalFuel"

# Part two
$TotalFuel = 0

foreach ($ModuleMass in $PuzzleInput) {
    [int]$MassLeft = $ModuleMass
    $ModuleTotalFuel = 0
    
    do {
        $MassLeft = [math]::Floor( $MassLeft / 3 ) - 2

        if ($MassLeft -gt 0) {
            $ModuleTotalFuel += $MassLeft
        }
    } while ($MassLeft -gt 0)

    $TotalFuel += $ModuleTotalFuel
}

# 4951265
"Part two: $TotalFuel"