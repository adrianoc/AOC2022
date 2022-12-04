if (args.Length == 1 && args[0] == "test")
{
    Test();
    return;
}

if (args.Length == 1 && args[0] == "test1")
{
    Test1();
    return;
}

var input = System.IO.File.ReadAllLines("../../inputs/day04/input");
var overlappingRanges = FullyOverlappingRanges(input);

System.Console.WriteLine($"Total: {overlappingRanges.Count}");

IList<OverlappingRange> FullyOverlappingRanges(string[] rangesInStringFormat)
{
    List<OverlappingRange> result = new();

    for(int i = 0; i < rangesInStringFormat.Length; i++)
    {
        ParseRanges(rangesInStringFormat[i], out var r1, out var r2);

        if (r1.IsSubRangeOf(r2) || r2.IsSubRangeOf(r1))
            result.Add(new OverlappingRange(r1, r2));
    }

    return result;
}

void Test()
{
    var input = new [] 
    {
        "2-4,6-8", 
        "2-3,4-5", 
        "5-7,7-9", 
        "2-8,3-7", 
        "6-6,4-6", 
        "2-6,4-8",
    };

    var overlapping = FullyOverlappingRanges(input);
    if (overlapping.Count != 2)
    {
        throw new Exception($"Expecting 2 overlapping ranges, got {overlapping.Count}");
    }

    if (overlapping[0].First.Start.Value != 2 || overlapping[0].First.End.Value != 8)
        throw new Exception($"Unexpected first overlapping range {overlapping[0].First}");

    if (overlapping[0].Second.Start.Value != 3 || overlapping[0].Second.End.Value != 7)
        throw new Exception($"Unexpected second overlapping range {overlapping[0].Second}");

    if (overlapping[1].First.Start.Value != 6 || overlapping[1].First.End.Value != 6)
        throw new Exception($"Unexpected first overlapping range {overlapping[1].First}");

    if (overlapping[1].Second.Start.Value != 4 || overlapping[1].Second.End.Value != 6)
        throw new Exception($"Unexpected Second overlapping range {overlapping[1].Second}");

    System.Console.WriteLine("Success");
}

void Test1()
{
    var input = new [] 
    {
        "20-40,30-35", 
        "5-7,7-9", 
        "2-8,3-7", 
        "60-61,50-65", 
    };

    var overlapping = FullyOverlappingRanges(input);
    if (overlapping.Count != 3)
    {
        throw new Exception($"Expecting 3 overlapping ranges, got {overlapping.Count}:\n{string.Join('\n', overlapping.ToArray())}");
    }

    System.Console.WriteLine("Success");
}

// Parses two ranges in a format: r1, r2
// each range is in the format: start-end
void ParseRanges(string twoRanges, out Range r1, out Range r2)
{
    var rangePairSeparator  = twoRanges.IndexOf(',');
    var firstRangeSeparator = twoRanges.IndexOf('-', 0, rangePairSeparator - 1);
    var secondRangeSeparator = twoRanges.IndexOf('-', rangePairSeparator);

    var twoRangesSpan = twoRanges.AsSpan();

    r1 = new Range(
        Int32.Parse(twoRangesSpan.Slice(0, firstRangeSeparator).Trim()),
        Int32.Parse(twoRangesSpan.Slice(firstRangeSeparator + 1, rangePairSeparator - firstRangeSeparator -1).Trim()));

    r2 = new Range(
        Int32.Parse(twoRangesSpan.Slice(rangePairSeparator + 1, secondRangeSeparator - rangePairSeparator - 1).Trim()),
        Int32.Parse(twoRangesSpan.Slice(secondRangeSeparator + 1).Trim()));
}

static class RangeExtensions
{
    public static bool IsSubRangeOf(this Range self, Range toBeChecked) => toBeChecked.Start.Value >= self.Start.Value && toBeChecked.End.Value <= self.End.Value;
}

record struct OverlappingRange(Range First, Range Second);