function Get-MarbleIndex
{
    param (
        [int]$CurrentIndex,
        [int]$MarbleCount,
        [int]$MarbleOffset
    )
    return (($CurrentIndex + $MarbleOffset + 1) % $MarbleCount + $MarbleCount) % $MarbleCount
    <#if (($CurrentIndex + $MarbleOffset) -gt $MarbleCount)
    {
        return (($CurrentIndex + $MarbleOffset) - $MarbleCount)
    }
    else
    {
        $reduction = (($CurrentIndex + $MarbleOffset) - $MarbleCount)
        return $MarbleCount - $reduction
    }#>

    if (($CurrentIndex + $MarbleOffset) -ge 0)
    {
        return (($CurrentIndex + $MarbleOffset + 1) % $MarbleCount + $MarbleCount) % $MarbleCount
    }
    else
    {
        $tempIndex = $CurrentIndex
        while ($true)
        {
            if ($tempIndex -lt 0)
            {

            }
        }
        return [Math]::Abs(($CurrentIndex + $MarbleOffset) % $MarbleCount)
    }
}

#$rawInput = Get-Content "$PSScriptRoot\input.txt"

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
$marbles = New-Object System.Collections.ArrayList
$marbleValue = 0
[void]$marbles.Add($marbleValue)
$marbleIndex = 0

$gameRunning = $true
while ($gameRunning)
{
    for ($i = 0; $i -lt $players.Count; $i++)
    {
        $marbleValue++
        
        # if the marble value is a multiple of 23
        if (($marbleValue % 23) -eq 0)
        {
            # get index of the one 7 steps counter-clockwise, needs -1 to get correct index
            #"index $marbleIndex from list with $($marbles.Count) marbles, offset is 1 clockwise"
            $marbleIndex = Get-MarbleIndex -MarbleCount $marbles.Count -MarbleOffset (-7-1) -CurrentIndex $marbleIndex

            $scoreIncrease = ($marbles[$marbleIndex] + $marbleValue)
            $players[$i].Score += $scoreIncrease

            $marbles.RemoveAt($marbleIndex)
        }
        else
        {
            # get index of the one 1 step clockwise
            $marbleIndex = Get-MarbleIndex -MarbleCount $marbles.Count -MarbleOffset 1 -CurrentIndex $marbleIndex

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
        
        if ($marbleValue -eq $points)
        {
            $gameRunning = $false
            break
        }
    }
}

# part 1
$players | Sort-Object Score -Descending | Select-Object -ExpandProperty Score -First 1