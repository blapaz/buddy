﻿using System.Collections.Generic;

namespace Blapaz.Buddy.Runtime.Library
{
    class Buffer
    {
        public List<object> buffer = new List<object>();
        public int pos = 0;

        public int ReadInt()
        {
            int ret = (int)buffer[pos];
            pos++;
            return ret;
        }

        public string ReadString()
        {
            string ret = (string)buffer[pos];
            pos++;
            return ret;
        }

        public void Write(int data)
        {
            buffer.Add(data);
        }

        public void Write(string data)
        {
            buffer.Add(data);
        }
    }
}
