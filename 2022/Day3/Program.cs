var input = File.ReadAllLines(".\\..\\..\\..\\input.txt");

int commonPriorityCount = 0;
foreach (var line in input)
{
    // Find the common character between first and second half of each string line
    var first = line.Substring(0, line.Length/2).ToCharArray();
    var second = line.Substring(line.Length / 2).ToCharArray();

    var commonCharacter = first.First(i => second.Contains(i));
    var priority = GetItemPriority(commonCharacter);
    commonPriorityCount += priority;
}
Console.WriteLine(commonPriorityCount);

var badgePriority = 0;
for (int i = 0; i < input.Length; i+=3)
{
    // Find the common character between each group of three lines
    var first = input[i].ToCharArray();
    var second = input[i+1].ToCharArray();
    var third = input[i+2].ToCharArray();

    var commonCharacter = first.First(i => second.Contains(i) && third.Contains(i));

    var priority = GetItemPriority(commonCharacter);
    badgePriority += priority;
}
Console.WriteLine(badgePriority);

static int GetItemPriority(char item)
{
    // Lowercase
    if (item >= 97)
    {
        return (int)item - 96;
    }
    else if (item <= 90) // Uppercase
    {
        return item - 38;
    }

    return -1;
}