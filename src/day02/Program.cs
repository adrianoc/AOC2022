const byte RockPoints = 1;
const byte PaperPoints = 2;
const byte ScissorsPoints = 3;

const byte WinPoints = 6;
const byte DrawPoints = 3;
const byte LosePoints = 0;

if (args.Length == 1 && args[0] == "test")
{
    Test(
        new [] { "A Y", "B X", "C Z" }, 
        DrawPoints + RockPoints + 
        LosePoints + RockPoints +
        WinPoints + RockPoints);

    Test(
        new [] { "A X", "B Y", "C Z" },
        LosePoints + ScissorsPoints +
        DrawPoints + PaperPoints +
        WinPoints + RockPoints);

    Test(
        new [] { "B X", "C Y", "A Z" }, 
        LosePoints + RockPoints +
        DrawPoints + ScissorsPoints +
        WinPoints + PaperPoints);

    return;
}

var input = System.IO.File.ReadAllLines("../../inputs/day02/input");
var result = ComputePoints(input);
System.Console.WriteLine($"Total: {result}");

int ComputePoints(string []input) 
{
    const char Lose = 'X';
    const char Draw = 'Y';
    const char Win = 'Z';

    var rules = new []
    {
        //       Z-X             Z-Y             Z-X
        //       Win,            Draw,           Lose               // Other
        new [] { PaperPoints,    RockPoints,     ScissorsPoints },  // Rock     (A): 0 -> A - A
        new [] { ScissorsPoints, PaperPoints,    RockPoints},       // Paper    (B): 1 -> B - A
        new [] { RockPoints,     ScissorsPoints, PaperPoints },     // Scissors (C): 2 -> C - A
    };

    int points = 0;
    for(int i = 0; i < input.Length; i++)
    {
        var pointsFromRound = input[i][2] switch
        {
            Win =>  WinPoints  + rules[input[i][0] - 'A'][0],
            Draw => DrawPoints + rules[input[i][0] - 'A'][1],
            Lose => LosePoints + rules[input[i][0] - 'A'][2],
            _ => throw new InvalidOperationException("Unexpected opponent round: {input[i][0]}")
        };

        points += pointsFromRound;
    }
    
    return points;
}

void Test(string []testInput, int expectedResult)
{
    var points = ComputePoints(testInput);

    if (points != expectedResult)
        throw new Exception($"Expecting {expectedResult} got {points}");
    
    System.Console.WriteLine("Success.");
}

record PointsPerResult(int Hash, uint Points);