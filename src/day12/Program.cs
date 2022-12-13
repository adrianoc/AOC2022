using System.Text;

if (args.Length == 1 && args[0] == "test")
{
    Test();
    return;
}

var solver = new Solver(File.ReadAllText("../../inputs/day12/input"));
var paths = solver.FindAllPaths();


void Test()
{
    var solver = new Solver(
@"Sabqponm
abcryxxl
accszExk
acctuvwj
abdefghi");

    var paths = solver.FindAllPaths();
    foreach(var path in paths)
        System.Console.WriteLine($"{path.Length} : {path}");

    Console.WriteLine($"Best path: {solver.BestPath} ({solver.BestPath.Length})");

    solver.Save("/tmp/output.cvs");

    if (solver.BestPath.Length != 31)
        throw new Exception($"Expected 31, got {solver.BestPath.Length}");
}

class Solver
{
    public Solver(string input)
    {
        Parse(input);

        Start = Find('S');
        Exit = Find('E');

        heightmap[Start.Y][Start.X] = 'w';
        heightmap[Exit.Y][Exit.X] = 'z';
        
        Console.WriteLine($"{height} x {width}");
        
        Console.WriteLine($"Start: {Start}");
        Console.WriteLine($"Exit: {Exit}");
    }

    public void Save(string path)
    {
        using var outputFile = File.CreateText(path);
        for (int i = 0; i < height; i++)
        {
            outputFile.WriteLine($"{string.Join(", ", heightmap[i])}");
        }
    }

    public IList<string> FindAllPaths()
    {
        var ret = new List<string>();
        FindPaths(Start.Y, Start.X, new StringBuilder(), ret);

        return ret;
    }

    void FindPaths(int y, int x, StringBuilder currentPath, IList<string> paths)
    {
        if (x == Exit.X && y == Exit.Y)
        {
            var foundPath = currentPath.ToString();
            paths.Add(foundPath);

            if (BestPath == null|| BestPath.Length > foundPath.Length)
                BestPath = foundPath;

            return;
        }

        visited[y][x] = 1;
        if (y > 0 && (heightmap[y - 1][x] - heightmap[y][x]) < 2)
        {
            GoTo(y - 1, x, '^', currentPath, paths);
        }
        
        if (y + 1 < height && (heightmap[y + 1][x] - heightmap[y][x]) < 2)
        {
            GoTo(y + 1, x, 'V', currentPath, paths);
        }

        if (x > 0 && (heightmap[y][x - 1] - heightmap[y][x]) < 2)
        {
            GoTo(y, x - 1, '<', currentPath, paths);
        }
        
        if (x + 1 < width && (heightmap[y][x + 1] - heightmap[y][x]) < 2)
        {
            GoTo(y, x + 1, '>', currentPath, paths);
        }
    }

    void GoTo(int y, int x, char dir, StringBuilder currentPath, IList<string> paths)
    {
        if (visited[y][x] != 0)
            return;
        
        currentPath.Append(dir);
        FindPaths(y, x, currentPath, paths);
        currentPath.Remove(currentPath.Length - 1, 1);
        visited[y][x] = 0;
    }

    public string BestPath { get; internal set; }

    public (int Y, int X) Start { get; internal set; }
    public (int Y, int X) Exit { get; internal set; }

    private (int Y, int X) Find(char toBeFound)
    {
        for(int line = 0; line < heightmap.Length; line++)
            for(int column = 0; column < heightmap[line].Length ; column++)
                if (heightmap[line][column] == toBeFound)
                    return (line, column);

        return (-1, -1);
    }

    private void Parse(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        height = lines.Length;        
        width = lines[0].Length;

        heightmap = new char[height][];
        visited = new byte[height][];

        for(int i = 0; i < lines.Length; i++)
        {
            heightmap[i] = lines[i].ToArray();
            visited[i] = new byte[width];
        }
    }

    char [][] heightmap;
    byte [][] visited;

    int width;
    int height;
}