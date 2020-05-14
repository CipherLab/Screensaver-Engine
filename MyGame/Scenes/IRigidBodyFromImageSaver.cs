namespace ScreenSaverEngine2.Scenes
{
    public interface IRigidBodyFromImageSaver
    {
        //set
        byte[] BackgroundImage { get; set; }
        bool HasRigidBorders { get; set; }
        bool IsFullScreen { get; set; }
        int Height { get; set; }
        int Width { get; set; }
        bool HasGui { get; set; }
        //call 
        void InitInterfaceProprties();

    }
}