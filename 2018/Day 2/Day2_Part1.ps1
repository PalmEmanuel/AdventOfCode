$rawInput = Get-Content $PSScriptRoot\input.txt

$duplicates = 0
$triples = 0

foreach ($id in $rawInput)
{
    # grouping the letters in an array of characters gets a count of each
    $idGrouped = $id.ToCharArray() | Group-Object
    
    # filter lists on triplets and duplicates and see if the filtered lists contain any members
    if (($idGrouped | Where-Object Count -eq 3).Count -gt 0)
    {
        $triples++
    }
    if (($idGrouped | Where-Object Count -eq 2).Count -gt 0)
    {
        $duplicates++
    }
}

$duplicates * $triples # 5750