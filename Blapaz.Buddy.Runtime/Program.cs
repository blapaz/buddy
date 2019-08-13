using System;
using System.Collections.Generic;
using Gma.System.MouseKeyHook;

namespace Blapaz.Buddy.Runtime
{
    public class Program
    {
        public static void Run(string compiledCode)
        {
            Runtime runtime = new Runtime(compiledCode);

            foreach (Func evnt in Runtime.Events)
            {
                string combination = evnt.name.Replace("_", "+");

                Hook.GlobalEvents().OnCombination(new Dictionary<Combination, Action>
                {
                    {Combination.FromString(combination), () => { Runtime.Run($"{combination.Replace("+", "_")}"); }}
                });
            }
        }
    }
}
