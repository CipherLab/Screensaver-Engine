using System;
using System.Reflection;
using ScreenSaverHelper;
using ScreenSaverHelper.Util;
using SharedKernel.Interfaces;
using Unity;
using Unity.Injection;

namespace ScreenSaverEngine2
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
           
        
            using (var game = new Game1())
                game.Run();

        }
       

    }
 
}
