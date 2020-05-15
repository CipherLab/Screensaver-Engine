using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using ScreenSaverHelper;
using SharedKernel.Enums;
using SharedKernel.Interfaces;
using Unity;
using Rectangle = System.Drawing.Rectangle;

namespace ScreenSaverEngine2.Shared
{
    public class RigidBorders : Component
    {
        public ISimpleImageHelper ImgHelper { get; }
        public ScreenEdgeSide EdgeSide { get; private set; }
        public int Thickness { get; private set; }

        public RigidBorders(int thickness, ScreenEdgeSide side, ISimpleImageHelper imageHelper)
        {
            ImgHelper = imageHelper;
            Thickness = thickness;
            EdgeSide = side;
        }

        //public void Initialize(int thickness, ScreenEdgeSide side)
        //{
        //}

        public override void OnAddedToEntity()
        {
            AddRigidBorders();
        }

        private ArcadeRigidbody CreateEntity(Vector2 position, int width, int height, Texture2D texture)
        {
            var rigidbody = new ArcadeRigidbody()
                .SetMass(0)
                .SetFriction(0)
                .SetElasticity(1f);
            rigidbody.ShouldUseGravity = false;
            Entity.AddComponent(rigidbody);
            Entity.AddComponent(new BoxCollider(width, height));
            Entity.SetPosition(position);
            Entity.AddComponent(new SpriteRenderer(texture));
            return rigidbody;
        }

        public void AddRigidBorders()
        {
            Texture2D xBorderTex;
            Texture2D yBorderTex;
            using (ImgHelper)
            {
                byte[] xBorder = ImgHelper.BlankImage(new Rectangle(0, 0, Screen.Width, 4), System.Drawing.Color.White);
                byte[] yBorder = ImgHelper.BlankImage(new Rectangle(0, 0, 4, Screen.Height), System.Drawing.Color.White);

                yBorderTex = Texture2D.FromStream(Graphics.Instance.Batcher.GraphicsDevice, new MemoryStream(yBorder));
                xBorderTex = Texture2D.FromStream(Graphics.Instance.Batcher.GraphicsDevice, new MemoryStream(xBorder));
            }

            ArcadeRigidbody rb;
            switch (EdgeSide)
            {
                case ScreenEdgeSide.Left:

                    //left
                    rb = CreateEntity(new Vector2(-0, Screen.Height / 2f), 0, Screen.Height, yBorderTex);
                    rb.Entity.GetComponent<SpriteRenderer>().Color = Color.Magenta;

                    break;

                case ScreenEdgeSide.Right:
                    //right
                    rb = CreateEntity(new Vector2(Screen.Width + 0, Screen.Height / 2f), 0, Screen.Height, yBorderTex);
                    rb.Entity.GetComponent<SpriteRenderer>().Color = Color.Magenta;
                    break;

                case ScreenEdgeSide.Top:
                    //top
                    rb = CreateEntity(new Vector2(Screen.Width / 2f, -0), 0, Screen.Width, xBorderTex);
                    rb.Entity.GetComponent<SpriteRenderer>().Color = Color.Magenta;

                    break;

                case ScreenEdgeSide.Bottom:
                    //bottom
                    rb = CreateEntity(
                        new Vector2(Screen.Width / 2f, Screen.Height + 0), 0, Screen.Width, xBorderTex);
                    rb.Entity.GetComponent<SpriteRenderer>().Color = Color.Magenta;

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void OnRemovedFromEntity()
        {
        }

        public void Dispose()
        {
            if (ImgHelper != null)
                ImgHelper.Dispose();
        }
    }
}