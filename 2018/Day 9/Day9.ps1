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

function Get-MarbleIndex
{
    param (
        [int]$CurrentMarble,
        [int]$MarbleCount,
        [int]$MarbleOffset
    )

    if (($MarbleCount - $MarbleOffset) -ge 0)
    {
        return ($CurrentMarble + 1) % $MarbleCount + $MarbleOffset
    }
    else
    {
        return $CurrentMarble % $MarbleCount + $MarbleOffset
    }
}

# set first marble
$marbles = New-Object System.Collections.ArrayList
$marbleValue = 0
[void]$marbles.Add($marbleValue)
$marbleIndex = 0

while ($marbleValue -lt $points)
{
    for ($i = 0; $i -lt $players.Count; $i++)
    {
        $marbleValue++
        
        # if the marble value is a multiple of 23
        if (($marbleValue % 23) -eq 0)
        {
            # get index of the one 7 steps counter-clockwise
            "index $marbleIndex from list with $($marbles.Count) marbles, offset is 1 clockwise"

            $marbleIndex = Get-MarbleIndex -MarbleCount $marbles.Count -MarbleOffset -7 -CurrentMarble $marbleIndex

            $players[$i].Score += ($marbles[$marbleIndex] * $marbleValue)

            $marbles.RemoveAt($marbleIndex)
        }
        else
        {
            # get index of the one 1 step clockwise
            "index $marbleIndex from list with $($marbles.Count) marbles, offset is 1 clockwise"

            $marbleIndex = Get-MarbleIndex -MarbleCount $marbles.Count -MarbleOffset 1 -CurrentMarble $marbleIndex

            # if the index is the same as count, it means it will end up at the last position of the list
            if ($marbleIndex -eq $marbles.Count)
            {
                [void]$marbles.Add($marbleValue)
            }
            else
            {
                [void]$marbles.Insert($marbleIndex, $marbleValue)
            }
        }
    }
}

