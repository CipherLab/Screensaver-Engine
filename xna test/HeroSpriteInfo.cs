using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGameTest
{
    public class HeroSpriteInfo
    {
        public string Category { get; set; }
        public string ItemType { get; set; }
        public string Plan { get; set; }
        public string Style { get; set; }
        public string FileName { get; set; }
    }
    public enum ObjectType
    {
        BodyParts,
        Equipment
    }
    public enum Name
    {
        Armor,
        Back,
        Beard,
        Body,
        Bow,
        Cape,
        Earrings,
        Ears,
        Eyebrows,
        Eyes,
        Glasses,
        Hair,
        Head,
        Helmet,
        Mask,
        MeleeWeapon1H,
        MeleeWeapon2H,
        MeleeWeaponTrail1H,
        MeleeWeaponTrail2H,
        Mouth,
        Other,
        Shield,
        Supplies
    }
    public enum Style
    {
        Basic,
        Bonus,
        Christmas,
        Emoji,
        Expressions,
        HeadMask,
        Knights,
        Military_Preview,
        Pro,
        Samurai,
        SandLords,
        Shadow,
        SpellMasters,
        Style_Preview_NoPaint,
        SwampLords,
        Thrones,
        Undead,
        Undead_Preview,
        Vikings
    }
}
