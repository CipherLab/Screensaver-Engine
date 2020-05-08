// Decompiled with JetBrains decompiler
// Type: HeroEditor.Common.SpriteCollection
// Assembly: HeroEditor.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D9D5B1BE-AD53-47A8-B0FE-D2D5DBF20268
// Assembly location: C:\Users\mnewp\source\repos\Screensaver-Engine\FantasyHeroes\Assets\HeroEditor\Common\HeroEditor.Common.dll

using System.Collections.Generic;

namespace HeroEditor.Common
{
  public class SpriteCollection 
  {
    [Header("Where to find sprites?")]
    public List<string> ScanFolderTags;
    [Header("Body Parts")]
    public List<SpriteGroupEntry> Head;
    public List<SpriteGroupEntry> Ears;
    public List<SpriteGroupEntry> Hair;
    public List<SpriteGroupEntry> Eyebrows;
    public List<SpriteGroupEntry> Eyes;
    public List<SpriteGroupEntry> Mouth;
    public List<SpriteGroupEntry> Beard;
    public List<SpriteGroupEntry> Body;
    [Header("Equipment")]
    public List<SpriteGroupEntry> Helmet;
    public List<SpriteGroupEntry> Glasses;
    public List<SpriteGroupEntry> Mask;
    public List<SpriteGroupEntry> Earrings;
    public List<SpriteGroupEntry> Armor;
    public List<SpriteGroupEntry> Cape;
    public List<SpriteGroupEntry> Back;
    public List<SpriteGroupEntry> MeleeWeapon1H;
    public List<SpriteGroupEntry> MeleeWeapon2H;
    public List<SpriteGroupEntry> MeleeWeaponTrail1H;
    public List<SpriteGroupEntry> MeleeWeaponTrail2H;
    public List<SpriteGroupEntry> Bow;
    public List<SpriteGroupEntry> Firearms1H;
    public List<SpriteGroupEntry> Firearms2H;
    public List<SpriteGroupEntry> Shield;
    public List<SpriteGroupEntry> Supplies;
    [Header("Service")]
    public bool UseTrailSprites;
    public bool DebugLogging;
    public static SpriteCollection Instance;

    public void Awake()
    {
      Instance = this;
    }
  }
}
