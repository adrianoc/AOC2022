using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

if (args.Length == 1 && args[0] == "test")
{
    Test();
    return;
}

var inputText = System.IO.File.ReadAllText("../../inputs/day05/input");

var inputData = ParseCrates(inputText);
        
var rearrangedCrates = RearrangeCrates(inputData);
var rearrangedStackTops = GetTopOfCrates(rearrangedCrates);
System.Console.WriteLine(rearrangedStackTops);

/*
 * Format:
 * 0         1         2
 * 01234567890123456789012
 * -----------------------
 *     [D]
 * [N] [C]
 * [Z] [M] [P] [X] [X] [X]
 *  1   2   3   4   5   6
 * -----------------------
 * 01234567890123456789012
 * 0         1         2
 */
InputData ParseCrates(string cratesAsTextInput)
{
    var lines = cratesAsTextInput.Split('\n', StringSplitOptions.RemoveEmptyEntries);
    var crateStacks = new List<Stack<string>>();

    var lineIndex = P2(crateStacks, lines, 0);

    var indexOfLastCrateIndex = lines[lineIndex].Trim().LastIndexOf(' ');
    if (Int32.TryParse(lines[lineIndex].Substring(indexOfLastCrateIndex + 1).Trim(), out var lastCrateIndex))
    {
        if (lastCrateIndex != crateStacks.Count)
            throw new Exception($"# of crate stacks != declared stacks from input ({crateStacks.Count} != {lastCrateIndex}).");
    }
    else
    {
        throw new Exception($"Invalid input. Expecting crate index line, got {lines[lineIndex]}");
    }

    var moveInstructions = ParseMoveInstructions(lines, lineIndex + 1);

    return new InputData(crateStacks, moveInstructions);
}

int P2(IList<Stack<String>> crateStacks, string []lines, int lineIndex)
{
    const byte CrateDataLength = 3;

    var line = lines[lineIndex];
    var crateStart = line.IndexOf('[');
    var toReturn = -1;
    if (crateStart != -1)
        toReturn = P2(crateStacks, lines, lineIndex + 1);
    else
        return lineIndex;

    while (crateStart != -1)
    {
        var crateIndex = crateStart / (CrateDataLength +1) + 1;
        while(crateIndex > crateStacks.Count)
            crateStacks.Add(new Stack<string>());

        // crateIndex is 1 based, whereas our stack list is 0 based.
        crateStacks[crateIndex-1].Push(line.Substring(crateStart + 1, line.IndexOf(']', crateStart) - crateStart - 1));
        crateStart = line.IndexOf('[', crateStart + 1);
    }

    return toReturn;
}

/*
 * Parses move instructions (one per line) following the syntax:
 * move x from source to destination
 */
IList<MoveInstruction> ParseMoveInstructions(string []lines, int startIndex)
{
    var moveInstructions = new List<MoveInstruction>();
    var regex = new Regex(@"\s*move\s+(?<count>\d+)\s+from\s+(?<source>\d+)\s+to\s+(?<destination>\d+)", RegexOptions.Compiled);
    for(int i = startIndex; i < lines.Length; i++)
    {
        var match = regex.Match(lines[i]);
        if (!match.Success)
        {
            throw new Exception($"Invalid movement instruction at line {i} : {lines[i]}");
        }

        moveInstructions.Add(new MoveInstruction(
            Int32.Parse(match.Groups["count"].Value),
            Int32.Parse(match.Groups["source"].Value) - 1, // 1 based -> 0 based
            Int32.Parse(match.Groups["destination"].Value) - 1));// 1 based -> 0 based
    }
    return moveInstructions;
}

IList<Stack<string>> RearrangeCrates(InputData inputData)
{
    foreach(var move in inputData.Moves)
    {
        var source = inputData.Crates[move.SourceCrate];
        var target = inputData.Crates[move.TargetCrate];

        for(int i = 0; i < move.Count; i++)
            target.Push(source.Pop());
    }
    return inputData.Crates;
}

string CrateStacksToString(IList<Stack<string>> crates)
{
    var maxDepth = crates.Max(crate => crate.Count);
    System.Console.WriteLine($"---> Max: {maxDepth}");

    var sb = new StringBuilder();
    for(int i = 0; i < crates.Count; i++)
    {
        while (crates[i].Count > 0)
            System.Console.WriteLine($"{i}:{crates[i].Pop()}");
    }
    return string.Empty;
}

string GetTopOfCrates(IList<Stack<string>> crates) => crates.Aggregate("", (acc, current) => acc + (current.Any() ? current.Peek() : string.Empty));

void Test()
{
    var input = @"    [D]
[N] [C]
[Z] [M] [P]
 1   2   3    

move 1 from 2 to 1
move 3 from 1 to 3
move 2 from 2 to 1
move 1 from 1 to 2";

    var inputData = ParseCrates(input);
        
    var rearrangedCrates = RearrangeCrates(inputData);
    var rearrangedStackTops = GetTopOfCrates(rearrangedCrates);

    if (rearrangedStackTops != "CMZ")
        throw new Exception($"Expecting CMZ, got {rearrangedStackTops}\nCrates after rearranging: {CrateStacksToString(rearrangedCrates)}");

    System.Console.WriteLine("Success");
}

record struct MoveInstruction(int Count, int SourceCrate, int TargetCrate);
record struct InputData(IList<Stack<string>> Crates, IList<MoveInstruction> Moves);