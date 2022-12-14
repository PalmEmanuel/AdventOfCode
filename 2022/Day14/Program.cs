using System.Text;

class Program
{
    private static int maxX;
    private static int maxY;
    private static int minX;
    private static int minY;

    static void Main()
    {
        var input = File.ReadAllLines(".\\..\\..\\..\\input.txt");

        // Part 1
        RunSandSimulation(input, false);
        // Part 2
        RunSandSimulation(input, true);
    }

    public static void RunSandSimulation(IEnumerable<string> input, bool floor)
    {
        var map = ParseMap(input, floor);
        PrintMap(map);
        while (AddSand(ref map)) { }
        PrintMap(map);
        Console.WriteLine(map.SelectMany(l => l.Select(p => p.State)).Where(p => p == State.Sand).Count());
    }

    // Return true if sand managed to settle
    public static bool AddSand(ref List<List<Pos>> map)
    {
        Pos sandPos = new Pos(500, 0, State.Sand);
        while (true)
        {
            // If one move down is still in bounds
            if (sandPos.Y + 1 <= maxY)
            {
                // If the map position below is air, move sand down
                if (map[sandPos.Y - minY + 1][sandPos.X - minX].State == State.Air) { sandPos.Y++; continue; }
                // Else if the map position diagonal down left exists and path is clear, move sand down left
                else if (sandPos.X - 1 >= minX && map[sandPos.Y - minY + 1][sandPos.X - minX - 1].State == State.Air) { sandPos.Y++; sandPos.X--; continue; }
                // Else if the map position diagonal down right exists and path is clear, move sand down right
                else if (sandPos.X + 1 <= maxX && map[sandPos.Y - minY + 1][sandPos.X - minX + 1].State == State.Air) { sandPos.Y++; sandPos.X++; continue; }
                // Sand has come to rest
                else { break; }
            } // If sand has not come to rest, it's going to fall endlessly
            else { return false; }
        }
        // Set map position of sand to sand state
        map[sandPos.Y - minY][sandPos.X - minX].State = State.Sand;

        // Return true unless it's the spawn position, in which case the map is full
        return !(sandPos.X == 500 && sandPos.Y == 0);
    }

    // Parse map from input, with option of adding a cave floor
    public static List<List<Pos>> ParseMap(IEnumerable<string> input, bool floor = false)
    {
        var result = new List<List<Pos>>();
        var cornerLists = new List<List<Pos>>();
        maxX = 500;
        maxY = 0;
        minX = 500;
        minY = 0;
        // Parse each line into list of positions that are corners of the instructions
        foreach (var line in input)
        {
            var instructions = line.Split(" -> ");
            cornerLists.Add(new List<Pos>());
            foreach (var instruction in instructions)
            {
                var xy = instruction.Split(',').Select(s => int.Parse(s));
                int x = xy.First();
                int y = xy.Last();

                if (x > maxX) maxX = x;
                if (y > maxY) maxY = y;
                if (x < minX) minX = x;
                if (y < minY) minY = y;

                cornerLists.Last().Add(new Pos(x, y, State.Rock));
            }
        }
        // If endless floor, expand map
        if (floor)
        {
            minX -= maxY;
            maxX += maxY;
        }
        else // otherwise just add border space to track abyss sand
        {
            minX--;
            maxX++;
        }
        // Initialize map with empty space
        for (int y = minY; y <= maxY; y++)
        {
            result.Add(new List<Pos>());
            for (int x = minX; x <= maxX; x++)
            {
                result.Last().Add(new Pos(x, y, State.Air));
            }
        }

        // Loop through the instruction lists and create rock
        for (int i = 0; i < cornerLists.Count(); i++)
        {
            for (int j = 0; j < cornerLists[i].Count-1; j++)
            {
                // Add the rock between current and next rock corner from instructions
                result = AddInstruction(result, cornerLists[i][j], cornerLists[i][j+1]);
            }
        }

        // Add floor
        if (floor)
        {
            for (int i = 0; i < 2; i++)
            {
                result.Add(new List<Pos>());
                for (int x = minX; x < maxX; x++)
                {
                    // Add air for first layer, rock for second
                    result.Last().Add(new Pos(x, maxY + i + 1, (State)i));
                }
            }
            maxY += 2;
        }

        return result;
    }

    public static List<List<Pos>> AddInstruction(List<List<Pos>> map, Pos from, Pos to)
    {
        // Create direction object to apply diff
        Pos direction = new Pos(0,0,0);
        // Set diff
        int diff = 0;
        if (to.X > from.X) { direction.X++; diff = to.X - from.X; }
        if (to.X < from.X) { direction.X--; diff = from.X - to.X; }
        if (to.Y > from.Y) { direction.Y++; diff = to.Y - from.Y; }
        if (to.Y < from.Y) { direction.Y--; diff = from.Y - to.Y; }

        // Create reference position to track between from and to
        var currentPos = new Pos(from.X, from.Y, 0);
        // For each tile between from and to, set to rock (+1 to include both from and to tiles)
        for (int i = 0; i < diff + 1; i++)
        {
            // Set map position by reference to rock
            map[currentPos.Y - minY][currentPos.X - minX].State = State.Rock;
            // Update reference
            currentPos.X += direction.X;
            currentPos.Y += direction.Y;
        }

        return map;
    }

    public static void PrintMap(List<List<Pos>> map)
    {
        var sb = new StringBuilder();
        for (int y = minY; y < maxY+1; y++)
        {
            for (int x = minX; x < maxX; x++)
            {
                sb.Append(map[y - minY][x - minX]);
            }
            sb.AppendLine();
        }
        Console.WriteLine(sb);
    }
}

public class Pos
{
    public int X;
    public int Y;
    public State State { get; set; }

    public Pos(int x, int y, State state)
    {
        X = x;
        Y = y;
        State = state;
    }

    public override string ToString() => State switch
    {
        State.Air => ".",
        State.Rock => "#",
        State.Sand => "o",
        _ => throw new ArgumentException("Invalid state!")
    };
}

public enum State
{
    Air,
    Rock,
    Sand
}