# takes a while to run
$rawInput = Get-Content "$PSScriptRoot\input.txt"

$frequency = 0

# initialize the frequency list
$frequencyList = @()

$index = 0
# run loop until found two matching frequencies
$matchingFrequency = $false
while ($matchingFrequency -eq $false)
{
    # casting strings to int works easily
    # "+25" to [int] is 25, casting "-10" to [int] is -10
    $frequency += [int]$rawInput[$index]
    
    if ($frequencyList.Contains($frequency))
    {
        $matchingFrequency = $true
    }

    # add to list
    $frequencyList += $frequency

    if ($matchingFrequency)
    {
        break
    }

    $index++

    # if we processed the last element, loop back to start of list
    if ($index -eq ($rawInput.Count))
    {
        $index = 0
    }
}

$frequencyList[-1] # 72330