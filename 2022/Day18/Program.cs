class Program
{
    static void Main()
    {
        // List of cubes, in the form of int lists
        var inputIntLists = File.ReadAllLines(".\\..\\..\\..\\input.txt").Select(l => l.Split(',').Select(s => int.Parse(s)).ToList()).ToList();
        int totalSurfaceArea = 0;
        foreach (var cubeList in inputIntLists)
        {
            int currentSurfaceArea = 6;

            totalSurfaceArea += currentSurfaceArea - AdjacentCubes(inputIntLists, cubeList);
        }
        // Part 1
        Console.WriteLine(totalSurfaceArea);
    }

    static int AdjacentCubes(List<List<int>> cubes, List<int> cubeList)
    {
        int adjacentCubes = 0;

        List<List<int>> touchingCubes = new List<List<int>>();
        touchingCubes.Add(new List<int> { cubeList[0] - 1, cubeList[1], cubeList[2] });
        touchingCubes.Add(new List<int> { cubeList[0] + 1, cubeList[1], cubeList[2] });
        touchingCubes.Add(new List<int> { cubeList[0], cubeList[1] - 1, cubeList[2] });
        touchingCubes.Add(new List<int> { cubeList[0], cubeList[1] + 1, cubeList[2] });
        touchingCubes.Add(new List<int> { cubeList[0], cubeList[1], cubeList[2] - 1 });
        touchingCubes.Add(new List<int> { cubeList[0], cubeList[1], cubeList[2] + 1 });

        foreach (var touchingCube in touchingCubes)
        {
            if (cubes.Any(l => l[0] == touchingCube[0] && l[1] == touchingCube[1] && l[2] == touchingCube[2]))
            {
                adjacentCubes++;
            }
        }

        return adjacentCubes;
    }
}