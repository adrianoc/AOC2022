/*
 *                 +-------------------------+
 *                 | Rock | Paper | Scissors |
 *                 +-------------------------+
 *                 |   A  |   B   |    C     |
 * +-----------------------------------------+
 * |  Rock     | X |   4  |   1   |    6     |
 * +-----------------------------------------+
 * |  Paper    | Y |   8  |   5   |    2     |
 * +-----------------------------------------+
 * |  Scissors | Z |   3  |   9   |    6     |
 * +-----------------------------------------+
 */

const byte RockPoints = 1;
const byte PaperPoints = 2;
const byte ScissorsPoints = 3;

const byte WinPoints = 6;
const byte DrawPoints = 3;
const byte LosePoints = 0;

var RockScissorsWin      = new PointsPerResult("CX".GetHashCode(), WinPoints + RockPoints);
var RockRockDraw         = new PointsPerResult("AX".GetHashCode(), DrawPoints + RockPoints);
var RockPaperLose        = new PointsPerResult("BX".GetHashCode(), LosePoints + RockPoints);

var PaperRockWin         = new PointsPerResult("AY".GetHashCode(), WinPoints + PaperPoints);
var PaperPaperDraw       = new PointsPerResult("BY".GetHashCode(), DrawPoints + PaperPoints);
var PaperScissorsLose    = new PointsPerResult("CY".GetHashCode(), LosePoints + PaperPoints);

var ScissorsPaperWin     = new PointsPerResult("BZ".GetHashCode(), WinPoints + ScissorsPoints);
var ScissorsScissorsDraw = new PointsPerResult("CZ".GetHashCode(), DrawPoints + ScissorsPoints);
var ScissorsRockLose     = new PointsPerResult("AZ".GetHashCode(), LosePoints + ScissorsPoints);


var pointsPerRoundResult = new []
{
    RockScissorsWin,
    RockRockDraw,
    RockPaperLose,

    PaperRockWin,
    PaperPaperDraw,
    PaperScissorsLose,

    ScissorsPaperWin,
    ScissorsScissorsDraw,
    ScissorsRockLose,
};

if (args.Length == 1 && args[0] == "test")
{
    Test(new [] { "A Y", "B X", "C Z" }, 15);
    Test(new [] { "A X", "B Y", "C Z" }, DrawPoints * 3 + RockPoints + PaperPoints + ScissorsPoints);
    Test(new [] { "B X", "C Y", "A Z" }, LosePoints * 3 + RockPoints + PaperPoints + ScissorsPoints);
    Test(new [] { "C X", "A Y", "B Z" }, WinPoints * 3  + RockPoints + PaperPoints + ScissorsPoints);
    Test(new [] { "C X", "B Y", "A Z" }, WinPoints + LosePoints + DrawPoints + RockPoints + PaperPoints + ScissorsPoints);
    return;
}

var input = System.IO.File.ReadAllLines("../../inputs/day02/input");
var result = ComputePoints(input);
System.Console.WriteLine($"Total: {result}");

uint ComputePoints(string []input) 
{
    uint points = 0;
    for(int i = 0; i < input.Length; i++)
    {
        var roundResult = input[i].Replace(" ", string.Empty).GetHashCode();
        for(byte j = 0; j < pointsPerRoundResult.Length; j++)
        {
            if (roundResult == pointsPerRoundResult[j].Hash)
            {
                points += pointsPerRoundResult[j].Points;
                break;
            }
        }
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