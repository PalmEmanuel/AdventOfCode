using System.Text.RegularExpressions;

using StreamReader sr = new StreamReader(".\\..\\..\\..\\test.txt");

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
        List<int> items = sr.ReadLine().Split(':').Last().Split(',').Select(s => int.Parse(s)).ToList();
        // Monkey operation
        string operation = sr.ReadLine().Split(':').Last().Trim();
        // Monkey test
        string test = sr.ReadLine().Split(':').Last().Trim();
        // Monkey if test is true
        int trueMonkey = int.Parse(sr.ReadLine().Split(':').Last().Trim().Split(' ').Last());
        // Monkey if test is false
        int falseMonkey = int.Parse(sr.ReadLine().Split(':').Last().Trim().Split(' ').Last());

        Monkey.CreateMonkey(id, items, operation, test, trueMonkey, falseMonkey);
    }
}

for (int round = 0; round < 20; round++)
{
    for (int i = 0; i < Monkey.Monkeys.Count(); i++)
    {
        Monkey monkey = Monkey.Monkeys[i];

        for (int j = 0; j < monkey.Items.Count(); j++)
        {
            monkey.InspectItem(j);
            monkey.Items[j] /= 3;
            monkey.TestAndThrowItem(j);
        }
    }
}

var twoMostActiveMonkeys = Monkey.Monkeys.OrderByDescending(m => m.TimesInspected).Take(2);
Console.WriteLine(twoMostActiveMonkeys.First().TimesInspected * twoMostActiveMonkeys.Last().TimesInspected);

class Monkey
{
    public static List<Monkey> Monkeys = new List<Monkey>();

    public int Id { get; set; }
    public List<int> Items { get; set; }
    public string TestString { get; set; }
    public int TrueMonkey { get; set; }
    public int FalseMonkey { get; set; }
    public int TimesInspected { get; private set; }

    public string OperationString { get; set; }

    public static Monkey CreateMonkey(int id, List<int> items, string operation, string test, int trueMonkey, int falseMonkey)
    {
        var newMonkey = new Monkey
        {
            Id = id,
            Items = items,
            OperationString = operation,
            TestString = test,
            TrueMonkey = trueMonkey,
            FalseMonkey = falseMonkey,
            TimesInspected = 0
        };
        Monkeys.Add(newMonkey);
        return newMonkey;
    }

    public void InspectItem(int index)
    {
        Items[index] = UseOperation(Items[index]);
        TimesInspected++;
    }

    public void TestAndThrowItem(int index)
    {
        var testParts = Regex.Match(TestString, "divisible by (\\d+)$");
        var divisibleBy = int.Parse(testParts.Groups[1].Value);

        int toMonkeyIndex = Items[index] % divisibleBy == 0 ? TrueMonkey : FalseMonkey;

        Monkeys[toMonkeyIndex].Items.Add(Items[index]);
        Items.RemoveAt(index);
    }

    public int UseOperation(int old)
    {
        var opParts = Regex.Match(OperationString, "new = (\\w+) (.) (\\w+)$");

        var first = opParts.Groups[1].Value;
        int firstValue = first == "old" ? old : int.Parse(first);
        var op = opParts.Groups[2].Value;
        var second = opParts.Groups[3].Value;
        int secondValue = second == "old" ? old : int.Parse(second);

        switch (op)
        {
            case "*":
                return firstValue * secondValue;
            case "-":
                return firstValue - secondValue;
            case "+":
                return firstValue + secondValue;
            default:
                throw new Exception("Incorrect value!");
        }
    }
}