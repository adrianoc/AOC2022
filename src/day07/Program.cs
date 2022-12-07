using System;
using System.Text.RegularExpressions;

if (args.Length == 1 && args[0] == "test")
{
    Test(@"$ cd /
$ ls
dir a
14848514 b.txt
8504156 c.dat
dir d
$ cd a
$ ls
dir e
29116 f
2557 g
62596 h.lst
$ cd e
$ ls
584 i
$ cd ..
$ cd ..
$ cd d
$ ls
4060174 j
8033020 d.log
5626152 d.ext
7214296 k", new DirectoryData("a", 94853), new DirectoryData("e", 584));

    return;
}

var input = System.IO.File.ReadAllText("../../inputs/day07/input");
var rootDir = Parse(input);

System.Console.WriteLine("---- DIR STRUCT --- ");
PrintDir(rootDir);
System.Console.WriteLine("---- DIR STRUCT --- ");

System.Console.WriteLine();
System.Console.WriteLine();


var matchingDirs = FindMatchingCriteria(rootDir);
System.Console.WriteLine("Matching: ");
var totalSize = 0L;
foreach(var m in matchingDirs)
{
    System.Console.WriteLine($"{m.Name} : {m.SizeInBytes}");
    totalSize += m.SizeInBytes;
}

System.Console.WriteLine($"Total ===> {totalSize}");


DirectoryState Parse(string input)
{
    var currentDir = string.Empty;    
    var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
    for(int i = 0; i < lines.Length; i++)
    {
        if (lines[i].StartsWith("$ cd"))
        {
            var targetDir = lines[i].Substring(5);

            if (targetDir == "..")
            {
                if (ParseContext.CurrentDirectory.Parent == null)
                    throw new Exception($"Command {lines[i]} requires a current directory ({ParseContext.CurrentDirectory.Name}) to have a parent ");

                ParseContext.CurrentDirectory = ParseContext.CurrentDirectory.Parent;
                continue;
            }
            
            var dir = targetDir == ParseContext.RootDirectory.Name 
                                        ? ParseContext.RootDirectory 
                                        : ParseContext.CurrentDirectory.Directories.SingleOrDefault(d => d.Name == targetDir);
            if (dir == null)
            {
                dir = new DirectoryState(targetDir, ParseContext.CurrentDirectory);
                ParseContext.CurrentDirectory.Directories.Add(dir);
            }

            ParseContext.CurrentDirectory = dir;
        }
        else if (lines[i].StartsWith("$ ls"))
        {       
            for (int j = i + 1; j < lines.Length && lines[j][0] != '$'; j++)
            {
                if (lines[j].StartsWith("dir"))
                {
                    var dirName = lines[j].Substring(4);
                    if (!ParseContext.CurrentDirectory.Directories.Any(d => d.Name == dirName))
                        ParseContext.CurrentDirectory.Directories.Add(new DirectoryState(dirName, ParseContext.CurrentDirectory));
                }
                else
                {
                    var fileParseResult = Regex.Match(lines[j], @"^(?<size>\d+)\s+(?<name>.+)$");
                    if (!fileParseResult.Success)
                    {
                        throw new Exception($"Invalid ls result for file. Current dir: {ParseContext.CurrentDirectory.Name}, Line={lines[j]}");
                    }

                    ParseContext.CurrentDirectory.Files.Add(new FileState(fileParseResult.Groups["name"].Value, Int64.Parse(fileParseResult.Groups["size"].Value)));
                }
            }
        }
    }

    return ParseContext.RootDirectory;
}

IList<DirectoryData> FindMatchingCriteria(DirectoryState root)
{
    var result = new List<DirectoryData>();

    ComputeResult(root);

    void ComputeResult(DirectoryState dir)
    {
        var dirSize = CalculateDirSize(dir);
        if (dirSize < 100_000)
        {
            result.Add(new DirectoryData(dir.Name, dirSize));
        }

        foreach(var subDir in dir.Directories)
        {
            ComputeResult(subDir);
        }
    }

    long CalculateDirSize(DirectoryState dir)
    {
        var size = 0L;
        foreach(var file in dir.Files)
            size += file.Size;

        foreach(var subDir in dir.Directories)
            size += CalculateDirSize(subDir);

        return size;
    }
    
    return result;
}

void Test(string input, params DirectoryData[] expected)
{
    var root = Parse(input);
    PrintDir(root);

    var matchingDirs = FindMatchingCriteria(root);

    if (matchingDirs.Count != expected.Length)
         throw new Exception($"Expecting {expected.Length} entries, found {matchingDirs.Count}");

    foreach(var e in expected)
        if (!matchingDirs.Any(c => e.SizeInBytes == c.SizeInBytes))
            throw new Exception($"Expected dir {e.Name} with size {e.SizeInBytes} not found.");


    System.Console.WriteLine("Success");
}
    
void PrintDir(DirectoryState dir, int level = 0)
{
    System.Console.WriteLine($"{new String(' ', level * 2)}- {dir.Name} (dir)");
    var contentsIndent = new String(' ', (level + 1) * 2);
        
    foreach(var file in dir.Files)
    {
        System.Console.WriteLine($"{contentsIndent}- {file.Name} (file, size={file.Size})");
    }

    foreach(var subDir in dir.Directories)
    {
        PrintDir(subDir, level + 1);
    }
}

class ParseContext
{
    public static DirectoryState RootDirectory = new DirectoryState("/");
    public static DirectoryState CurrentDirectory = RootDirectory;
}

class DirectoryState
{
    public DirectoryState(string name, DirectoryState? parent = null) =>  (Name, Parent) = (name, parent);

    public DirectoryState? Parent { get; set; }

    public IList<FileState> Files = new List<FileState>();
    public IList<DirectoryState> Directories = new List<DirectoryState>();

    public string Name { get; set; }
}

record struct FileState(string Name, long Size);
record struct DirectoryData(string Name, long SizeInBytes);

