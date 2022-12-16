//#define DEBUG_STEP
//#define DEBUG_SHOW_POINT
using System.Text;

var dataFilePath = $"../../inputs/day14/{(args.Length >= 1 ? args[0] : "input")}";
var input = File.ReadAllText(dataFilePath);

Console.WriteLine($"Data file: {dataFilePath}");

var runThroughStep = args.Length == 2 ? Int32.Parse(args[1]) : -1;
Run(input, runThroughStep);

void Run(string input, int runThroughStep)
{
    var cave = new Cave(input);

#if DEBUG_STEP
    var windowTopIndex = 0;
    var windowLinesCount = 30;
#endif
    
    var count = 0;
    bool b = true;
    do
    {
#if DEBUG_STEP
        if (count > runThroughStep)
        {
            cave.WriteTo(Console.Out, windowTopIndex, windowLinesCount, true);
            Console.WriteLine($"Count: {count}");

            var c = Console.ReadKey();
            switch (c.Key)
            {
                case ConsoleKey.Q:
                    return;
                
                case ConsoleKey.UpArrow:
                    windowTopIndex = Math.Max(0, windowTopIndex - windowLinesCount);
                    continue;
                
                case ConsoleKey.DownArrow:
                    windowTopIndex += windowLinesCount;
                    continue;
                
                case ConsoleKey.LeftArrow:
                    count--;
                    cave.Undo();
                    continue;

                case ConsoleKey.RightArrow:
                    break;
            }
        }
#endif
        
        b = cave.PourSand();
        count++;
    } while (b);
    
    Console.WriteLine(count);
}

class Cave
{
    public Cave(string inputScan)
    {
        Parse(inputScan);
    }
    
    public bool PourSand()
    {
        return PourSand(startPoint);
    }
    
    DateTime last = DateTime.Now;

    private bool PourSand(Point pos)
    {
        var sandPos = Fall(pos);
        if (sandPos.Y >= maxY)
            return false;

#if DEBUG_SHOW_POINT
        if (DateTime.Now.Subtract(last).Seconds > 5)
        {
            last = DateTime.Now;
            Console.WriteLine($"-> {sandPos}, max = {xxx}");
        }
#endif
        var leftDownCandidate = new Point(sandPos.X-1, sandPos.Y+1);
        var rightDownCandidate = new Point(sandPos.X+1, sandPos.Y+1);

        if (IsPositionFree(leftDownCandidate))
        {
            return PourSand(leftDownCandidate);
        }
        
        if (IsPositionFree(rightDownCandidate))
        {
            return PourSand(rightDownCandidate);
        }

        caveShape[sandPos.Y, sandPos.X] = 'o';
        
#if DEBUG_STEP
        lastPoints.Push(sandPos);
#endif
        return true;
    }

    private Point Fall(Point sandPos)
    {
        while (sandPos.Y + 1 < maxY && caveShape[sandPos.Y + 1, sandPos.X] == '.')
            sandPos.Y++;
        
        return sandPos;
    }

    bool IsPositionFree(in Point p) => 
        p.X >= 0 && p.X < maxX && p.Y > 0 && p.Y < maxY && caveShape[p.Y, p.X] == '.'
        || p.X < 0 && p.Y == maxY;
    
    #if DEBUG_STEP
    public void Undo(int count = 1)
    {
        while (count-- > 0 && lastPoints.TryPop(out var point))
        {
            caveShape[point.Y, point.X] = '.';
        }
    }
    #endif

    void DrawHorizontalRuler(TextWriter w, string spaces, int start, int count)
    {
        var digits = 0;
        for (var max = (start + count); max > 0; max = max / 10)
            digits++;


        var stack = new Stack<string>();
        var buffer = new StringBuilder();
        int divisor = 1;
        for (int i = 0; i < digits; i++)
        {
            int last = -1;
            buffer.Append(spaces);
            for (int value = start; value < (start + count); value++)
            {
                var digit = (value / divisor) % 10;
                buffer.Append($"{(digit != last ? digit.ToString() : " ")}");
                last = digit;
            }
            stack.Push(buffer.ToString());
            buffer.Clear();

            divisor *= 10;
        }

        while (stack.TryPop(out var line))
        {
            w.WriteLine(line);
        }
    }
    
