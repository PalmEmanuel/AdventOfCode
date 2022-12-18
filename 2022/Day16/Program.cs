using System.Text.RegularExpressions;

class Program
{
    static async Task Main()
    {
        var valves = await ParseValves(".\\..\\..\\..\\test.txt");
        // Part 1
        Console.WriteLine(OpenValves(valves));
    }

    static int OpenValves(List<Valve> valves)
    {
        var openedValves = new List<Valve>();
        var valvesBeingEvaluated = valves.Where(v => v.FlowRate > 0).ToList();

        // Create list of valves with flow rate to explore, ordered by distance from start
        int totalPressure = 0;
        var currentValve = valves.First();
        for (int minutesLeft = 30; minutesLeft > 0; minutesLeft--)
        {
            currentValve = valvesBeingEvaluated.OrderByDescending(v => v.GetRemainingPressure(minutesLeft - v.DistanceTo[currentValve.Id])).FirstOrDefault();

            if (currentValve is not null)
            {
                valvesBeingEvaluated.Remove(currentValve);

                // Get highest pressure valve possible to reach within the time left, which we haven't opened yet
                var nextValve = valvesBeingEvaluated
                    .Where(v => v.DistanceTo[currentValve.Id] < minutesLeft && !openedValves.Any(o => o.Id == v.Id))
                    .OrderByDescending(v => v.GetRemainingPressure(minutesLeft))
                    .FirstOrDefault();
                if (nextValve is not null)
                {
                    // Move to next valve
                    var distance = nextValve.DistanceTo[currentValve.Id];
                    minutesLeft -= distance;
                    // for each minute that passes during travel, add pressure of opened valves
                    Enumerable.Range(1, distance).ToList().ForEach(i => openedValves.ForEach(v => totalPressure += v.FlowRate));
                    openedValves.Add(nextValve);
                }
                else
                {
                    // let the minute pass and increase pressure by flow rate of open valves
                    openedValves.ForEach(v => totalPressure += v.FlowRate);
                }
            }
            else
            {
                // let the minute pass and increase pressure by flow rate of open valves
                openedValves.ForEach(v => totalPressure += v.FlowRate);
            }
        }

        return totalPressure;
    }

    static async Task<List<Valve>> ParseValves(string path)
    {
        var input = await File.ReadAllLinesAsync(path);
        var valves = new List<Valve>();
        var tunnelStrings = new List<IEnumerable<string>>();
        foreach (var line in input)
        {
            var lineParts = line.Split("; ");
            // Find id and flow rate
            var matches = Regex.Match(lineParts.First(), "^Valve ([A-Z]{2}) has flow rate=(\\d+)$").Groups.Values.Skip(1);
            valves.Add(new Valve(matches.First().Value, int.Parse(matches.Last().Value)));
            // Add strings of tunnels
            tunnelStrings.Add(lineParts.Last().Replace(",","").Split(' ')[4..]);
        }
        // All valves are created, add tunnel references
        for (int i = 0; i < valves.Count(); i++)
        {
            foreach (var tunnel in tunnelStrings[i])
            {
                valves[i].TunnelsTo.Add(valves.First(v => v.Id == tunnel));
            }
            valves[i].TunnelsTo.OrderByDescending(t => t.FlowRate);
        }

        // A*-ish to set shortest distance to each valve
        foreach (var fromValve in valves)
        {
            foreach (var toValve in valves.Where(v => v != fromValve))
            {
                var distance = FindValveDistance(fromValve, toValve);
                fromValve.DistanceTo.Add(toValve.Id,distance);
            }
        }

        return valves;
    }

    static int FindValveDistance(Valve fromValve, Valve toValve)
    {
        List<Valve> explored = new();
        List<Tuple<Valve, int>> valvesToExplore = new();
        // Create a list of tuples to track the depth of each tunnel found
        valvesToExplore.Add(new Tuple<Valve, int>(fromValve, 0));

        // Until we've found the shortest path
        while (valvesToExplore.Any())
        {
            // Get the current valve to explore, and ordered by shortest relative depth
            var currentValve = valvesToExplore.OrderBy(t => t.Item2).First();
            var depth = currentValve.Item2;

            // If we found the valve
            if (currentValve.Item1.Id == toValve.Id)
            {
                // Return relative depth of toValve from fromValve
                return depth;
            }

            // If we didn't find the right valve, mark valve as explored
            explored.Add(currentValve.Item1);
            valvesToExplore.Remove(currentValve);

            // For each tunnel from current valve
            foreach (var possibleValve in currentValve.Item1.TunnelsTo)
            {
                // If we already explored it
                if (explored.Any(v => v.Id == possibleValve.Id))
                    continue;

                if (valvesToExplore.Any(t => t.Item1.Id == possibleValve.Id))
                {
                    var potentialValve = valvesToExplore.First(t => t.Item1.Id == possibleValve.Id);
                    if (potentialValve.Item2 < depth + 1)
                    {
                        valvesToExplore.Remove(potentialValve);
                        valvesToExplore.Add(new Tuple<Valve, int>(possibleValve, depth + 1));
                    }
                }
                else
                {
                    valvesToExplore.Add(new Tuple<Valve, int>(possibleValve, depth + 1));
                }
            }
        }

        throw new Exception("Could not find path between valves.");
    }
}

class Valve
{
    public string Id { get; }
    public int FlowRate { get; }
    public List<Valve> TunnelsTo { get; }
    public Dictionary<string, int> DistanceTo { get; }

    public Valve(string id, int flowRate)
    {
        DistanceTo = new Dictionary<string, int>();
        TunnelsTo = new List<Valve>();
        Id = id;
        FlowRate = flowRate;
    }

    public int GetRemainingPressure(int minutesLeft)
    {
        return FlowRate * minutesLeft;
    }
}