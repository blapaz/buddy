using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Blapaz.Buddy.Runtime.Utilities;
using WindowsInput;

namespace Blapaz.Buddy.Runtime
{
    class Runtime
    {
        public static List<Func> Events { get; private set; } = new List<Func>();

        private static List<Func> _funcs = new List<Func>();
        private static List<Block> _blocks = new List<Block>();
        private static List<Var> _globalVars = new List<Var>();
        private static List<Var> _vars = new List<Var>();
        private static Stack<object> _stack = new Stack<object>();
        private static Buffer _code = new Buffer();
        private static bool _isRunning = true;
        private static Func _currentFunc = null;
        private static Stack<Call> _callstack = new Stack<Call>();
        private static bool _ifWorked = false;
        private static InputSimulator _inputSimulator = new InputSimulator();

        public Runtime(string input)
        {
            Lexer lexer = new Lexer(input);
            Events = lexer.Events;
            _funcs = lexer.Funcs;
            _globalVars = lexer.GlobalVars;
            _blocks = lexer.Blocks;
            _code = lexer.Code;
            
            Run("Main");
        }

        public static void Run(string funcName)
        {
            Func func = GetFunc(funcName);

            if (func != null)
            {
                _isRunning = true;
                _code.pos = func.location;
                _currentFunc = func;

                int opcode = 0;
                Block currentBlock = null;
                Stack<Block> block_stack = new Stack<Block>();

                while (_isRunning)
                {
                    try
                    {
                        opcode = _code.ReadInt();
                    }
                    catch
                    {
                    }

                    if (opcode == Opcodes.pushInt)
                    {
                        _stack.Push(_code.ReadInt());
                    }
                    else if (opcode == Opcodes.pushString)
                    {
                        _stack.Push(_code.ReadString());
                    }
                    else if (opcode == Opcodes.pushVar)
                    {
                        _stack.Push(GetVarValue(_code.ReadString()));
                    }
                    else if (opcode == Opcodes.pop)
                    {
                        _stack.Pop();
                    }
                    else if (opcode == Opcodes.popa)
                    {
                        _stack.Clear();
                    }
                    else if (opcode == Opcodes.decVar)
                    {
                        _vars.Add(new Var(_code.ReadString()));
                    }
                    else if (opcode == Opcodes.setVar)
                    {
                        SetVarValue(_code.ReadString(), _stack.Pop());
                    }
                    else if (opcode == Opcodes.add)
                    {
                        object value1 = _stack.Pop();
                        object value2 = _stack.Pop();

                        if (value1 is string && value2 is string)
                        {
                            string value = ((string) value2) + ((string) value1);
                            _stack.Push(value);
                        }
                        else if (value1 is int && value2 is int)
                        {
                            int value = ((int) value1) + ((int) value2);
                            _stack.Push(value);
                        }
                    }
                    else if (opcode == Opcodes.sub)
                    {
                        int value1 = (int) _stack.Pop();
                        int value2 = (int) _stack.Pop();
                        _stack.Push(value1 + value2);
                    }
                    else if (opcode == Opcodes.mul)
                    {
                        int value1 = (int) _stack.Pop();
                        int value2 = (int) _stack.Pop();
                        _stack.Push(value1 * value2);
                    }
                    else if (opcode == Opcodes.div)
                    {
                        int value1 = (int) _stack.Pop();
                        int value2 = (int) _stack.Pop();
                        _stack.Push(value1 / value2);
                    }
                    else if (opcode == Opcodes.clear)
                    {
                        Console.Clear();
                    }
                    else if (opcode == Opcodes.ife)
                    {
                        int blockNumber = _code.ReadInt();
                        IfBlock ifblock = GetIf(blockNumber);

                        object value1 = _stack.Pop();
                        object value2 = _stack.Pop();

                        if (IfEqual(value1, value2))
                        {
                            if (currentBlock == null)
                            {
                                currentBlock = ifblock;
                            }
                            else
                            {
                                block_stack.Push(currentBlock);
                                currentBlock = ifblock;
                            }

                            IncVars();
                            _ifWorked = true;
                        }
                        else
                        {
                            _code.pos = ifblock.endBlock;
                            _ifWorked = false;
                        }
                    }
                    else if (opcode == Opcodes.ifn)
                    {
                        int blockNumber = _code.ReadInt();
                        IfBlock ifblock = GetIf(blockNumber);

                        object value1 = _stack.Pop();
                        object value2 = _stack.Pop();

                        if (!IfEqual(value1, value2))
                        {
                            if (currentBlock == null)
                            {
                                currentBlock = ifblock;
                            }
                            else
                            {
                                block_stack.Push(currentBlock);
                                currentBlock = ifblock;
                            }

                            IncVars();
                            _ifWorked = true;
                        }
                        else
                        {
                            _code.pos = ifblock.endBlock;
                            _ifWorked = false;
                        }
                    }
                    else if (opcode == Opcodes.ifgt)
                    {
                        int blockNumber = _code.ReadInt();
                        IfBlock ifblock = GetIf(blockNumber);

                        object value1 = _stack.Pop();
                        object value2 = _stack.Pop();

                        if (IfGreater(value1, value2))
                        {
                            if (currentBlock == null)
                            {
                                currentBlock = ifblock;
                            }
                            else
                            {
                                block_stack.Push(currentBlock);
                                currentBlock = ifblock;
                            }

                            IncVars();
                            _ifWorked = true;
                        }
                        else
                        {
                            _code.pos = ifblock.endBlock;
                            _ifWorked = false;
                        }
                    }
                    else if (opcode == Opcodes.ifgte)
                    {
                        int blockNumber = _code.ReadInt();
                        IfBlock ifblock = GetIf(blockNumber);

                        object value1 = _stack.Pop();
                        object value2 = _stack.Pop();

                        if (IfGreater(value1, value2) || IfEqual(value1, value2))
                        {
                            if (currentBlock == null)
                            {
                                currentBlock = ifblock;
                            }
                            else
                            {
                                block_stack.Push(currentBlock);
                                currentBlock = ifblock;
                            }

                            IncVars();
                            _ifWorked = true;
                        }
                        else
                        {
                            _code.pos = ifblock.endBlock;
                            _ifWorked = false;
                        }
                    }
                    else if (opcode == Opcodes.iflt)
                    {
                        int blockNumber = _code.ReadInt();
                        IfBlock ifblock = GetIf(blockNumber);

                        object value1 = _stack.Pop();
                        object value2 = _stack.Pop();

                        if (IfLesser(value1, value2))
                        {
                            if (currentBlock == null)
                            {
                                currentBlock = ifblock;
                            }
                            else
                            {
                                block_stack.Push(currentBlock);
                                currentBlock = ifblock;
                            }

                            IncVars();
                            _ifWorked = true;
                        }
                        else
                        {
                            _code.pos = ifblock.endBlock;
                            _ifWorked = false;
                        }
                    }
                    else if (opcode == Opcodes.iflte)
                    {
                        int blockNumber = _code.ReadInt();
                        IfBlock ifblock = GetIf(blockNumber);

                        object value1 = _stack.Pop();
                        object value2 = _stack.Pop();

                        if (IfLesser(value1, value2) || IfEqual(value1, value2))
                        {
                            if (currentBlock == null)
                            {
                                currentBlock = ifblock;
                            }
                            else
                            {
                                block_stack.Push(currentBlock);
                                currentBlock = ifblock;
                            }

                            IncVars();
                            _ifWorked = true;
                        }
                        else
                        {
                            _code.pos = ifblock.endBlock;
                            _ifWorked = false;
                        }
                    }
                    else if (opcode == Opcodes.elseife)
                    {
                        int blockNumber = _code.ReadInt();
                        ElseIfBlock elseifblock = GetElseIf(blockNumber);

                        if (!_ifWorked)
                        {
                            object value1 = _stack.Pop();
                            object value2 = _stack.Pop();

                            if (IfEqual(value1, value2))
                            {
                                if (currentBlock == null)
                                {
                                    currentBlock = elseifblock;
                                }
                                else
                                {
                                    block_stack.Push(currentBlock);
                                    currentBlock = elseifblock;
                                }

                                IncVars();
                                _ifWorked = true;
                            }
                            else
                            {
                                _code.pos = elseifblock.endBlock;
                                _ifWorked = false;
                            }
                        }
                        else
                        {
                            _code.pos = elseifblock.endBlock;
                        }
                    }
                    else if (opcode == Opcodes.elseifn)
                    {
                        int blockNumber = _code.ReadInt();
                        ElseIfBlock elseifblock = GetElseIf(blockNumber);

                        if (!_ifWorked)
                        {
                            object value1 = _stack.Pop();
                            object value2 = _stack.Pop();

                            if (!IfEqual(value1, value2))
                            {
                                if (currentBlock == null)
                                {
                                    currentBlock = elseifblock;
                                }
                                else
                                {
                                    block_stack.Push(currentBlock);
                                    currentBlock = elseifblock;
                                }

                                IncVars();
                                _ifWorked = true;
                            }
                            else
                            {
                                _code.pos = elseifblock.endBlock;
                                _ifWorked = false;
                            }
                        }
                        else
                        {
                            _code.pos = elseifblock.endBlock;
                        }
                    }
                    else if (opcode == Opcodes.els)
                    {
                        int blockNumber = _code.ReadInt();
                        ElseBlock elseblock = GetElse(blockNumber);

                        if (!_ifWorked)
                        {
                            if (currentBlock == null)
                            {
                                currentBlock = elseblock;
                            }
                            else
                            {
                                block_stack.Push(currentBlock);
                                currentBlock = elseblock;
                            }

                            IncVars();
                        }
                        else
                        {
                            _code.pos = elseblock.endBlock;
                        }
                    }
                    else if (opcode == Opcodes.endif)
                    {
                        if (block_stack.Count > 0)
                        {
                            currentBlock = block_stack.Pop();
                        }
                        else
                        {
                            currentBlock = null;
                        }

                        DecVars();
                    }
                    else if (opcode == Opcodes.call)
                    {
                        string name = _code.ReadString();
                        Func f = GetFunc(name);
                        Call c = new Call(_currentFunc, _code.pos, new List<Var>(_vars));
                        _callstack.Push(c);
                        _currentFunc = f;
                        _code.pos = f.location;
                        _vars.Clear();
                    }
                    else if (opcode == Opcodes.got)
                    {
                        string name = _code.ReadString();
                        int location = GetLabel(name);
                        _code.pos = location;
                    }
                    else if (opcode == Opcodes.ret)
                    {
                        if (_callstack.Count > 0)
                        {
                            Call c = _callstack.Pop();
                            _currentFunc = c.func;
                            _code.pos = c.ret;
                            _vars = c.vars;
                        }
                        else
                        {
                            _isRunning = false;
                        }
                    }
                    else if (opcode == Opcodes.print)
                    {
                        Console.WriteLine(_stack.Pop());
                    }
                    else if (opcode == Opcodes.printLine)
                    {
                        Console.WriteLine(_stack.Pop());
                    }
                    else if (opcode == Opcodes.read)
                    {
                        Console.Read();
                    }
                    else if (opcode == Opcodes.readLine)
                    {
                        Console.ReadLine();
                    }
                    else if (opcode == Opcodes.inputInt32)
                    {
                        _stack.Push(Convert.ToInt32(Console.ReadLine()));
                    }
                    else if (opcode == Opcodes.inputString)
                    {
                        _stack.Push(Console.ReadLine());
                    }
                    else if (opcode == Opcodes.delay)
                    {
                        System.Threading.Thread.Sleep(Convert.ToInt32(_stack.Pop()));
                    }
                    else if (opcode == Opcodes.captureScreen)
                    {
                        string value = _stack.Pop().ToString();

                        if (value.Equals(""))
                        {
                            value = Guid.NewGuid().ToString();
                        }

                        Screenshot.CaptureAndOpen($"{value}.jpg", ImageFormat.Jpeg);
                    }
                    else if (opcode == Opcodes.getClipboard)
                    {
                        _stack.Push(Clipboard.GetText());
                    }
                    else if (opcode == Opcodes.setClipboard)
                    {
                        Clipboard.SetText(_stack.Pop().ToString());
                    }
                    else if (opcode == Opcodes.write)
                    {
                        _inputSimulator.Keyboard.TextEntry(_stack.Pop().ToString());
                    }
                    else if (opcode == Opcodes.exit)
                    {
                        Environment.Exit(0);
                        Application.Exit();
                    }
                }
            }
            else
            {
                if (!funcName.Equals("Main"))
                {
                    throw new Exception($"No function named {funcName} was found");
                }
            }
        }

