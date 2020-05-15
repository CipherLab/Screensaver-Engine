using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using ScreenSaverHelper;
using SharedKernel.Enums;
using SharedKernel.Interfaces;
using Unity;
using Random = Nez.Random;
using Rectangle = System.Drawing.Rectangle;

namespace ScreenSaverEngine2.Shared
{
    public class RigidBodies : Component
    {
        public ICroppedImagePart CroppedImagePart { get; }
        public int RenderLayer { get; }

        public RigidBodies(ICroppedImagePart croppedImagePart, int renderLayer)
        {
            CroppedImagePart = croppedImagePart;
            RenderLayer = renderLayer;
        }

        //public void Initialize(int thickness, ScreenEdgeSide side)
        //{
        //}

        public override void OnAddedToEntity()
        {
            AddRigidBodies();
        }

        private ArcadeRigidbody CreateEntity(Vector2 position, int width, int height, Texture2D texture, float friction, float mass, float elasticity)
        {
            //
            var rigidbody = new ArcadeRigidbody()
                .SetMass(mass)
                .SetFriction(friction)
                .SetElasticity(elasticity)
                .SetVelocity(Vector2.Zero);
            rigidbody.ShouldUseGravity = false;
            Entity.AddComponent(rigidbody);
            Entity.AddComponent(new BoxCollider(width, height));
            Entity.SetPosition(position);
            var spriteRenderer = new SpriteRenderer(texture);
            spriteRenderer.SetRenderLayer(RenderLayer);
            Entity.AddComponent(spriteRenderer);
            return rigidbody;
        }

        public void AddRigidBodies()
        {
            Texture2D tex = Texture2D.FromStream(Graphics.Instance.Batcher.GraphicsDevice, new MemoryStream(CroppedImagePart.ImageData));
            ArcadeRigidbody rb = CreateEntity(
                new Vector2(
                    CroppedImagePart.ImageProperties.X + (CroppedImagePart.ImageProperties.Width / 2f),
                    CroppedImagePart.ImageProperties.Y + (CroppedImagePart.ImageProperties.Height / 2f)),
                CroppedImagePart.ImageProperties.Width,
                CroppedImagePart.ImageProperties.Height,
                tex,
                0,
                CroppedImagePart.ImageProperties.X * CroppedImagePart.ImageProperties.Y,
                1);
            var task = Task.Run(async () =>
            {
                for (; ; )
                {
                    await Task.Delay(Random.Range(2000, 20000));
                    rb.SetVelocity(new Vector2(Random.NextAngle(), Random.NextAngle()) * 2);
                    rb.AddImpulse(Vector2.One);
                }
            });
            //rb.Entity.GetComponent<SpriteRenderer>().Color = Color.Magenta;
        }

        public override void OnRemovedFromEntity()
        {
        }

        public void Dispose()
        {
        }
    }
}