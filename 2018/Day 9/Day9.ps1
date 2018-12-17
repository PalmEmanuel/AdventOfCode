$rawInput = Get-Content "$PSScriptRoot\input.txt"

[void]($rawInput -match "(?<players>[0-9]*) players; last marble is worth (?<points>[0-9]*) points")
# create list of players
$players = 1..([int]$Matches['players']) | ForEach-Object {
    [PSCustomObject]@{
        ID = $_
        Score = 0
    }
}
# set match end
$points = $Matches['points']

# set first marble
$marbles = New-Object System.Collections.Generic.LinkedList[PSCustomObject]
$marbleValue = 0
$currentMarble = $marbles.AddLast($marbleValue)

$lastMarble = 0
$highScorePart1 = 0
$gameRunning = $true
while ($gameRunning)
{
    for ($i = 0; $i -lt $players.Count; $i++)
    {
        $marbleValue++
        
        # if the marble value is a multiple of 23
        if (($marbleValue % 23) -eq 0)
        {
            for ($j = 0; $j -lt 6; $j++)
            {
                if ($currentMarble -eq $marbles.First)
                {
                    $currentMarble = $marbles.Last
                }
                else
                {
                    $currentMarble = $currentMarble.Previous
                }
            }

            if ($currentMarble -eq $marbles.First)
            {
                $scoreIncrease = $marbles.Last.Value + $marbleValue
                [void]$marbles.RemoveLast()
            }
            else
            {
                $scoreIncrease = $currentMarble.Previous.Value + $marbleValue

                $marbleToRemove = $currentMarble.Previous
                [void]$marbles.Remove($marbleToRemove)
            }

            $players[$i].Score += $scoreIncrease
        }
        else
        {
            if ($currentMarble -eq $marbles.Last)
            {
                $currentMarble = $marbles.AddAfter($marbles.First, $marbleValue)
            }
            else
            {
                $currentMarble = $marbles.AddAfter($currentMarble.Next, $marbleValue)
            }
        }
        
        if ($marbleValue -eq $points)
        {
            $lastMarble = $marbleValue
            $highScorePart1 = $players | Sort-Object Score -Descending | Select-Object -ExpandProperty Score -First 1
        }

        if ($marbleValue -eq ($lastMarble * 100))
        {
            $gameRunning = $false
            break
        }
    }
}

# part 1 - 367634
$highScorePart1

# part 2 - 3020072891
$players | Sort-Object Score -Descending | Select-Object -ExpandProperty Score -First 1