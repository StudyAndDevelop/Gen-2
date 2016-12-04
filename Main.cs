using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ZPG_Server;

namespace ZPG_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            World.TotalLoader();
            while (true)
            {
                int x, y;
                Console.Write("Write X ");
                x = Convert.ToInt16(Console.ReadLine());
                Console.Write("Write Y ");
                y = Convert.ToInt16(Console.ReadLine());
                if (x == 0 && y == 0)
                    World.Chunk.SpawnCreate();
                else World.Chunk.GenerateChunk(x, y);
            }
        }
    }
}
