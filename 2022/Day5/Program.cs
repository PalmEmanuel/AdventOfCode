using System.Text.RegularExpressions;

var input = File.ReadAllLines(".\\..\\..\\..\\input.txt");

// Get diagram until empty line
var diagramString = input.TakeWhile(line => !string.IsNullOrWhiteSpace(line)).ToList();
// Get pileCount from last integer in line of diagram
var pileCount = int.Parse(diagramString.Last().Replace(" ", "").Last().ToString());
// Get instructions after diagram
var instructions = input.Skip(diagramString.Count()+1).ToList();

// Get list of list of strings as diagram
diagramString.Reverse();
var diagram = ParseDiagram(diagramString.Skip(1));

foreach (var line in instructions)
{
	var numbers = Regex.Matches(line, "\\d{1,}");

	diagram = ExecuteMove(diagram, int.Parse(numbers[0].Value), int.Parse(numbers[1].Value), int.Parse(numbers[2].Value), false);
}

//PrintDiagram(diagram);
// Solution part 1
Console.WriteLine(string.Join("", diagram.Select(l => l.Last(s => !string.IsNullOrWhiteSpace(s)))));

var diagramMultiple = ParseDiagram(diagramString.Skip(1));

foreach (var line in instructions)
{
    var numbers = Regex.Matches(line, "\\d{1,}");

    diagramMultiple = ExecuteMove(diagramMultiple, int.Parse(numbers[0].Value), int.Parse(numbers[1].Value), int.Parse(numbers[2].Value), true);
}

//PrintDiagram(diagramMultiple);
// Solution part 2
Console.WriteLine(string.Join("", diagramMultiple.Select(l => l.Last(s => !string.IsNullOrWhiteSpace(s)))));

static List<List<string>> ParseDiagram(IEnumerable<string> diagram)
{
	var result = new List<List<string>>();
    for (int i = 0; i < diagram.First().Length; i+=4)
    {
        result.Add(new List<string>());
        for (int j = 0; j < diagram.Count(); j++)
        {
            result.Last().Add(diagram.ElementAt(j).Substring(i + 1, 1));
        }
    }

	return result;
}

static List<List<string>> ExecuteMove(List<List<string>> diagram, int amount, int from, int to, bool multipleMove)
{
	if (!multipleMove)
    {
        for (int i = 0; i < amount; i++)
        {
            // Add a row to list if there's no space to move crates to
            if (!string.IsNullOrEmpty(diagram.Last()[to - 1]))
            {
                foreach (var list in diagram)
                {
                    list.Add(" ");
                }
            }

            var fromIndex = diagram[from - 1].FindLastIndex(s => !string.IsNullOrWhiteSpace(s));
            var toIndex = diagram[to - 1].FindIndex(s => string.IsNullOrWhiteSpace(s));
            diagram[to - 1][toIndex] = diagram[from - 1][fromIndex];
            diagram[from - 1][fromIndex] = " ";
        }
    }
	else if (multipleMove)
    {
        // Add a row to list if there's no space to move crates to
        for (int x = 0; x < amount; x++)
        {
            // Add a row for each row that isn't empty from the end
            if (!string.IsNullOrWhiteSpace(diagram[to - 1][diagram[to-1].Count() - 1 - x]))
            {
                foreach (var list in diagram)
                {
                    list.Add(" ");
                }
            }
        }

        var fromStartIndex = diagram[from - 1].FindLastIndex(s => !string.IsNullOrWhiteSpace(s)) - amount + 1;
        var toStartIndex = diagram[to - 1].FindIndex(s => string.IsNullOrWhiteSpace(s));

        var crates = diagram[from - 1].GetRange(fromStartIndex, amount);
        for (int i = 0; i < amount; i++)
        {
            diagram[to - 1][i + toStartIndex] = crates[i];
            diagram[from - 1][i + fromStartIndex] = " ";
        }
    }

	return diagram;
}

static void PrintDiagram(List<List<string>> diagram)
{
	for (int i = 0; i < diagram.First().Count(); i++)
	{
		for (int j = 0; j < diagram.Count(); j++)
		{
			Console.Write($"[{diagram[j][i] ?? " "}]");
			Console.Write(" ");
        }
		Console.WriteLine("");
	}
}