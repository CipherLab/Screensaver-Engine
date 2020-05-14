using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Nez.Sprites;
using ScreenSaverHelper;
using ScreenSaverHelper.Util;
using SharedKernel.Interfaces;
using Graphics = Nez.Graphics;
using Rectangle = System.Drawing.Rectangle;

namespace MonoGameTest
{
    public class HeroSpriteInfoDto
    {
        public string Category { get; set; }
        public string Class { get; set; }
        public string FileName { get; set; }
        public Dictionary<string, SpriteInfo> Position { get; set; }
        [JsonIgnore]
        public string FullFileName
        {
            get
            {
                string ifename = Path.Combine(Environment.CurrentDirectory, "Sprites");
                ifename = Path.Combine(ifename, "FantasyHeroes");
                ifename = Path.Combine(ifename, $"{this.Category}");
                ifename = Path.Combine(ifename, $"{this.Class}");
                ifename = Path.Combine(ifename, $"{this.FileName}");
                return ifename;
            }
        }
    }

    //public class SpriteInfo
    //{
    //    public SpriteInfo(Rectangle rectangle, string name)
    //    {
    //        Position = rectangle;
    //        Name = name;
    //    }

    //    public Rectangle Position { get; }
    //    public string Name { get; }

    //    public byte[] ImageData { get; set; }

    //}
    //public class SpriteParts
    //{
    //    public SpriteInfo Torso = new SpriteInfo(
    //    public SpriteInfo ArmR = new SpriteInfo( 
    //    public SpriteInfo ArmL = new SpriteInfo( 
    //}

    //public class HeroSpriteCharacter
    //{
    //    private List<HeroSpriteInfoDto> BodyParts { get; set; }
    //    private HeroSpriteInfoDto Armor { get; set; }
    //    private List<HeroSpriteInfoDto> Equipment { get; set; }
    //    private HeroSpriteInfoDto Helmet { get; set; }
    //    private HeroSpriteInfoDto Shield { get; set; }
    //    private List<HeroSpriteInfoDto> Supplies { get; set; }
    //    private HeroSpriteInfoDto Weapon { get; set; }

    //    public HeroSpriteCharacter()
    //    {

    //    }
    //}
    public class SpriteInfo
    {
        public SpriteInfo(Rectangle position, PointF partPosition, PointF pivot, bool flipH)
        {
            PartPosition = partPosition;
            Position = position;
            Pivot = pivot;
            FlipH = flipH;
        }

        public Rectangle Position { get; }
        public PointF Pivot { get; }
        [JsonIgnore]
        public byte[] ImageData { get; set; }
        public PointF PartPosition { get; }
        public bool FlipH { get; }
    }

    public class HeroSpriteInfoRepo
    {
        [JsonIgnore]
        private List<HeroSpriteInfoDto> HeroSprites { get; }

        public HeroSpriteInfoRepo()
        {
            HeroSprites = new List<HeroSpriteInfoDto>();

            string spriteInfo = Path.Combine(Environment.CurrentDirectory, @"HeroSpriteInfo.json");
            HeroSprites = JsonConvert.DeserializeObject<List<HeroSpriteInfoDto>>(File.ReadAllText(spriteInfo));
            //var bodySprite = HeroSprites.FirstOrDefault(x =>
            //    x.Category == "BodyParts" &&
            //    x.Class == "Body");

        }

        private static void GetSpriteImageData(HeroSpriteInfoDto bodySprite)
        {
            using (IHeroSpriteImageHelper ih = new ImageHelper(bodySprite.FullFileName))
            {
                foreach (var pos in bodySprite.Position)
                {
                    ICroppedImagePart imgPart = ih.GetSpriteFromImage(pos.Value.Position, pos.Value.FlipH, false);
                    pos.Value.ImageData = imgPart.ImageData;
                }
            }
        }

        public HeroSpriteInfoDto GetHumanBody()
        {
            HeroSpriteInfoDto bodySprite = HeroSprites.FirstOrDefault(x =>
                x.Category == "BodyParts" &&
                x.Class == "Body" &&
                x.FileName.StartsWith("Human"));
            GetSpriteImageData(bodySprite);
            return bodySprite;
        }

