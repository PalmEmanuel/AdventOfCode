$rawInput = Get-Content "$PSScriptRoot\input.txt"

function Get-stepList
{   
    # list to hold all steps
    $stepList = @()
    for ($i = 0; $i -lt $rawInput.Count; $i++)
    {
        # get our steps
        [void]($rawInput[$i] -match "Step (?<req>[A-Z]) must be finished before step (?<step>[A-Z]) can begin.")

        # requirement of step to add to lists
        $req = $matches['req']
        # letter of step
        $step = $matches['step']

        # make sure to add all steps, even those without requirements
        if ($stepList.Step -notcontains $req)
        {
            $stepList += [PSCustomObject]@{
                Step = $req
                Requirements = New-Object System.Collections.ArrayList
                Completed = $false
            }
        }

        # if we already added the step
        if ($stepList.Step -contains $step)
        {
            $stepIndex = $stepList.Step.IndexOf($step)
            [void]$stepList[$stepIndex].Requirements.Add($req)
        }
        else
        {
            $stepList += [PSCustomObject]@{
                Step = $step
                Requirements = New-Object System.Collections.ArrayList
                Completed = $false
            }
            [void]$stepList[-1].Requirements.Add($req)
        }
    }
    return $stepList
}

$stepList = Get-stepList

# get first step by getting steps without requirements and taking the first one alphabetically
$currentStep = $stepList | Where-Object { $_.Requirements.Count -eq 0 } | Select-Object -ExpandProperty Step | Sort-Object | Select-Object -First 1

$order = ""
$completed = $false
while ($completed -eq $false)
{    
    # store step to orderlist only if it's a step that executes
    $order += $currentStep

    # set step to completed
    $stepIndex = $stepList.Step.IndexOf($currentStep)
    $stepList[$stepIndex].Completed = $true

    # remove requirement of step from other steps
    $stepList | Where-Object { $_.Requirements -contains $currentStep } | ForEach-Object { $_.Requirements.Remove($currentStep) }
    
    # get those without requirements left to complete, sort alphabetically and get first
    $currentStep = $stepList | Where-Object { $_.Requirements.Count -eq 0 -and $_.Completed -eq $false } | Select-Object -ExpandProperty Step | Sort-Object | Select-Object -First 1

    if ($null -eq $currentStep)
    {
        $completed = $true
    }
}

# part 1 - EUGJKYFQSCLTWXNIZMAPVORDBH
$order

$stepList = Get-stepList

$order = ""
$totalTime = 0
$completed = $false
$workers = 1..5 | ForEach-Object { [PSCustomObject]@{
        Task = ''
        Busy = $false
        Time = 0
    }
}

# loop until all steps are completed
while (($stepList.Completed | Where-Object { $_ -eq $false }).Count -gt 0)
{
    # get all steps without current requirements that are not yet completed
    $availableSteps = $stepList | Where-Object { $_.Requirements.Count -eq 0 -and $_.Completed -eq $false } | Sort-Object Step
    "AvailableSteps: " + ($availableSteps.Step -join ', ')

    for ($i = 0; $i -lt @($availableSteps).Count; $i++)
    {
        for ($j = 0; $j -lt 5; $j++)
        {
            if ($workers[$j].Busy)
            {
                continue
            }
            elseif ($workers.Task -notcontains $availableSteps[$i].Step)
            {
                $workers[$j].Busy = $true
                $workers[$j].Task = $availableSteps[$i].Step
                $workers[$j].Time = [int]([char]($workers[$j].Task))-4
                break
            }
            else
            {
                break
            }
        }
    }

    # while all workers are busy or there are no tasks available
    while ((($workers | Where-Object { $_.Busy -eq $false }).Count -eq 0) -or (($availableSteps | Where-Object { $workers.Task -notcontains $_.Step }).Count -eq 0))
    {
        $totalTime++
        for ($i = 0; $i -lt $workers.Count; $i++)
        {
            if ($workers[$i].Busy)
            {
                $workers[$i].Time--
                # if the worker is done, complete step
                if ($workers[$i].Time -eq 0)
                {
                    # remove requirement of step from other steps
                    $stepList | Where-Object { $_.Requirements -contains $workers[$i].Task } | ForEach-Object { $_.Requirements.Remove($workers[$i].Task) }

                    # set step to complete
                    $stepIndex = $stepList.Step.IndexOf($workers[$i].Task)
                    $stepList[$stepIndex].Completed = $true
                    
                    # add to order and set worker free
                    $order += $workers[$i].Task
                    $workers[$i].Task = ''
                    $workers[$i].Busy = $false
                }
            }
        }
    }
}

# part 2
$totalTime # - 1014 
$order