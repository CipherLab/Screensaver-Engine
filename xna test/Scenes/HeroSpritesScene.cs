using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameTest.Sprites;
using Newtonsoft.Json;
using Nez;
using Nez.Sprites;
using Nez.Tiled;
using ScreenSaverHelper.Util;

namespace MonoGameTest.Scenes
{
    [WindowScene("HeroSpritesScene", 120,
        "Work in progress...\nArrows, d-pad or left stick to move, z key or a button to jump")]
    public class HeroSpritesScene : SubSceneHelper
    {
        public HeroSpritesScene() : base(true, true)
        {
        }


        public override void Initialize()
        {
            this.SimpleFont = Content.Load<SpriteFont>("Shared\\SimpleFont");

            HeroSpriteInfoRepo repo = new HeroSpriteInfoRepo();

            var bodyParts = repo.GetSkeleBody();
            foreach (var bodyPart in bodyParts.Position)
            {
                ShowImage(bodyPart.Key, bodyPart.Value);
            }

            arml = Entities.FindEntity("ArmL");
            var forearmL = Entities.FindEntity("ForearmL");
            var handL = Entities.FindEntity("HandL");
            var finger = Entities.FindEntity("Finger");

            //finger.Parent = handL.Transform;
            //handL.Parent = forearmL.Transform;
            //forearmL.Parent = arml.Transform;
            // setup a pixel perfect screen that fits our map
            //SetDesignResolution(640, 480, Scene.SceneResolutionPolicy.ShowAllPixelPerfect);
            //Screen.SetSize(640 * 2, 480 * 2);

            SetDesignResolution(1280, 720, Scene.SceneResolutionPolicy.ShowAllPixelPerfect);
            Screen.SetSize(1280, 720);

            // load up our TiledMap
            //var map = Content.LoadTiledMap("Content/Platformer/tiledMap.tmx");
            //var spawnObject = map.GetObjectGroup("objects").Objects["spawn"];

            // var tiledEntity = CreateEntity("tiled-map-entity");
            // tiledEntity.AddComponent(new TiledMapRenderer(map, "main"));


            // create our Player and add a TiledMapMover to handle collisions with the tilemap
            //var playerEntity = CreateEntity("player", new Vector2(spawnObject.X, spawnObject.Y));
            //playerEntity.AddComponent(new Caveman());
            //playerEntity.AddComponent(new BoxCollider(-8, -16, 16, 32));
            //playerEntity.AddComponent(new TiledMapMover(map.GetLayer<TmxLayer>("main")));
            //arml.SetRotation(.001f);

            AddPostProcessor(new VignettePostProcessor(1));
            allowUpdate = true;
            sw.Start();
        }

        private Entity arml { get; set; }
        private bool allowUpdate { get; set; }
        Stopwatch sw = new Stopwatch();
        public float rot = 0f;
        public override void Update()
        {
            if (!allowUpdate)
                return;

            if (sw.Elapsed.Milliseconds > 100)
            {
                //arml.
                //arml.SetRotation(rot += .1f);
                sw.Restart();
            }
            base.Update();
        }

        private void ShowImage(string name, SpriteInfo spritInfo)
        {

            var box = spritInfo.Position;

            float x = spritInfo.PartPosition.X + box.Width / 2;
            float y = spritInfo.PartPosition.Y + box.Height / 2;

            x += Screen.Width / 2f;
            y += Screen.Height / 2f;

            var tempEntity = CreateEntity(name, new Vector2(x, y));
            tempEntity.UpdateOrder = 1;

            var img = Texture2D.FromStream(Graphics.Instance.Batcher.GraphicsDevice,
                new MemoryStream(spritInfo.ImageData));


            tempEntity.AddComponent(new SpriteRenderer(img));
        }
    }
}