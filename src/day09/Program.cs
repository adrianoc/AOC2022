if (args.Length == 1 && args[0] == "test")
{
    Test(@"R 4
U 4
L 3
D 1
R 4
D 1
L 5
R 2", 13);

return;
}

var simulator = new RopeSimulator();

var input = System.IO.File.ReadAllText("../../inputs/day09/input");
var visited = simulator.Simulate(input);
System.Console.WriteLine($"Visited: {visited}");

void Test(string input, int expected)
{
    var simulator = new RopeSimulator();

    var visited = simulator.Simulate(input);
    if (visited != expected)
        throw new Exception($"Expected {expected}, got {visited}");

    System.Console.WriteLine("Success");
}

class RopeSimulator
{
    private (int X, int Y) head;
    private (int X, int Y) tail;

    public int Simulate(string input)
    {
        var visited = new HashSet<(int X, int Y)>();

        var headMovements = ParseInput(input);

        foreach(var m in headMovements)            
        {
            for(int a = 0; a < m.Amount; a++)
            {
                head.X += m.Direction.X;
                head.Y += m.Direction.Y;

                var diffX = head.X - tail.X;
                var diffY = head.Y - tail.Y;

                if (Math.Abs(diffX) > 1 && diffY == 0)
                {
                    tail.X += MoveDiff(diffX);
                }
                else if (Math.Abs(diffY) > 1 && diffX == 0)
                {                    
                    tail.Y += MoveDiff(diffY);
                }
                else if (Math.Abs(diffY) > 1 || Math.Abs(diffX) > 1)  // diagonal
                {
                    tail.X += MoveDiff(diffX);
                    tail.Y += MoveDiff(diffY);
                }

                visited.Add(tail);
            }
        }

        return visited.Count;
    }

    private int MoveDiff(int amount)
    {
        if (amount < 0) return -1;
        if (amount > 0) return 1;

        return 0;
    }
    
    private List<((int X, int Y) Direction, int Amount)> ParseInput(string input)
    {
        var result = new List<((int X, int Y), int Amount)>();
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach(var line in lines)
        {
            var movement = ParseMovement(line);
            result.Add(movement);
        }

        return result;
    }

    private ((int X, int Y) Direction, int Amount) ParseMovement(string movementLine)
    {
        var splitted = movementLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (splitted.Length != 2)
            throw new Exception($"Invalid line {movementLine}.");

        var amount = Int32.Parse(splitted[1]);

        return splitted[0] switch 
        {
            "D" => ((0, 1), amount),
            "U" => ((0, -1), amount),
            "R" => ((1, 0), amount),
            "L" => ((-1, 0), amount),
            _ => throw new Exception($"Invalid movement: {movementLine}")
        };
    }
}
