using System.Text;

class Program
{
    static void Main()
    {
        var map = ParseMap(".\\..\\..\\..\\input.txt");

        var start = map.SelectMany(l => l.Select(t => t)).First(t => t.HeightSymbol == "S");
        var goal = map.SelectMany(l => l.Select(t => t)).First(t => t.HeightSymbol == "E");
        start.Cost = 1;

        // Part 1
        // From S to E
        Console.WriteLine(GetShortestPathTileCount(map, start, goal));

        // Part 2 (takes a bit longer)
        // From each a where X == 0 to E, take shortest
        Console.WriteLine(GetShortestReversePathTileCount(map, goal));
    }

    private static int GetShortestPathTileCount(List<List<Tile>> map, Tile start, Tile goal)
    {
        map.ForEach(l => l.ForEach(t => t.SetDistance(goal)));

        var visited = new List<Tile>();
        var active = new List<Tile>();
        active.Add(start);

        while (active.Any())
        {
            var currentTile = active.OrderBy(t => t.CostDistance).First();

            // If we arrived at goal, it should be by lowest cost
            if (currentTile.Id == goal.Id)
            {
                if (currentTile is not null)
                {
                    var loopTile = currentTile;
                    var steps = 0;
                    while (loopTile.Previous is not null)
                    {
                        steps++;
                        loopTile = loopTile.Previous;
                    }
                    return steps;
                }
            }

            visited.Add(currentTile);
            active.Remove(currentTile);

            var possibleTiles = GetPossibleMoves(map, currentTile, goal);
            possibleTiles.ForEach(t =>
            {
                t.Cost = currentTile.Cost + 1;
                t.Previous = currentTile;
            });

            foreach (var possibleTile in possibleTiles)
            {
                // Already visited
                if (visited.Any(t => t.Id == possibleTile.Id))
                    continue;

                // Is already to-be-evaluated in active list
                if (active.Any(t => t.Id == possibleTile.Id))
                {
                    // If the other path is more expensive, replace it
                    var existingTile = active.First(t => t.Id == possibleTile.Id);
                    if (existingTile.CostDistance > possibleTile.CostDistance)
                    {
                        active.Remove(existingTile);
                        active.Add(possibleTile);
                    }
                }
                else // Is new
                {
                    active.Add(possibleTile);
                }
            }
            //PrintMap(map, currentTile);
        }

        return -1;
    }

    private static int GetShortestReversePathTileCount(List<List<Tile>> map, Tile goal)
    {
        map.ForEach(l => l.ForEach(t => t.SetDistance(goal)));

        var startTiles = map.SelectMany(l => l.Select(t => t)).Where(t => t.Height == 'a' && t.X == 0).OrderBy(t => t.Distance);
        var pathCounts = new List<int>();
        foreach (var tile in startTiles)
        {
            var count = GetShortestPathTileCount(map, tile, goal);
            if (count > 0)
            {
                pathCounts.Add(count);
            }
        }
        return pathCounts.OrderBy(t => t).First();
    }

    private static List<Tile> GetPossibleMoves(List<List<Tile>> map, Tile currentTile, Tile target)
    {
        var possibleTiles = new List<Tile>();
        var width = map.First().Count() - 1;
        var height = map.Count() - 1;
        
        // Add neighbouring tiles as copies, to retain values but make it possible to modify Previous and cost for path
        if (currentTile.X > 0) { possibleTiles.Add((Tile)map[currentTile.Y][currentTile.X - 1].Clone()); }
        if (currentTile.X < width) { possibleTiles.Add((Tile)map[currentTile.Y][currentTile.X + 1].Clone()); }
        if (currentTile.Y > 0) { possibleTiles.Add((Tile)map[currentTile.Y - 1][currentTile.X].Clone()); }
        if (currentTile.Y < height) { possibleTiles.Add((Tile)map[currentTile.Y + 1][currentTile.X].Clone()); }

        possibleTiles.ForEach(t => t.SetDistance(target));

	    return possibleTiles.Where(t => t.Height <= currentTile.Height + 1).ToList();
    }

    public static void InitializeMap(List<List<Tile>> map)
    {
    }

    public static List<List<Tile>> ParseMap(string path)
    {
        using StreamReader sr = new StreamReader(path);

        List<List<Tile>> map = new();
        while (!sr.EndOfStream)
        {
            map.Add(new List<Tile>());

            var currentLine = sr.ReadLine();
            for (int i = 0; i < currentLine!.Length; i++)
            {
                map.Last().Add(new Tile(i, map.Count() - 1, currentLine[i]));
            }
        }

        return map;
    }

    public static void PrintMap(List<List<Tile>> map, Tile? pathTile)
    {
        var sb = new StringBuilder();

        for (int y = 0; y < map.Count(); y++)
        {
            for (int x = 0; x < map[y].Count(); x++)
            {
                if (pathTile is not null && pathTile.Id == map[y][x].Id)
                {
                    sb.Append("*");
                }
                else
                {
                    sb.Append(map[y][x].HeightSymbol);
                }
            }
            sb.AppendLine();
        }
        Console.WriteLine(sb.ToString());
    }

    public class Tile : ICloneable
    {
        public int X { get; }
        public int Y { get; }
        public int Height { get; }
        public string HeightSymbol { get; set; }

        public int CostDistance => Cost + Distance;
        public int Cost { get; set; }
        public int Distance { get; private set; }
        public Tile? Previous { get; set; }

        public string Id => $"{Y},{X}";

        public Tile(int x, int y, char height)
        {
            X = x;
            Y = y;
            HeightSymbol = height.ToString();

            if (height == 'S')
            {
                height = 'a';
            }
            else if (height == 'E')
            {
                height = 'z';
            }

            Height = height;
        }

        public override string ToString()
        {
            return Id;
        }

        public void SetDistance(Tile goal)
        {
            Distance = Math.Abs(goal.X - X) + Math.Abs(goal.Y - Y);
        }

        public object Clone()
        {
            return new Tile(X, Y, (char)Height);
        }
    }
}