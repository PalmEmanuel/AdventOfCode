var input = File.ReadAllLines(".\\..\\..\\..\\input.txt");

List<Elf> elfList = new List<Elf>();
elfList.Add(new Elf());
foreach (var line in input)
{
    if (string.IsNullOrEmpty(line))
    {
        elfList.Add(new Elf());
    }
    else
    {
        elfList[elfList.Count() - 1].Calories += int.Parse(line);
    }
}

Console.WriteLine(elfList.OrderByDescending(elf => elf.Calories).First().Calories);

Console.WriteLine(elfList.OrderByDescending(elf => elf.Calories).Take(3).Sum(elf => elf.Calories));

class Elf
{
    public int Calories { get; set; } = 0;
}