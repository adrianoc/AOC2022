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

    for(int i = 0; i < rucksacks.Length; i += 3)
    {
        prioritySum += PriorityForGroup(rucksacks[i], rucksacks[i+1], rucksacks[i+2]);
    }

    return prioritySum;
}

int PriorityForGroup(string elf1Items, string elf2Items, string elf3Items)
{
    for(int i = 0; i < elf1Items.Length; i++)
    {
        for(int j = 0; j < elf2Items.Length; j++)
        {
            if (elf1Items[i] == elf2Items[j])
            {
                for(int k = 0; k < elf3Items.Length; k++)
                {
                    if (elf3Items[k] == elf2Items[j])
                    {
                        return Char.IsLower(elf1Items[i]) 
                                    ? (elf1Items[i] - 'a' + 1) 
                                    : (elf1Items[i] - 'A' + 27);
                    }
                }
            }
        }
    }

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
    if (70 != computedPrioritySum)
    {
        throw new Exception($"Expected sum = 70, got {computedPrioritySum}");
    }

    System.Console.WriteLine("Success.");
}