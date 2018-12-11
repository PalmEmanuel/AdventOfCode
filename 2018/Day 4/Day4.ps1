$rawInput = Get-Content "$PSScriptRoot\input.txt"

# had a weird experience with this one where the results differ between using PS ISE and PowerShell Console or VS Code, the latter giving correct results
# my hunch is that it has to do with the sorting of the lists and minutes, but I haven't investigated further

# sort list by date
$sortedEntries = @()
foreach ($entry in $rawInput)
{
    # get only date
    [void]($entry -match "\[.*\]")
    $time = Get-Date ($matches[0] -replace '[][]','')
    # create list for sorting holding both the original value but also the time as a DateTime object
    $sortedEntries += [PSCustomObject]@{
        Time = $time
        Entry = $entry
    }
}
$sortedEntries = $sortedEntries | Sort-Object Time

# hashtable containing guard IDs and sleep minutes
$guards = @()
for ($i = 0; $i -lt $sortedEntries.Count; $i++)
{
    # guard begins shift
    if ($sortedEntries[$i].Entry -match "#[^ ]+")
    {
        # get ID of most recent guard
        $ID = $matches[0].Substring(1)
        # if new guard, add to list
        if ($guards.ID -notcontains $ID)
        {
            # initialize new guard 
            # sleep sum is total sleep minutes
            # minutecounter is a hashtable with the amount of times each minute was spent asleep  
            $guards += [PSCustomObject]@{
                ID = $ID
                SleepSum = 0
                MinuteCounter = @{}
            }
            # initialize all minutes in counter to 0 of most recently added guard
            1..60 | ForEach-Object { $guards[-1].MinuteCounter[$_] = 0 }
        }
    } # else if guard wakes up (meaning he slept last entry)
    elseif ($sortedEntries[$i].Entry -match "wakes up")
    {
        $guardIndex = $guards.ID.IndexOf($ID)
        $startMinute = $sortedEntries[$i-1].Time.Minute
        $endMinute = $sortedEntries[$i].Time.Minute
        $guards[$guardIndex].SleepSum += $endMinute - $startMinute

        # for each minute between start and end, add one minute to the guard's MinuteCounter hashtable
        for ($j = $startMinute; $j -le $endMinute; $j++)
        {
            $guards[$guardIndex].MinuteCounter[$j]++
            $guards[$guardIndex].SleepSum++
        }
    }
}

$sleepyGuard = $guards | Sort-Object SleepSum | Select-Object -Last 1
$mostSleptMinute = $sleepyGuard.MinuteCounter.GetEnumerator() | Sort-Object Value | Select-Object -Last 1 -ExpandProperty Key

# part 1
[int]$mostSleptMinute * [int]$sleepyGuard.ID

# create object to hold the ID of the guard, highest minute and the amount of times slept on that minute
$frequentSleeper = [PSCustomObject]@{
    ID = ""
    Minute = ""
    Count = ""
}
foreach ($guard in $guards)
{
    # get most frequently slept minute and check if it's higher than the already saved value
    $mostFrequent = $guard.MinuteCounter.GetEnumerator() | Sort-Object Value | Select-Object -Last 1
    $minute = $mostFrequent | Select-Object -ExpandProperty Key
    $count = $mostFrequent | Select-Object -ExpandProperty Value
    
    if ($count -gt $frequentSleeper.Count)
    {
        # save ID of guard and the minute they slept the most during
        $frequentSleeper.ID = $guard.ID
        $frequentSleeper.Minute = $minute
        $frequentSleeper.Count = $count
    }
}

# part 2
[int]$frequentSleeper.ID * [int]$frequentSleeper.Minute