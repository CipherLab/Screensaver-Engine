using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace MonoGameTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var game = new Game1())
                game.Run();
        }
    }
}
