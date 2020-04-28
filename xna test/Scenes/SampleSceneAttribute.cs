using System;

namespace MonoGameTest.Scenes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SampleSceneAttribute : Attribute
    {
        public string ButtonName;
        public int Order;
        public string InstructionText;


        public SampleSceneAttribute(string buttonName, int order, string instructionText = null)
        {
            ButtonName = buttonName;
            Order = order;
            InstructionText = instructionText;
        }
    }
}