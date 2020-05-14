using System;

namespace ScreenSaverEngine2.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StartupGuiSceneAttribute : Attribute
    {
        public string ButtonName { get; }
        public int Order { get; }
        public string InstructionText;
        public StartupGuiSceneAttribute(string buttonName, int order, string instructionText = null)
        {
            ButtonName = buttonName;
            Order = order;
            InstructionText = instructionText;
        }
    }
}