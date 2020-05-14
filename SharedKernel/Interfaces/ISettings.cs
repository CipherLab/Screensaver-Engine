namespace SharedKernel.Interfaces
{
    public interface ISettings
    {
        string Path { get; set; }
        int Speed { get; set; }
        bool Shuffle { get; set; }
        bool Fancytile { get; set; }
        void Load();

        /// <summary>
        /// Save text into the Registry.
        /// </summary>
        void SaveSettings();
    }
}