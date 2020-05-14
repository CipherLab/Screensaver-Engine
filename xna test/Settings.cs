using SharedKernel;
using SharedKernel.Interfaces;

namespace MonoGameTest
{
    public class Settings : ISettings
    {
        public Settings()
        {
        }
        public string Path { get; set; }
        public int Speed { get; set; }
        public bool Shuffle { get; set; }
        public bool Fancytile { get; set; }
        private void Deserialize()
        {
            this.Path = @"G:\AD\Amazon Drive\Pictures\backgrounds";
            this.Speed = 15;
            this.Shuffle = true;
            this.Fancytile = true;
        }

        public void Load()
        {
            Deserialize();
        }

        public void SaveSettings()
        {
            Serialize(this);
        }

        private void Serialize(Settings settings)
        {
        }
    }
}