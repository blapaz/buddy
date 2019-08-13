using System;
using System.Collections.Generic;
using System.Linq;
using Gma.System.MouseKeyHook;

namespace Blapaz.Buddy.Runtime
{
    public class Program
    {
        public static void Run(string compiledCode)
        {
            Runtime runtime = new Runtime(compiledCode);

            foreach (string e in Runtime.Funcs.Where(x => x.name.StartsWith("e_")).Select(x => x.name).ToList())
            {
                string combination = e.Substring(2, e.Length - 2).Replace("_", "+");

                Hook.GlobalEvents().OnCombination(new Dictionary<Combination, Action>
                {
                    {Combination.FromString("Control+C"), () => { Console.WriteLine("Clipboard"); }},
                    {Combination.FromString(combination), () => { Runtime.Run($"e_{combination.Replace("+", "_")}"); }}
                });
            }
        }
    }
}
