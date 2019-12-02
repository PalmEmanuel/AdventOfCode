$PuzzleInput = (Get-Content .\input.txt) -split ',' | ForEach-Object { [int]$_ }

function Get-IntcodeOutput {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [int[]]$PuzzleInput,

        [Parameter(Mandatory)]
        [int]$Verb,

        [Parameter(Mandatory)]
        [int]$Noun
    )

    $ProgramState = $PuzzleInput
    
    $PuzzleInput[1] = $Verb
    $PuzzleInput[2] = $Noun

    :outerloop for ($i = 0; $i -lt $ProgramState.Count - 1; $i += 4) {
        switch ([int]$ProgramState[$i]) {
            1 {
                $ProgramState[$ProgramState[$i + 3]] = $ProgramState[$ProgramState[$i + 1]] + $ProgramState[$ProgramState[$i + 2]]
            }
            2 {
                $ProgramState[$ProgramState[$i + 3]] = $ProgramState[$ProgramState[$i + 1]] * $ProgramState[$ProgramState[$i + 2]]
            }
            99 { break :outerloop }
            Default { throw "Something went wrong, opcode is $i" }
        }
    }

    return $ProgramState[0]
}

$Result = Get-IntcodeOutput -PuzzleInput $PuzzleInput -Verb 12 -Noun 2

# 3409710
"Part one: $Result"

# Brute force it to find answers where noun x and verb y produces output 19690720
:outerloop for ($i = 0; $i -lt 99; $i++) {
    for ($j = 0; $j -lt 99; $j++) {
        $TempResult = Get-IntcodeOutput -PuzzleInput $PuzzleInput -Verb $j -Noun $i

        if ($TempResult -eq 19690720)
        {
            $FinalParams = @{
                Verb = $i
                Noun = $j
            }

            break :outerloop
        }
    }
}

# Times 100
$Result = 100 * $FinalParams.Noun + $FinalParams.Verb

# 7912
"Part two: $Result"