using System.Collections.Generic;

namespace Blapaz.Buddy.Runtime.Library
{
    class Func
    {
        public string name;
        public int location;
        public List<Label> labels = new List<Label>();

        public Func(string n, int loc)
        {
            name = n;
            location = loc;
        }
    }

    class Label
    {
        public string name;
        public int location;

        public Label(string n, int loc)
        {
            name = n;
            location = loc;
        }
    }
}
