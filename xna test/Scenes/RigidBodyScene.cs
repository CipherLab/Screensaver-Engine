using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using Nez.Verlet;
using ScreenSaverHelper;
using ScreenSaverHelper.Util;
using Random = Nez.Random;
using Rectangle = System.Drawing.Rectangle;

namespace MonoGameTest.Scenes
{
    [SampleScene("Rigid Bodies", 100,
        "")]
    public class RigidBodyScene : SubSceneHelper
    {
        public override void Initialize()
        {
            base.Initialize();

            int originalWidth = 1280;
            int originalHeight = 720;

            this.SimpleFont = Content.Load<SpriteFont>("Shared\\SimpleFont");
            SetDesignResolution(originalWidth, originalHeight, Scene.SceneResolutionPolicy.ShowAll);
            Screen.SetSize(originalWidth, originalHeight);

            Screen.IsFullscreen = false;
            Screen.ApplyChanges();



            //var ih = new ImageHelper(new System.Drawing.Rectangle(0, 0, originalWidth, originalHeight), false);
            //string modelsDirectory = Path.Combine(Environment.CurrentDirectory, @"Content\Shared");
            //modelsDirectory = Path.Combine(modelsDirectory, "maxresdefault.jpg");

            //byte[] xBorder = ih.BlankImage(new Rectangle(0, 0, Screen.Width, 2), System.Drawing.Color.White);
            //byte[] yBorder = ih.BlankImage(new Rectangle(0, 0, 2, Screen.Height), System.Drawing.Color.White);
            //byte[] originalImage = ih.GetImageByteArrayFromFile(modelsDirectory);
            //byte[] originalImageBlur = ih.GetBlurImageByteArrayFromFile(modelsDirectory, 3);
            //var bgEntity1 = CreateEntity("bg1", new Vector2(Screen.Width / 2, Screen.Height / 2));

            ////the original background pic
            //var originalImageBlurTex =
            //    Texture2D.FromStream(Graphics.Instance.Batcher.GraphicsDevice, new MemoryStream(originalImageBlur));
            //var yBorderTex = Texture2D.FromStream(Graphics.Instance.Batcher.GraphicsDevice, new MemoryStream(yBorder));
            //var xBorderTex = Texture2D.FromStream(Graphics.Instance.Batcher.GraphicsDevice, new MemoryStream(xBorder));
            //bgEntity1.AddComponent(new SpriteRenderer(originalImageBlurTex));
            //bgEntity1.UpdateOrder = 0;

            ////a black line alpha edge detected img. suitable for sprite finding/unpacking
            ////fill white, alpha black for lines
            //byte[] mask = ih.EdgeDetector(originalImage, System.Drawing.Color.Black, System.Drawing.Color.White, true);
            ////box locations for found stuff
            //List<Rectangle> boxes = ih.UnpackSpriteSheet(mask, 3, 5, 20, 100);

            ////alpha sprites cut from original img using box locations
            //List<CroppedImagePart> detectedObjectImages = ih.GetSpritesFromImage(boxes, originalImage, true);




            //// create an Entity and Component to manage the Verlet World and tick its update method
            //var verletSystem = CreateEntity("verlet-system")
            //    .AddComponent<VerletSystem>();

            //foreach (var b in detectedObjectImages)
            //{
            //    verletSystem.World.AddComposite(new Ball(b.Vector2, 25));

            //}

            RigidBody(originalWidth, originalHeight);
        }

        private void RigidBody(int originalWidth, int originalHeight)
        {
            string modelsDirectory = Path.Combine(Environment.CurrentDirectory, @"Content\Shared");
            modelsDirectory = Path.Combine(modelsDirectory, "maxresdefault.jpg");


            Texture2D yBorderTex;
            Texture2D xBorderTex;
            List<CroppedImagePart> detectedObjectImages;
            using (var origImageHelper = new ImageHelper(new System.Drawing.Rectangle(0, 0, originalWidth, originalHeight), modelsDirectory))
            {
                var bgEntity1 = CreateEntity("bg1", new Vector2(Screen.Width / 2f, Screen.Height / 2f));
                byte[] originalImage = origImageHelper.GetImageByteArrayFromFile(modelsDirectory);



                //transparent edge detected, filled black
                byte[] maskTransparent = origImageHelper.EdgeDetectedFilledBlack(ImageFormat.Png);
                byte[] maskImageBlur;
                byte[] maskSolid = origImageHelper.EdgeDetectedFilledBlack(ImageFormat.Jpeg);
                List<Rectangle> boxes;
                using (var maskImageHelper = new ImageHelper(maskSolid))
                {
                    //box locations for found stuff
                    boxes = maskImageHelper.GetSpriteBoundingBoxesInImage(2, 2);
                }


                //var originalImageTex = Texture2D.FromStream(Graphics.Instance.Batcher.GraphicsDevice, new MemoryStream(originalImage));
                // bgEntity1.AddComponent(new SpriteRenderer(originalImageTex));

                //trying to put a black mask of cut icons over the bg, but rigid bodies drawing over for some reason.
                using (var maskImageHelper = new ImageHelper(maskTransparent))
                {
                    maskImageBlur = maskImageHelper.GetBlurImageByteArrayFromData(4);
                    // bgEntity1.AddComponent(new SpriteRenderer(maskBlurTex));
                    using (ISimpleImageHelper sih = new ImageHelper())
                    {

                        var result = sih.FlattenImages(originalImage, maskImageBlur, maskImageBlur, maskImageBlur);
                        var maskBlurTex = Texture2D.FromStream(Graphics.Instance.Batcher.GraphicsDevice, new MemoryStream(result));
                        bgEntity1.AddComponent(new SpriteRenderer(maskBlurTex));

                    }
                }

                bgEntity1.UpdateOrder = 0;


                detectedObjectImages = origImageHelper.GetSpritesFromImage(boxes, true);
                AddRigidBodyEntities(detectedObjectImages);

                //alpha sprites cut from original img using box locations
            }


        }

