using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameTest
{
    public class Camera2d
    {
        public int ViewportWidth { get; }
        public int ViewportHeight { get; }
        public float Zoom { get; private set; } // Camera Zoom
        public Matrix Transform { get; private set; } // Matrix Transform
        public Vector2 Pos { get; private set; }// Camera Position
        public float Rotation { get; private set; } // Camera Rotation

        public Camera2d(int viewportWidth, int viewportHeight)
        {
            ViewportWidth = viewportWidth;
            ViewportHeight = viewportHeight;
            Zoom = 1.0f; 
            Rotation = 0.0f;
            Pos = CenterScreen;
        }
        public Matrix get_transformation(GraphicsDevice graphicsDevice)
        {
            Transform =       // Thanks to o KB o for this solution
                Matrix.CreateTranslation(new Vector3(-Pos.X, -Pos.Y, 0)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                Matrix.CreateTranslation(new Vector3(ViewportWidth * 0.5f, ViewportHeight * 0.5f, 0));
            return Transform;
        }

        public void SetPosition(Vector2 vector2)
        {
            this.Pos = vector2;
        }
        public void SetZoom(float zoom)
        {
            this.Zoom += zoom;
        }

        public void  SetRotation(float rotation)
        {
            this.Rotation = rotation;
        }

        public void Move(Vector2 pos)
        {
            this.Pos = pos;
        }


        public string GetString()
        {
            return $"Zoom: {this.Zoom}\r\nPosX:{Pos.X}-PosY:{Pos.Y}\r\nRotation{this.Rotation}";
        }

        internal void Reset()
        {
            Zoom = 1.0f;
            Rotation = 0.0f;
            Pos = CenterScreen;
        }

        internal Vector2 CenterScreen => new Vector2(ViewportWidth / 2f, ViewportHeight / 2f);
    }
}
