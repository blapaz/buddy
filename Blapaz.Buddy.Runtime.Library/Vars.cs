namespace Blapaz.Buddy.Runtime.Library
{
    class Var
    {
        public string name;
        public object value;
        public int scope = 1;

        public Var(string n)
        {
            name = n;
            value = null;
        }

        public Var(string n, string v)
        {
            name = n;
            value = v;
        }
    }
}
