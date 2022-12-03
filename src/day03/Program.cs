using System.Threading.Tasks;

if (args.Length == 1 && args[0] == "test")
{
    Test();
    return;
}

var input = System.IO.File.ReadAllLines("../../inputs/day03/input");
System.Console.WriteLine($"# rucksacks: {input.Length}");
System.Console.WriteLine($"Priority Sum: {ComputePrioritySum(input)}");

int ComputePrioritySum(string []rucksacks)
{
    int prioritySum = 0;

    Parallel.ForEach(
        rucksacks,
        rucksack => Interlocked.Add(ref prioritySum, PriorityForRucksack(rucksack)));

    return prioritySum;
}

int PriorityForRucksack(string rucksack)
{
    var rs = rucksack.AsSpan();
    if (rs.Length % 2 != 0)
    {
        throw new ArgumentException($"Rucksack has an odd # of items ({rucksack.Length}).", nameof(rucksack));
    }

    var firstCompartment = rs.Slice(0, rucksack.Length / 2);
    var secondCompartment = rs.Slice(rucksack.Length / 2);

    for(int i = 0; i < firstCompartment.Length; i++)
    {
        for(int j = 0; j < secondCompartment.Length; j++)
        {
            if (firstCompartment[i] == secondCompartment[j])
            {
                return Char.IsLower(firstCompartment[i]) 
                            ? (firstCompartment[i] - 'a' +1) 
                            : (firstCompartment[i] - 'A' + 27);
            }
        }
    }

#if VALIDATE_VALUES

    throw new Exception("No common items found.");
#endif

    return 0;
}

void Test()
{
    var input = new [] 
    {
        "vJrwpWtwJgWrhcsFMMfFFhFp",
        "jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL",
        "PmmdzqPrVvPwwTWBwg",
        "wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn",
        "ttgJtRGJQctTZtZT",
        "CrZsJsPPZsGzwwsLwLmpwMDw",
    };

    var computedPrioritySum = ComputePrioritySum(input);
    if (157 != computedPrioritySum)
    {
        throw new Exception($"Expected sum = 157, got {computedPrioritySum}");
    }

    System.Console.WriteLine("Success.");
}