        static Func GetFunc(string name)
        {
            Func func = null;

            foreach (Func f in _funcs)
            {
                if (f.name == name)
                {
                    func = f;
                }
            }

            return func;
        }

        static int GetLabel(string name)
        {
            int location = 0;

            foreach (Label l in _currentFunc.labels)
            {
                if (l.name == name)
                {
                    location = l.location;
                }
            }

            return location;
        }

        static object GetVarValue(string name)
        {
            foreach (Var v in _vars)
            {
                if (v.name == name)
                {
                    return v.value;
                }
            }

            // Look for global var since no local var was found
            foreach (Var gvar in _globalVars)
            {
                if (gvar.name == name)
                {
                    return gvar.value;
                }
            }

            return "null";
        }

        static void SetVarValue(string name, object value)
        {
            bool found = false;

            foreach (Var v in _vars)
            {
                if (v.name == name)
                {
                    v.value = value;
                    found = true;
                }
            }

            if (!found)
            {
                Var v = new Var(name);
                v.value = value;
                _vars.Add(v);
            }
        }

        static IfBlock GetIf(int blockNumber)
        {
            IfBlock ifblock = null;

            foreach (Block b in _blocks)
            {
                if (b is IfBlock)
                {
                    IfBlock bl = (IfBlock)b;

                    if (bl.blockNumber == blockNumber)
                    {
                        ifblock = bl;
                    }
                }
            }

            return ifblock;
        }

