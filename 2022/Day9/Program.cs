using System.Text.RegularExpressions;

class Program
{
    const char Up = 'U';
    const char Down = 'D';
    const char Left = 'L';
    const char Right = 'R';

    static void Main()
    {
        // Setup instructions from input
        using StreamReader sr = new StreamReader(".\\..\\..\\..\\input.txt");
        List<Instruction> instructions = new List<Instruction>();
        while (!sr.EndOfStream)
        {
            var matches = Regex.Match(sr.ReadLine()!, "([A-Z]) (\\d+)");

            instructions.Add(new Instruction
            {
                Direction = matches.Groups[1].Value.First(),
                Steps = int.Parse(matches.Groups[2].Value)
            });
        }

        // Part 1
        Console.WriteLine(ProcessInstructions(2, instructions).Count());
        // Part 2
        Console.WriteLine(ProcessInstructions(10, instructions).Count());

        List<Pos> ProcessInstructions(int knots, List<Instruction> instructions)
        {
            List<Pos> tailPositions = new List<Pos> { { new Pos { X = 0, Y = 0 } } };
            List<Pos> ropeKnots = Enumerable.Range(1, knots).Select(i => new Pos { X = 0, Y = 0 }).ToList();

            foreach (var instruction in instructions)
            {
                var dir = instruction.Direction;

                // For each step
                for (int i = 0; i < instruction.Steps; i++)
                {
                    // Move the first knot
                    ropeKnots[0] = Move(ropeKnots[0], dir);

                    // Start from the knot after the head
                    for (int j = 1; j < ropeKnots.Count(); j++)
                    {
                        // If current knot should follow last one
                        if (KnotShouldMove(ropeKnots[j - 1], ropeKnots[j]))
                        {
                            // Move (overload) current knot in relation to the last position of the previous one
                            ropeKnots[j] = Move(ropeKnots[j - 1], ropeKnots[j]);
                            // If we're at the last knot (tail) and the position is not in the list, add it
                            if (j == ropeKnots.Count() - 1 && !tailPositions.Any(p => p.X == ropeKnots[j].X && p.Y == ropeKnots[j].Y))
                            {
                                tailPositions.Add(new Pos { X = ropeKnots[j].X, Y = ropeKnots[j].Y });
                            }
                        }
                        else // If this knot shouldn't move, no knots after it should move either
                        {
                            break;
                        }
                    }
                }
            }

            return tailPositions;
        }
    }

    // Returns X and Y direction that the tail must move one step of
    static bool KnotShouldMove(Pos head, Pos tail)
    {
        // If head is at least 2 steps away from tail
        int xDiff = Math.Abs(Math.Max(head.X, tail.X) - Math.Min(head.X, tail.X));
        int yDiff = Math.Abs(Math.Max(head.Y, tail.Y) - Math.Min(head.Y, tail.Y));
        return xDiff > 1 || yDiff > 1;
    }

    static Pos Move(Pos pos, char dir)
    {
        pos.LastPos = new Pos { X = pos.X, Y = pos.Y };
        switch (dir)
        {
            case Up: pos.Y++; break;
            case Down: pos.Y--; break;
            case Right: pos.X++; break;
            case Left: pos.X--; break;
        }

        return pos;
    }

    static Pos Move(Pos head, Pos tail)
    {
        tail.LastPos = new Pos { X = tail.X, Y = tail.Y };

        // If the previous knot moved diagonally
        if (head.LastPos!.X != head.X && head.LastPos.Y != head.Y)
        {
            // Current knot is not in the same column or row
            // See if we need to move diagonally
            if (tail.X != head.X && tail.Y != head.Y)
            {
                if (head.X > tail.X)
                {
                    tail.X++;
                }
                else
                {
                    tail.X--;
                }
                if (head.Y > tail.Y)
                {
                    tail.Y++;
                }
                else
                {
                    tail.Y--;
                }
            }
            else
            {
                // If we don't need to move diagonally
                // Calculate which direction to move
                // If X difference is further away than Y
                char dir;
                if (Math.Abs(tail.X - head.X) > Math.Abs(tail.Y - head.Y))
                {
                    dir = head.X > tail.X ? Right : Left;
                } else
                {
                    dir = head.Y > tail.Y ? Up : Down;
                }
                tail = Move(tail, dir);
            }
        }
        else // If last knot did not move diagonally, set to that position
        {
            tail.X = head.LastPos!.X;
            tail.Y = head.LastPos!.Y;
        }

        return tail;
    }

    static void PrintRope(List<Pos> knots, int width)
    {
        for (int y = width; y >= -width; y--)
        {
            for (int x = -width; x < width; x++)
            {
                if (knots.Any(k => k.X == x && k.Y == y))
                {
                    int index = knots.FindIndex(k => k.X == x && k.Y == y);
                    Console.Write(index == 0 ? "H" : index);
                }
                else
                {
                    Console.Write("x");
                }
            }
            Console.WriteLine();
        }
    }

    record Pos
    {
        public int X;
        public int Y;
        public Pos? LastPos;
    }

    record Instruction
    {
        public char Direction;
        public int Steps;
    }
}