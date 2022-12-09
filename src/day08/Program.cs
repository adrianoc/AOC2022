if (args.Length == 1 && args[0] == "test")
{
    Test();
    return;
}

var input = System.IO.File.ReadAllText("../../inputs/day08/input");
var count = CountVisibleTrees(input);
Console.WriteLine(count);

int CountVisibleTrees(string input)
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

    var count = (n * 2) + (m - 2) * 2; // all edges are visible by definition

    for (int i = 1; i < n-1; i++)
    {
        for (int j = 1; j < m-1; j++)
        {
            bool found = true;
            for (int x = 0; x < i; x++) // 0..i
            {
                if (lines[i][j] <= lines[x][j])
                {
                    found = false;
                    break;
                }
            }
            
            if (!found)
            {
                found = true;
                for (int x = i + 1; x < n; x++) // i + 1..n
                {
                    if (lines[i][j] <= lines[x][j])
                    {
                        found = false;
                        break;
                    }
                }
            }

            if (!found)
            {
                found = true;
                for (int y = 0; y < j; y++) // 0..i
                {
                    if (lines[i][j] <= lines[i][y])
                    {
                        found = false;
                        break;
                    }
                }
            }
            
            if (!found)
            {
                found = true;
                for (int y = j + 1; y < m; y++) // i + 1..n
                {
                    if (lines[i][j] <= lines[i][y])
                    {
                        found = false;
                        break;
                    }
                }
            }

            if (found)
            {
                count++;
                System.Console.WriteLine($"({i},{j}): {count}");
            }
        }
    }

    return count;
}

void Test()
{

    var input = @"30373
25512
65332
33549
35390";

    var visibleTreesCount = CountVisibleTrees(input);
    if (visibleTreesCount != 21)
        throw new Exception($"Expecting 21 got {visibleTreesCount}");
    
    System.Console.WriteLine("Success");
}

