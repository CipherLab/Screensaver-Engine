using System;
using System.Collections.Generic;
using System.IO;
using ImageMagick;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.BitmapFonts;
using Nez.Sprites;
using OnnxGuiDetection;
using OnnxGuiDetection.ML.DataModels;
using ScreenSaverHelper;
using ScreenSaverHelper.Util;
using BoundingBox = SharedKernel.BoundingBox;
using Graphics = Nez.Graphics;
using Rectangle = System.Drawing.Rectangle;

namespace MonoGameTest.Scenes
{
    [SampleScene("Basic Scene", 9999, "")]
    public class DetectGuiItemsScene : SubSceneHelper
    {
        public override void Initialize()
        {
            base.Initialize();


            this.SimpleFont = Content.Load<SpriteFont>("Shared\\SimpleFont");
            int originalWidth = 1280;
            int originalHeight = 720;
            SetDesignResolution(originalWidth, originalHeight, Scene.SceneResolutionPolicy.ShowAll);
            Screen.SetSize(originalWidth, originalHeight);

            string modelsDirectory = Path.Combine(Environment.CurrentDirectory, @"Content\Shared");
            modelsDirectory = Path.Combine(modelsDirectory, "maxresdefault.jpg");

            var ih = new ImageHelper(new System.Drawing.Rectangle(0, 0, originalWidth, originalHeight), false);

            //temporary... will come from screenshot or whatev
            byte[] originalImage = ih.GetImageByteArrayFromFile(modelsDirectory);
            byte[] mask = ih.EdgeDetector(originalImage, MagickColors.White, MagickColors.Black, true);

            var bg = Texture2D.FromStream(Graphics.Instance.Batcher.GraphicsDevice, new MemoryStream(originalImage));
            //var bg_mask = Texture2D.FromStream(Graphics.Instance.Batcher.GraphicsDevice, new MemoryStream(mask));

            var bgEntity1 = CreateEntity("bg1", new Vector2(Screen.Width / 2, Screen.Height / 2));
            var bgEntity2 = CreateEntity("bg2", new Vector2(Screen.Width / 2, Screen.Height / 2));
            bgEntity1.AddComponent(new SpriteRenderer(bg));
            //bgEntity2.AddComponent(new SpriteRenderer(bg_mask));
            bgEntity1.UpdateOrder = 0;
            bgEntity2.UpdateOrder = 3;
            List<Rectangle> boxes = ih.UnpackSpriteSheet(originalImage, 3, 5);

            //var contentFilledImg = ih.ContentAwareFillFromSpriteImageMask(boxes, originalImage);
            //var contentFilledTexture = Texture2D.FromStream(Graphics.Instance.Batcher.GraphicsDevice, new MemoryStream(contentFilledImg));
           
            //bgEntity2.AddComponent(new SpriteRenderer(contentFilledTexture));

            List<CroppedImagePart> detectedObjectImages = ih.GetSpritesFromImage(boxes, originalImage, true);
            ShowImagesBehindBoundingBoxes(detectedObjectImages);

            // AddBoundingBoxes(boxes);


            Screen.IsFullscreen = false;
            Screen.ApplyChanges();

            //  Graphics.Instance.BitmapFont = Core.Content.LoadBitmapFont("Content\\Shared\\montserrat-32.fnt");

        }
        private void ShowImagesBehindBoundingBoxes(List<CroppedImagePart> boxes)
        {
            foreach (var t in boxes)
            {
                var box = t.ImageProperties;

                float x = box.X + box.Width / 2;
                float y = box.Y + box.Height / 2;

                var tempEntity = CreateEntity("", new Vector2(x, y));
                tempEntity.UpdateOrder = 4;

                var cropImg = Texture2D.FromStream(Graphics.Instance.Batcher.GraphicsDevice, new MemoryStream(t.ImageData));


                tempEntity.AddComponent(new SpriteRenderer(cropImg));
            }
        }

        private void ShowBoundingBoxes(List<Rectangle> boxes)
        {
            for (var b = 0; b < boxes.Count; b++)
            {
                boxes[b] = new Rectangle(boxes[b].X, boxes[b].Y, boxes[b].Width + 4, boxes[b].Height + 4);

                // process output boxes
                float x = boxes[b].X + boxes[b].Width / 2;
                float y = boxes[b].Y + boxes[b].Height / 2;
                int width = boxes[b].Width;
                int height = boxes[b].Height;

                // process output boxes ONNX
                //float x = Math.Max(boxes[b].Dimensions.X, 0);
                //float y = Math.Max(boxes[b].Dimensions.Y, 0);
                //int width = Convert.ToInt32(Math.Min(originalWidth - x, boxes[b].Dimensions.Width));
                //int height = Convert.ToInt32(Math.Min(originalHeight - y, boxes[b].Dimensions.Height));
                // fit to current image size
                //x = originalWidth * x / ImageSettings.imageWidth;
                //y = originalHeight * y / ImageSettings.imageHeight;
                //width = originalWidth * width / ImageSettings.imageWidth;
                //height = originalHeight * height / ImageSettings.imageHeight;


                var tempEntity = CreateEntity("", new Vector2(x, y));
                tempEntity.UpdateOrder = 2;

                Texture2D rect = new Texture2D(Graphics.Instance.Batcher.GraphicsDevice, width, height);

                Color[] data = new Color[width * height];
                Color color = Color.DarkRed;

                for (int i = 0; i < data.Length; ++i)
                    data[i] = new Color(color.R, color.G, color.B, color.A / 2);

                rect.SetData(data);

                tempEntity.AddComponent(new SpriteRenderer(rect));
            }
        }

    }
}