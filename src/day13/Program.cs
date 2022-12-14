var dataFilePath = $"../../inputs/day13/{(args.Length == 1 ? args[0] : "input")}";
var input = File.ReadAllText(dataFilePath);

Console.WriteLine($"Data file: {dataFilePath}");

/*
[1,1,3,1,1]
[1,1,5,1,1]

[[1],[2,3,4]]
[[1],4]

[9]
[[8,7,6]]

[[4,4],4,4]
[[4,4],4,4,4]

[7,7,7,7]
[7,7,7]

[]
[3]

[[[]]]
[[]]

[1,[2,[3,[4,[5,6,7]]]],8,9]
[1,[2,[3,[4,[5,6,0]]]],8,9]
*/

Run(input);

void Print(Packet p, int l)
{
    foreach (var pi in p.GetItems())
    {
        if (pi.Kind == PacketItemKind.Value) 
            Console.WriteLine($"{new string(' ', l)}{pi.Value}");
        else
            Print(pi.Packet, l + 4);
    }
}

void Run(string input)
{
    var packetList = new List<Packet>();
    
    var pairs = input.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
    foreach(var pair in pairs)
    {
        var packets = pair.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        packetList.Add(new Packet(packets[0]));
        packetList.Add(new Packet(packets[1]));
        
        // Console.Write($"Left:\n\t\t{packets[0]}\n\t\t");
        // PrintPacket(new Packet(packets[0]));
        // Console.WriteLine();
        //
        // Console.Write($"Right:\n\t\t{packets[1]}\n\t\t");
        // PrintPacket(new Packet(packets[1]));
        // Console.WriteLine();
    }

    var d1 = new Packet("[[2]]");
    var d2 = new Packet("[[6]]");
    packetList.Add(d1);
    packetList.Add(d2);
    packetList.Sort((l, r) => ComparePackets2(l, r));
    foreach (var packet in packetList)
        Console.WriteLine(packet);

    var d1Index = packetList.IndexOf(d1) + 1;
    var d2Index = packetList.IndexOf(d2) + 1;

    Console.WriteLine($"Decoder key: {d1Index * d2Index}");
}

int ComparePackets(string l, string r)
{
    var leftPacket = new Packet(l);
    var rightPaket = new Packet(r);

    return ComparePackets2(leftPacket, rightPaket);
}

int ComparePackets2(Packet leftPacket, Packet rightPaket)
{
    var left = leftPacket.GetItems().GetEnumerator();
    var right = rightPaket.GetItems().GetEnumerator();

    var p1x = left.MoveNext(); 
    var p2x = right.MoveNext();

    while (p1x && p2x)
    {
        var result = (left.Current.Kind, right.Current.Kind) switch
        {
            (PacketItemKind.Value, PacketItemKind.Value) =>left.Current.Value - right.Current.Value, 
            (PacketItemKind.Value, PacketItemKind.Packet) => ComparePackets2(new Packet(left.Current.Value), right.Current.Packet), 
            (PacketItemKind.Packet, PacketItemKind.Value) => ComparePackets2(left.Current.Packet, new Packet(right.Current.Value)), 
            (PacketItemKind.Packet, PacketItemKind.Packet) => ComparePackets2(left.Current.Packet, right.Current.Packet), 
        };

        if (result != 0)
            return result;
        
        p1x = left.MoveNext(); 
        p2x = right.MoveNext();
    }

    if (p1x && !p2x)
        return 1;
    
    return !p1x && !p2x ? 0 : -1;
}

void PrintPacket(Packet p)
{
    byte count = 0;
    Console.Write("[");
    foreach (var pi in p.GetItems())
    {
        if (count > 0)
            System.Console.Write(",");

        if (pi.Kind == PacketItemKind.Value)
            Console.Write($"{pi.Value}");
        else
            PrintPacket(pi.Packet);
        count++;
    }
    Console.Write("]");
}

//[[[[1]]]]
//[[3, [1,2]]]
//[[[]]]
//[[]]

//[[4,4],4,4]
//[1, [2,3]]
//[1]
//[[1, 2]]
class Packet
{
    public Packet(params byte[] data)
    {
        this.data = $"[{string.Join(',', data)}]";
    }
    
    public Packet(string data)
    {
        this.data = data;
    }

    public IEnumerable<PacketItem> GetItems()
    {
        char []itemSeparators = {',', ']'};
        char state = data[0];
        
        int i = 0;
        while (i < data.Length)
        {
            switch(state)
            {
                case '[':
                    i++;
                    state = '.';
                    break;

                case '.':
                    if (Char.IsNumber(data[i])) // parsing int.
                    {
                        var sepIndex = data.IndexOfAny(itemSeparators, i + 1);
                        state = data[sepIndex];
                        yield return new PacketItem(Byte.Parse(data.Substring(i, sepIndex-i).Trim()));
                        i = sepIndex;
                    }
                    else if (data[i] == ']')
                    {
                        yield break;
                    }
                    else if (data[i] == '[') // packet
                    {
                        var endOfPacketIndex = data.IndexOf(']', i + 1);
                        // find the `balanced` closing packet char ']', i.e, take into account
                        // sub packets. For instance, give the packet:
                        //           1         2 
                        // 012345678901234567890123
                        // [[[4,[6,3,10,5],5,9,4]]]
                        // 
                        // when processing [ at index 1 it:
                        // 1. Finds ] at index 14
                        // 2. Since between index 2 ~ 14 there are 2 [ we need to find 2 more ]
                        // 3. First extra ] (from 2) is at index 21. [ # between 14 ~ 21 = 0, so we don't need extra
                        //    ] (other than the remaining 1 from step 2)
                        // 4. Second extra ] (from 2) is at index 22. [ # between 21 ~ 22 = 0 => no extra ]
                        // 5. We conclude that ] at index 22 is the closing brace for [ at index 1
                        var subPacketsCount = data.AsSpan(i + 1, endOfPacketIndex - i - 1).Count('[');
                        while (subPacketsCount > 0)
                        {
                            var t = endOfPacketIndex;
                            while (subPacketsCount-- > 0)
                                endOfPacketIndex = data.IndexOf(']', endOfPacketIndex + 1); // next ]
                            
                            subPacketsCount = data.AsSpan(t + 1, endOfPacketIndex - t - 1).Count('[');
                        }
                        yield return new PacketItem(new Packet(data.Substring(i, endOfPacketIndex - i + 1)));
                        i = endOfPacketIndex + 1;
                        state = data[i];
                    }
                    break;

                case ']':
                    yield break;

                case ',':
                    state = '.';
                    i++;
                    break;
            }
        }
    }

    public override string ToString() => data;

    string data;
}

class PacketItem
{
    public PacketItem(byte value)
    {
        Value = value;
        Kind = PacketItemKind.Value;
    }
    
    public PacketItem(Packet value)
    {        
        Packet = value;
        Kind = PacketItemKind.Packet;
    }

    public PacketItemKind Kind { get; init; }
    
    public Packet Packet { get; init; }

    public byte Value { get; init; }
}

enum PacketItemKind
{
    Packet,
    Value
}

static class ReadOnlySpanExtensions
{
    public static int Count(this ReadOnlySpan<char> span, char ch)
    {
        int count = 0;
        foreach(var toCheck in span)
            if (toCheck == ch)
                count++;

        return count;
    }
}