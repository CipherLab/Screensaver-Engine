using System;

namespace ScreenSaverHelper.Util.Random
{    
    public class Helper
    {
        private static System.Random random = new System.Random();

        public static Single GetNextSingle(Single min, Single max)
        {
            return (Single)(min + (random.NextDouble() * (max - min)));
        }            
    }
}
