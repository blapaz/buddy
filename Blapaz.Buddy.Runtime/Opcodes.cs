namespace Blapaz.Buddy.Runtime
{
    class Opcodes
    {
        public static readonly int pushInt = 0;
        public static readonly int pushString = 1;
        public static readonly int pushVar = 2;  
        public static readonly int pop = 3;
        public static readonly int popa = 4;
        public static readonly int decVar = 5;
        public static readonly int setVar = 6;
        public static readonly int add = 7;
        public static readonly int sub = 8;
        public static readonly int mul = 9;
        public static readonly int div = 10;
        public static readonly int clear = 11;
        public static readonly int ife = 12;
        public static readonly int ifn = 13;
        public static readonly int elseife = 14;
        public static readonly int elseifn = 15;
        public static readonly int els = 16;
        public static readonly int endif = 17;
        public static readonly int call = 18;
        public static readonly int got = 19;
        public static readonly int ret = 20;

        public static readonly int setGlobalVar = 21;
        public static readonly int ifgt = 22;
        public static readonly int ifgte = 23;
        public static readonly int iflt = 24;
        public static readonly int iflte = 25;
        public static readonly int elseifgt = 26;
        public static readonly int elseifgte = 27;
        public static readonly int elseiflt = 28;
        public static readonly int elseiflte = 29;

        // System functions
        public static readonly int print = 200;
        public static readonly int printLine = 201;
        public static readonly int read = 202;
        public static readonly int readLine = 203;
        public static readonly int inputInt32 = 204;
        public static readonly int inputString = 205;
        public static readonly int captureScreen = 206;
        public static readonly int getClipboard = 207;
        public static readonly int setClipboard = 208;
        public static readonly int delay = 209;
        public static readonly int write = 210;
    }
}