        static ElseIfBlock GetElseIf(int blockNumber)
        {
            ElseIfBlock elseifblock = null;

            foreach (Block b in _blocks)
            {
                if (b is ElseIfBlock)
                {
                    ElseIfBlock bl = (ElseIfBlock)b;

                    if (bl.blockNumber == blockNumber)
                    {
                        elseifblock = bl;
                    }
                }
            }

            return elseifblock;
        }

        static ElseBlock GetElse(int blockNumber)
        {
            ElseBlock elseblock = null;

            foreach (Block b in _blocks)
            {
                if (b is ElseBlock)
                {
                    ElseBlock bl = (ElseBlock)b;

                    if (bl.blockNumber == blockNumber)
                    {
                        elseblock = bl;
                    }
                }
            }

            return elseblock;
        }

        static bool IfEqual(object value1, object value2)
        {
            if ((value1 is int && value2 is int) && ((int)value1 == (int)value2))
            {
                return true;
            }
            else if ((value1 is string && value2 is string) && (((string)value1).Length == ((string)value2).Length))
            {
                return true;
            }

            return false;
        }

        static bool IfGreater(object value1, object value2)
        {
            if ((value1 is int && value2 is int) && ((int)value1 > (int)value2))
            {
                return true;
            }
            else if ((value1 is string && value2 is string) && (((string)value1).Length > ((string)value2).Length))
            {
                return true;
            }

            return false;
        }

        static bool IfLesser(object value1, object value2)
        {
            if ((value1 is int && value2 is int) && ((int)value1 < (int)value2))
            {
                return true;
            }
            else if ((value1 is string && value2 is string) && (((string)value1).Length < ((string)value2).Length))
            {
                return true;
            }

            return false;
        }

        static void IncVars()
        {
            foreach (Var v in _vars)
            {
                v.scope++;
            }
        }

        static void DecVars()
        {
            foreach (Var v in _vars)
            {
                if (v.scope == 1)
                {
                    _vars.Remove(v);
                }
                else
                {
                    v.scope--;
                }
            }
        }
    }
}
