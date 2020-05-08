// Decompiled with JetBrains decompiler
// Type: HeroEditor.Common.CharacterEditorBase
// Assembly: HeroEditor.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D9D5B1BE-AD53-47A8-B0FE-D2D5DBF20268
// Assembly location: C:\Users\mnewp\source\repos\Screensaver-Engine\FantasyHeroes\Assets\HeroEditor\Common\HeroEditor.Common.dll


using HeroEditor.Common.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HeroEditor.Common
{
  public abstract class CharacterEditorBase : MonoBehaviour
  {
    public SpriteCollection SpriteCollection;
    public AnimationManager AnimationManager;
    public CharacterBase Character;
    [Header("UI")]
    public GameObject Editor;
    public GameObject CommonPalette;
    public GameObject SkinPalette;
    public Dropdown HeadDropdown;
    public Dropdown EarsDropdown;
    public Dropdown HairDropdown;
    public Dropdown EyebrowsDropdown;
    public Dropdown EyesDropdown;
    public Dropdown MouthDropdown;
    public Dropdown BeardDropdown;
    public Dropdown BodyDropdown;
    public Dropdown HelmetDropdown;
    public Dropdown EarringsDropdown;
    public Dropdown GlassesDropdown;
    public Dropdown MaskDropdown;
    public Dropdown ArmorDropdown;
    public Dropdown CapeDropdown;
    public Dropdown BackDropdown;
    public Dropdown MeleeWeapon1HDropdown;
    public Dropdown MeleeWeaponPairedDropdown;
    public Dropdown MeleeWeapon2HDropdown;
    public Dropdown BowDropdown;
    public Dropdown Firearms1HDropdown;
    public Dropdown Firearms2HDropdown;
    public Dropdown ShieldDropdown;
    public Dropdown SuppliesDropdown;
    public List<Button> EditorOnlyButtons;
    [Header("Expressions")]
    public Dropdown AngryEyebrowsDropdown;
    public Dropdown AngryEyesDropdown;
    public Dropdown AngryMouthDropdown;
    public Dropdown DeadEyebrowsDropdown;
    public Dropdown DeadEyesDropdown;
    public Dropdown DeadMouthDropdown;
    [Header("Armor Parts")]
    public Dropdown ArmorArmLDropdown;
    public Dropdown ArmorArmRDropdown;
    public Dropdown ArmorForearmLDropdown;
    public Dropdown ArmorForearmRDropdown;
    public Dropdown ArmorHandLDropdown;
    public Dropdown ArmorHandRDropdown;
    public Dropdown ArmorFingerDropdown;
    public Dropdown ArmorSleeveRDropdown;
    public Dropdown ArmorLegDropdown;
    public Dropdown ArmorPelvisDropdown;
    public Dropdown ArmorShinDropdown;
    public Dropdown ArmorTorsoDropdown;
    public Dropdown UpperArmorDropdown;
    public Dropdown LowerArmorDropdown;
    public Dropdown GlovesDropdown;
    public Dropdown BootsDropdown;
    [Header("Editors")]
    public GameObject MainEditor;
    public GameObject ExpressionEditor;
    public GameObject ArmorPartsEditor;
    [Header("Links")]
    public string LinkToBasicVersion;
    public string LinkToProVersion;
    [Header("Debug")]
    public Material DefaultMaterial;
    public bool ForceDefaultMaterial;
    public bool ForcePaint;
    private PaletteButton A;
    [NonSerialized]
    private Component A;
    [NonSerialized]
    private Color A;
    private const string A = "CE:FeedbackRequestTime";
    private const string a = "CE:FeedbackTime";

    public abstract void Save(string path);

    public abstract void Load(string path);

    protected abstract void SetFirearmParams(SpriteGroupEntry entry);

    protected abstract void OpenPalette(GameObject palette, Color selected);

    protected abstract void FeedbackTip();

    public void Start()
    {
      this.Character.Initialize();
      this.InitializeDropdowns();
      this.EditorOnlyButtons.ForEach((Action<Button>) (obj0 => ((Selectable) obj0).set_interactable(Application.get_isEditor())));
      this.a();
    }

    public void OpenPalette(PaletteButton paletteButton)
    {
      this.A = paletteButton;
      this.OpenPalette(this.B(paletteButton.Target), this.a(paletteButton.Target)[0].get_color());
    }

    public void ClosePalette()
    {
      this.CommonPalette.SetActive(false);
      this.SkinPalette.SetActive(false);
      this.Editor.SetActive(true);
    }

    public void Flip()
    {
      Vector3 localScale = ((Component) this.Character).get_transform().get_localScale();
      ref __Null local = ref localScale.x;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local = ^(float&) ref local * -1f;
      ((Component) this.Character).get_transform().set_localScale(localScale);
    }

    public virtual void Load(CharacterBase character)
    {
      this.Character.CopyFrom(character);
      this.InitializeDropdowns();
    }

    public void Navigate(string url)
    {
      Application.OpenURL(url);
    }

    public void PickColor(Color color)
    {
      using (List<SpriteRenderer>.Enumerator enumerator = this.a(this.A.Target).GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          SpriteRenderer current = enumerator.Current;
          current.set_color(color);
          ((Renderer) current).set_sharedMaterial(Color.op_Equality(color, Color.get_white()) || this.ForceDefaultMaterial ? this.DefaultMaterial : this.A.PaintMaterial);
        }
      }
    }

    public void SwitchToExpressions()
    {
      this.MainEditor.SetActive(!this.MainEditor.get_activeSelf());
      this.ExpressionEditor.SetActive(!this.ExpressionEditor.get_activeSelf());
      this.InitializeDropdowns();
      this.Character.Expression = this.ExpressionEditor.get_activeSelf() ? \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.c() : \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.C();
      this.Character.Initialize();
    }

    public void SwitchToArmorParts()
    {
      this.MainEditor.SetActive(!this.MainEditor.get_activeSelf());
      this.ArmorPartsEditor.SetActive(!this.ArmorPartsEditor.get_activeSelf());
      this.InitializeDropdowns();
      List<SpriteRenderer> spriteRendererList = new List<SpriteRenderer>();
      spriteRendererList.Add(this.Character.PrimaryMeleeWeaponRenderer);
      spriteRendererList.Add(this.Character.SecondaryMeleeWeaponRenderer);
      spriteRendererList.Add(this.Character.ShieldRenderer);
      spriteRendererList.AddRange((IEnumerable<SpriteRenderer>) this.Character.BowRenderers);
      spriteRendererList.AddRange((IEnumerable<SpriteRenderer>) this.Character.FirearmsRenderers);
      spriteRendererList.ForEach((Action<SpriteRenderer>) (obj0 => obj0.set_color(this.MainEditor.get_activeSelf() ? Color.get_white() : new Color(1f, 1f, 1f, 0.25f))));
    }

    public void Randomize(bool armorParts)
    {
      Dropdown[] dropdownArray = new Dropdown[3]
      {
        this.HairDropdown,
        this.HelmetDropdown,
        this.ArmorDropdown
      };
      foreach (Dropdown dropdown in dropdownArray)
        CharacterEditorBase.A(dropdown, 0);
      if (armorParts)
      {
        if (!this.ArmorPartsEditor.get_activeSelf())
          this.A();
        foreach (Dropdown componentsInChild in (Dropdown[]) this.ArmorPartsEditor.GetComponentsInChildren<Dropdown>(true))
          CharacterEditorBase.A(componentsInChild, 1);
      }
      this.Character.HairRenderer.set_color(Color32.op_Implicit(new Color32((byte) Random.Range(0, 256), (byte) Random.Range(0, 256), (byte) Random.Range(0, 256), byte.MaxValue)));
      switch (Random.Range(0, 5 + (((Component) this.Firearms1HDropdown).get_gameObject().get_activeInHierarchy() ? 2 : 0)))
      {
        case 0:
          CharacterEditorBase.A(this.MeleeWeapon1HDropdown, 1);
          CharacterEditorBase.A(this.ShieldDropdown, 1);
          break;
        case 1:
          CharacterEditorBase.A(this.MeleeWeapon2HDropdown, 1);
          break;
        case 2:
          CharacterEditorBase.A(this.MeleeWeapon1HDropdown, 1);
          CharacterEditorBase.A(this.MeleeWeaponPairedDropdown, 1);
          break;
        case 3:
          CharacterEditorBase.A(this.BowDropdown, 1);
          break;
        case 4:
          CharacterEditorBase.A(this.SuppliesDropdown, 1);
          break;
        case 5:
          CharacterEditorBase.A(this.Firearms1HDropdown, 1);
          break;
        case 6:
          CharacterEditorBase.A(this.Firearms2HDropdown, 1);
          break;
      }
      this.InitializeDropdowns();
    }

    private static void A([In] Dropdown obj0_1, int _param1 = 0)
    {
      List<Dropdown.OptionData> list = ((IEnumerable<Dropdown.OptionData>) obj0_1.get_options()).Where<Dropdown.OptionData>((Func<Dropdown.OptionData, bool>) (obj0_2 => !obj0_2.get_text().Contains(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.K()))).ToList<Dropdown.OptionData>();
      if (list.Count <= _param1)
        return;
      int index = Random.Range(_param1, list.Count);
      obj0_1.set_value(obj0_1.get_options().IndexOf(list[index]));
    }

    protected void InitializeDropdowns()
    {
      if (this.MainEditor.get_activeSelf())
      {
        this.A(this.HeadDropdown, this.SpriteCollection.Head, this.Character.Head, (Action<SpriteGroupEntry>) (obj0 => this.Character.Head = CharacterEditorBase.F(obj0)));
        this.A(this.EarsDropdown, this.SpriteCollection.Ears, this.Character.Ears, (Action<SpriteGroupEntry>) (obj0 => this.Character.Ears = CharacterEditorBase.F(obj0)));
        this.A(this.HairDropdown, this.SpriteCollection.Hair, this.Character.Hair, (Action<SpriteGroupEntry>) (obj0 => this.Character.Hair = CharacterEditorBase.F(obj0)));
        this.A(this.EyebrowsDropdown, this.SpriteCollection.Eyebrows, this.Character.Expressions[0].Eyebrows, (Action<SpriteGroupEntry>) (obj0 => this.Character.Expressions[0].Eyebrows = CharacterEditorBase.F(obj0)));
        this.A(this.EyesDropdown, this.SpriteCollection.Eyes, this.Character.Expressions[0].Eyes, (Action<SpriteGroupEntry>) (obj0 => this.Character.Expressions[0].Eyes = CharacterEditorBase.F(obj0)));
        this.A(this.MouthDropdown, this.SpriteCollection.Mouth, this.Character.Expressions[0].Mouth, (Action<SpriteGroupEntry>) (obj0 => this.Character.Expressions[0].Mouth = CharacterEditorBase.F(obj0)));
        this.A(this.BeardDropdown, this.SpriteCollection.Beard, this.Character.Beard, (Action<SpriteGroupEntry>) (obj0 => this.Character.Beard = CharacterEditorBase.F(obj0)));
        this.A(this.BodyDropdown, this.SpriteCollection.Body, ((IEnumerable<Sprite>) this.Character.Body).FirstOrDefault<Sprite>(), (Action<SpriteGroupEntry>) (obj0 => this.Character.Body = CharacterEditorBase.f(obj0)));
        this.A(this.HelmetDropdown, this.SpriteCollection.Helmet, this.Character.Helmet, (Action<SpriteGroupEntry>) (obj0 => this.Character.Helmet = CharacterEditorBase.F(obj0)));
        this.A(this.EarringsDropdown, this.SpriteCollection.Earrings, this.Character.Earrings, (Action<SpriteGroupEntry>) (obj0 => this.Character.Earrings = CharacterEditorBase.F(obj0)));
        this.A(this.GlassesDropdown, this.SpriteCollection.Glasses, this.Character.Glasses, (Action<SpriteGroupEntry>) (obj0 => this.Character.Glasses = CharacterEditorBase.F(obj0)));
        this.A(this.MaskDropdown, this.SpriteCollection.Mask, this.Character.Mask, (Action<SpriteGroupEntry>) (obj0 => this.Character.Mask = CharacterEditorBase.F(obj0)));
        this.A(this.ArmorDropdown, this.SpriteCollection.Armor, ((IEnumerable<Sprite>) this.Character.Armor).FirstOrDefault<Sprite>(), (Action<SpriteGroupEntry>) (obj0 => this.Character.Armor = CharacterEditorBase.f(obj0)));
        this.A(this.CapeDropdown, this.SpriteCollection.Cape, this.Character.Cape, (Action<SpriteGroupEntry>) (obj0 => this.Character.Cape = CharacterEditorBase.F(obj0)));
        this.A(this.BackDropdown, this.SpriteCollection.Back, this.Character.Back, (Action<SpriteGroupEntry>) (obj0 => this.Character.Back = CharacterEditorBase.F(obj0)));
        this.A(this.MeleeWeapon1HDropdown, this.SpriteCollection.MeleeWeapon1H, this.Character.PrimaryMeleeWeapon, new Action<SpriteGroupEntry>(this.B));
        this.A(this.MeleeWeapon2HDropdown, this.SpriteCollection.MeleeWeapon2H, this.Character.PrimaryMeleeWeapon, new Action<SpriteGroupEntry>(this.b));
        this.A(this.MeleeWeaponPairedDropdown, this.SpriteCollection.MeleeWeapon1H, this.Character.SecondaryMeleeWeapon, new Action<SpriteGroupEntry>(this.C));
        this.A(this.BowDropdown, this.SpriteCollection.Bow, ((IEnumerable<Sprite>) this.Character.Bow).FirstOrDefault<Sprite>(), new Action<SpriteGroupEntry>(this.e));
        this.A(this.Firearms1HDropdown, this.SpriteCollection.Firearms1H, ((IEnumerable<Sprite>) this.Character.Firearms).Any<Sprite>() ? this.Character.Firearms[0] : (Sprite) null, new Action<SpriteGroupEntry>(this.c));
        this.A(this.Firearms2HDropdown, this.SpriteCollection.Firearms2H, ((IEnumerable<Sprite>) this.Character.Firearms).Any<Sprite>() ? this.Character.Firearms[0] : (Sprite) null, new Action<SpriteGroupEntry>(this.D));
        this.A(this.ShieldDropdown, this.SpriteCollection.Shield, this.Character.Shield, new Action<SpriteGroupEntry>(this.d));
        this.A(this.SuppliesDropdown, this.SpriteCollection.Supplies, this.Character.PrimaryMeleeWeapon, new Action<SpriteGroupEntry>(this.E));
      }
      else if (this.ExpressionEditor.get_activeSelf())
      {
        this.A(this.AngryEyebrowsDropdown, this.SpriteCollection.Eyebrows, this.Character.Expressions[1].Eyebrows, (Action<SpriteGroupEntry>) (obj0 =>
        {
          this.Character.Expressions[1].Eyebrows = CharacterEditorBase.F(obj0);
          this.Character.Expression = \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.c();
        }));
        this.A(this.AngryEyesDropdown, this.SpriteCollection.Eyes, this.Character.Expressions[1].Eyes, (Action<SpriteGroupEntry>) (obj0 =>
        {
          this.Character.Expressions[1].Eyes = CharacterEditorBase.F(obj0);
          this.Character.Expression = \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.c();
        }));
        this.A(this.AngryMouthDropdown, this.SpriteCollection.Mouth, this.Character.Expressions[1].Mouth, (Action<SpriteGroupEntry>) (obj0 =>
        {
          this.Character.Expressions[1].Mouth = CharacterEditorBase.F(obj0);
          this.Character.Expression = \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.c();
        }));
        this.A(this.DeadEyebrowsDropdown, this.SpriteCollection.Eyebrows, this.Character.Expressions[2].Eyebrows, (Action<SpriteGroupEntry>) (obj0 =>
        {
          this.Character.Expressions[2].Eyebrows = CharacterEditorBase.F(obj0);
          this.Character.Expression = \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.Y();
        }));
        this.A(this.DeadEyesDropdown, this.SpriteCollection.Eyes, this.Character.Expressions[2].Eyes, (Action<SpriteGroupEntry>) (obj0 =>
        {
          this.Character.Expressions[2].Eyes = CharacterEditorBase.F(obj0);
          this.Character.Expression = \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.Y();
        }));
        this.A(this.DeadMouthDropdown, this.SpriteCollection.Mouth, this.Character.Expressions[2].Mouth, (Action<SpriteGroupEntry>) (obj0 =>
        {
          this.Character.Expressions[2].Mouth = CharacterEditorBase.F(obj0);
          this.Character.Expression = \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.Y();
        }));
      }
      else
      {
        if (!this.ArmorPartsEditor.get_activeSelf())
          return;
        this.A();
      }
    }

    private void A()
    {
      this.A(this.UpperArmorDropdown, this.SpriteCollection.Armor, this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.D()), new Action<SpriteGroupEntry>(this.A));
      this.A(this.LowerArmorDropdown, this.SpriteCollection.Armor, this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.d()), new Action<SpriteGroupEntry>(this.a));
      this.A(this.GlovesDropdown, this.SpriteCollection.Armor, this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.E()), (Action<SpriteGroupEntry>) (obj0 => this.A(obj0, \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.E(), \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.H(), \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.h())));
      this.A(this.BootsDropdown, this.SpriteCollection.Armor, this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.e()), (Action<SpriteGroupEntry>) (obj0 => this.A(obj0, \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.e())));
      this.A(this.ArmorArmLDropdown, this.SpriteCollection.Armor, this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.F()), (Action<SpriteGroupEntry>) (obj0 => this.A(obj0, \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.F())));
      this.A(this.ArmorArmRDropdown, this.SpriteCollection.Armor, this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.f()), (Action<SpriteGroupEntry>) (obj0 => this.A(obj0, \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.f())));
      this.A(this.ArmorForearmLDropdown, this.SpriteCollection.Armor, this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.G()), (Action<SpriteGroupEntry>) (obj0 => this.A(obj0, \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.G())));
      this.A(this.ArmorForearmRDropdown, this.SpriteCollection.Armor, this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.g()), (Action<SpriteGroupEntry>) (obj0 => this.A(obj0, \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.g())));
      this.A(this.ArmorHandLDropdown, this.SpriteCollection.Armor, this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.E()), (Action<SpriteGroupEntry>) (obj0 => this.A(obj0, \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.E())));
      this.A(this.ArmorHandRDropdown, this.SpriteCollection.Armor, this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.H()), (Action<SpriteGroupEntry>) (obj0 => this.A(obj0, \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.H())));
      this.A(this.ArmorFingerDropdown, this.SpriteCollection.Armor, this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.h()), (Action<SpriteGroupEntry>) (obj0 => this.A(obj0, \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.h())));
      this.A(this.ArmorSleeveRDropdown, this.SpriteCollection.Armor, this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.I()), (Action<SpriteGroupEntry>) (obj0 => this.A(obj0, \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.I())));
      this.A(this.ArmorLegDropdown, this.SpriteCollection.Armor, this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.i()), (Action<SpriteGroupEntry>) (obj0 => this.A(obj0, \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.i())));
      this.A(this.ArmorPelvisDropdown, this.SpriteCollection.Armor, this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.d()), (Action<SpriteGroupEntry>) (obj0 => this.A(obj0, \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.d())));
      this.A(this.ArmorShinDropdown, this.SpriteCollection.Armor, this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.e()), (Action<SpriteGroupEntry>) (obj0 => this.A(obj0, \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.e())));
      this.A(this.ArmorTorsoDropdown, this.SpriteCollection.Armor, this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.D()), (Action<SpriteGroupEntry>) (obj0 => this.A(obj0, \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.D())));
    }

    private void A([In] SpriteGroupEntry obj0)
    {
      if (obj0 == null)
      {
        string[] strArray = new string[9]
        {
          \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.F(),
          \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.f(),
          \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.h(),
          \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.G(),
          \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.g(),
          \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.E(),
          \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.H(),
          \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.I(),
          \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.D()
        };
        foreach (string str in strArray)
        {
          // ISSUE: object of a compiler-generated type is created
          // ISSUE: reference to a compiler-generated method
          this.Character.Armor.RemoveAll(new Predicate<Sprite>(new CharacterEditorBase.a()
          {
            A = str
          }.A));
        }
      }
      else
      {
        this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.F(), obj0);
        this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.f(), obj0);
        this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.h(), obj0);
        this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.G(), obj0);
        this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.g(), obj0);
        this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.E(), obj0);
        this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.H(), obj0);
        this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.I(), obj0);
        this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.D(), obj0);
      }
    }

    private void a([In] SpriteGroupEntry obj0)
    {
      if (obj0 == null)
      {
        string[] strArray = new string[3]
        {
          \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.i(),
          \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.d(),
          \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.e()
        };
        foreach (string str in strArray)
        {
          // ISSUE: object of a compiler-generated type is created
          // ISSUE: reference to a compiler-generated method
          this.Character.Armor.RemoveAll(new Predicate<Sprite>(new CharacterEditorBase.B()
          {
            A = str
          }.A));
        }
      }
      else
      {
        this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.i(), obj0);
        this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.d(), obj0);
        this.A(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.e(), obj0);
      }
    }

    private Sprite A([In] string obj0)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: reference to a compiler-generated method
      return ((IEnumerable<Sprite>) this.Character.Armor).SingleOrDefault<Sprite>(new Func<Sprite, bool>(new CharacterEditorBase.b()
      {
        A = obj0
      }.A));
    }

    private void A([In] string obj0, [In] SpriteGroupEntry obj1)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      CharacterEditorBase.C c = new CharacterEditorBase.C();
      // ISSUE: reference to a compiler-generated field
      c.A = obj0;
      // ISSUE: reference to a compiler-generated method
      Sprite sprite = obj1 != null ? ((IEnumerable<Sprite>) obj1.Sprites).SingleOrDefault<Sprite>(new Func<Sprite, bool>(c.A)) : (Sprite) null;
      for (int index = 0; index < this.Character.Armor.Count; ++index)
      {
        // ISSUE: reference to a compiler-generated field
        if (Object.op_Inequality((Object) this.Character.Armor[index], (Object) null) && ((Object) this.Character.Armor[index]).get_name() == c.A)
        {
          if (Object.op_Equality((Object) sprite, (Object) null))
          {
            this.Character.Armor.RemoveAt(index);
            return;
          }
          this.Character.Armor[index] = sprite;
          return;
        }
      }
      if (!Object.op_Inequality((Object) sprite, (Object) null))
        return;
      this.Character.Armor.Add(sprite);
    }

    private void B([In] SpriteGroupEntry obj0)
    {
      this.A(this.MeleeWeapon1HDropdown, this.MeleeWeaponPairedDropdown, this.ShieldDropdown);
      this.Character.PrimaryMeleeWeapon = CharacterEditorBase.F(obj0);
      this.Character.PrimaryMeleeWeaponTrailRenderer.set_sprite(obj0?.LinkedSprite);
      this.Character.WeaponType = this.Character.WeaponType == WeaponType.MeleePaired ? WeaponType.MeleePaired : WeaponType.Melee1H;
      this.Character.UpdateAnimation();
      if (obj0 == null || !Object.op_Equality((Object) obj0.LinkedSprite, (Object) null))
        return;
      Debug.LogWarningFormat(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.J(), new object[1]
      {
        (object) obj0.Name
      });
    }

    private void b([In] SpriteGroupEntry obj0)
    {
      this.A(this.MeleeWeapon2HDropdown);
      this.Character.PrimaryMeleeWeapon = CharacterEditorBase.F(obj0);
      this.Character.PrimaryMeleeWeaponTrailRenderer.set_sprite(obj0?.LinkedSprite);
      this.Character.WeaponType = WeaponType.Melee2H;
      this.Character.UpdateAnimation();
      if (obj0 == null || !Object.op_Equality((Object) obj0.LinkedSprite, (Object) null))
        return;
      Debug.LogWarningFormat(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.J(), new object[1]
      {
        (object) obj0.Name
      });
    }

    private void C([In] SpriteGroupEntry obj0)
    {
      this.A(this.MeleeWeapon1HDropdown, this.MeleeWeaponPairedDropdown);
      this.Character.SecondaryMeleeWeapon = CharacterEditorBase.F(obj0);
      this.Character.SecondaryMeleeWeaponTrailRenderer.set_sprite(obj0?.LinkedSprite);
      this.Character.WeaponType = WeaponType.MeleePaired;
      this.Character.UpdateAnimation();
      if (obj0 == null || !Object.op_Equality((Object) obj0.LinkedSprite, (Object) null))
        return;
      Debug.LogWarningFormat(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.J(), new object[1]
      {
        (object) obj0.Name
      });
    }

    private void c([In] SpriteGroupEntry obj0)
    {
      this.A(this.Firearms1HDropdown, this.ShieldDropdown);
      this.Character.Firearms = CharacterEditorBase.f(obj0);
      this.Character.WeaponType = WeaponType.Firearms1H;
      this.SetFirearmParams(obj0);
      this.Character.UpdateAnimation();
    }

    private void D([In] SpriteGroupEntry obj0)
    {
      this.A(this.Firearms2HDropdown);
      this.Character.Firearms = CharacterEditorBase.f(obj0);
      this.Character.WeaponType = WeaponType.Firearms2H;
      this.SetFirearmParams(obj0);
      this.Character.UpdateAnimation();
    }

    private void A([In] SpriteGroupEntry obj0, params string[] parts)
    {
      foreach (string part in parts)
        this.A(part, obj0);
    }

    private void A(params Dropdown[] except)
    {
      using (IEnumerator<Dropdown> enumerator = ((IEnumerable<Dropdown>) new Dropdown[8]
      {
        this.MeleeWeapon1HDropdown,
        this.MeleeWeaponPairedDropdown,
        this.MeleeWeapon2HDropdown,
        this.BowDropdown,
        this.Firearms1HDropdown,
        this.Firearms2HDropdown,
        this.ShieldDropdown,
        this.SuppliesDropdown
      }).Except<Dropdown>((IEnumerable<Dropdown>) except).GetEnumerator())
      {
        while (((IEnumerator) enumerator).MoveNext())
          enumerator.Current.set_value(0);
      }
    }

    private void d([In] SpriteGroupEntry obj0)
    {
      this.A(this.ShieldDropdown, this.MeleeWeapon1HDropdown, this.Firearms1HDropdown);
      this.Character.Shield = CharacterEditorBase.F(obj0);
      if (this.Character.WeaponType != WeaponType.Melee1H && this.Character.WeaponType != WeaponType.Firearms1H)
        this.Character.WeaponType = WeaponType.Melee1H;
      this.Character.UpdateAnimation();
    }

    private void E([In] SpriteGroupEntry obj0)
    {
      this.A(this.SuppliesDropdown, this.ShieldDropdown, this.MeleeWeaponPairedDropdown);
      this.Character.PrimaryMeleeWeapon = CharacterEditorBase.F(obj0);
      this.Character.WeaponType = WeaponType.Supplies;
      this.Character.UpdateAnimation();
    }

    private void e([In] SpriteGroupEntry obj0)
    {
      this.A(this.BowDropdown);
      this.Character.Bow = CharacterEditorBase.f(obj0);
      this.Character.WeaponType = WeaponType.Bow;
      this.Character.UpdateAnimation();
    }

    private static Sprite F([In] SpriteGroupEntry obj0)
    {
      return obj0?.Sprite;
    }

    private static List<Sprite> f([In] SpriteGroupEntry obj0)
    {
      return obj0 != null ? obj0.Sprites : new List<Sprite>();
    }

    private void A(
      [In] Dropdown obj0_1,
      [In] List<SpriteGroupEntry> obj1,
      [In] Sprite obj2,
      [In] Action<SpriteGroupEntry> obj3)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      CharacterEditorBase.c c = new CharacterEditorBase.c();
      // ISSUE: reference to a compiler-generated field
      c.A = this;
      // ISSUE: reference to a compiler-generated field
      c.A = obj0_1;
      // ISSUE: reference to a compiler-generated field
      c.A = obj3;
      // ISSUE: reference to a compiler-generated field
      ((UnityEventBase) c.A.get_onValueChanged()).RemoveAllListeners();
      // ISSUE: reference to a compiler-generated field
      Dropdown a = c.A;
      List<Dropdown.OptionData> optionDataList = new List<Dropdown.OptionData>();
      optionDataList.Add(new Dropdown.OptionData(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.j()));
      a.set_options(optionDataList);
      // ISSUE: reference to a compiler-generated field
      c.A.set_value(0);
      // ISSUE: reference to a compiler-generated field
      c.A = new List<SpriteGroupEntry>()
      {
        (SpriteGroupEntry) null
      };
      foreach (IGrouping<string, SpriteGroupEntry> grouping in obj1.OrderBy<SpriteGroupEntry, string>((Func<SpriteGroupEntry, string>) (obj0_2 => obj0_2.Collection)).ThenBy<SpriteGroupEntry, string>((Func<SpriteGroupEntry, string>) (obj0_2 => obj0_2.Name)).ToList<SpriteGroupEntry>().GroupBy<SpriteGroupEntry, string>((Func<SpriteGroupEntry, string>) (obj0_2 => obj0_2.Collection)))
      {
        // ISSUE: reference to a compiler-generated field
        c.A.get_options().Add(new Dropdown.OptionData(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.K() + grouping.Key + \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.k()));
        // ISSUE: reference to a compiler-generated field
        c.A.Add((SpriteGroupEntry) null);
        foreach (SpriteGroupEntry spriteGroupEntry in (IEnumerable<SpriteGroupEntry>) grouping)
        {
          // ISSUE: reference to a compiler-generated field
          c.A.get_options().Add(new Dropdown.OptionData(spriteGroupEntry.Name));
          // ISSUE: reference to a compiler-generated field
          c.A.Add(spriteGroupEntry);
          if (Object.op_Inequality((Object) obj2, (Object) null) && spriteGroupEntry.Sprites.Contains(obj2))
          {
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            c.A.set_value(c.A.get_options().Count - 1);
          }
        }
        if (Object.op_Equality((Object) obj2, (Object) null))
        {
          // ISSUE: reference to a compiler-generated field
          c.A.set_value(0);
        }
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.A((Component) c.A, c.A[c.A.get_value()]);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      c.A = new UnityAction<int>((object) c, __methodptr(A));
      // ISSUE: reference to a compiler-generated field
      c.A.RefreshShownValue();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      ((UnityEvent<int>) c.A.get_onValueChanged()).AddListener(c.A);
      EventTrigger.Entry entry1 = new EventTrigger.Entry()
      {
        eventID = (__Null) 3
      };
      EventTrigger.Entry entry2 = new EventTrigger.Entry()
      {
        eventID = (__Null) 1
      };
      // ISSUE: method pointer
      ((UnityEvent<BaseEventData>) entry1.callback).AddListener(new UnityAction<BaseEventData>((object) c, __methodptr(A)));
      // ISSUE: method pointer
      ((UnityEvent<BaseEventData>) entry2.callback).AddListener(new UnityAction<BaseEventData>((object) c, __methodptr(a)));
      // ISSUE: reference to a compiler-generated field
      M0 component = ((Component) c.A).GetComponent<EventTrigger>();
      List<EventTrigger.Entry> entryList = new List<EventTrigger.Entry>();
      entryList.Add(entry1);
      entryList.Add(entry2);
      ((EventTrigger) component).set_triggers(entryList);
    }

    private void A([In] Component obj0, [In] SpriteGroupEntry obj1)
    {
      PaletteButton componentInChildren = (PaletteButton) ((Component) obj0.get_transform().get_parent()).GetComponentInChildren<PaletteButton>(true);
      if (!Object.op_Inequality((Object) componentInChildren, (Object) null))
        return;
      if (obj1 == null)
      {
        ((Selectable) ((Component) componentInChildren).GetComponent<Button>()).set_interactable(false);
      }
      else
      {
        bool flag = this.ForcePaint || !componentInChildren.Optional || obj1.Name.Contains(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.L()) || obj1.Collection.Contains(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.L());
        if (obj1.Name.Contains(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.l()) || obj1.Collection.Contains(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.l()))
          flag = false;
        ((Selectable) ((Component) componentInChildren).GetComponent<Button>()).set_interactable(flag);
        using (List<SpriteRenderer>.Enumerator enumerator = this.a(((Object) obj0.get_transform().get_parent()).get_name()).GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            SpriteRenderer current = enumerator.Current;
            if (flag)
            {
              if (Object.op_Equality((Object) obj0, (Object) this.A))
                current.set_color(this.A);
            }
            else if (((Object) current).get_name() != \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.M())
            {
              if (Object.op_Inequality((Object) obj0, (Object) this.A))
              {
                this.A = obj0;
                this.A = current.get_color();
              }
              current.set_color(Color.get_white());
            }
            if (this.ForceDefaultMaterial)
              ((Renderer) current).set_material(this.DefaultMaterial);
          }
        }
      }
    }

    private static IEnumerator A(
      [In] Dropdown obj0,
      [In] List<SpriteGroupEntry> obj1,
      [In] UnityAction<int> obj2)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new CharacterEditorBase.E(0)
      {
        A = obj0,
        A = obj1,
        A = obj2
      };
    }

    private List<SpriteRenderer> a([In] string obj0)
    {
      // ISSUE: reference to a compiler-generated method
      switch (A.a.A(obj0))
      {
        case 293362042:
          if (obj0 == \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.U())
          {
            List<SpriteRenderer> spriteRendererList = new List<SpriteRenderer>();
            spriteRendererList.Add(this.Character.SecondaryMeleeWeaponRenderer);
            return spriteRendererList;
          }
          goto default;
        case 373104978:
          if (obj0 == \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.T())
            break;
          goto default;
        case 995063962:
          if (obj0 == \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.S())
          {
            List<SpriteRenderer> spriteRendererList = new List<SpriteRenderer>();
            spriteRendererList.Add(this.Character.CapeRenderer);
            return spriteRendererList;
          }
          goto default;
        case 1116344469:
          if (obj0 == \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.n())
            return this.Character.BodyRenderers;
          goto default;
        case 1314418569:
          if (obj0 == \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.R())
          {
            List<SpriteRenderer> spriteRendererList = new List<SpriteRenderer>();
            spriteRendererList.Add(this.Character.MaskRenderer);
            return spriteRendererList;
          }
          goto default;
        case 1424250329:
          if (obj0 == \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.O())
          {
            List<SpriteRenderer> spriteRendererList = new List<SpriteRenderer>();
            spriteRendererList.Add(this.Character.HairRenderer);
            return spriteRendererList;
          }
          goto default;
        case 1714767456:
          if (obj0 == \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.N())
          {
            List<SpriteRenderer> spriteRendererList = new List<SpriteRenderer>();
            spriteRendererList.Add(this.Character.EarsRenderer);
            return spriteRendererList;
          }
          goto default;
        case 2039097040:
          if (obj0 == \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.W())
          {
            List<SpriteRenderer> spriteRendererList = new List<SpriteRenderer>();
            spriteRendererList.Add(this.Character.ShieldRenderer);
            return spriteRendererList;
          }
          goto default;
        case 2226667892:
          if (obj0 == \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.r())
            return this.Character.ArmorRenderers;
          goto default;
        case 2326062884:
          if (obj0 == \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.V())
            goto label_37;
          else
            goto default;
        case 2865582955:
          if (obj0 == \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.o())
          {
            List<SpriteRenderer> spriteRendererList = new List<SpriteRenderer>();
            spriteRendererList.Add(this.Character.EyesRenderer);
            return spriteRendererList;
          }
          goto default;
        case 2959601814:
          if (obj0 == \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.w())
          {
            List<SpriteRenderer> spriteRendererList = new List<SpriteRenderer>();
            spriteRendererList.Add(this.Character.PrimaryMeleeWeaponRenderer);
            return spriteRendererList;
          }
          goto default;
        case 2996251363:
          if (obj0 == \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.m())
          {
            List<SpriteRenderer> spriteRendererList = new List<SpriteRenderer>();
            spriteRendererList.Add(this.Character.HeadRenderer);
            return spriteRendererList;
          }
          goto default;
        case 3264564162:
          if (obj0 == \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.s())
          {
            List<SpriteRenderer> spriteRendererList = new List<SpriteRenderer>();
            spriteRendererList.Add(this.Character.BackRenderer);
            return spriteRendererList;
          }
          goto default;
        case 3364388149:
          if (obj0 == \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.P())
          {
            List<SpriteRenderer> spriteRendererList = new List<SpriteRenderer>();
            spriteRendererList.Add(this.Character.BeardRenderer);
            return spriteRendererList;
          }
          goto default;
        case 3590872896:
          if (obj0 == \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.p())
          {
            List<SpriteRenderer> spriteRendererList = new List<SpriteRenderer>();
            spriteRendererList.Add(this.Character.HelmetRenderer);
            return spriteRendererList;
          }
          goto default;
        case 3637216139:
          if (obj0 == \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.v())
            return this.Character.BowRenderers;
          goto default;
        case 3665767010:
          if (obj0 == \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.Q())
          {
            List<SpriteRenderer> spriteRendererList = new List<SpriteRenderer>();
            spriteRendererList.Add(this.Character.EarringsRenderer);
            return spriteRendererList;
          }
          goto default;
        case 3970710831:
          if (obj0 == \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.u())
            goto label_37;
          else
            goto default;
        case 4012353755:
          if (obj0 == \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.q())
          {
            List<SpriteRenderer> spriteRendererList = new List<SpriteRenderer>();
            spriteRendererList.Add(this.Character.GlassesRenderer);
            return spriteRendererList;
          }
          goto default;
        case 4030375657:
          if (obj0 == \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.t())
            break;
          goto default;
        default:
          return (List<SpriteRenderer>) null;
      }
      List<SpriteRenderer> spriteRendererList1 = new List<SpriteRenderer>();
      spriteRendererList1.Add(this.Character.PrimaryMeleeWeaponRenderer);
      return spriteRendererList1;
label_37:
      return this.Character.FirearmsRenderers;
    }

    private GameObject B([In] string obj0)
    {
      return obj0 == \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.m() || obj0 == \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.N() || obj0 == \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.n() ? this.SkinPalette : this.CommonPalette;
    }

    private void a()
    {
      if (PlayerPrefs.HasKey(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.X()) && (DateTime.UtcNow - new DateTime(long.Parse(PlayerPrefs.GetString(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.X())))).TotalDays < 7.0)
        return;
      if (!PlayerPrefs.HasKey(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.x()))
      {
        string str1 = \u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.x();
        DateTime dateTime = DateTime.UtcNow;
        dateTime = dateTime.AddHours(-23.0);
        string str2 = dateTime.Ticks.ToString();
        PlayerPrefs.SetString(str1, str2);
      }
      else
      {
        if ((DateTime.UtcNow - new DateTime(long.Parse(PlayerPrefs.GetString(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.x())))).TotalDays <= 1.0)
          return;
        this.FeedbackTip();
      }
    }

    protected void RequestFeedbackResult(bool success, bool basic)
    {
      if (success)
      {
        PlayerPrefs.SetString(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.X(), DateTime.UtcNow.Ticks.ToString());
        Application.OpenURL(basic ? this.LinkToBasicVersion : this.LinkToProVersion);
      }
      else if (PlayerPrefs.HasKey(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.X()))
        PlayerPrefs.SetString(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.x(), DateTime.UtcNow.AddDays(6.0).Ticks.ToString());
      else
        PlayerPrefs.SetString(\u00392021ECF\u002DBC6D\u002D4200\u002DB1D5\u002D2DBF42B793BF.x(), DateTime.UtcNow.Ticks.ToString());
    }

    protected CharacterEditorBase()
    {
      base.\u002Ector();
    }
  }
}
