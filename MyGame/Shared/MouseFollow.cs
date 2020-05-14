using Nez;

namespace ScreenSaverEngine2.Shared
{
	public class MouseFollow : Component, IUpdatable
	{
		public void Update()
		{
			Entity.SetPosition(Input.ScaledMousePosition);
		}
	}
}