using System.Text.RegularExpressions;

var input = File.ReadAllLines(".\\..\\..\\..\\input.txt");

Dictionary<string, IDiskItem> items = new Dictionary<string, IDiskItem>();
DirectoryItem? currentDir = null;
foreach (var line in input)
{
    if (line.StartsWith('$')) // $
    {
        var match = Regex.Match(line, "\\$ ([a-z]+)\\s*([a-z/.]*)");

        if (match.Groups[1].Value == "cd")
        {
            string name = match.Groups[2].Value;
            if (name == "..") // cd ..
            {
                currentDir = currentDir!.Parent;
            }
            else // cd <dir name>
            {
                string expectedName = currentDir is not null ? currentDir.Name + "|" + name : name;
                if (items.ContainsKey(expectedName))
                {
                    currentDir = (DirectoryItem)items[expectedName];
                }
                else
                {
                    var newDir = new DirectoryItem(name,currentDir);
                    currentDir?.Children.Add(newDir);
                    currentDir = newDir;
                    items.Add(expectedName, newDir);
                }
            }
        }
    }
    else if (line.StartsWith("dir")) // dir <name>
    {
        string name = line.Split(" ").Last();
        var newDir = new DirectoryItem(name, currentDir);
        if (!items.ContainsKey(newDir.Name))
        {
            currentDir?.Children.Add(newDir);
            items.Add(newDir.Name, newDir);
        }
    }
    else // <filesize> <filename>
    {
        var match = Regex.Match(line, "(\\d+) ([a-z.]+)");
        int size = int.Parse(match.Groups[1].Value);
        string name = match.Groups[2].Value;
        var fileItem = new FileItem(size, name, currentDir!);
        currentDir?.Children.Add(fileItem);
        items.Add(fileItem.Name, fileItem);
    }
}

// part 1
Console.WriteLine(items.Where(i => i.Value.Size <= 100000 && i.Value is DirectoryItem).Sum(i => i.Value.Size));

// part 2
int totalSize = items["/"].Size;
int neededSpace = 30000000;
int availableSpace = 70000000 - totalSize;
Console.WriteLine(items.Select(i => i.Value).OrderBy(i => i.Size).First(i => i.Size >= neededSpace - availableSpace).Size);

class DirectoryItem : IDiskItem
{
    private DirectoryItem? _parent;
    public DirectoryItem? Parent => _parent;

    public int Size => Children.Sum(c => c.Size);

    private string _name;
    public string Name { get { return Parent is not null ? Parent.Name + "|" + _name : _name; } }

    private List<IDiskItem> _children;
    public List<IDiskItem> Children => _children;

    public DirectoryItem(string name, DirectoryItem? parent)
    {
        _children = new List<IDiskItem>();
        _name = name;
        _parent = parent;
    }
}

class FileItem : IDiskItem
{
    private DirectoryItem _parent;
    public DirectoryItem Parent => _parent;

    private int _size;
    public int Size => _size;

    private string _name;
    public string Name => Parent.Name + "|" + _name;

    public List<IDiskItem> Children => new ();

    public FileItem(int size, string name, DirectoryItem parent)
    {
        _size = size;
        _name = name;
        _parent = parent;
    }
}

interface IDiskItem
{
    public DirectoryItem? Parent { get; }
    public int Size { get; }
    public string Name { get; }
    public List<IDiskItem> Children { get; }
}