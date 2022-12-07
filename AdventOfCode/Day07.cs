using System.Text;

namespace AdventOfCode;

public class Directory
{
    public Directory(string name, int size, Directory parent, List<Directory> subdirs)
    {
        Name = name;
        Size = size;
        Parent = parent;
        Subdirs = subdirs;
    }

    public int TotalSize()
    {
        int totalSize = Size;
        foreach (Directory dir in Subdirs)
        {
            totalSize += dir.TotalSize();
        }

        return totalSize;
    }

    public void PrintDir(int currIndent)
    {
        Console.Write(new StringBuilder().Insert(0, "    ", currIndent).ToString());
        Console.Write(Name + " Size:");
        Console.WriteLine(Size);
        foreach (Directory dir in Subdirs)
        {
            dir.PrintDir(currIndent + 1);
        }
    }

    public string Name { get; set; }
    public int Size { get; set; }
    public List<Directory> Subdirs { get; set; }
    public Directory Parent { get; set; }
}

public class Day07 : BaseDay
{
    private readonly Directory _input;

    public Day07()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        int result = 0;
        //_input.PrintDir(0);
        foreach(Directory dir in SmallFiles(_input, new List<Directory>()))
        {
            result += dir.TotalSize();
        }

        return new(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        int result = _input.TotalSize();
        int neededSpace = 30000000 - (70000000 - _input.TotalSize());
        foreach (Directory dir in Files(_input, new List<Directory>()))
        {
            if (dir.TotalSize() < result && dir.TotalSize() >= neededSpace)
            {
                result = dir.TotalSize();
            }
        }

        return new(result.ToString());
    }

    private List<Directory> SmallFiles(Directory dir, List<Directory> result)
    {
        foreach (Directory directory in dir.Subdirs)
        {
            if (directory.TotalSize() <= 100000 && directory.Size == 0) { 
                result.Add(directory);
            }
            result.Concat(SmallFiles(directory, result));
        }
        return result;
    }

    private List<Directory> Files(Directory dir, List<Directory> result)
    {
        foreach (Directory directory in dir.Subdirs)
        {
            if (directory.Size == 0)
            {
                result.Add(directory);
            }
            result.Concat(Files(directory, result));
        }
        return result;
    }

    private Directory ParseInput()
    {
        string[] input = File.ReadAllText(InputFilePath).Replace("$ ", "").Split("\n");
        Directory root = new("/", 0, null, new List<Directory> {});
        root.Parent = root;
        Directory cd = root;

        for (int i = 0; i < input.Length; i++)
        {
            string line = input[i];

            if (line.StartsWith("cd"))
            {
                string newDir = line.Substring(3);

                if (newDir == "..")
                {
                    cd = cd.Parent;
                }

                foreach (var dir in cd.Subdirs)
                {
                    if (dir.Name == newDir)
                    {
                        cd = dir;
                        break;
                    }
                }
            }
            else if (line.StartsWith("ls")) { }
            else
            {
                string[] split = line.Split(" ");

                if (split[0] == "dir")
                {
                    cd.Subdirs.Add(new Directory(split[1], 0, cd, new List<Directory> { }));
                }
                else
                {
                    cd.Subdirs.Add(new Directory(split[1], Convert.ToInt32(split[0]), cd, new List<Directory> { }));
                }
            }
        }
        return root;
    }
}