using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.BitmapFonts;
using Nez.Sprites;
using Nez.Textures;
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
        //public RigidBodyScene() : base(true, true)
        //{ }

        private int RigidBodiesRenderLayer = 5;
        private int BackgroundRenderLayer = 15;
        public override void Initialize()
        {
            base.Initialize();

            // create a Renderer that renders all but the light layer and screen space layer
            //AddRenderer(new RenderLayerRenderer(0, 3, ScreenSpaceRenderLayer));
            //AddRenderer(new RenderLayerExcludeRenderer(0, ScreenSpaceRenderLayer, RigidBodiesRenderLayer));



            // add a PostProcessor that renders the light render target and blurs it
            //AddPostProcessor(new PolyLightPostProcessor(0, lightRenderer.RenderTexture))
            //    .SetEnableBlur(true)
            //    .SetBlurAmount(0.5f);



            int originalWidth = 1280;
            int originalHeight = 720;

            this.SimpleFont = Content.Load<SpriteFont>("Shared\\SimpleFont");
            SetDesignResolution(originalWidth, originalHeight, Scene.SceneResolutionPolicy.ShowAllPixelPerfect);
            Screen.SetSize(originalWidth, originalHeight);

            Screen.IsFullscreen = false;
            Screen.ApplyChanges();

            // renderer for the background image and the light map (created by StencilLightRenderer)
           var bg= AddRenderer(new RenderLayerRenderer(0, BackgroundRenderLayer));
           var rb = AddRenderer(new RenderLayerRenderer(1, RigidBodiesRenderLayer));
           bg.WantsToRenderAfterPostProcessors = false;
           rb.WantsToRenderAfterPostProcessors = true;

           this.AddRenderer(bg);
           this.AddRenderer(rb);
           RigidBody(originalWidth, originalHeight);
        }

        private void RigidBody(int originalWidth, int originalHeight)
        {
            string modelsDirectory = Path.Combine(Environment.CurrentDirectory, @"Content\Shared");
            modelsDirectory = Path.Combine(modelsDirectory, "maxresdefault.jpg");

            using (var origImageHelper = new ImageHelper(new System.Drawing.Rectangle(0, 0, originalWidth, originalHeight), modelsDirectory))
            {
             
                byte[] originalImage = origImageHelper.GetImageByteArrayFromFile(modelsDirectory);


                //transparent edge detected, filled black
                //byte[] maskTransparent = origImageHelper.EdgeDetectedFilledBlack(ImageFormat.Png);
                byte[] maskSolid = origImageHelper.EdgeDetectedFilledBlack(ImageFormat.Jpeg);
                List<Rectangle> boxes;
                using (var maskImageHelper = new ImageHelper(maskSolid))
                {
                    //box locations for found stuff
                    boxes = maskImageHelper.GetSpriteBoundingBoxesInImage(2, 2);
                }

                var dark = origImageHelper.ChangeBrightness(-30);
                var blur = origImageHelper.GetBlurImageByteArrayFromData(3);
                var result = origImageHelper.FlattenImages(dark, blur);
                var originalImageTex = Texture2D.FromStream(Graphics.Instance.Batcher.GraphicsDevice, new MemoryStream(result));

                var spriteR = new SpriteRenderer(originalImageTex);
                spriteR.SetRenderLayer(BackgroundRenderLayer);

                CreateEntity("bg")
                    .SetPosition(Screen.Center)
                    .AddComponent(spriteR)
                    .SetRenderLayer(BackgroundRenderLayer);


                //var overlayImg = origImageHelper.ContentAwareFillFromSpriteImageMask(boxes);
                ////using (var overlayHelper = new ImageHelper(maskTransparent))
                //using (var overlayHelper = new ImageHelper(overlayImg))
                //{
                //    var blurImage = overlayHelper.GetBlurImageByteArrayFromData(4);
                //    // bgEntity1.AddComponent(new SpriteRenderer(maskBlurTex));
                //    using (ISimpleImageHelper sih = new ImageHelper())
                //    {
                //        var result = sih.FlattenImages(originalImage, blurImage, blurImage, blurImage);
                //        var maskBlurTex = Texture2D.FromStream(Graphics.Instance.Batcher.GraphicsDevice, new MemoryStream(result));
                //        bgEntity1.AddComponent(new SpriteRenderer(maskBlurTex));
                //    }
                //}


                var detectedObjectImages = origImageHelper.GetSpritesFromImage(boxes, true);

               
                //AddPostProcessor(new PolyLightPostProcessor(0, lightRenderer.RenderTexture))
                //    .SetEnableBlur(true)
                //    .SetBlurAmount(0.5f);

                var post = AddPostProcessor(new PixelGlitchPostProcessor(1));
                AddRigidBodyEntities(detectedObjectImages);
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
                float mass = (b.ImageProperties.Width * b.ImageProperties.Height) / 100f;
                var v = new Vector2(Random.NextAngle(), Random.NextAngle());
                var impulse = new Vector2(1f, 1f);

                int max = 200;
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
                if (b.ImageProperties.Height > max && b.ImageProperties.Width > max)
                {
                    v = new Vector2(.001f, .0001f);
                    impulse = new Vector2(0,0);
                }
                var tex = Texture2D.FromStream(Graphics.Instance.Batcher.GraphicsDevice,
                    new MemoryStream(b.ImageData));

           
                CreateEntity(b.Vector2, mass, friction, elasticity,
                        v, tex,
                        false, false)
                    .AddImpulse(impulse);
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
            var r = new SpriteRenderer(texture);
            r.SetRenderLayer(RigidBodiesRenderLayer);

            var entity = CreateEntity(Utils.RandomString(3))
                .SetPosition(position)
                .AddComponent(rigidbody)
                .AddComponent(r)
                .SetRenderLayer(RigidBodiesRenderLayer);

            if (circle)
                entity.AddComponent<CircleCollider>();
            else
                entity.AddComponent<BoxCollider>();
            
            //entity.UpdateOrder = RigidBodiesRenderLayer;
            return rigidbody;
        }
    }
}