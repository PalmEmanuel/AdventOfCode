using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        using StreamReader sr = new StreamReader(".\\..\\..\\..\\input.txt");

        int index = 1;
        int correctIndexSum = 0;
        List<Package> packageList = new List<Package>();
        while (!sr.EndOfStream)
        {
            // If first isn't empty, then we can pick the second line too ("right")
            string lineLeft = sr.ReadLine();
            if (string.IsNullOrEmpty(lineLeft)) { continue; }
            string lineRight = sr.ReadLine();

            // Parse string lines to lists of lists or ints
            List<object> left = ParseLine(lineLeft);
            List<object> right = ParseLine(lineRight);
            // Add lists for sorting in part 2
            packageList.Add(new Package(lineLeft, left));
            packageList.Add(new Package(lineRight, right));
            // Evaluate order to get sum of correct indices
            if (IsRightOrder(left, right) == 1) { correctIndexSum += index; }
            index++;
        }

        // Part 1
        Console.WriteLine(correctIndexSum);

        // Add divider packages
        var divider1 = new Package("[[2]]", new List<object> { new List<object> { 2 } });
        var divider2 = new Package("[[6]]", new List<object> { new List<object> { 6 } });
        packageList.Add(divider1);
        packageList.Add(divider2);

        // Sort list ascending
        packageList.Sort((left, right) => right.CompareTo(left));

        PrintPackages(packageList);
        // Part 2
        Console.WriteLine((packageList.IndexOf(divider1) + 1) * (packageList.IndexOf(divider2) + 1));
    }

    public static void PrintPackages(List<Package> packages)
    {
        foreach (var package in packages)
        {
            Console.WriteLine(package);
        }
    }

    public static List<object> ParseLine(string line)
    {
        List<object> result = new List<object>();

        // Create a stack which tracks each list of items and the index in string
        Stack<List<object>> stack = new Stack<List<object>>();
        string numberSubstring = "";
        for (int i = 0; i < line.Length; i++)
        {
            var currentChar = line[i];
            // Create substring for numbers (since they can be more than 1 character)
            // Add to numberstring if it's a digit
            if (Regex.IsMatch(currentChar.ToString(), "\\d+"))
            {
                numberSubstring += currentChar.ToString();
            }
            else // If it's not a number (bracket or comma)
            {
                // If we've found at least one number
                if (!string.IsNullOrEmpty(numberSubstring))
                {
                    // Add number to the top list by peeking
                    stack.Peek().Add(int.Parse(numberSubstring));
                    numberSubstring = "";
                }

                // Start a new list
                if (currentChar == '[')
                {
                    // Add result to stack if there's no outer scope bracket
                    if (!stack.Any())
                    {
                        stack.Push(result);
                    }
                    else // Otherwise create a new list and add it to the last one
                    {
                        var newList = new List<object>();
                        stack.Peek().Add(newList);
                        stack.Push(newList);
                    }
                } // Close the list and add to results
                else if (currentChar == ']')
                {
                    var list = stack.Pop();
                }
            }
        }

        return result;
    }

    public static int IsRightOrder(List<object> left, List<object> right)
    {
        for (int i = 0; i < left.Count(); i++)
        {
            bool leftIsInt = left[i] is int;
            bool rightIsInt; // If right is smaller than left, return false
            try { rightIsInt = right[i] is int; } catch (ArgumentOutOfRangeException) { return -1; }

            // If at least one is a list, run method again as list
            if (!leftIsInt || !rightIsInt)
            {
                List<object> leftList;
                List<object> rightList;
                if (leftIsInt) { leftList = new List<object> { (int)left[i] }; } else { leftList = (List<object>)left[i]; }
                if (rightIsInt) { rightList = new List<object> { (int)right[i] }; } else { rightList = (List<object>)right[i]; }

                // If nested list comparison has a value, return it
                // Otherwise continue
                var listComparison = IsRightOrder(leftList, rightList);
                if (listComparison != 0) { return listComparison; } else continue;
            }
            // Continue if equal ints
            if ((int)left[i] == (int)right[i])
                continue;

            // True if smaller, false if larger (equal case already hit above)
            return (int)left[i] < (int)right[i] ? 1 : -1;
        }

        // If we reached end of left before end of right, return true
        if (left.Count() < right.Count()) { return 1; }

        return 0;
    }

    public class Package : IComparable<Package>
    {
        private string _string;
        public List<object> Value { get; }

        public Package(string @string, List<object> value)
        {
            _string = @string;
            Value = value;
        }

        public override string ToString()
        {
            return _string;
        }

        public int CompareTo(Package? right)
        {
            return IsRightOrder(this.Value, right!.Value);
        }
    }
}
