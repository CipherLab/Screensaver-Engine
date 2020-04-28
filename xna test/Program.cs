#region Using Statements

using System;


#if MONOMAC
using MonoMac.AppKit;
using MonoMac.Foundation;
#elif __IOS__ || __TVOS__
using Foundation;
using UIKit;
#endif

#endregion

namespace MonoGameTest
{
#if __IOS__ || __TVOS__
    [Register("AppDelegate")]
    class Program : UIApplicationDelegate
#else
    static class Program
#endif
    {
        private static Game1 _game;

        internal static void RunGame()
        {
            _game = new Game1(1280,720,false,true,"Screen Saver","Content");
            _game.Run();
#if !__IOS__ && !__TVOS__
            _game.Dispose();
#endif
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
#if !MONOMAC && !__IOS__ && !__TVOS__
        [STAThread]
#endif
        static void Main(string[] args)
        {
#if MONOMAC
            NSApplication.Init ();

            using (var p = new NSAutoreleasePool ()) {
                NSApplication.SharedApplication.Delegate = new AppDelegate();
                NSApplication.Main(args);
            }
#elif __IOS__ || __TVOS__
            UIApplication.Main(args, null, "AppDelegate");
#else
            RunGame();
#endif
        }

#if __IOS__ || __TVOS__
        public override void FinishedLaunching(UIApplication app)
        {
            RunGame();
        }
#endif
    }

#if MONOMAC
    class AppDelegate : NSApplicationDelegate
    {
        public override void FinishedLaunching (MonoMac.Foundation.NSObject notification)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (object sender, ResolveEventArgs a) =>  {
                if (a.Name.StartsWith("MonoMac")) {
                    return typeof(MonoMac.AppKit.AppKitFramework).Assembly;
                }
                return null;
            };
            Program.RunGame();
        }

        public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
        {
            return true;
        }
    }
#endif
    /*
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
    */
}
