using Nez;

namespace MonoGameTest.Shared
{
	public class MouseFollow : Component, IUpdatable
	{
		public void Update()
		{
			Entity.SetPosition(Input.ScaledMousePosition);
		}
	}
}