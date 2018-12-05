$RawInput = Get-Content "$PSScriptRoot\input.txt"

$frequency = 0

foreach ($number in $RawInput)
{
    # casting strings to int works easily
    # "+25" to [int] is 25, casting "-10" to [int] is -10
    $frequency += [int]$number
}

$frequency # frequency is 505