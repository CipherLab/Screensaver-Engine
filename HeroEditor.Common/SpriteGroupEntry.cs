// Decompiled with JetBrains decompiler
// Type: HeroEditor.Common.SpriteGroupEntry
// Assembly: HeroEditor.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D9D5B1BE-AD53-47A8-B0FE-D2D5DBF20268
// Assembly location: C:\Users\mnewp\source\repos\Screensaver-Engine\FantasyHeroes\Assets\HeroEditor\Common\HeroEditor.Common.dll


using System;
using System.Collections.Generic;
using System.Linq;
using Nez.Textures;

namespace HeroEditor.Common
{
    [Serializable]
    public class SpriteGroupEntry
    {
        public string Name;
        public string Collection;
        public Sprite Sprite;
        public Sprite LinkedSprite;
        public List<Sprite> Sprites;
        public bool Multiple;
        public string Hash;
        public string Config;

        public string Id
        {
            get
            {
                return Collection + Name;
            }
        }

        public Dictionary<string, string> ConfigToDictionary
        {
            get
            {
                return Config.Split(new string[1]
                {
                    Environment.NewLine
                }, StringSplitOptions.None).Select(obj0 => obj0.Split('=')).ToDictionary(obj0 => obj0[0], obj0 => obj0[1]);
            }
        }

        public SpriteGroupEntry(string name, string collection, Sprite sprite)
        {
            //if (Object.op_Equality((Object) sprite, (Object) null))
            //  throw new Exception(string.Format(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.z(), (object) collection, (object) name));
            Name = name;
            Collection = collection;
            Sprite = sprite;
            Multiple = false;
            Hash = sprite.Texture2D.Name;
        }

        public SpriteGroupEntry(string name, string collection, Sprite sprite, List<Sprite> sprites)
        {
            if (sprites == null || sprites.Count == 0)
                throw new Exception();
            Name = name;
            Collection = collection;
            Sprite = sprite;
            Sprites = sprites.OrderBy(obj0 => obj0.Texture2D.Name).ToList();
            Multiple = true;
            Hash = sprite.Texture2D.Name;
        }

        public Sprite GetSprite(string name)
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: reference to a compiler-generated method
            return Sprites.Single(x => x.Texture2D.Name == name);

            //return ((IEnumerable<Sprite>) this.Sprites).Single<Sprite>(new Func<Sprite, bool>(new SpriteGroupEntry.a()
            //{
            //  A = name
            //}.A));
        }
    }
}
