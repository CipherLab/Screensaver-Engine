// Decompiled with JetBrains decompiler
// Type: HeroEditor.Common.CharacterBase
// Assembly: HeroEditor.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D9D5B1BE-AD53-47A8-B0FE-D2D5DBF20268
// Assembly location: C:\Users\mnewp\source\repos\Screensaver-Engine\FantasyHeroes\Assets\HeroEditor\Common\HeroEditor.Common.dll


using System;
using System.Collections.Generic;
using HeroEditor.Common.Data;
using HeroEditor.Common.Enums;
using Nez.Sprites;
using Nez.Textures;

namespace HeroEditor.Common
{
  public abstract class CharacterBase : MonoBehaviour
  {
    [Header("Body")]
    public Sprite Head;
    public Sprite HeadMask;
    public Sprite Ears;
    public Sprite Hair;
    public SpriteMask HairMask;
    public Sprite Beard;
    public List<Sprite> Body;
    [Header("Expressions")]
    public string Expression;
    public List<Expression> Expressions;
    [Header("Equipment")]
    public Sprite Helmet;
    public Sprite Earrings;
    public Sprite Glasses;
    public Sprite Mask;
    public Sprite PrimaryMeleeWeapon;
    public Sprite SecondaryMeleeWeapon;
    public List<Sprite> Armor;
    public Sprite Cape;
    public Sprite Back;
    public Sprite Shield;
    public List<Sprite> Bow;
    public List<Sprite> Firearms;
    [Header("Renderers")]
    public SpriteRenderer HeadRenderer;
    public SpriteRenderer EarsRenderer;
    public SpriteRenderer HairRenderer;
    public SpriteRenderer EyebrowsRenderer;
    public SpriteRenderer EyesRenderer;
    public SpriteRenderer MouthRenderer;
    public SpriteRenderer BeardRenderer;
    public List<SpriteRenderer> BodyRenderers;
    public SpriteRenderer HelmetRenderer;
    public SpriteRenderer GlassesRenderer;
    public SpriteRenderer MaskRenderer;
    public SpriteRenderer EarringsRenderer;
    public SpriteRenderer PrimaryMeleeWeaponRenderer;
    public SpriteRenderer PrimaryMeleeWeaponTrailRenderer;
    public SpriteRenderer SecondaryMeleeWeaponRenderer;
    public SpriteRenderer SecondaryMeleeWeaponTrailRenderer;
    public List<SpriteRenderer> ArmorRenderers;
    public SpriteRenderer CapeRenderer;
    public SpriteRenderer BackRenderer;
    public SpriteRenderer ShieldRenderer;
    public List<SpriteRenderer> BowRenderers;
    public List<SpriteRenderer> FirearmsRenderers;
    [Header("Animation")]
    public WeaponType WeaponType;

    public abstract void Initialize();

    public abstract void UpdateAnimation();

    public abstract string ToJson();

    public abstract void LoadFromJson(string serialized);

    public void CopyFrom(CharacterBase character)
    {
      Head = character.Head;
      Body = character.Body;
      Ears = character.Ears;
      Hair = character.Hair;
      Expression = character.Expression;
      Expressions = character.Expressions;
      Beard = character.Beard;
      Helmet = character.Helmet;
      Glasses = character.Glasses;
      Mask = character.Mask;
      Armor = character.Armor;
      PrimaryMeleeWeapon = character.PrimaryMeleeWeapon;
      SecondaryMeleeWeapon = character.SecondaryMeleeWeapon;
      Cape = character.Cape;
      Back = character.Back;
      Shield = character.Shield;
      Bow = character.Bow;
      Firearms = character.Firearms;
      //using (IEnumerator<SpriteRenderer> enumerator = ((IEnumerable<SpriteRenderer>) ((Component) this).GetComponentsInChildren<SpriteRenderer>(true)).Where<SpriteRenderer>((Func<SpriteRenderer, bool>) (obj0 => Object.op_Inequality((Object) obj0.get_sprite(), (Object) null))).GetEnumerator())
      //{
      //  while (((IEnumerator) enumerator).MoveNext())
      //  {
      //    SpriteRenderer current = enumerator.Current;
      //    foreach (SpriteRenderer componentsInChild in (SpriteRenderer[]) ((Component) character).GetComponentsInChildren<SpriteRenderer>(true))
      //    {
      //      if (((Object) current).get_name() == ((Object) componentsInChild).get_name() && ((Object) ((Component) current).get_transform().get_parent()).get_name() == ((Object) ((Component) componentsInChild).get_transform().get_parent()).get_name())
      //      {
      //        current.set_color(componentsInChild.get_color());
      //        break;
      //      }
      //    }
      //  }
      //}
      WeaponType = character.WeaponType;
      Initialize();
    }
  }

  public class HeaderAttribute : Attribute
  {
      public HeaderAttribute(string body)
      {
      }
  }
}
