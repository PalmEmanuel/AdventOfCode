# Add drawing namespace for point struct
Add-Type -AssemblyName System.Drawing

$PuzzleInput = Get-Content '.\2019\Day 3\input.txt'

# Create a list of dictionaries for each wire to store positions
$Script:Wires = New-Object System.Collections.Generic.List["System.Collections.Generic.Dictionary[System.Drawing.Point,int]"]
# Add dictionaries of <point,int> to list
$PuzzleInput | ForEach-Object { $Script:Wires.Add((New-Object "System.Collections.Generic.Dictionary[System.Drawing.Point,int]")) }

# For each wire
for ($i = 0; $i -lt $PuzzleInput.Count; $i++) {
    $Instructions = $PuzzleInput[$i] -split ','

    $x = 0
    $y = 0
    $Steps = 0

    # For each instruction
    foreach ($Instruction in $Instructions) {
        $Direction = $Instruction[0]
        [int]$Distance = $Instruction.Substring(1)

        # Move distance step by step
        for ($j = 0; $j -lt $Distance; $j++) {
            # Change current position based on direction
            switch ($Direction) {
                U { $y++ }
                D { $y-- }
                L { $x-- }
                R { $x++ }
            }

            # Store current step, throws if already exists
            try {
                $Script:Wires[$i].Add([System.Drawing.Point]::new($x, $y), ++$Steps)
            }
            catch { }
        }
    }
}

$Intersections = [System.Linq.Enumerable]::Intersect($Script:Wires[0].Keys,$Script:Wires[1].Keys)

$Distances = New-Object "System.Collections.Generic.List[int]"
foreach ($Intersection in $Intersections) {
    $Distances.Add($Script:Wires[0][$Intersection] + $Script:Wires[1][$Intersection])
}

"Part one: $(($Intersections | ForEach-Object { [math]::Abs($_.x) + [math]::Abs($_.y) } | Sort-Object)[0])"
"Part two: $(($Distances | Sort-Object)[0])"