using System.ComponentModel;
using System.Text.RegularExpressions;

class Program
{
    static async Task Main()
    {
        var valves = await ParseValves(".\\..\\..\\..\\input.txt");
        var start = valves.Values.First(v => v.Id == "AA");
        var valvesWithPressure = valves.Values.Where(v => v.FlowRate > 0).Select(v => (v.Id, v.FlowRate)).ToArray();
        // Part 1
        Console.WriteLine(GetPressure(30, valves.Values.Where(v => v.FlowRate > 0), start));
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

    static int GetPressureWithElephant(int[] minutesLeft, (string id, int flowRate)[] valvesWithPressure, string[] currentValves, Dictionary<string,Valve> valves)
    {
        int best = 0;
        int actor = minutesLeft[0] > minutesLeft[1] ? 0 : 1;

        var cur = valves[currentValves[actor]];
        foreach (var t in valvesWithPressure)
        {
            int newTimeToGo = minutesLeft[actor] - cur.DistanceTo[t.id] - 1;
            if (newTimeToGo > 0)
            {
                var newTimes = new int[] { newTimeToGo, minutesLeft[1 - actor] };
                var newLocs = new string[] { t.id, currentValves[1 - actor] };
                int gain = newTimeToGo * t.flowRate + GetPressureWithElephant(newTimes, valvesWithPressure.Where(c => c.id != t.id).ToArray(), newLocs, valves);
                if (best < gain) best = gain;
            }
        }
        return best;
    }

    static async Task<Dictionary<string,Valve>> ParseValves(string path)
    {
        var input = await File.ReadAllLinesAsync(path);
        var valves = new Dictionary<string, Valve>();
        var tunnelStrings = new List<IEnumerable<string>>();
        foreach (var line in input)
        {
            var lineParts = line.Split("; ");
            // Find id and flow rate
            var matches = Regex.Match(lineParts.First(), "^Valve ([A-Z]{2}) has flow rate=(\\d+)$").Groups.Values.Skip(1);
            string id = matches.First().Value;
            valves.Add(id, new Valve(matches.First().Value, int.Parse(matches.Last().Value)));
            // Add strings of tunnels
            valves[id].TunnelsTo.AddRange(lineParts.Last().Replace(",", "").Split(' ')[4..]);
        }

        // A*-ish to set shortest distance to each valve
        SetShortestPath(valves);

        return valves;
    }

    static void SetShortestPath(Dictionary<string, Valve> valves)
    {
        foreach (var valve in valves.Values)
        {
            valve.DistanceTo[valve.Id] = 0;
            SetValveDistance(valve, valve.Id, valves);
        }
    }

    static void SetValveDistance(Valve currentValve, string toValve, Dictionary<string, Valve> valves)
    {
        HashSet<string> explored = new();

        // Until we've found the shortest path
        while (currentValve != null && explored.Count < valves.Count)
        {
            explored.Add(currentValve.Id);

            // Get the current valve to explore, and ordered by shortest relative distance
            var distance = currentValve.DistanceTo[toValve] + 1;

            // If we didn't find the right valve, mark valve as explored
            explored.Add(currentValve.Id);

            // For each tunnel from current valve
            foreach (var valveString in currentValve.TunnelsTo)
            {
                // If we didn't already explore it
                if (!explored.Contains(valveString))
                {
                    var tunnelValve = valves[valveString];

                    if (tunnelValve.DistanceTo.ContainsKey(toValve))
                    {
                        if (distance < tunnelValve.DistanceTo[toValve]) tunnelValve.DistanceTo[toValve] = distance;
                    }
                    else
                    {
                        tunnelValve.DistanceTo[toValve] = distance;
                    }
                }
            }
            currentValve = valves.Values.Where(v => !explored.Contains(v.Id) && v.DistanceTo.ContainsKey(toValve)).OrderBy(c => c.DistanceTo[toValve]).FirstOrDefault();
        }
    }
}

class Valve
{
    public string Id { get; }
    public int FlowRate { get; }
    public List<string> TunnelsTo { get; }
    public Dictionary<string, int> DistanceTo { get; }

    public Valve(string id, int flowRate)
    {
        DistanceTo = new Dictionary<string, int>();
        TunnelsTo = new List<string>();
        Id = id;
        FlowRate = flowRate;
    }

    public int GetRemainingPressure(int minutesLeft)
    {
        return FlowRate * minutesLeft;
    }
}