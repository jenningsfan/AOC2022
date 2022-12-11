using System.Text.Json;
using System.Text.Json.Serialization;

namespace AdventOfCode;

public class Monkey
{
    public Monkey(int id, long[] startingItems, string operation, int test, int ifTrue, int ifFalse, Monkey[] monkeys)
    {
        Id = id;
        Items = startingItems.ToList();
        OldItems = new List<long>();
        OldItems.AddRange(Items);
        Monkeys = monkeys;
        Operation = operation;
        this.test = test;
        this.ifTrue = ifTrue;
        this.ifFalse = ifFalse;
    }

    public void Inspect(int worryDivider, int worryModulo)
    {
        OldItems.Clear();
        OldItems.AddRange(Items);

        for (int i = 0; i < OldItems.Count; i++) {
            string[] operation = Operation.Split(" ");
            long currentItem = OldItems[i];

            switch (operation[1])
            {
                case "+":
                    if (operation[2] == "old")
                    {
                        OldItems[i] += OldItems[i];
                    }
                    else
                    {
                        OldItems[i] += Convert.ToInt64(operation[2]);
                    }                        
                    break;
                case "-":
                    if (operation[2] == "old")
                    {
                        OldItems[i] -= OldItems[i];
                    }
                    else
                    {
                        OldItems[i] -= Convert.ToInt64(operation[2]);
                    }
                    break;
                case "*":
                    if (operation[2] == "old")
                    {
                        OldItems[i] *= OldItems[i];
                    }
                    else
                    {
                        OldItems[i] *= Convert.ToInt64(operation[2]);
                    }
                    break;
                case "/":
                    if (operation[2] == "old")
                    {
                        OldItems[i] /= OldItems[i];
                    }
                    else
                    {
                        OldItems[i] /= Convert.ToInt64(operation[2]);
                    }
                    break;
            }

            OldItems[i] /= worryDivider;
            OldItems[i] %= worryModulo;

            if (OldItems[i] % test == 0)
            {
                Monkeys[ifTrue].Items.Add(OldItems[i]);
                Items.Remove(currentItem);
            }
            else
            {
                Monkeys[ifFalse].Items.Add(OldItems[i]);
                Items.Remove(currentItem);
            }
        }

        OldItems.Clear();
        OldItems.AddRange(Items);
    }

    public Monkey Clone()
    {
        // Create a JsonSerializerOptions instance with the ReferenceHandler.Preserve option
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve
        };

        // Serialize the object to JSON using the JsonSerializerOptions with the ReferenceHandler.Preserve option
        string json = JsonSerializer.Serialize(this, this.GetType(), options);

        // Deserialize the JSON using the JsonSerializerOptions with the ReferenceHandler.Preserve option
        return JsonSerializer.Deserialize<Monkey>(json, options);
    }

    public int Id { get; set; }
    public List<long> Items { get; set; }
    public List<long> OldItems { get; set; }
    public Monkey[] Monkeys { get; set; }
    public string Operation { get; set; }
    public int test { get; set; }
    public int ifTrue { get; set; }
    public int ifFalse { get; set; }
}

public class Day11 : BaseDay
{
    private readonly List<Monkey> _input;

    public Day11()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        List<Monkey> monkeys = new List<Monkey>();
        for (int i = 0; i < _input.Count / 2; i++)
        {
            monkeys.Add(_input[i]);
        }

        int[] inspections = new int[monkeys.Count];

        for (int j = 0; j < 20; j++)
        {
            for (int i = 0; i < monkeys.Count; i++)
            {
                inspections[i] += monkeys[i].Items.Count;
                monkeys[i].Inspect(3, 1);               
            }     
        }

        inspections = inspections.OrderByDescending(c => c).ToArray();
        return new((inspections[0] * inspections[1]).ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        List<Monkey> monkeys = new List<Monkey>();
        for (int i = _input.Count / 2; i < _input.Count; i++)
        {
            monkeys.Add(_input[i]);
        }

        int worryModulo = monkeys.Aggregate(1, (x, y) => x * y.test);

        long[] inspections = new long[monkeys.Count];

        for (int j = 1; j < 10001; j++)
        {
            for (int i = 0; i < monkeys.Count; i++)
            {
                inspections[i] += monkeys[i].Items.Count;
                monkeys[i].Inspect(1, worryModulo);               
            }
        }

        inspections = inspections.OrderByDescending(c => c).ToArray();
        return new((inspections[0] * inspections[1]).ToString());
    }

    private void PrintInspections(int round, long[] inspections)
    {
        Console.WriteLine($"== Round {round} ==");
        for (int i = 0; i < inspections.Length; i++)
        {
            Console.WriteLine($"Monkey {i} inspected items {inspections[i]} times.");
        }
    }

    private List<Monkey> ParseInput()
    {
        string[] input = File.ReadAllText(InputFilePath).Split("\n");
        List<Monkey> monkeys = new();

        for (int j = 0; j < 2; j++)
        {
            for (int i = 0; i < input.Length; i += 7)
            {
                string[] monkey = input[i..(i + 6)];
                int id = Convert.ToInt32(monkey[0].Split(" ")[1].Replace(":", ""));
                long[] items = Array.ConvertAll(monkey[1][18..].Split(","), long.Parse);
                string operation = monkey[2][19..];
                int test = Convert.ToInt32(monkey[3][21..]);
                int ifTrue = Convert.ToInt32(monkey[4][29..]) + (((input.Length + 1) / 7) * j);
                int ifFalse = Convert.ToInt32(monkey[5][30..]) + (((input.Length + 1) / 7) * j);

                monkeys.Add(new Monkey(id, items, operation, test, ifTrue, ifFalse, null));
            }
        }       

        for (int i = 0; i < monkeys.Count; i++)
        {
            monkeys[i].Monkeys = monkeys.ToArray();
        }

        return monkeys;
    }
}
