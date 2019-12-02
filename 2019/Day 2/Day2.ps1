$PuzzleInput = (Get-Content .\input.txt) -split ',' | ForEach-Object { [int]$_ }

$PuzzleInput[1] = 12
$PuzzleInput[2] = 2

$ProgramState = $PuzzleInput

:outerloop for ($i = 0; $i -lt $ProgramState.Count-1; $i += 4) {
    "$i / $($ProgramState.Count)"
    ""
    switch ([int]$ProgramState[$i]) {
        1 {
            $ProgramState[$ProgramState[$i+3]] = $ProgramState[$ProgramState[$i+1]] + $ProgramState[$ProgramState[$i+2]]
        }
        2 {
            $ProgramState[$ProgramState[$i+3]] = $ProgramState[$ProgramState[$i+1]] * $ProgramState[$ProgramState[$i+2]]
        }
        99 { break :outerloop }
        Default { throw "Something went wrong, opcode is $i" }
    }
}

"Part one: $($ProgramState[0])"