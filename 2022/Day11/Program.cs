using System.Text.RegularExpressions;

// Parse monkeys
using StreamReader sr = new StreamReader(".\\..\\..\\..\\input.txt");
while (!sr.EndOfStream)
{
    string currentLine = sr.ReadLine();

    // If it's empty, skip it
    // Otherwise start finding properties of monkey
    if (!string.IsNullOrEmpty(currentLine))
    {
        // Monkey id
        int id = int.Parse(currentLine.Split(' ').Last().TrimEnd(':'));
        // Monkey items
        List<long> items = sr.ReadLine().Split(':').Last().Split(',').Select(s => long.Parse(s)).ToList();
        // Monkey operation
        string operation = sr.ReadLine().Split(':').Last().Trim();
        // Monkey test
        string test = sr.ReadLine().Split(':').Last().Trim();
        // Monkey if test is true
        int trueMonkey = int.Parse(sr.ReadLine().Split(':').Last().Trim().Split(' ').Last());
        // Monkey if test is false
        int falseMonkey = int.Parse(sr.ReadLine().Split(':').Last().Trim().Split(' ').Last());

        new Monkey(id, items, operation, test, trueMonkey, falseMonkey);
    }
}

// Get least common multiple to use with modulo
var lcm = Monkey.Monkeys.Select(m => m.DivisibleBy).Aggregate((a, b) => a * b);

// Cause of the static list in the Monkey class, only run one of them
//CalculateRounds(20, (i => i / 3));
CalculateRounds(10000, (i => i % lcm));

void CalculateRounds(int rounds, Func<long, long> reduce)
{
    for (int round = 0; round < rounds; round++)
    {
        for (int i = 0; i < Monkey.Monkeys.Count(); i++)
        {
            int monkeyItemCount = Monkey.Monkeys[i].Items.Count();
            for (int j = 0; j < monkeyItemCount; j++)
            {
                // Since the first item is thrown, we will always inspect the first one
                Monkey.Monkeys[i].InspectItem(0);

                Monkey.Monkeys[i].Items[0] = reduce(Monkey.Monkeys[i].Items[0]);

                Monkey.Monkeys[i].TestAndThrowItem(0);
            }
        }
    }

    var twoMostActiveMonkeys = Monkey.Monkeys.OrderByDescending(m => m.TimesInspected).Take(2);
    Console.WriteLine(twoMostActiveMonkeys.First().TimesInspected * twoMostActiveMonkeys.Last().TimesInspected);
}

void PrintMonkeys(int? round = null)
{
    if (round != null) { Console.WriteLine($"== After round {round} ==:"); }
    for (int i = 0; i < Monkey.Monkeys.Count(); i++)
    {
        Console.WriteLine($"Monkey {Monkey.Monkeys[i].Id} inspected items {Monkey.Monkeys[i].TimesInspected} times.");
    }
}

class Monkey
{
    public static List<Monkey> Monkeys = new List<Monkey>();

    public int Id { get; set; }
    public List<long> Items { get; set; }
    public int TrueMonkey { get; set; }
    public int FalseMonkey { get; set; }
    public long TimesInspected { get; private set; }

    public int DivisibleBy { get; private set; }
    public Func<long, long> Operation { get; set; }

    public Monkey(int id, List<long> items, string operation, string test, int trueMonkey, int falseMonkey)
    {
        Id = id;
        Items = items;
        TrueMonkey = trueMonkey;
        FalseMonkey = falseMonkey;
        TimesInspected = 0;

        DivisibleBy = ParseTest(test);
        Operation = ParseOperation(operation);
        Monkeys.Add(this);
    }

    public void InspectItem(int index)
    {
        var item = Operation(Items[index]);
        Items[index] = item;

        TimesInspected++;
    }

    public void TestAndThrowItem(int index)
    {
        int toMonkeyIndex = Items[index] % DivisibleBy == 0 ? TrueMonkey : FalseMonkey;

        Monkeys[toMonkeyIndex].Items.Add(Items[index]);
        Items.RemoveAt(index);
    }

    private int ParseTest(string testString)
    {
        var testParts = Regex.Match(testString, "divisible by (\\d+)$");
        return int.Parse(testParts.Groups[1].Value);
    }

    public Func<long, long> ParseOperation(string operationString)
    {
        var opParts = Regex.Match(operationString, "new = (\\w+) (.) (\\w+)$");
        var first = opParts.Groups[1].Value;
        var op = opParts.Groups[2].Value;
        var second = opParts.Groups[3].Value;

        return (first, op, second) switch
        {
            ("old", "*", "old") => old => old * old,
            ("old", "*", var x) when long.TryParse(x, out var num) => old => old * num,
            ("old", "+", var x) when long.TryParse(x, out var num) => old => old + num,
            _ => throw new FormatException()
        };
    }
}