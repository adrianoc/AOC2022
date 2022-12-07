using System;

if (args.Length == 1 && args[0] == "test")
{


    Test("mjqjpqmgbljsphdztnvjfqwrcgsmlb", 19);
    Test("bvwbjplbgvbhsrlpgdmjqwftvncz",23);
    Test("nppdvjthqldpwncqszvftbrmjlhg",23);
    Test("nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg",29);
    Test("zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw",26);

    return;
}

var input = System.IO.File.ReadAllText("../../inputs/day06/input");
System.Console.WriteLine(StartOfPacketMarkerIndexFor(input, 14));

int StartOfPacketMarkerIndexFor(string stream, byte windowSize)
{
    var buffer = stream.AsSpan();
    for(int i = 0; i < buffer.Length; i++)
    {
        var duplicatedIndexInWindow = FirstIndexOfDuplicate(buffer.Slice(i, windowSize));
        if (duplicatedIndexInWindow == -1)
        {
            return i + windowSize;
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
    var startOfPacketMarkerIndex = StartOfPacketMarkerIndexFor(stream, 14);
    if (startOfPacketMarkerIndex != expectedStart)
        throw new Exception($"Expected: {expectedStart}, Got: {startOfPacketMarkerIndex}");
    
    System.Console.WriteLine($"Success: {stream}");
}
