using SharedKernel;

namespace FancyTiling
{
    public class Settings : ISettings
    {
        public string Path { get; set; }
        public int Speed { get; set; }
        public bool Shuffle { get; set; }
        public bool Fancytile { get; set; }
        public ISettings LoadFromReg()
        {
            this.Path = @"G:\AD\Amazon Drive\Pictures\backgrounds";
            this.Speed = 15;
            this.Shuffle = true;
            this.Fancytile = true;
            return this;
        }

        public void SaveSettings()
        {
        }
    }
}