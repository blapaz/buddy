using System;
using System.Diagnostics;
using System.IO;
using BuddyCompilerLibrary = Blapaz.Buddy.Compiler.Library.Program;

namespace Blapaz.Buddy.Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No script file not found");
            }
            else
            {
                string scriptFile = args[0];

                if (File.Exists(scriptFile))
                {
                    if (Path.GetExtension(scriptFile).Equals(".bud"))
                    {
                        Stopwatch watch = new Stopwatch();
                        watch.Start();

                        string compiledCode = BuddyCompilerLibrary.Compile(scriptFile);

                        using (FileStream fs = new FileStream(Path.Combine(Path.GetDirectoryName(scriptFile), Path.GetFileNameWithoutExtension(scriptFile) + ".buddy"), FileMode.Create))
                        using (BinaryWriter bw = new BinaryWriter(fs))
                        {
                            bw.Write(compiledCode);
                        }

                        watch.Stop();

                        Console.WriteLine($"'{Path.GetFileName(scriptFile)}' compiled in {watch.ElapsedMilliseconds} ms");
                    }
                    else if (Path.GetExtension(scriptFile).Equals(".buddy"))
                    {
                        Console.WriteLine($"{Path.GetFileName(scriptFile)} is already compiled");
                    }
                    else
                    {
                        Console.WriteLine("Only files with the *.bud extension are compilable");
                    }
                }
                else
                {
                    Console.WriteLine("File path specified is invalid");
                }
            }
        }
    }
}
