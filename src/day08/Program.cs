if (args.Length == 1 && args[0] == "test")
{
    Test();
    return;
}

var input = System.IO.File.ReadAllText("../../inputs/day08/input");
var score = ComputeScenicScore(input);
Console.WriteLine(score);

int ComputeScenicScore(string input)
{
    /*
    3 0 3 7 3
    2 5 5 1 2
    6 5 3 3 2
    3 3 5 4 9
    3 5 3 9 0

    */
    var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
    var n = lines.Length;
    var m = lines[0].Length; // assumes all lines have the same length

    int maxScenicScore = 0; 

    for (int i = 1; i < n-1; i++)
    {
        for (int j = 1; j < m-1; j++)
        {
            int leftCount = 0;
            for (int x = i - 1; x >=0; x--) // 0..i
            {
                leftCount++;
                if(lines[i][j] <= lines[x][j])
                {
                    break;
                }
            }
            
            int rightCount  =0;
            for (int x = i + 1; x < n; x++) // i + 1..n
            {
                rightCount++;
                if (lines[i][j] <= lines[x][j])
                {
                    break;
                }
            }            

            int upCount = 0;
            for (int y = j - 1; y >=0; y--) // 0..i
            {
                upCount++;
                if (lines[i][j] <= lines[i][y])
                {
                    break;
                }
            }
            
            int downCount = 0;
            for (int y = j + 1; y < m; y++) // i + 1..n
            {
                downCount++;
                if (lines[i][j] <= lines[i][y])
                {
                    break;
                }
            }

            int scenicScore = leftCount * rightCount * upCount * downCount;

            if (scenicScore > maxScenicScore)
            {
                System.Console.WriteLine($"({i},{j}) : {scenicScore}");
                maxScenicScore = scenicScore;
            }
        }
    }

    return maxScenicScore;
}

void Test()
{

    var input = @"30373
25512
65332
33549
35390";

    var score = ComputeScenicScore(input);
    if (score != 8)
        throw new Exception($"Expecting 8 got {score}");
    
    System.Console.WriteLine("Success");
}

