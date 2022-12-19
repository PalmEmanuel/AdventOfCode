using System.Text.RegularExpressions;

class Program
{
    static async Task Main()
    {
        var valves = await ParseValves(".\\..\\..\\..\\input.txt");
        var start = valves.First(v => v.Id == "AA");
        var valvesWithPressure = valves.Where(v => v.FlowRate > 0).Select(v => (v.Id, v.FlowRate)).ToArray();
        // Part 1
        Console.WriteLine(GetPressure(30, valves.Where(v => v.FlowRate > 0), start));
        // Part 2
        Console.WriteLine(GetPressureWithElephant(new int[] { 26, 26 }, valvesWithPressure, new string[] { "AA", "AA" }, valves));
    }

    static int GetPressure(int minutesLeft, IEnumerable<Valve> valvesToEvaulate, Valve currentValve)
    {
        int pressure = 0;
        // For every valve that has a higher flow rate than 0 (and has not been opened, because recursion)
        foreach (var valve in valvesToEvaulate)
        {
            // Get the amount of time left after moving to and opening the valve
            int currentMinutesLeft = minutesLeft - currentValve.DistanceTo[valve.Id] - 1;
            // If we haven't gone "overtime"
            if (currentMinutesLeft > 0)
            {
                // Recurse down again, saving it as an openedValve
                var pressureValvePath = GetPressure(currentMinutesLeft, valvesToEvaulate.Where(v => v.Id != valve.Id), valve);
                var currentPressure = currentMinutesLeft * valve.FlowRate + pressureValvePath;
                // If the result of the subpath is higher than the previous ones we've found, set the optimal pressure path to this one instead
                if (currentPressure > pressure)
                    pressure = currentPressure;
            }
        }

        return pressure;
    }

    static int GetPressureWithElephant(int[] minutesLeft, (string id, int flowRate)[] valvesWithPressure, string[] currentValves, List<Valve> valves)
    {
        // Track minutes separately, like a turn-based thing
        // Check if person already acted (by moving and opening, ie spending minutes)
        // This enables alternating between the person and elephant
        // Set actor to 0 or 1 based on relation between minutes left
        int actor = minutesLeft[0] > minutesLeft[1] ? 0 : 1;
        int pressure = 0;
        var currentValve = valves.First(v => v.Id == currentValves[actor]);
        foreach (var valve in valvesWithPressure)
        {
            // Person is index 0, elephant is 1
            // Get the minutes left of current actor, minus the distance to travel and the minute to open the valve
            int currentMinutesLeft = minutesLeft[actor] - currentValve.DistanceTo[valve.id] - 1;
            // If there's time left, try another valve
            if (currentMinutesLeft > 0)
            {
                // Set up new minutes and valve pairs, the second element is the current one for the other actor
                var newMinutesLeft = new int[] { currentMinutesLeft, minutesLeft[1 - actor] };
                var newValves = new string[] { valve.id, currentValves[1 - actor] };
                var potentialPressure = GetPressureWithElephant(newMinutesLeft, valvesWithPressure.Where(v => v.id != valve.id).ToArray(), newValves, valves);
                int currentPressure = currentMinutesLeft * valve.flowRate + potentialPressure;
                if (currentPressure > pressure)
                {
                    pressure = currentPressure;
                }
            }
        }

        return pressure;
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