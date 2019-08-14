using System;
using System.Collections.Generic;
using System.Linq;

namespace Blapaz.Buddy.Runtime
{
    class Lexer
    {     
        public List<Func> Events { get; private set; } = new List<Func>();
        public List<Func> Funcs { get; private set; } = new List<Func>();
        public List<Block> Blocks { get; private set; } = new List<Block>();
        public Buffer Code { get; private set; } = new Buffer();

        public Lexer(string compiledCode)
        {
            Func currentFunc = null;
            Block currentBlock = null;
            int blockNumber = 0;
            Stack<Block> blockstack = new Stack<Block>();

            foreach (string line in compiledCode.Replace(((char)13).ToString(), "").Split('\n'))
            {
                if (line.StartsWith(":"))
                {
                    string op = line.Substring(1);

                    if (currentFunc == null)
                    {
                        currentFunc = new Func(op, Code.buffer.Count);
                    }
                    else
                    {
                        Code.Write(Opcodes.ret);
                        Funcs.Add(currentFunc);
                        currentFunc = new Func(op, Code.buffer.Count);
                    }
                }
                else if (line.StartsWith("e:"))
                {
                    string op = line.Substring(2);

                    if (currentFunc == null)
                    {
                        currentFunc = new Func(op, Code.buffer.Count);
                    }
                    else
                    {
                        Code.Write(Opcodes.ret);
                        Funcs.Add(currentFunc);
                        currentFunc = new Func(op, Code.buffer.Count);
                    }

                    Events.Add(currentFunc);
                }
                else if (line.StartsWith("."))
                {
                    string name = line.Substring(1);
                    Label l = new Label(name, Code.buffer.Count());
                    currentFunc.labels.Add(l);
                }
                else if (line.StartsWith("pushInt "))
                {
                    int value = Convert.ToInt32(line.Substring(10));
                    Code.Write(Opcodes.pushInt);
                    Code.Write(value);
                }
                else if (line.StartsWith("pushString "))
                {
                    string temp = line.Substring(11);
                    string value = temp.Substring(temp.IndexOf("\"") + 1, temp.LastIndexOf("\"") - 1);
                    Code.Write(Opcodes.pushString);
                    Code.Write(value);
                }
                else if (line.StartsWith("pushVar "))
                {
                    string name = line.Substring(8);
                    Code.Write(Opcodes.pushVar);
                    Code.Write(name);
                }
                else if (line.Equals("pop"))
                {
                    Code.Write(Opcodes.pop);
                }
                else if (line.Equals("popa"))
                {
                    Code.Write(Opcodes.popa);
                }
                else if (line.StartsWith("decVar "))
                {
                    string name = line.Substring(7);
                    Code.Write(Opcodes.decVar);
                    Code.Write(name);
                }
                else if (line.StartsWith("setVar "))
                {
                    string name = line.Substring(7);
                    Code.Write(Opcodes.setVar);
                    Code.Write(name);
                }
                else if (line.Equals("add"))
                {
                    Code.Write(Opcodes.add);
                }
                else if (line.Equals("sub"))
                {
                    Code.Write(Opcodes.sub);
                }
                else if (line.Equals("mul"))
                {
                    Code.Write(Opcodes.mul);
                }
                else if (line.Equals("div"))
                {
                    Code.Write(Opcodes.div);
                }
                else if (line.Equals("clear"))
                {
                    Code.Write(Opcodes.clear);
                }
                else if (line.Equals("ife"))
                {
                    if (currentBlock != null)
                    {
                        blockstack.Push(currentBlock);
                    }

                    currentBlock = new IfBlock(blockNumber);
                    Code.Write(Opcodes.ife);
                    Code.Write(blockNumber);
                    blockNumber++;
                }
                else if (line.Equals("ifn"))
                {
                    if (currentBlock != null)
                    {
                        blockstack.Push(currentBlock);
                    }

                    currentBlock = new IfBlock(blockNumber);
                    Code.Write(Opcodes.ifn);
                    Code.Write(blockNumber);
                    blockNumber++;
                }
                else if (line.Equals("elseife"))
                {
                    if (currentBlock != null)
                    {
                        blockstack.Push(currentBlock);
                    }

                    currentBlock = new ElseIfBlock(blockNumber);
                    Code.Write(Opcodes.elseife);
                    Code.Write(blockNumber);
                    blockNumber++;
                }
                else if (line.Equals("elseifn"))
                {
                    if (currentBlock != null)
                    {
                        blockstack.Push(currentBlock);
                    }

                    currentBlock = new ElseIfBlock(blockNumber);
                    Code.Write(Opcodes.elseifn);
                    Code.Write(blockNumber);
                    blockNumber++;
                }
                else if (line.Equals("else"))
                {
                    if (currentBlock != null)
                    {
                        blockstack.Push(currentBlock);
                    }

                    currentBlock = new ElseBlock(blockNumber);
                    Code.Write(Opcodes.els);
                    Code.Write(blockNumber);
                    blockNumber++;
                }
                else if (line.Equals("endif"))
                {
                    Code.Write(Opcodes.endif);
                    currentBlock.endBlock = Code.buffer.Count();
                    Blocks.Add(currentBlock);

                    if (blockstack.Count > 0)
                    {
                        currentBlock = blockstack.Pop();
                    }
                    else
                    {
                        currentBlock = null;
                    }
                }
                else if (line.StartsWith("call "))
                {
                    string name = line.Substring(5);
                    Code.Write(Opcodes.call);
                    Code.Write(name);
                }
                else if (line.StartsWith("goto "))
                {
                    string name = line.Substring(5);
                    Code.Write(Opcodes.got);
                    Code.Write(name);
                }
                else if (line.Equals("print"))
                {
                    Code.Write(Opcodes.print);
                }
                else if (line.Equals("printLine"))
                {
                    Code.Write(Opcodes.printLine);
                }
                else if (line.Equals("read"))
                {
                    Code.Write(Opcodes.read);
                }
                else if (line.Equals("readLine"))
                {
                    Code.Write(Opcodes.readLine);
                }
                else if (line.Equals("delay"))
                {
                    Code.Write(Opcodes.delay);
                }
                else if (line.Equals("captureScreen"))
                {
                    Code.Write(Opcodes.captureScreen);
                }
                else if (line.Equals("getClipboard"))
                {
                    Code.Write(Opcodes.getClipboard);
                }
                else if (line.Equals("setClipboard"))
                {
                    Code.Write(Opcodes.setClipboard);
                }
                else if (line.Equals("write"))
                {
                    Code.Write(Opcodes.write);
                }
                else if (line.Equals("inputInt32"))
                {
                    Code.Write(Opcodes.inputInt32);
                }
                else if (line.Equals("inputString"))
                {
                    Code.Write(Opcodes.inputString);
                }
                else if (line.Equals("ret"))
                {
                    Code.Write(Opcodes.ret);
                }
            }

            Code.Write(Opcodes.ret);
            Funcs.Add(currentFunc);
        }
    }
}
