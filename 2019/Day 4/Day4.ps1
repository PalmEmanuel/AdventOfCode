$PuzzleInput = '234208-765869'
[int]$Min, [int]$Max = $PuzzleInput -split '-'

# It is a six-digit number.
# The value is within the range given in your puzzle input.
# Two adjacent digits are the same (like 22 in 122345).
# Going from left to right, the digits never decrease; they only ever increase or stay the same (like 111123 or 135679).

# 111111 meets these criteria (double 11, never decreases).
# 223450 does not meet these criteria (decreasing pair of digits 50).
# 123789 does not meet these criteria (no double).

$PassList = New-Object System.Collections.Generic.List[string]
for ($i = $Min; $i -le $Max; $i++) {
    # Create string to compare with
    $PassString = [string]$i

    $CharArray = $PassString.ToCharArray()

    # If the list is the same sorted, it never decreases
    # If the list is not the same when filtering on unique numbers, it has duplicates (in order)
    if ($PassString -eq (($CharArray | Sort-Object) -join '') -and 
        $PassString -ne (($CharArray | Sort-Object -Unique) -join '')) {
        $PassList.Add($PassString)
    }
}

# 1246
"Part one: $($PassList.Count)"

$FilteredPassList = New-Object System.Collections.Generic.List[string]
foreach ($Pass in $PassList) {
    $GroupCount = ($Pass.ToCharArray() | Group-Object | Select-Object -ExpandProperty Count)

    if ($GroupCount.Count -gt 1) {
        if ($GroupCount.Contains(2)) {
            $FilteredPassList.Add($Pass)
        }
    }
}

# 814
"Part two: $($FilteredPassList.Count)"