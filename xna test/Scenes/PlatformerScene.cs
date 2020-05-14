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
    //[SampleScene("Platformer", 120,
    //    "Work in progress...\nArrows, d-pad or left stick to move, z key or a button to jump")]
    public class PlatformerScene : RigidBodyScene
    {
        public PlatformerScene() : base()
        {
        }


        public override void Initialize()
        {
            base.Initialize();

            this.SimpleFont = Content.Load<SpriteFont>("Shared\\SimpleFont");


            //SetDesignResolution(1280, 720, Scene.SceneResolutionPolicy.ShowAllPixelPerfect);
            //Screen.SetSize(1280, 720);
            // load up our TiledMap
            //TmxMap map = Content.LoadTiledMap("Content/Platformer/tiledMap.tmx");

            //var rowCount = Screen.Height / map.TileHeight;
            //var columnCount = Screen.Width / map.TileWidth;
            //map.Height = columnCount;
            //map.Width = rowCount;

            //var mapLayer = map.TileLayers.First();
            //mapLayer.Height = columnCount;
            //mapLayer.Width = rowCount;

            //var tile = mapLayer.Tiles.First();

            //mapLayer.Tiles = new TmxLayerTile[columnCount * rowCount];

            ////int idx = 0;
            ////for (int x = 0; x < columnCount; x++)
            ////{
            ////    for (int y = 0; y < rowCount; y++)
            ////    {
            ////        tile.X = x;
            ////        tile.Y = y;
            ////        mapLayer.Tiles[idx++] = tile;

            ////    }
            ////}
            //var spawnObject = map.GetObjectGroup("objects").Objects["spawn"];

            // var tiledEntity = CreateEntity("tiled-map-entity");
            // tiledEntity.AddComponent(new TiledMapRenderer(map, "main"));

            var rowCount = Screen.Height / 16;
            var columnCount = Screen.Width / 16;
            var playerEntity = CreateEntity("player", new Vector2(100, 100));
            playerEntity.AddComponent(new Caveman());
            var playerCollider = new BoxCollider(-8, -16, 16, 32);
            playerEntity.AddComponent(playerCollider);
            var mover = new TiledMapMover(new TmxLayer
            {
                Map = new TmxMap
                {
                    TmxDirectory = null,
                    Version = null,
                    TiledVersion = null,
                    Width = columnCount,
                    Height = rowCount,
                    TileWidth = 16,
                    TileHeight = 16,
                    HexSideLength = null,
                    Orientation = OrientationType.Orthogonal,
                    StaggerAxis = StaggerAxisType.X,
                    StaggerIndex = StaggerIndexType.Odd,
                    RenderOrder = RenderOrderType.RightDown,
                    BackgroundColor = default,
                    NextObjectID = null,
                    Layers = null,
                    Tilesets = null,
                    TileLayers = null,
                    ObjectGroups = null,
                    ImageLayers = null,
                    Groups = null,
                    Properties = null,
                    MaxTileWidth = 16,
                    MaxTileHeight = 16
                },
                Name = null,
                Opacity = 0,
                Visible = false,
                OffsetX = 0,
                OffsetY = 0,
                Properties = null,
                Width = columnCount,
                Height = rowCount,
                Tiles = new TmxLayerTile[columnCount * rowCount]
            })
            {
                Entity = playerEntity,
                Enabled = true,
                UpdateOrder = 1,
                ColliderHorizontalInset = 0,
                ColliderVerticalInset = 0,

            };

            playerEntity.AddComponent(mover);

            //AddPostProcessor(new VignettePostProcessor(1));
            allowUpdate = true;
            sw.Start();
        }

        private bool allowUpdate { get; set; }
        Stopwatch sw = new Stopwatch();
        public override void Update()
        {
            if (!allowUpdate)
                return;

            if (sw.Elapsed.Milliseconds > 100)
            {

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