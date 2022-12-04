var input = File.ReadAllLines(".\\..\\..\\..\\input.txt");

int overlappingPairs = 0;

// Examples
// 2-4,6-8
// 2-3,4-5
// 5-7,7-9
// 2-8,3-7
// 6-6,4-6
// 2-6,4-8
foreach (var line in input)
{
    string[] parts = line.Split(',');
    var firstElf = parts[0].Split('-');
    var secondElf = parts[1].Split('-');

    var firstRange = Enumerable.Range(int.Parse(firstElf[0]), (int.Parse(firstElf[1]) - int.Parse(firstElf[0]) + 1));
    var secondRange = Enumerable.Range(int.Parse(secondElf[0]), (int.Parse(secondElf[1]) - int.Parse(secondElf[0]) + 1));

    // If either list contains the other one fully
    if (!firstRange.Except(secondRange).Any() || !secondRange.Except(firstRange).Any())
    {
        overlappingPairs++;
    }
}

Console.WriteLine(overlappingPairs);

int anyOverlappingPairs = 0;
foreach (var line in input)
{
    string[] parts = line.Split(',');
    var firstElf = parts[0].Split('-');
    var secondElf = parts[1].Split('-');

    var firstRange = Enumerable.Range(int.Parse(firstElf[0]), (int.Parse(firstElf[1]) - int.Parse(firstElf[0]) + 1));
    var secondRange = Enumerable.Range(int.Parse(secondElf[0]), (int.Parse(secondElf[1]) - int.Parse(secondElf[0]) + 1));

    // If either list contains any of the other list
    if (firstRange.Except(secondRange).Count() != firstRange.Count() || secondRange.Except(firstRange).Count() != secondRange.Count())
    {
        anyOverlappingPairs++;
    }
}

Console.WriteLine(anyOverlappingPairs);