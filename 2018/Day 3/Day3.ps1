$rawInput = Get-Content $PSScriptRoot\input.txt

$claims = @()
#
foreach ($claim in $rawInput)
{
    # Format string into temp object for list storage
    # ID skips '#' and takes until @
    [int]$ID = $claim.substring(1,$claim.indexOf('@')-1).Trim()
    # Left margin takes from '@' until ','
    [int]$LeftMargin = $claim.substring($claim.indexOf('@')+1, $claim.IndexOf(',')-$claim.IndexOf('@')-1).Trim()
    # Right margin takes from ',' until ':'
    [int]$TopMargin = $claim.substring($claim.IndexOf(',')+1, $claim.IndexOf(':')-$claim.IndexOf(',')-1).Trim()
    # Width takes from ':' until 'x'
    [int]$Width = $claim.substring($claim.LastIndexOf(' '), $claim.IndexOf('x')-$claim.LastIndexOf(' ')).Trim()
    # Height takes from 'x' until end
    [int]$Height = $claim.substring($claim.IndexOf('x')+1).Trim()

    $temp = New-Object PSObject -Property @{
        "ID" = $ID
        "LeftMargin" = $LeftMargin
        "TopMargin" = $TopMargin
        "Width" = $Width
        "Height" = $Height
    }

    $claims += $temp
}

# Create 1000x1000 2D array and populate with dots
$fabric = New-Object 'string[][]' 1000,1000
for ($i = 0; $i -lt $fabric.Count; $i++)
{
    for ($j = 0; $j -lt $fabric[$i].Count; $j++)
    {
        $fabric[$i][$j] = '.'
    }
}

# go through each formatted claim
foreach ($claim in $claims)
{
    for ($i = $claim.LeftMargin; $i -lt $claim.LeftMargin+$claim.Width; $i++)
    {
        for ($j = $claim.TopMargin; $j -lt $claim.TopMargin+$claim.Height; $j++)
        {
            # if the current element is not a dot, it's a claimed square inch
            if ($fabric[$i][$j] -ne '.')
            {
                $fabric[$i][$j] = 'X'
            }
            else # otherwise we should set the ID to claim it
            {
                $fabric[$i][$j] = $claim.ID
            }
        }
    }
}

$claimed = 0
# go through the whole 2D array to count claimed square inches
for ($i = 0; $i -lt $fabric.Count; $i++)
{
    for ($j = 0; $j -lt $fabric[$i].Count; $j++)
    {
        # if the current element is X, it is added to the claimed total square inches
        if ($fabric[$i][$j] -eq 'X')
        {
            $claimed++
        }
    }
}

$claimed # Part one 105231

<#$break = $false
for ($i = 0; $i -lt $fabric.Count; $i++)
{
    if ($break)
    {
        break
    }
    for ($j = 0; $j -lt $fabric[$i].Count; $j++)
    {
        # if the current element is not a dot and not an X, we found the only ID left - store it and break
        if ($fabric[$i][$j] -ne '.' -and $fabric[$i][$j] -ne 'X')
        {
            $unclaimedID = $fabric[$i][$j]
            $break = $true
            break
        }
    }
}#>

$unclaimedID = ""
# go through each claim and see if if none of the claimed inches are crossovers
foreach ($claim in $claims)
{
    # assume it's unclaimed until proven otherwise
    $break = $false
    $isClaimed = $false
    for ($i = $claim.LeftMargin; $i -lt $claim.LeftMargin+$claim.Width; $i++)
    {
        if ($break)
        {
            break
        }
        for ($j = $claim.TopMargin; $j -lt $claim.TopMargin+$claim.Height; $j++)
        {
            # if it doesn't match the current claim, it's claimed by a different one as well or empty
            if ($fabric[$i][$j] -ne $claim.ID)
            {
                $isClaimed = $true
                $break = $true
                break
            }
        }
    }

    # if we haven't been able to find a crossover claim for the current iteration we found the correct answer
    if ($isClaimed -eq $false)
    {
        $unclaimedID = $claim.ID
        break
    }
}

$unclaimedID # Part two 164