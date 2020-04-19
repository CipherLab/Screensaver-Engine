namespace MonoGameTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var game = new Game1())
            {
                if (args.Length > 0)
                {
                    switch (args[0].ToLower().Trim().Substring(0, 2))
                    {
                        //show
                        case "/s":
                            //run the screen saver
                            game.Run();
                            break;
                        //preview
                        case "/p":
                            //show the screen saver preview
                            //Application.Run(new MainForm(new IntPtr(long.Parse(args[1])))); //args[1] is the handle to the preview window
                            break;
                        //configure
                        case "/c":
                            //show settings
                            break;
                        //an argument was passed, but it wasn't /s, /p, or /c, so we don't care wtf it was
                        default:
                            //show the screen saver anyway
                            break;
                    }
                }
                else //no arguments were passed
                {
                    game.Run();
                }
            }


        }
    }
}