        public HeroSpriteInfoDto GetSkeleBody()
        {
            HeroSpriteInfoDto bodySprite = HeroSprites.FirstOrDefault(x =>
                x.Category == "BodyParts" &&
                x.Class == "Body" &&
                x.FileName.StartsWith("Skeleton"));
            GetSpriteImageData(bodySprite);
            return bodySprite;
        }
        //public HeroSpriteInfo GetSprite(Category category, Name name, Plan plan, Style style, FileName fileName)
        //{
        //    return HeroSprites.FirstOrDefault(s =>
        //        s.Category == category.ToString() &&
        //        s.Name == name.ToString() &&
        //        s.Plan == plan.ToString() &&
        //        s.Style == style.ToString() &&
        //        s.FileName == fileName.ToString() 
        //    );
        //}
        //public IEnumerable<HeroSpriteInfo> GetSprite(Category category, Name name, Plan plan, Style style)
        //{
        //    return HeroSprites.Where(s =>
        //        s.Category == category.ToString() &&
        //        s.Name == name.ToString() &&
        //        s.Plan == plan.ToString() &&
        //        s.Style == style.ToString() 
        //    );
        //}
        //public IEnumerable<HeroSpriteInfo> GetSprite(Category category, Name name, Plan plan)
        //{
        //    return HeroSprites.Where(s =>
        //        s.Category == category.ToString() &&
        //        s.Name == name.ToString() &&
        //        s.Plan == plan.ToString() 
        //    );
        //}
        //public IEnumerable<HeroSpriteInfo> GetSprite(Category category, Name name)
        //{
        //    return HeroSprites.Where(s =>
        //        s.Category == category.ToString() &&
        //        s.Name == name.ToString() 
        //    );
        //}
        //public IEnumerable<HeroSpriteInfo> GetCategories(Category category)
        //{
        //    return HeroSprites.Where(s =>
        //        s.Category == category.ToString() 
        //    );
        //}
        //public IEnumerable<HeroSpriteInfo> GetNames(Name name)
        //{
        //    return HeroSprites.Where(s =>
        //        s.Name == name.ToString() 
        //    );
        //}
        //public IEnumerable<HeroSpriteInfo> GetPlans(Plan plan)
        //{
        //    return HeroSprites.Where(s =>
        //        s.Plan == plan.ToString()
        //    );
        //}
        //public IEnumerable<HeroSpriteInfo> GetStyles(Style style)
        //{
        //    return HeroSprites.Where(s =>
        //        s.Style == style.ToString()
        //    );
        //}
        //public IEnumerable<HeroSpriteInfo> GetStyles(FileName fileName)
        //{
        //    return HeroSprites.Where(s =>
        //        s.FileName == fileName.ToString()
        //    );
        //}

    }
    /*
    public class Category1
    {
        private static int CurrentId = 0;

        private readonly int id;

        private Category1()
        {

            id = CurrentId;
            CurrentId++;
        }
        public static class BodyParts
        {
            public static Category1 s = new Category1();
            public static Category1 Equipment = new Category1();
        }
    }
    public enum Category
    {
        Armor,
        BodyParts,
        Equipment,
        Helmet,
        Shield,
        Supplies,
        Weapons
    }
    public enum Armor
    {
        Basic,
        Christmas,
        Knights,
        Military,
        Samurai,
        SandLords,
        SpellMasters,
        SwampLords,
        Thrones,
        Undead,
        Vikings
    }
    public enum BodyParts
    {
        Beard,
        Body,
        Ears,
        Eyebrows,
        Eyes,
        Hair,
        Head,
        Mouth,
    }

    public enum Equipment
    {
        Back,
        Cape,
        Earrings,
        Glasses,
        Mask,
    }
    public enum Helmets
    {
        Basic,
        Christmas,
        Knights,
        Military,
        Samurai,
        SandLords,
        SpellMasters,
        SwampLords,
        Thrones,
        Undead,
        Vikings,
    }
    public enum Shields
    {
        Basic,
        Knights,
        Military,
        Samurai,
        SpellMasters,
        Undead,
        Vikings,
    }
    public enum Supplies
    {
        Bombs,
        Food,
        Potions,
    }
    public enum Weapons
    {
        Axe,
        Bow,
        Dagger,
        Hammer,
        Spear,
        Staff,
        Sword,
        Throwing,
        TwoHAxe,
        TwoHHammer,
        TwoHSword,
        Wand,
    }


    public enum FileName
    {
        Apple,
        AspenStake,
        AssassinDagger_Paint,
        Axeorang,
        BalancedSword_Paint,
        BerserkAxe,
        Bilbo,
        BishopWand,
        BlackHammer,
        BoneClub,
        Boomerang_Paint,
        Boomerang,
        Bottle,
        BroadSword,
        BronzeDagger,
        CardinalWand,
        CataphractSpear,
        Cheese,
        ChickenLeg,
        ClericWand1,
        ClericWand2,
        CursedBone,
        DarkStoneHammer,
        DarkSword,
        DeserterDagger_Paint,
        DestroyerAxe,
        DreadCleaver,
        DrownedGoblinHand,
        DruidWand,
        DwellerWand,
        ElderStaff,
        EliteGuardSword_Paint,
        EliteKnightSword_Paint,
        FamilySword,
        FireAdeptWand1,
        FireAdeptWand2,
        FireKnightSword_Paint,
        FireWarriorSword1,
        FireWarriorSword2,
        FireWizardWand,
        FreeFolkAxe,
        GateCrusher,
        Grog,
        GuardSword1_Paint,
        GuardSword2_Paint,
        Halberd,
        Ham,
        Hamburger,
        HandBomb,
        HardwoodWand,
        Harpoon,
        HeavySword_Paint,
        HermitWand,
        HorseLordSickle,
        HunterKnife_Paint,
        IceWizardWand,
        InquisitorSword1_Paint,
        InquisitorSword2_Paint,
        InquisitorSword3_Paint,
        InquisitorSword4_Paint,
        IronIslandsSword_Paint,
        Katana1_Paint,
        Katana2_Paint,
        Katana3_Paint,
        KnightSword_Paint,
        Kunai,
        LagrePotion_Paint,
        LagrePotion,
        LionSword_Paint,
        LittleLionSword_Paint,
        LongHammer,
        Machete_Paint,
        MagicBomb,
        MagicSword_Paint,
        MalformedStaff,
        ManticoreSword,
        MediumPotion_Paint,
        MediumPotion,
        MercenarySword1_Paint,
        MercenarySword2_Paint,
        MilitiamanShortSword_Paint,
        Morgana,
        Morgenstern,
        MountainSword_Paint,
        Ninjato_Paint,
        NorthWardenSword_Paint,
        OldPoleaxe,
        PaladinSword_Paint,
        Potion1_Paint,
        Potion2_Paint,
        Potion3_Paint,
        PyromancerWand,
        RunicAxe,
        RustySword,
        Sai_Paint,
        SamuraiElderSword,
        SamuraiSword1,
        SamuraiSword2_Paint,
        SamuraiSword2,
        SamuraiSword3_Paint,
        SamuraiSword3,
        SandElderStaff,
        SandHealerStaff1,
        SandHealerStaff2,
        SandMageStaff1,
        SandMageStaff2,
        SandWarriorHalberd,
        SandWarriorSword1,
        SandWarriorSword2,
        SantaHelperWand,
        SantaWand,
        SavageBoomerang,
        Scythe,
        ShadowSword,
        ShamanWand,
        ShortDagger_Paint,
        ShortIronKatana_Paint,
        ShortKnightSword_Paint,
        ShortSword_Paint,
        SiegeSpear,
        SlaveSword_Paint,
        SmallAxe,
        SmallPotion_Paint,
        SmallPotion,
        Smallsword,
        Spear,
        SpikeBomb,
        SteelAxe,
        Stoccata,
        StoneAxe,
        StormBringerWand,
        SunriseSword,
        SwampHealer1Staff,
        SwampHealer2Staff,
        SwampMage1Staff,
        SwampMage2Staff,
        SwampWarriorSpear,
        ThrowingStar,
        ThrowingStar2,
        TitanHammer,
        TrainingSword_Paint,
        TribalBoomerang,
        TwoHandedSword_Paint,
        TwoHandedVikingAxe1,
        TwoHandedVikingAxe2,
        TwoHandedVikingAxe3,
        TwoHandedVikingSword1,
        TwoHandedVikingSword2_Paint,
        TwoHandedVikingSword3_Paint,
        TwoHandedWoodenMace,
        VikingAxe1,
        VikingAxe2,
        VikingAxe3,
        VikingSword1,
        VikingSword2_Paint,
        VikingSword3_Paint,
        WarClub,
        WarHammer,
        WarlockWand,
        WaterAdeptWand1,
        WaterAdeptWand2,
        WaterBag,
        WaterWizardWand,
        WitchWand,
        WolfSword_Paint,
        WoodcutterAxe,
        WoodcutterPoleaxe,
        WoodenClub,
        WoodenMace,
        WoodenSpikedClub,
        WoodenStake,
        Zweihander_Paint,
        //1,
        //10,
        //11,
        //2,
        //3,
        //4,
        //5,
        //6,
        //7,
        //8,
        //83,
        //9,
        AdvancedDipperHat,
        Afro,
        AfroSmall,
        AgileHat,
        AmateurDipperHat,
        AmateurDodgeHat,
        Angry,
        AnimeGirl,
        Annoyed,
        ArcherHat,
        ArielDress_Paint,
        Army,
        ArrowQuiver,
        ArrowQuiver2,
        ArrowQuiver3,
        ArrowQuiverBig,
        AssassinArmor_Paint,
        AssassinHood_Paint,
        Banan,
        BanditArmor,
        BanditLightArmor_Paint,
        BanditMask_Paint,
        BatCape_Paint,
        BattleBow,
        Beard,
        Beard1,
        Beard2,
        Beard3,
        Beard4,
        BerserkArmor,
        BerserkHelm,
        BishopArmor,
        BishopHat_FullHair,
        Blade,
        Blank,
        Bored,
        BottleBomb,
        BowlCut,
        BowmanHat_Paint,
        Bundle,
        BunnyFace,
        BuzzCut,
        CardinalArmor,
        CardinalBook,
        CardinalHat_Paint,
        CarnivalMask,
        CataphractArmor_Paint,
        CataphractHelm_Paint,
        ChainmailLightArmor_Paint,
        ChampionArmor,
        ChampionHelm,
        Cleric_Paint,
        ClericHood_Paint,
        Clumsy,
        Confused,
        CoolBoy,
        CotttonCape_Paint,
        CrusaderChainmail_Paint,
        CrusaderHelm_Paint,
        CrusaderShield,
        Crying,
        Dandy,
        DarkKnight,
        DarkMountain_Paint,
        DarkMountain,
        Dead,
        Deserter,
        DeserterBandage_FullHair,
        DestroyerArmor_Paint,
        DestroyerHelm_Paint,
        Dizzy,
        Drooling,
        DruidArmor,
        DruidHat_Paint,
        DwellerArmor,
        DwellerHood_Paint,
        Eh,
        ElfEar,
        EliteGuardArmor_Paint,
        EliteKnightHelm,
        EnforcedHelm_Paint,
        Enrage,
        ExpertBowmanHat,
        ExpertHunterHat,
        Eyebrows1,
        Eyebrows2,
        Eyebrows3,
        Eyebrows4,
        Eyebrows6,
        Eyes1,
        Eyes10,
        Eyes11,
        Eyes12,
        Eyes13,
        Eyes14,
        Eyes15,
        Eyes2,
        Eyes3,
        Eyes4,
        Eyes5,
        Eyes6,
        Eyes7,
        Eyes8,
        Eyes9,
        FalconIronHelm,
        FalconLightArmor,
        FamilyBow,
        FarmerClothes,
        FauxHawk,
        Female,
        FireAdept,
        FireAdeptHood,
        FireKnightArmor_Paint,
        FireWarriorArmor,
        FireWarriorShield,
        FireWizardArmor,
        FireWizardHood,
        Fishy,
        FreeFolk_Paint,
        FringedUp,
        GguardsmenArmor_Paint,
        GladiatorArmor,
        GladiatorHelm1,
        GladiatorHelm2,
        GladiatorShield,
        GlowingEyes,
        GoldDragon,
        GoldenEarring,
        GoldLion,
        GrandmaCape_Paint,
        Greedy,
        GreyKnight,
        GuardArmor_Paint,
        GuardHelm_Paint,
        GuardHelm1_Paint,
        GuardHelm2_Paint,
        GuardHelm3_Paint,
        GuardShield1,
        GuardShield2,
        GypsyHat,
        Hair1,
        Hair2_HideEars,
        Hair3,
        Hair4,
        Hair5,
        Hair6,
        Hair7_HideEars,
        HalberdierArmor_Paint,
        HalberdierHelm_Paint,
        HalfmoonHat,
        Happy,
        HawkHelm,
        Head,
        HeadScar,
        HeavyArmor_Paint,
        HeavyHelm_Paint,
        Hermit,
        HermitHat,
        HermitHood_Paint,
        HermitLamp,
        HeroicCape_Paint,
        HeroicCape2_Paint,
        HornBow,
        HorseLord,
        Human,
        HumanEar,
        Hunter,
        HunterBow,
        HunterHat,
        HunterHat1,
        HunterHat2,
        HunterHelm_Paint,
        HunterLightArmor,
        HunterShortBow,
        HunterTunic,
        IceWizardArmor,
        IceWizardHood,
        Injured,
        InLove,
        InquisitorArmor,
        InquisitorHat1_Paint,
        InquisitorHat2_Paint,
        IronArmor_Paint,
        IronIslands,
        IronPlateArmor_Paint,
        IronShield1,
        IronShield2,
        IronShield3,
        JesterHat,
        KiteShield,
        Kitty,
        KnightArmor_Paint,
        KnightArmor,
        KnightHelm_Paint,
        KnightShield,
        LandsknechtArmor,
        LandsknechtHelm_Paint,
        Leaky,
        LeatherCap,
        LeatherHelm,
        LeatherJacket,
        LeatherLightArmor,
        LeatherRivetedArmor,
        LibrarianGlasses,
        LittleLion,
        LongHair,
        Male,
        ManticoreArmor,
        ManticoreHelmet,
        MarauderArmor_Paint,
        MarauderBow,
        MarauderHood,
        Mask1_Paint,
        Mask2_Paint,
        MasterDodgeHat,
        MercenaryArmor_Paint,
        MercenaryHelm1_Paint,
        MercenaryHelm2_Paint,
        Militiaman,
        MilitiamanHelmet1_Paint,
        MilitiamanHelmet2_Paint,
        MilitiamanShield,
        Mohawk,
        Mountain_Paint,
        NecromancerArmor,
        NecromancerHood,
        NightsWatch_Paint,
        Ninja1_Paint,
        Ninja2_Paint,
        Ninja3,
        NinjaHelm1_Paint,
        NinjaHelm2_Paint,
        NinjaHelm3_Paint,
        Nomad,
        Normal,
        NorthWarden,
        OldCape_Paint,
        OldCape2_Paint,
        OMG,
        OrcBow,
        Orly,
        PaladinArmor_Paint,
        PaladinHelm_Paint,
        PathfinderArmor,
        PathFinderBow,
        PathfinderHat_Paint,
        PathfinderHood,
        PeasantCap,
        PeasantClothing,
        PeasantHead,
        Perverted,
        PushedBack,
        PyromancerArmor,
        Rags1,
        Rags2,
        Rags3,
        RangerArmor,
        RangerBow,
        RangerHat_Paint,
        RedLion,
        Robe,
        Rocky,
        RoyalArcherArmor,
        RoyalArcherBow,
        RoyalArcherHat,
        RoyalCape_Paint,
        RoyalGuard,
        RustyShield1,
        Sad,
        SamuraiBow1,
        SamuraiBow2,
        SamuraiBow3,
        SamuraiElder,
        SamuraiElderHelm,
        SamuraiHeavy1,
        SamuraiHeavy2,
        SamuraiHeavy3,
        SamuraiHelm1,
        SamuraiHelm2,
        SamuraiHelm3,
        SamuraiLight1,
        SamuraiLight2,
        SamuraiLight3,
        SamuraiShield1,
        SamuraiShield2,
        SamuraiShield3,
        SandElder,
        SandElderTurban,
        SandHealer1,
        SandHealer2,
        SandHealerHood,
        SandMage1,
        SandMage2,
        SandShooter1,
        SandShooter2,
        SandShooterBow1,
        SandShooterBow2,
        SandShooterTurban,
        SandWarrior,
        SandWarrior1,
        SandWarrior2,
        SandWarriorHelm1,
        SandWarriorHelm2,
        Santa1,
        Santa2,
        SantaHat,
        SantaHelper,
        SantaHelperHat1,
        SantaHelperHat2,
        SantaHelperHat3,
        Scout_Paint,
        ScoutBow,
        ScoutHelm,
        ScoutShortBow,
        Shaggy,
        ShamanArmor,
        ShamanHelm_FullHair,
        ShamanMask_FullHair,
        ShamanMask_Paint,
        ShamanMask2_FullHair,
        ShamanMask3_FullHair,
        SharpHelm_Paint,
        ShortHair,
        ShortPonytail,
        Shy,
        Sick,
        SideFringe,
        SideFringeDisheveled,
        SiegeArcherArmor,
        SiegeArcherHelm_Paint,
        Sigh,
        Skeleton1,
        Skeleton1Eyes,
        SkeletonArmor1,
        SkeletonHelm,
        Slave,
        SlaveHeadband_FullHair,
        Slicked,
        SlickedTwintales,
        SlickedTwintalesDisheveled,
        Smirk,
        SniperArmor,
        SniperBow,
        SnperHood,
        SpearmanArmor,
        SpearmanHelm1,
        SpearmanHelm2,
        SpikedArmor,
        SteampunkGlasses,
        SteelHelm_Paint,
        SteelPotHelm,
        SteelShield,
        SteelTowerShield,
        StormBringerArmor,
        StrawHat,
        Surprised,
        SwampElder,
        SwampHealer1,
        SwampHealer2,
        SwampLordMask1_FullHair,
        SwampLordMask2_FullHair,
        SwampMage1,
        SwampMage2,
        SwampShooter1,
        SwampShooter1Bow,
        SwampShooter2,
        SwampShooter2Bow,
        SwampWarrior1,
        SwampWarrior2,
        SWAT,
        SweatDrop,
        ThiefArmor,
        ThiefHelm_Paint,
        TowerShield,
        Twisty,
        UberSoldier,
        UberSoldierHelm,
        VampireCape_Paint,
        VerySurprised,
        VikingBeard,
        VikingFurArmor,
        VikingIronBow1,
        VikingIronBow2,
        VikingIronBow3,
        VikingIronHelm1,
        VikingIronHelm2,
        VikingIronHelm3,
        VikingLeatherHelm,
        VikingLightArmor1,
        VikingLightArmor2,
        VikingLightArmor3,
        VikingLongBow,
        VikingRoughArmor1,
        VikingRoughArmor2,
        VikingRoughArmor3_Paint,
        VikingShortBow,
        VikingWoodenBow,
        VillageGirl,
        VisorKnightHelm_Paint,
        Wanderer,
        Wanting,
        WarlockArmor,
        WarlockHood,
        WarriorHelm_Paint,
        WarriorShield,
        WaterAdept,
        WaterWizardArmor,
        WaterWizardHelm,
        Wavy,
        WitchArmor,
        WitchHat_Paint,
        WitchHunterArmor_Paint,
        WitchHunterBow,
        WitchHunterHat_Paint,
        WoodenBuckler,
        WoodenShield,
        WoodenShield1,
        WoodenShield2,
        WoodenShield3,
        Worm,
        WTF,
        XD,
        Zombie,
        ZombieEyes1,
        ZombieEyes2,
        ZombieEyes3,
        ZombieEyes4,
        ZombieEyes5,
        ZombieEyes6,
        ZombieEyes7,
        ZombieShabby,
        ZombieSticks,
        HeadMask,
        Shadow,

    }
    */

}
