using System;
using System.Text.RegularExpressions;

if (args.Length == 1 && args[0] == "test")
{
    Test(@"Monkey 0:
  Starting items: 79, 98
  Operation: new = old * 19
  Test: divisible by 23
    If true: throw to monkey 2
    If false: throw to monkey 3

Monkey 1:
  Starting items: 54, 65, 75, 74
  Operation: new = old + 6
  Test: divisible by 19
    If true: throw to monkey 2
    If false: throw to monkey 0

Monkey 2:
  Starting items: 79, 60, 97
  Operation: new = old * old
  Test: divisible by 13
    If true: throw to monkey 1
    If false: throw to monkey 3

Monkey 3:
  Starting items: 74
  Operation: new = old + 3
  Test: divisible by 17
    If true: throw to monkey 0
    If false: throw to monkey 1");

    return;
}

var input = System.IO.File.ReadAllText("../../inputs/day11/input");
var keepAway = new KeepAway();
keepAway.Process(input, 20);

var staticsSortedByCounter = keepAway.Monkeys.OrderByDescending(stat => stat.InspectionCounter);
var top2 = staticsSortedByCounter.Take(2).ToArray();
var monkeyBusinessLevel = top2[0].InspectionCounter * top2[1].InspectionCounter;

System.Console.WriteLine($"Result: {monkeyBusinessLevel}");

void Test(string input)
{
    var keepAway = new KeepAway();
    keepAway.Process(input, 20);

    var staticsSortedByCounter = keepAway.Monkeys.OrderByDescending(stat => stat.InspectionCounter);
    foreach(var stat in staticsSortedByCounter)
    {
        System.Console.WriteLine($"{stat.Id} inspected items {stat.InspectionCounter} times.");
    }

    var top2 = staticsSortedByCounter.Take(2).ToArray();

    var monkeyBusinessLevel = top2[0].InspectionCounter * top2[1].InspectionCounter;
    if (monkeyBusinessLevel != 10605)
        throw new Exception($"Expected 10605, got {monkeyBusinessLevel}");

    
    Console.WriteLine("Success");
}


class KeepAway
{
    public void Process(string input, int roundCount)
    {
        Parse(input);
        for(int round = 0; round < roundCount; round++)
        {
            System.Console.WriteLine($"Round #: {round + 1}");
            foreach(var monkey in Monkeys)
            {
                RunTurn(monkey);
            }

            System.Console.WriteLine();

            foreach(var monkey in Monkeys)
            {
                System.Console.WriteLine($"Monkey {monkey.Id}: {string.Join(", ", monkey.StressItems)}");
            }

            System.Console.WriteLine();
        }
    }

/*
Monkey inspects an item with a worry level of 79.
    Worry level is multiplied by 19 to 1501.
    Monkey gets bored with item. Worry level is divided by 3 to 500.
    Current worry level is not divisible by 23.
    Item with worry level 500 is thrown to monkey 3.
  Monkey inspects an item with a worry level of 98.
*/
    void RunTurn(Monkey monkey)
    {
        foreach(var item in monkey.StressItems)
        {
            monkey.InspectionCounter++;

            var worryLevel = (int) monkey.Operation.Invoke(item) / 3;
            var targetMonkey = worryLevel % monkey.Action.Divisor == 0 ? monkey.Action.TrueId : monkey.Action.FalseId;
            Monkeys[targetMonkey].StressItems.Add(worryLevel);
            System.Console.WriteLine($"Monkey:{monkey.Id} ({monkey.InspectionCounter}), Item ({item} : {worryLevel}) -> {targetMonkey}");
        }

        monkey.StressItems.Clear();
    }

/*
Monkey 1:
  Starting items: 54, 65, 75, 74
  Operation: new = old + 6
  Test: divisible by 19
    If true: throw to monkey 2
    If false: throw to monkey 0*/
    private void Parse(string input)
    {
        var monkeyUnparsed = input.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
        foreach(var mi in monkeyUnparsed)
        {
            var lines = mi.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            
            var id = Int32.Parse(ParseSingle(lines[0], @"^Monkey (\d+):$"));
            var items = ParseSingle(lines[1], @"^\s+Starting items:\s(.+)$").Split(',', StringSplitOptions.RemoveEmptyEntries).Select(item => Int32.Parse(item)).ToList();
            var operation = ParseOperation(lines[2]);
            var test = Int32.Parse(ParseSingle(lines[3], @"\s+Test: divisible by (\d+)"));
            var trueTarget = Int32.Parse(ParseSingle(lines[4], @"\s+If true: throw to monkey (\d+)"));
            var falseTarget = Int32.Parse(ParseSingle(lines[5], @"\s+If false: throw to monkey (\d+)"));

            Monkeys.Add(new Monkey(id, items, operation, new DecisionInfo(test, trueTarget, falseTarget)) { InspectionCounter = 0 } );
        }
    }

    string ParseSingle(string value, string regex)
    {
        var match = Regex.Match(value, regex);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }
        
        throw new Exception($"{value}! NO MATCH");        
    }

    Func<int, int> ParseOperation(string input)
    {
        var x = Regex.Match(input, @"\s+Operation: new = old (?<operator>\+|\*|-|/) (?<operand>.+)$");
        if (!x.Success)
            throw new Exception($"Invalid operation: {input}");
        

        if (x.Groups[2].Value =="old")
        {
            return x.Groups[1].Value switch
            {
                "*" => (int old) => old * old,
                "-" => (int old) => 0,
                "+" => (int old) => old + old,
                "/" => (int old) => 1,
            };
        }

        var value = Int32.Parse(x.Groups[2].Value);
        return x.Groups[1].Value switch
        {
            "*" => (int old) => old * value,
            "-" => (int old) => old - value,
            "+" => (int old) => old + value,
            "/" => (int old) => old / value,
        };
    }

    public IList<Monkey> Monkeys  { get; } = new List<Monkey>();
}

record Monkey(int Id, IList<int> StressItems, Func<int, int> Operation, DecisionInfo Action)
{
    public int InspectionCounter = 0;
}

record DecisionInfo(int Divisor, int TrueId, int FalseId);

record struct InspectionStatistic(int Id, int Counter);