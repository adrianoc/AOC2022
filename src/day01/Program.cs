var input = System.IO.File.ReadAllText("inputs/day01/input");

var perElf = input
                .Split("\n\n")
                .Select(elfCalories => elfCalories.Split("\n")
                    .Select(cal => Int64.Parse(cal))
                    .Sum()
                );

var topThree = perElf.Select((elfCalories, elfIndex) => new ElfCalories(elfIndex, elfCalories)).OrderByDescending(elfData => elfData.Calories).Take(3);

foreach(var item in topThree)
    System.Console.WriteLine($"{item.Index}:{item.Calories}");

var total = topThree.Sum(elf => elf.Calories);
System.Console.WriteLine($"\nTotal: {total}");

record struct ElfCalories (int Index, long Calories);