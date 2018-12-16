$rawInput = Get-Content "$PSScriptRoot\input.txt"

# get a list of all numbers only
$numbers = $rawInput -split " "

# total of metadataentries
$script:metadataTotal = 0

# keep track of where in the list
$script:numberIndex = 0

# recursive function to get children
function Get-ChildNodesRecursively
{
    ######### HEADER ##########
    # amount of children
    $childrenNumber = $numbers[$script:numberIndex]
    $script:numberIndex++
    
    # amount of dataentries
    $metadataEntries = $numbers[$script:numberIndex]
    $script:numberIndex++

    ######### Children ##########
    $children = @()
    for ($i = 0; $i -lt $childrenNumber; $i++)
    {
        $children += Get-ChildNodesRecursively $script:numberIndex
    }

    ######### Metadata ##########
    $metadata = @()
    for ($i = 0; $i -lt $metadataEntries; $i++)
    {
        $currentMetadata = $numbers[$script:numberIndex]
        $metadata += $currentMetadata
        $script:metadataTotal += $currentMetadata
        $script:numberIndex++
    }

    ######### Complete node  ##########
    $temp = [PSCustomObject]@{
        NumberOfChildren = $childrenNumber
        MetadataEntries = $metadataEntries
        Children = $children
        Metadata = $metadata
    }

    return $temp
}

$nodes = @()
$nodes += Get-ChildNodesRecursively $script:numberIndex

# part 1
$script:metadataTotal # 46096

function Get-NodeValueRecursively {
    param (
        $node
    )

    $nodeValue = 0

    for ($i = 0; $i -lt $node.Metadata.Count; $i++)
    {
        $childReference = $node.Metadata[$i]-1

        if ($node.Children[$childReference].NumberOfChildren -eq 0)
        {
            $nodeValue += $node.Children[$childReference].MetaData | Measure-Object -Sum | Select-Object -ExpandProperty Sum
        }
        elseif ($childReference -lt $node.Children.Count -and $childReference -ge 0)
        {
            $nodeValue += Get-NodeValueRecursively $node.Children[$childReference]
        }
    }
    
    return $nodeValue
}

$rootValue = Get-NodeValueRecursively $nodes[0]

# part 2
$rootValue # 24820