    public void WriteTo(TextWriter w, int firstLine = 0, int numberOfLinesToShow = Int32.MaxValue, bool showRulers = false)
    {
        if (showRulers)
        {
            DrawHorizontalRuler(w,"    ", 0, 60);
        }

        numberOfLinesToShow = Math.Min(numberOfLinesToShow, caveShape.GetUpperBound(0) - firstLine);
        var lastLine = firstLine + numberOfLinesToShow;
        for(int y = firstLine; y <= lastLine; y++)
        {
            if (showRulers)
                w.Write($"{y,-4}");

            for(int x = 0; x <= caveShape.GetUpperBound(1); x++)
            {
                w.Write(caveShape[y,x]);
            }
            w.WriteLine();
        }
        
        #if DEBUG_STEP
        w.WriteLine($"Last settle: {(lastPoints.TryPeek(out var p) ? p.ToString() : "NA")}");
        #endif
    }

    void Parse(string inputScan)
    {
        /*
            498,4 -> 498,6 -> 496,6
            503,4 -> 502,4 -> 502,9 -> 494,9
        */
        var scanLines = inputScan.Split('\n', System.StringSplitOptions.RemoveEmptyEntries);

        int minXRange = Int32.MaxValue;
        int maxXRange = 0;

        HashSet<int> foundLines = new();
        var paths = new List<Path>();
        foreach(var line in scanLines)
        {
            if (!foundLines.Add(line.GetHashCode()))
            {
                Console.WriteLine($"Ignoring duplication: {line}");
                continue;
            }
                
            var pointsInScan = line.Split("->", System.StringSplitOptions.RemoveEmptyEntries);            
            var pathPoints = new List<Point>(pointsInScan.Length);

            foreach(var p in pointsInScan)
            {
                var point = Point.Parse(p);
                pathPoints.Add(point);

                if (point.X < minXRange)
                    minXRange = point.X;

                if (point.X > maxXRange)
                    maxXRange = point.X;

                if (point.Y > maxY)
                    maxY = point.Y;
            }
            paths.Add(new Path(pathPoints));
        }

        /*
            498,4 -> 498,6 -> 496,6
            503,4 -> 502,4 -> 502,9 -> 494,9
        */

        startX = minXRange;
        maxX = maxXRange - minXRange;
        caveShape = new char[maxY + 1, maxX + 1];
        
        // Initially the cave is full of air.
        for(int y = 0; y < maxY + 1; y++)
        {
            for (int x = 0; x <= caveShape.GetUpperBound(1); x++)
            {
                caveShape[y, x] = '.';
            }
        }
        
        caveShape[0, 500 - minXRange] = '+';

        // put in the rocks
        foreach(var p in paths)
        {
            var previousPoint = p.Points.First();
            foreach(var point in p.Points.Skip(1))
            {
                if (previousPoint.X == point.X)
                {
                    var yBegin = Math.Min(previousPoint.Y, point.Y);
                    var yEnd = Math.Max(previousPoint.Y, point.Y);

                    var x = point.X - minXRange;
                    for (int y = yBegin; y <= yEnd; y++)
                    {
                        caveShape[y, x] = '#';
                    }
                }
                else
                {
                    var xBegin = Math.Min(previousPoint.X, point.X);
                    var xEnd = Math.Max(previousPoint.X, point.X);

                    for(int x = xBegin; x <= xEnd; x++)
                        caveShape[point.Y, x - minXRange] = '#';
                }

                previousPoint = point;
            }            
        }

        startPoint = new Point(500 - minXRange, 0);
    }

    private Point startPoint;
    char[,] caveShape;
    #if DEBUG_STEP
    private Stack<Point> lastPoints = new Stack<Point>();
    #endif

    private int startX;
    
    int maxY;
    int maxX;
}

record struct Point(int X, int Y)
{
    public static Point Parse(string pointStr)
    {
        var pointComponents = pointStr.Split(',', System.StringSplitOptions.RemoveEmptyEntries);

        return new Point(Int32.Parse(pointComponents[0].Trim()), Int32.Parse(pointComponents[1].Trim()));
    }   
}

record struct Path(IList<Point> Points);