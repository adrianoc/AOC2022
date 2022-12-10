using System;
using System.Linq;

if (args.Length == 1 && args[0] == "test")
{
    Test(@"addx 15
addx -11
addx 6
addx -3
addx 5
addx -1
addx -8
addx 13
addx 4
noop
addx -1
addx 5
addx -1
addx 5
addx -1
addx 5
addx -1
addx 5
addx -1
addx -35
addx 1
addx 24
addx -19
addx 1
addx 16
addx -11
noop
noop
addx 21
addx -15
noop
noop
addx -3
addx 9
addx 1
addx -3
addx 8
addx 1
addx 5
noop
noop
noop
noop
noop
addx -36
noop
addx 1
addx 7
noop
noop
noop
addx 2
addx 6
noop
noop
noop
noop
noop
addx 1
noop
noop
addx 7
addx 1
noop
addx -13
addx 13
addx 7
noop
addx 1
addx -33
noop
noop
noop
addx 2
noop
noop
noop
addx 8
noop
addx -1
addx 2
addx 1
noop
addx 17
addx -9
addx 1
addx 1
addx -3
addx 11
noop
noop
addx 1
noop
addx 1
noop
noop
addx -13
addx -19
addx 1
addx 3
addx 26
addx -30
addx 12
addx -1
addx 3
addx 1
noop
noop
noop
addx -9
addx 18
addx 1
addx 2
noop
noop
addx 9
noop
noop
noop
addx -1
addx 2
addx -37
addx 1
addx 3
noop
addx 15
addx -21
addx 22
addx -6
addx 1
noop
addx 2
addx 1
noop
addx -10
noop
noop
addx 20
addx 1
addx 2
addx 2
addx -6
addx -11
noop
noop
noop", 13140);

    return;
}

var input = System.IO.File.ReadAllLines("../../inputs/day10/input");
var videoDevice = new VideoDevice(input);
var signal = CalculateSignalFor(videoDevice);

System.Console.WriteLine($"Result = {signal}");

int CalculateSignalFor(VideoDevice videoDevice)
{
    var signal = 0;    
    foreach(uint cycles in new[] {20, 40, 40, 40, 40, 40})
    {
        videoDevice.Execute(cycles);
        System.Console.WriteLine($"[{videoDevice.Cycle}] PC = {videoDevice.CurrentInstruction}, X = {videoDevice.RegisterX}, Signal = {videoDevice.RegisterX * videoDevice.Cycle}");
        signal += videoDevice.RegisterX * videoDevice.Cycle;
    }

    return signal;
}

void Test(string instructions, int expected)
{
    var videoSystem = new VideoDevice(instructions.Split('\n', StringSplitOptions.RemoveEmptyEntries));
    
    var signal = CalculateSignalFor(videoSystem);
    if (signal != expected)
        throw new Exception($"Expected: {expected}, Got: {signal}");
    
    System.Console.WriteLine($"Success");
}

class VideoDevice
{
    public VideoDevice(IList<string> instructions, bool debug = false)
    {
        this.instructions = Parse(instructions);
        Cycle = 0;
        Debug = debug;
    }

    public void Execute(uint cyclesCount)
    {
        for (int i = 0; i < cyclesCount; i++)
        {
            Cycle++;
            update?.Invoke();
            update = null;

            var previousPC = PC;
            if (instructions[PC].Clock(this))
                PC++;
            
            if (Debug)
                System.Console.WriteLine($"[{Cycle,3}] X = {RegisterX, 4}, PC = {previousPC} : {instructions[previousPC].ToString()} ");
        }
    }

    public void RegisterUpdate(Action a) => update = a;

    private Action update;

    IList<Instruction> Parse(IEnumerable<string> instructions) => instructions.Where(line => !string.IsNullOrEmpty(line)).Select(InstructionFactory.FromSource).ToList();

    public int RegisterX { get; set; } = 1;
    public int Cycle { get; set; }
    int PC {get; set; } = 0;
    bool Debug { get; init; }
    public string CurrentInstruction => instructions[PC].ToString();
    private IList<Instruction> instructions;


    struct InstructionFactory
    {
        internal static Instruction FromSource(string code)
        {
            var instructionParts = code.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return instructionParts[0] switch
            {
                "noop" => new Noop(),
                "addx" => new AddX(Int32.TryParse(instructionParts[1], out var value) ? value : throw new Exception()),

                _ => throw new Exception($"Invalid instruction : {code}")
            };
        }
    }
}

abstract class Instruction
{
    public string OpCode { get; init; }
    public byte Cycles { get; set; }

    public override string ToString() => OpCode;

    public abstract bool Clock(VideoDevice device);
}

class  Noop : Instruction
{
    public Noop()
    {
        OpCode = "noop";
        Cycles = 1;
    }

    // Takes only one cycle
    public override bool Clock(VideoDevice device) => true;
}

class  AddX : Instruction
{
    public AddX(int operand)
    {
        OpCode = "addx";
        Cycles = 2;
        Operand = operand;
    }

    public int Operand { get; init; }

    public override string ToString() => $"{OpCode} {Operand}";

    public override bool Clock(VideoDevice device)
    {
        if (--Cycles == 0)
            device.RegisterUpdate(() => device.RegisterX += Operand);

        return Cycles == 0;
    }
}

