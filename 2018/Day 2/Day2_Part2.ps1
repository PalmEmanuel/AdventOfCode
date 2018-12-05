$rawInput = Get-Content $PSScriptRoot\input.txt | Sort-Object

# set a variable with length of a sample string in the input file
$stringLength = $rawInput[0].Length

# create hashtable to store match values between strings
$matchHash = @{}
# populate hashtable with "zero match values", i.e. the string length
$rawInput | ForEach-Object { $matchHash."$_" = $stringLength }

# loop through inputlist
for ($i = 0; $i -lt $rawInput.Count; $i++)
{
    # loop to end of inputlist from current iteration
    for ($j = $i+1; $j -lt $rawInput.Count; $j++)
    {
        $matchCount = 0
        # loop through characters of and compare strings
        for ($k = 0; $k -lt $stringLength; $k++)
        {
            # convert to char array and compare 
            if ($rawInput[$i].ToCharArray()[$k] -eq $rawInput[$j].ToCharArray()[$k])
            {
                $matchCount++
            }
        }

        # the amount of characters differing between the strings
        $matchValue = $stringLength - $matchCount

        # if the match is better than before, set the value of the strings individually        
        if ($matchHash."$($rawInput[$i])" -gt $matchValue)
        {
            $matchHash."$($rawInput[$i])" = $matchValue
        }
        if ($matchHash."$($rawInput[$j])" -gt $matchValue)
        {
            $matchHash."$($rawInput[$j])" = $matchValue
        }
    }
}

# string to populate with characters that match
$commonCharactersString = ""
# list with correct IDs extracted from hashTable
$comparisonList = $matchhash.GetEnumerator() | Where-Object Value -eq 1 | Select-Object -ExpandProperty Key
# loop through the strings per character
for ($i = 0; $i -lt $stringLength; $i++)
{
    if ($comparisonList[0].ToCharArray()[$i] -eq $comparisonList[1].ToCharArray()[$i])
    {
        # populate string with matching character
        $commonCharactersString += $comparisonList[0].ToCharArray()[$i]
    }
}

$commonCharactersString # correct answer is tzyvunogzariwkpcbdewmjhxi