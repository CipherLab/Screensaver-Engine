// Decompiled with JetBrains decompiler
// Type: HeroEditor.Common.SpriteHelper
// Assembly: HeroEditor.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D9D5B1BE-AD53-47A8-B0FE-D2D5DBF20268
// Assembly location: C:\Users\mnewp\source\repos\Screensaver-Engine\FantasyHeroes\Assets\HeroEditor\Common\HeroEditor.Common.dll


using Microsoft.Xna.Framework.Graphics;
using Nez.Textures;

namespace HeroEditor.Common
{
  public static class SpriteHelper
  {
    public static Texture2D CopyNotReadableSprite(Sprite sprite)
    {
      //Texture2D texture2D1 = new Texture2D(((Texture) sprite.get_texture()).get_width(), ((Texture) sprite.get_texture()).get_height());
      //RenderTexture temporary = RenderTexture.GetTemporary(((Texture) sprite.get_texture()).get_width(), ((Texture) sprite.get_texture()).get_height(), 0, (RenderTextureFormat) 7, (RenderTextureReadWrite) 1);
      //Graphics.Blit((Texture) sprite.get_texture(), temporary);
      //RenderTexture.set_active(temporary);
      //texture2D1.ReadPixels(new Rect(0.0f, 0.0f, (float) ((Texture) temporary).get_width(), (float) ((Texture) temporary).get_height()), 0, 0);
      //texture2D1.Apply();
      //Rect rect = sprite.get_rect();
      //int width1 = (int) ((Rect) ref rect).get_width();
      //rect = sprite.get_rect();
      //int height1 = (int) ((Rect) ref rect).get_height();
      //Texture2D texture = new Texture2D(width1, height1);
      //Rect textureRect1 = sprite.get_textureRect();
      //int x1 = (int) ((Rect) ref textureRect1).get_x();
      //textureRect1 = sprite.get_textureRect();
      //int y1 = (int) ((Rect) ref textureRect1).get_y();
      //textureRect1 = sprite.get_textureRect();
      //int width2 = (int) ((Rect) ref textureRect1).get_width();
      //textureRect1 = sprite.get_textureRect();
      //int height2 = (int) ((Rect) ref textureRect1).get_height();
      //Color[] pixels = texture2D1.GetPixels(x1, y1, width2, height2);
      //SpriteHelper.ClearTexture(texture);
      //Texture2D texture2D2 = texture;
      //int x2 = (int) sprite.get_textureRectOffset().x;
      //int y2 = (int) sprite.get_textureRectOffset().y;
      //Rect textureRect2 = sprite.get_textureRect();
      //int width3 = (int) ((Rect) ref textureRect2).get_width();
      //textureRect2 = sprite.get_textureRect();
      //int height3 = (int) ((Rect) ref textureRect2).get_height();
      //Color[] colorArray = pixels;
      //texture2D2.SetPixels(x2, y2, width3, height3, colorArray);
      //texture.Apply();
      //return texture;
    }

    public static void ClearTexture(Texture2D texture)
    {
      //Color[] colorArray = new Color[((Texture) texture).get_width() * ((Texture) texture).get_height()];
      //Color clear = Color.get_clear();
      //for (int index = 0; index < colorArray.Length; ++index)
      //  colorArray[index] = clear;
      //texture.SetPixels(colorArray);
      //texture.Apply();
    }
  }
}
