function Save-CurrentPosition {
    [CmdletBinding()]
    param(
        [int]$WireIndex,
        [int]$OldX,
        [int]$OldY,
        [int]$NewX,
        [int]$NewY,
        [string]$Direction,
        [int]$PositionDistance,
        [int]$Distance
    )

    [void]$Script:WirePositions[$WireIndex].Add([PSCustomObject]@{
            TotalDistance = $PositionDistance + $Distance
            Distance      = $Distance
            Direction     = $Direction
            OldX          = $OldX
            OldY          = $OldY
            NewX          = $NewX
            NewY          = $NewY
        }
    )
}

$PuzzleInput = Get-Content .\input.txt

# Create a list of lists for each wire to store positions
$Script:WirePositions = New-Object System.Collections.ArrayList
$PuzzleInput | ForEach-Object { [void]$Script:WirePositions.Add((New-Object System.Collections.ArrayList)) }

# For each wire
for ($i = 0; $i -lt $PuzzleInput.Count; $i++) {
    $Turns = $PuzzleInput[$i] -split ','

    $x = 0
    $y = 0
    $TotalDistance = 0

    # For each position change
    foreach ($Turn in $Turns) {
        $Direction = $Turn[0]
        [int]$Distance = $Turn.Substring(1)
        $TotalDistance += $Distance

        $OldX = $x
        $OldY = $y

        # Change current position based on direction
        switch ($Direction) {
            U { $y += $Distance }
            D { $y -= $Distance }
            L { $x -= $Distance }
            R { $x += $Distance }
        }

        # Save current position
        Save-CurrentPosition -WireIndex $i -OldX $OldX -OldY $OldY -NewX $x -NewY $y -Direction $Direction -Distance $Distance -PositionDistance $TotalDistance
    }
}

# See if the wires cross anywhere
# Start at 1 and compare with the previous wire's positions
$CrossPositions = New-Object System.Collections.ArrayList
for ($i = 1; $i -lt $Script:WirePositions.Count; $i++) {
    foreach ($PosB in $Script:WirePositions[$i]) {
        foreach ($PosA in $Script:WirePositions[$i - 1]) {
            
            $IntersectionX = $false
            $IntersectionY = $false

            # Find if X of wire 1 is within range of X position of wire 2
            if (($PosB.NewX -ge $PosA.OldX -and $PosB.NewX -le $PosA.NewX) -or ($PosB.NewX -ge $PosA.NewX -and $PosB.NewX -le $PosA.OldX)) {
                $IntersectionX = $true
            }

            # Find if Y of wire 2 is within range of Y position of wire 1
            if (($PosA.NewY -ge $PosB.OldY -and $PosA.NewY -le $PosB.NewY) -or ($PosA.NewY -ge $PosB.NewY -and $PosA.NewY -le $PosB.OldY)) {
                $IntersectionY = $true
            }
            
            # Exclude center cross position
            if ($IntersectionX -and $IntersectionY -and !($PosB.NewX -eq 0 -and $PosA.NewY -eq 0)) {
                [void]$CrossPositions.Add(
                    [PSCustomObject]@{
                        x             = $PosB.NewX
                        y             = $PosA.NewY
                        TotalDistance = $PosB.TotalDistance + $PosA.TotalDistance
                    }
                )
                $CrossPositions[$CrossPositions.Count - 1]
            }
        }
    }
}

# Part one: 870
"Part one: $(($CrossPositions | ForEach-Object { [math]::Abs($_.x) + [math]::Abs($_.y) } | Sort-Object)[0])"