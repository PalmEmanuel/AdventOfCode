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