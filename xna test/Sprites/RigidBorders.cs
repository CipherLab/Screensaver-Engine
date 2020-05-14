using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;
using ScreenSaverHelper;
using SharedKernel.Interfaces;
using Rectangle = System.Drawing.Rectangle;

namespace MonoGameTest.Sprites
{
	public class RigidBorders : Component
	{
		private void AddRigidBorders(int thickness = 4)
		{
            Texture2D xBorderTex;
            Texture2D yBorderTex;
            using (ISimpleImageHelper imgHelper = new ImageHelper())
            {
                byte[] xBorder = imgHelper.BlankImage(new Rectangle(0, 0, Screen.Width, 4), System.Drawing.Color.White);
                byte[] yBorder = imgHelper.BlankImage(new Rectangle(0, 0, 4, Screen.Height), System.Drawing.Color.White);

                yBorderTex = Texture2D.FromStream(Graphics.Instance.Batcher.GraphicsDevice, new MemoryStream(yBorder));
                xBorderTex = Texture2D.FromStream(Graphics.Instance.Batcher.GraphicsDevice, new MemoryStream(xBorder));
            }
            var yRenderer = new SpriteRenderer(yBorderTex);
            var xRenderer = new SpriteRenderer(xBorderTex);

            //bottom
            var rb = CreateEntity(
				new Vector2(Screen.Width / 2f, Screen.Height + thickness), 0, Screen.Width);
            rb.AddComponent<SpriteRenderer>(xRenderer);
			rb.Entity.GetComponent<SpriteRenderer>().Color = Color.Magenta;

			//top
			rb = CreateEntity(new Vector2(Screen.Width / 2f, -thickness), 0,  Screen.Width);
            rb.AddComponent<SpriteRenderer>(xRenderer);
			rb.Entity.GetComponent<SpriteRenderer>().Color = Color.Magenta;

			//left
			rb = CreateEntity(new Vector2(-thickness, Screen.Height / 2f), 0, Screen.Height);
            rb.AddComponent<SpriteRenderer>(yRenderer);
			rb.Entity.GetComponent<SpriteRenderer>().Color = Color.Magenta;

			//right
			rb = CreateEntity(new Vector2(Screen.Width + thickness, Screen.Height / 2f), 0, Screen.Height);
            rb.AddComponent<SpriteRenderer>(yRenderer);
			rb.Entity.GetComponent<SpriteRenderer>().Color = Color.Magenta;
		}
        ArcadeRigidbody CreateEntity(Vector2 position, int width, int height)
        {
			var rigidbody = new ArcadeRigidbody()
                .SetMass(0)
                .SetFriction(0)
                .SetElasticity(.1f);
            rigidbody.ShouldUseGravity = false;
            Entity.AddComponent(rigidbody);
            Entity.AddComponent(new BoxCollider(width, height));
            Entity.SetPosition(position);
            return rigidbody;
        }

		public override void OnAddedToEntity()
        {
            AddRigidBorders();
        }

		public override void OnRemovedFromEntity()
		{
		}
	}
}