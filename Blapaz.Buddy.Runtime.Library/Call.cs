using System.Collections.Generic;

namespace Blapaz.Buddy.Runtime.Library
{
    class Call
    {
        public Func func;
        public int ret;
        public List<Var> vars;

        public Call(Func f, int loc, List<Var> v)
        {
            func = f;
            ret = loc;
            vars = v;
        }
    }
}
