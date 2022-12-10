using StreamReader sr = new StreamReader(".\\..\\..\\..\\input.txt");
List<Instruction> instructions = new List<Instruction>();
while (!sr.EndOfStream)
{
    instructions.Add(new Instruction(sr.ReadLine()));
}

Dictionary<int, int> signalStrengths = new Dictionary<int, int>
{
    {20, 0},
    {60, 0},
    {100, 0},
    {140, 0},
    {180, 0},
    {220, 0}
};

var x = 1;
int currentInstructionIndex = 0;
var cycleCountDown = instructions[currentInstructionIndex].CycleCost;
var cycle = 0;
while (cycleCountDown > 0)
{
    cycleCountDown--;
    cycle++;
    
    if (signalStrengths.ContainsKey(cycle))
    {
        signalStrengths[cycle] = GetSignalStrength(cycle, x);
    }

    // If we spent cycles for instruction, process it
    if (cycleCountDown == 0)
    {
        x += instructions[currentInstructionIndex].Value;

        // Then go to the next instruction if any
        currentInstructionIndex++;
        if (currentInstructionIndex < instructions.Count())
        {
            cycleCountDown = instructions[currentInstructionIndex].CycleCost;
        }
    }
}

// Part 1
Console.WriteLine(signalStrengths.Values.Sum());

int GetSignalStrength(int cycle, int x)
{
    return x * cycle;
}

class Instruction
{
    private static int id = 0;
    private static int Id
    {
        get
        {
            return id++;
        }
    }

    private Dictionary<string, int> cycleValues = new Dictionary<string, int>
    {
        { "noop", 1 },
        { "addx", 2 },
    };

    public int InstructionId { get; set; }
    public string Text { get; set; }
    public int Value { get; set; }
    public int CycleCost { get; set; }

    public Instruction(string text)
    {
        InstructionId = Id;
        Text = text;
        var instructionParts = text.Split(" ");
        var instructionText = instructionParts.First();
        CycleCost = cycleValues[instructionText];

        Value = instructionParts.Count() > 1 ? int.Parse(instructionParts[1]) : 0;
    }
}