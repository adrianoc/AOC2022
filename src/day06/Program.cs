using System;

if (args.Length == 1 && args[0] == "test")
{
    Test("mjqjpqmgbljsphdztnvjfqwrcgsmlb", 7);
    
    Test("bvwbjplbgvbhsrlpgdmjqwftvncz", 5);
    Test("nppdvjthqldpwncqszvftbrmjlhg", 6);
    Test("nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", 10);
    Test("zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw", 11);

    return;
}

var input = System.IO.File.ReadAllText("../../inputs/day06/input");
System.Console.WriteLine(StartOfPacketMarkerIndexFor(input));

int StartOfPacketMarkerIndexFor(string stream)
{
    var buffer = stream.AsSpan();
    for(int i = 0; i < buffer.Length; i++)
    {
        var duplicatedIndexInWindow = FirstIndexOfDuplicate(buffer.Slice(i, 4));
        if (duplicatedIndexInWindow == -1)
        {
            return i + 4;
        }

        i = i + duplicatedIndexInWindow;
    }

    return -1;
}


int FirstIndexOfDuplicate(ReadOnlySpan<char> buffer)
{
    for(int i = 0; i < buffer.Length; i++)
    {
        for(int j = i + 1; j < buffer.Length; j++)
        {
            if (buffer[i] == buffer[j])
                return i;
        }
    }

    return -1;
}

void Test(string stream, int expectedStart)
{
    var startOfPacketMarkerIndex = StartOfPacketMarkerIndexFor(stream);
    if (startOfPacketMarkerIndex != expectedStart)
        throw new Exception($"Expected: {expectedStart}, Got: {startOfPacketMarkerIndex}");
    
    System.Console.WriteLine($"Success: {stream}");
}
