$Content = Get-Content .\input.txt
[int[]]$Numbers = $Content[0] -split ','

class Board {
    [System.Collections.Generic.List[System.Collections.Generic.List[Tile]]]$Board
    [bool]$Bingo
}
class Tile {
    [int]$Value
    [bool]$Matched
}
function New-Board {
    param(
        [string[]]$Lines
    )
    $List = [System.Collections.Generic.List[System.Collections.Generic.List[Tile]]]::new()
    foreach ($Line in $Lines) {
        $Sublist = [System.Collections.Generic.List[Tile]]::new()
        $Line -split ' ' | ForEach-Object {
            $Tile = [Tile]::new()
            $Tile.Value = $_
            $Sublist.Add($Tile)
        }
        $List.Add($Sublist)
    }
    $Board = [Board]::new()
    $Board.Board = $List
    Write-Output $Board
}
function Test-Bingo {
    param(
        [Board]$Board
    )
    $Bingo = $false
    for ($i = 0; $i -lt 5; $i++) {
        $VerticalMatch = $true
        $HorizontalMatch = $true
        for ($j = 0; $j -lt 5; $j++) {
            if (-not $Board.Board[$i][$j].Matched) {
                $VerticalMatch = $false
            }
            if (-not $Board.Board[$j][$i].Matched) {
                $HorizontalMatch = $false
            }
        }
        if ($HorizontalMatch -or $VerticalMatch) {
            $Bingo = $true
            break
        }
    }

    return $Bingo
}
function Find-Bingo {
    param(
        [int[]]$Numbers,
        [Board[]]$Boards,
        [switch]$FindLast
    )
    
    $OutputSum = 0
    foreach ($Number in $Numbers) {
        foreach ($Board in $Boards) {
            foreach ($Column in $Board.Board) {
                foreach ($Tile in $Column) {
                    if ($Number -eq $Tile.Value) {
                        $Tile.Matched = $true
                    }
                }
            }
            if (-not $Board.Bingo) {
                if ((Test-Bingo $Board)) {
                    $Board.Bingo = $true
                    $BingoBoardSum = 0
                    foreach ($Column in $Board.Board) {
                        foreach ($Tile in $Column) {
                            if (-not $Tile.Matched) {
                                $BingoBoardSum += $Tile.Value
                            }
                        }
                    }
                    $OutputSum = $BingoBoardSum * $Number
                    if (-not $FindLast.IsPresent) {
                        return $BingoBoardSum * $Number
                    }
                }
            }
        }
    }
    return $OutputSum
}

# Loop through all boards, skip first two lines of file
$Boards = for ($i = 2; $i -lt $Content.Count; $i += 6) {
    if ([string]::IsNullOrWhiteSpace($Content[$i])) {
        continue
    }
    New-Board ($Content[$i..($i + 4)])
}

# Write-Host "Part 1: $(Find-Bingo $Numbers $Boards)"
Write-Host "Part 2: $(Find-Bingo $Numbers $Boards -FindLast)"