using System;

namespace MonoGameTest.Scenes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WindowSceneAttribute : Attribute
    {
        public string ButtonName;
        public int Order;
        public string InstructionText;

        public bool SetupGui = true;
        public WindowSceneAttribute(string buttonName, int order, string instructionText = null)
        {
            ButtonName = buttonName;
            Order = order;
            InstructionText = instructionText;
        }
    }
}