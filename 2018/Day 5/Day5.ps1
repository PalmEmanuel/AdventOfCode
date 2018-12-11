$rawInput = Get-Content "$PSScriptRoot\input.txt"

function CancelUnitsFromPolymer
{
    param (
        [Parameter(Mandatory=$true)]
        [String]
        $PolymerSource,
        [Parameter(Mandatory=$false)]
        [Char]
        $UnitToRemove
    )
    if ($UnitToRemove)
    {
        $Polymer = ($PolymerSource -replace $UnitToRemove,'').ToCharArray()
    }
    else
    {
        $Polymer = $PolymerSource.ToCharArray()
    }
    
    # start from 1 to be able to check one index backwards
    for ($i = 1; $i -lt $Polymer.Count; $i++)
    {
        # check if the character is the same, but not the same case
        if (($Polymer[$i] -eq $Polymer[$i-1]) -and ($Polymer[$i] -cne $Polymer[$i-1]))
        {
            # create string to manipulate from the array
            $PolymerString = [string]::new($Polymer)

            # remove last and current char from string, then replace the array being looped through and jump in list because of removal
            $Polymer = $PolymerString.Remove($i-1,2).ToCharArray()

            # jump back twice if that won't make us out of range
            if ($i -lt 2)
            {
                $i = 0
            }
            else
            {
                $i -= 2
            }
        }
    }

    return $Polymer
}

$Polymer = CancelUnitsFromPolymer -PolymerSource $rawInput

# part 1
$Polymer.Count

# hashtable to save results of removing a unit type
$unitRemovals = @{}

# select all unique units in the Polymer
$units = $rawInput.ToCharArray() | Select-Object -Unique

foreach ($unit in $units)
{
    $Polymer = CancelUnitsFromPolymer -PolymerSource $rawInput -UnitToRemove $unit
    $unitRemovals[$unit] = $Polymer.Count
}

# part 2
$unitRemovals.GetEnumerator() | Sort-Object Value | Select-Object -First 1 -ExpandProperty Value