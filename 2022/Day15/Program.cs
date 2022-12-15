using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

class Program
{
    static async Task Main()
    {
        var sensors = await ParseInput(".\\..\\..\\..\\input.txt");

        // Only use with part 1
        //PrintMap(sensors);

        // Part 1
        Console.WriteLine(FindImpossibleBeaconCount(sensors, 2000000));

        // Part 2
        Console.WriteLine(FindTuningFrequency(sensors, 4000000));
    }

    static async Task<List<Sensor>> ParseInput(string path)
    {
        var input = await File.ReadAllLinesAsync(path);
        var sensorBeacons = new List<Sensor>();
        foreach (var line in input)
        {
            var matches = Regex.Match(line, "^Sensor at x=(.+), y=(.+): closest beacon is at x=(.+), y=(.+)$").Groups.Values.Skip(1).Select(i => int.Parse(i.Value)).ToList();
            sensorBeacons.Add(new Sensor(matches[0], matches[1], matches[2], matches[3]));
        }

        return sensorBeacons;
    }

    static int FindImpossibleBeaconCount(List<Sensor> map, int row)
    {
        int result = 0;
        int minX = map.Min(p => Math.Min(p.X, p.BeaconX) - p.BeaconDistance);
        int maxX = map.Max(p => Math.Max(p.X, p.BeaconX) + p.BeaconDistance);

        for (int x = minX; x <= maxX; x++)
        {
            // Skip if beacon or position is not further away from beacon than sensor
            if (map.Any(s => s.BeaconX == x && s.BeaconY == row))
                continue;

            if (!CanBeBeaconSpot(map, x, row))
                result++;
        }
        return result;
    }

    static bool CanBeBeaconSpot(List<Sensor> map, int x, int y)
    {
        // The distance to the closest sensor is longer than the distance between sensor and beacon
        return !map.Any(s => s.DistanceTo(x, y) <= s.BeaconDistance);
    }

    static long FindTuningFrequency(List<Sensor> map, int max)
    {
        int minX = Math.Max(map.Min(p => Math.Min(p.X, p.BeaconX)),0);
        int minY = Math.Max(map.Min(p => Math.Min(p.Y, p.BeaconY)),0);
        int maxX = Math.Min(map.Max(p => Math.Max(p.X, p.BeaconX)), max);
        int maxY = Math.Min(map.Max(p => Math.Max(p.Y, p.BeaconY)), max);

        // For storing unique positions
        HashSet<Tuple<int, int>> sensorBorderPositions = new HashSet<Tuple<int, int>>();
        foreach (var sensor in map)
        {
            // For each sensor, start to the right of the beacon border
            var distance = sensor.BeaconDistance;
            var x = sensor.X+distance+1;
            var y = sensor.Y;
            // From the right diamond corner down left >
            for (int i = 0; i < distance; i++)
            {
                sensorBorderPositions.Add(new Tuple<int, int>(x, y));
                x--;
                y++;
            }
            // From the bottom diamond corner up left V
            for (int i = 0; i < distance; i++)
            {
                sensorBorderPositions.Add(new Tuple<int, int>(x, y));
                x--;
                y--;
            }
            // From the left diamond corner up right <
            for (int i = 0; i < distance; i++)
            {
                sensorBorderPositions.Add(new Tuple<int, int>(x, y));
                x++;
                y--;
            }
            // From the top diamond corner down right <
            for (int i = 0; i < distance; i++)
            {
                sensorBorderPositions.Add(new Tuple<int, int>(x, y));
                x++;
                y--;
            }
        }
        // Filter out positions where it's impossible for a beacon to be
        sensorBorderPositions = sensorBorderPositions.Where(pos =>
            pos.Item1 <= maxX && pos.Item1 >= minX &&
            pos.Item2 <= maxY && pos.Item2 >= minY &&
            CanBeBeaconSpot(map, pos.Item1, pos.Item2)
        ).ToHashSet();

        foreach (var pos in sensorBorderPositions)
        {
            // We're not on a beacon or sensor
            if (!map.Any(s => (s.BeaconX == pos.Item1 && s.BeaconY == pos.Item2) || (s.X == pos.Item1 && s.Y == pos.Item2)))
            {
                // If any position around the spot can also be a beacon spot, we're not on the right location
                if (CanBeBeaconSpot(map, pos.Item1, pos.Item2 - 1) ||
                    CanBeBeaconSpot(map, pos.Item1 - 1, pos.Item2) ||
                    CanBeBeaconSpot(map, pos.Item1 - 1, pos.Item2 - 1) ||
                    CanBeBeaconSpot(map, pos.Item1 - 1, pos.Item2 + 1) ||
                    CanBeBeaconSpot(map, pos.Item1 + 1, pos.Item2 - 1) ||
                    CanBeBeaconSpot(map, pos.Item1 + 1, pos.Item2 + 1) ||
                    CanBeBeaconSpot(map, pos.Item1 + 1, pos.Item2) ||
                    CanBeBeaconSpot(map, pos.Item1, pos.Item2 + 1))
                    continue;

                return ((long)pos.Item1 * 4000000) + pos.Item2;
            }
        }

        return 0;
    }

    static void PrintMap(List<Sensor> map, int? xSpot = null, int? ySpot = null)
    {
        // Split beacon pairs and figure out bounds of map
        int minX = map.Min(p => Math.Min(p.X, p.BeaconX));
        int minY = map.Min(p => Math.Min(p.Y, p.BeaconY));
        int maxX = map.Max(p => Math.Max(p.X, p.BeaconX));
        int maxY = map.Max(p => Math.Max(p.Y, p.BeaconY));

        var sb = new StringBuilder();
        for (int i = 0; i < maxX.ToString().Length; i++)
        {
            sb.Append(string.Join("", Enumerable.Range(0, $"{maxY:00} ".Length).Select(s => " ")));
            for (int x = minX; x <= maxX; x++)
            {
                sb.Append(x % 5 == 0 ? $"{x:00}"[i] : " ");
            }
            sb.AppendLine();
        }
        for (int y = minY-2; y <= maxY; y++)
        {
            sb.Append($"{y:00;-0} ");
            for (int x = minX; x <= maxX; x++)
            {
                // See if there's a sensor or beacon at the position
                var pos = map.FirstOrDefault(p => (p.X == x && p.Y == y) || (p.BeaconX == x && p.BeaconY == y));
                string symbol = ".";
                if (xSpot.HasValue && ySpot.HasValue && xSpot == x && ySpot == y)
                {
                    symbol = "X";
                }
                else if (pos is not null)
                {
                    // In that case, print S or B
                    symbol = pos.X == x && pos.Y == y ? "S" : "B";
                } // If position is not further away from beacon than sensor, print #
                else if (map.Any(s => s.DistanceTo(x, y) <= s.BeaconDistance))
                {
                    symbol = "#";
                }
                sb.Append(symbol);
            }
            sb.AppendLine();
        }
        Console.WriteLine(sb);
    }
}

public class Sensor
{
    public int X { get; }
    public int Y { get; }
    public int BeaconX { get; }
    public int BeaconY { get; }

    public int BeaconDistance { get; }

    public Sensor(int x, int y, int beaconx, int beacony)
    {
        X = x;
        Y = y;
        BeaconX = beaconx;
        BeaconY = beacony;

        BeaconDistance = DistanceTo(beaconx, beacony);
    }

    public int DistanceTo(int x, int y) => Math.Abs(X - x) + Math.Abs(Y - y);
}