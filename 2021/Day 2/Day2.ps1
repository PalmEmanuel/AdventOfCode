function New-Instruction {
    param(
        [string]$Instruction
    )
    $SplitInstruction = $Instruction -split ' '
    return [pscustomobject]@{
        Direction = $SplitInstruction[0]
        Distance = $SplitInstruction[1]
    }
}
function Start-Instruction {
    param(
        $Instructions
    )
    $HorizontalPosition = ($Instructions | Where-Object Direction -eq 'forward' | Select-Object -ExpandProperty Distance | Measure-Object -Sum).Sum
    $Depth = ($Instructions | Where-Object Direction -eq 'down' | Select-Object -ExpandProperty Distance | Measure-Object -Sum).Sum -
        ($Instructions | Where-Object Direction -eq 'up' | Select-Object -ExpandProperty Distance | Measure-Object -Sum).Sum

    return $HorizontalPosition * $Depth
}

$Instructions = Get-Content .\input.txt | ForEach-Object { New-Instruction $_ }

$Part1Result = Start-Instruction $Instructions

Write-Host "Part 1: $Part1Result"

function Start-AdjustedInstruction {
    param(
        $Instructions
    )
    $Aim = 0
    $Depth = 0
    $HorizontalPosition = 0

    foreach ($Instruction in $Instructions) {
        switch ($Instruction.Direction) {
            'forward' {
                $HorizontalPosition += [int]$Instruction.Distance
                $Depth += [int]$Instruction.Distance * $Aim
            }
            'down' { $Aim += [int]$Instruction.Distance }
            'up' { $Aim -= [int]$Instruction.Distance }
        }
    }

    return $HorizontalPosition * $Depth
}

$Part2Result = Start-AdjustedInstruction $Instructions

Write-Host "Part 2: $Part2Result"