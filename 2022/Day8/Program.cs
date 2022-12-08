var input = File.ReadAllLines(".\\..\\..\\..\\input.txt");
int width = input[0].Length;
int height = input.Count();

// populate tree grid
var treeGrid = new List<List<Tree>>();
for (int x = 0; x < width; x++)
{
    treeGrid.Add(new List<Tree>());
	for (int y = 0; y < height; y++)
	{
		treeGrid[x].Add(new Tree
		{
			Height = int.Parse(input[x][y].ToString()),
			IdentifierXY = $"{x},{y}",
            X = x,
            Y = y
		});
	}
}

var visibleTrees = GetVisibleTrees(treeGrid);

PrintGrid(treeGrid, visibleTrees);

// Part 1
Console.WriteLine(visibleTrees.Count());

// Part 2
Console.WriteLine(CalculateBestScenicScore(treeGrid));

int CalculateBestScenicScore(List<List<Tree>> treeGrid)
{
    int width = treeGrid.Count();
    int height = treeGrid[0].Count();

    int bestScore = 0;

    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            int currentTreeScore = CalculateScenicScore(treeGrid[x][y], treeGrid);

            if (currentTreeScore > bestScore)
                bestScore = currentTreeScore;
        }
    }

    return bestScore;
}

int CalculateScenicScore(Tree tree, List<List<Tree>> treeGrid)
{
    int width = treeGrid.Count();
    int height = treeGrid[0].Count();

    int leftScore = 0;
    int rightScore = 0;
    int upScore = 0;
    int downScore = 0;

    // Right
    for (int x = tree.X; x < width-1; x++)
    {
        rightScore++;
        if (tree.Height <= treeGrid[x+1][tree.Y].Height)
        {
            break;
        }
    }

    // Down
    for (int y = tree.Y; y < height-1; y++)
    {
        downScore++;
        if (tree.Height <= treeGrid[tree.X][y+1].Height)
        {
            break;
        }
    }

    // Left
    for (int x = tree.X; x > 0; x--)
    {
        leftScore++;
        if (tree.Height <= treeGrid[x-1][tree.Y].Height)
        {
            break;
        }
    }

    // Up
    for (int y = tree.Y; y > 0; y--)
    {
        upScore++;
        if (tree.Height <= treeGrid[tree.X][y-1].Height)
        {
            break;
        }
    }

    return leftScore * rightScore * upScore * downScore;
}

List<Tree> GetVisibleTrees(List<List<Tree>> treeGrid)
{
    int width = treeGrid.Count();
    int height = treeGrid[0].Count();

    int treeHeightThreshold = 0;
    List<Tree> visibleTrees = new List<Tree>();
    // Left to right
    for (int y = 0; y < height; y++)
    {
        treeHeightThreshold = 0;
        for (int x = 0; x < width; x++)
        {
            var currentTree = treeGrid[x][y];

            if ((y == 0 || x == 0 || y == height - 1 || x == width - 1 || treeHeightThreshold < currentTree.Height) && !visibleTrees.Any(t => t.IdentifierXY == currentTree.IdentifierXY))
            {
                visibleTrees.Add(currentTree);
            }

            if (currentTree.Height > treeHeightThreshold)
                treeHeightThreshold = currentTree.Height;
        }
    }
    // Top to bottom
    for (int x = 0; x < width; x++)
    {
        treeHeightThreshold = 0;
        for (int y = 0; y < height; y++)
        {
            var currentTree = treeGrid[x][y];

            if ((x == 0 || y == 0 || y == height - 1 || x == width - 1 || treeHeightThreshold < currentTree.Height) && !visibleTrees.Any(t => t.IdentifierXY == currentTree.IdentifierXY))
            {
                visibleTrees.Add(currentTree);
            }

            if (currentTree.Height > treeHeightThreshold)
                treeHeightThreshold = currentTree.Height;
        }
    }

    // Right to left
    for (int y = height - 1; y >= 0; y--)
    {
        treeHeightThreshold = 0;
        for (int x = width - 1; x >= 0; x--)
        {
            var currentTree = treeGrid[x][y];

            if ((y == 0 || x == 0 || y == height - 1 || x == width - 1 || treeHeightThreshold < currentTree.Height) && !visibleTrees.Any(t => t.IdentifierXY == currentTree.IdentifierXY))
            {
                visibleTrees.Add(currentTree);
            }

            if (currentTree.Height > treeHeightThreshold)
                treeHeightThreshold = currentTree.Height;
        }
    }
    // Bottom to top
    for (int x = width - 1; x >= 0; x--)
    {
        treeHeightThreshold = 0;
        for (int y = height - 1; y >= 0; y--)
        {
            var currentTree = treeGrid[x][y];

            if ((x == 0 || y == 0 || y == height - 1 || x == width - 1 || treeHeightThreshold < currentTree.Height) && !visibleTrees.Any(t => t.IdentifierXY == currentTree.IdentifierXY))
            {
                visibleTrees.Add(currentTree);
            }

            if (currentTree.Height > treeHeightThreshold)
                treeHeightThreshold = currentTree.Height;
        }
    }

    return visibleTrees;
}

void PrintGrid(List<List<Tree>> grid, List<Tree> visibleTrees)
{
    for (int x = 0; x < grid[0].Count(); x++)
    {
        for (int y = 0; y < grid.Count(); y++)
        {
            if (visibleTrees.Any(t => t.IdentifierXY == grid[x][y].IdentifierXY))
            {
                Console.Write(grid[x][y].Height);
            }
            else
            {
                Console.Write("x");
            }
        }
        Console.WriteLine();
    }
}

class Tree
{
	public int Height { get; set; }
	public int X { get; set; }
	public int Y { get; set; }
	public string IdentifierXY { get; set; }
}