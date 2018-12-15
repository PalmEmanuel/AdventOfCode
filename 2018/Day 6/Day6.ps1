$rawInput = Get-Content "$PSScriptRoot\input.txt"

# make a list of the input coordinates with X, Y and the amount of squares that are closest to them
$points = $rawInput | ForEach-Object {
    $pointsplit = $_ -split ','
    [PSCustomObject]@{
        X = [int]$pointsplit[0]
        Y = [int]$pointsplit[1]
        Squares = 0
    }
}

$regionSize = 0

# set min and max values to loop through
$xMax = ($points.X | Sort-Object -Descending | Select-Object -First 1)
$yMax = ($points.Y | Sort-Object -Descending | Select-Object -First 1)
$xMin = ($points.X | Sort-Object | Select-Object -First 1)
$yMin = ($points.Y | Sort-Object | Select-Object -First 1)

# go through all possible points between the smallest and largest coordinate
for ($i = $xMin; $i -lt $xMax; $i++)
{
    for ($j = $yMin; $j -lt $yMax; $j++)
    {
        # set large value to begin with for comparison since we want smaller distances
        $closestDistance = [int]::MaxValue

        # sum of the distances to all points
        $distanceSum = 0
        # go through each point and find the closest one for the current position
        for ($k = 0; $k -lt $points.Count; $k++)
        {
            # find distance to the current point in the iteration
            $distance = [Math]::Abs($i - $points[$k].X) + [Math]::Abs($j - $points[$k].Y)

            $distanceSum += $distance

            # if the distance is less than the previously closest found distance
            if ($distance -lt $closestDistance)
            {
                # set the new closest distance to compare to, and the index of the closest found point
                $closestDistance = $distance
                $closestIndex = $k
            }
        }
        if ($distanceSum -lt 10000)
        {
            $regionSize++
        }
        $points[$closestIndex].Squares++
    }
}

# part 1
$points | Sort-Object Squares -Descending | Select-Object -First 1 -ExpandProperty Squares

# part 2
$regionSize