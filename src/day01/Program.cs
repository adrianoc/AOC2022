var input = System.IO.File.ReadAllText("inputs/day01/input");

var perElf = input
                .Split("\n\n")
                .Select(elfCalories => elfCalories.Split("\n")
                    .Select(cal => Int64.Parse(cal))
                    .Sum()
                );

var found = perElf.Select((elfCalories, elfIndex) => new ElfCalories(elfIndex, elfCalories)).MaxBy(elfData => elfData.Calories);
System.Console.WriteLine($"{found.Index} = {found.Calories}");

if (args.Length == 2)
{
    var i = 0;
    foreach(var elfCalories in perElf)
        System.Console.WriteLine($"{i++,3}:{elfCalories}");
}

record struct ElfCalories (int Index, long Calories);