        private void AddRigidBodyEntities(List<CroppedImagePart> detectedObjectImages)
        {
            Texture2D xBorderTex;
            Texture2D yBorderTex;
            using (ISimpleImageHelper imgHelper = new ImageHelper())
            {
                byte[] xBorder = imgHelper.BlankImage(new Rectangle(0, 0, Screen.Width, 2), System.Drawing.Color.White);
                byte[] yBorder = imgHelper.BlankImage(new Rectangle(0, 0, 2, Screen.Height), System.Drawing.Color.White);

                yBorderTex = Texture2D.FromStream(Graphics.Instance.Batcher.GraphicsDevice, new MemoryStream(yBorder));
                xBorderTex = Texture2D.FromStream(Graphics.Instance.Batcher.GraphicsDevice, new MemoryStream(xBorder));
            }
            var friction = 0f;
            var elasticity = 1f;
            foreach (var b in detectedObjectImages)
            {
                float mass =( b.ImageProperties.Width * b.ImageProperties.Height) / 100f;
                var v = new Vector2(Random.NextAngle(), Random.NextAngle());
                //int max = 400;
                //if ((b.ImageProperties.Width > 10 && b.ImageProperties.Height > 10) &&
                //    (b.ImageProperties.Width < max && b.ImageProperties.Height < max))
                //{
                //    v = new Vector2(Random.NextAngle(), Random.NextAngle());
                //    mass = 1f;
                //}
                //else if (b.ImageProperties.Height > max && b.ImageProperties.Width > max)
                //{
                //    v = new Vector2(0, 0);
                //    mass = 0;
                //}
                //else
                //{
                //    continue;
                //}

                var tex = Texture2D.FromStream(Graphics.Instance.Batcher.GraphicsDevice,
                    new MemoryStream(b.ImageData));

                CreateEntity(b.Vector2, mass, friction, elasticity,
                        v, tex,
                        false, false)
                    .AddImpulse(new Vector2(1f, 1f));
            }

            //bottom
            var rb = CreateEntity(
                new Vector2(Screen.Width / 2f, Screen.Height - 1),
                0, friction, elasticity,
                new Vector2(0, 0), xBorderTex, false, false);
            rb.Entity.GetComponent<SpriteRenderer>().Color = Color.DarkMagenta;

            //top
            rb = CreateEntity(
                new Vector2(Screen.Width / 2f, 2),
                0, friction, elasticity,
                new Vector2(0, 0), xBorderTex, false, false);
            rb.Entity.GetComponent<SpriteRenderer>().Color = Color.DarkMagenta;

            //left
            rb = CreateEntity(
                new Vector2(1, Screen.Height / 2f),
                0, friction, elasticity,
                new Vector2(0, 0), yBorderTex, false, false);
            rb.Entity.GetComponent<SpriteRenderer>().Color = Color.DarkMagenta;

            //right
            rb = CreateEntity(
                new Vector2(Screen.Width - 1, Screen.Height / 2f),
                0, friction, elasticity,
                new Vector2(0, 0), yBorderTex, false, false);
            rb.Entity.GetComponent<SpriteRenderer>().Color = Color.DarkMagenta;
        }


        ArcadeRigidbody CreateEntity(Vector2 position, float mass, float friction, float elasticity, Vector2 velocity,
                                     Texture2D texture, bool shouldUseGravity, bool circle = true)
        {
            var rigidbody = new ArcadeRigidbody()
                .SetMass(mass)
                .SetFriction(friction)
                .SetElasticity(elasticity)
                .SetVelocity(velocity);
            rigidbody.ShouldUseGravity = shouldUseGravity;
            var entity = CreateEntity(Utils.RandomString(3));
            entity.Position = position;
            entity.AddComponent(new SpriteRenderer(texture));
            entity.AddComponent(rigidbody);
            if (circle)
                entity.AddComponent<CircleCollider>();
            else
                entity.AddComponent<BoxCollider>();

            entity.UpdateOrder = 3;
            return rigidbody;
        }
    }
}