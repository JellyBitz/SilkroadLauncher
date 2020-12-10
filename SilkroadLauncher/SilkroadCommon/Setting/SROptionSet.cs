using System.Collections.Generic;
namespace SilkroadLauncher.SilkroadCommon.Setting
{
    /// <summary>
    /// SROptinoSet.dat file structure
    /// </summary>
    public class SROptionSet
    {
        public uint Version { get; set; } = 13;
        public byte unkByte01 { get; set; } = 15;
        public uint unkUInt01 { get; set; } = 39;
        public Dictionary<OptionID, object> Options { get; } = new Dictionary<OptionID, object>()
        {
            // Video
            // Graphic #1
            {OptionID.Graphic01_ShadowDetail,(ushort)0},
            {OptionID.Graphic01_BackgroundSightRange,(ushort)514}, // 2
            {OptionID.Graphic01_CharacterSightRange,(ushort)1028}, // 4
            {OptionID.Graphic01_WaterReflection,(ushort)0},
            {OptionID.Graphic01_WaterDetail,(ushort)0},
            {OptionID.Graphic01_MetallicSheen,(ushort)0},
            {OptionID.Graphic01_LightEffect,(ushort)0},
            {OptionID.Graphic01_TextureFiltering,(ushort)0},
            {OptionID.Graphic01_TextureDetail,(ushort)0},
            {OptionID.Graphic01_LensFlare,(ushort)0},
            {OptionID.Graphic01_BloomEffect,(ushort)0},
            {OptionID.Graphic01_DynamicAnimation,(ushort)0},
            {OptionID.Graphic01_EffectQuallity,(ushort)0},
            {OptionID.Graphic01_Unknown14,(ushort)4626}, // 18 ?
            {OptionID.Graphic01_Unknown15,(ushort)0},
            // Graphic #2
            {OptionID.Graphic02_ShadowDetail,(ushort)257}, // 1
            {OptionID.Graphic02_BackgroundSightRange,(ushort)514}, // 2
            {OptionID.Graphic02_CharacterSightRange,(ushort)1028}, // 4
            {OptionID.Graphic02_WaterReflection,(ushort)257}, // 1
            {OptionID.Graphic02_WaterDetail,(ushort)0},
            {OptionID.Graphic02_MetallicSheen,(ushort)257}, // 1
            {OptionID.Graphic02_LightEffect,(ushort)257}, // 1
            {OptionID.Graphic02_TextureFiltering,(ushort)257}, // 1
            {OptionID.Graphic02_TextureDetail,(ushort)257}, // 1
            {OptionID.Graphic02_LensFlare,(ushort)257}, // 1
            {OptionID.Graphic02_BloomEffect,(ushort)257}, // 1
            {OptionID.Graphic02_DynamicAnimation,(ushort)257}, // 1
            {OptionID.Graphic02_EffectQuallity,(ushort)514}, // 2
            {OptionID.Graphic02_Unknown14,(ushort)29812}, // 116 ?
            {OptionID.Graphic02_Unknown15,(ushort)0},
            // Screen
            // Graphic #1
            {OptionID.Graphic01_Unknown501,(byte)4},
            {OptionID.Graphic01_Brightness,(byte)2},
            {OptionID.Graphic01_Width,(uint)800},
            {OptionID.Graphic01_Height,(uint)600},
            // Graphic #2
            {OptionID.Graphic02_Unknown501,(byte)1},
            {OptionID.Graphic02_Brightness,(byte)2},
            {OptionID.Graphic02_Width,(uint)800},
            {OptionID.Graphic02_Height,(uint)600},
            // Sound
            {OptionID.BackgroundVolumeSlider,(uint)50}, // %
            {OptionID.FXVolumeSlider,(uint)50}, // %
            {OptionID.EnvironmentalVolumeSlider,(uint)50}, // %
            {OptionID.BackgroundVolumeCheckbox,true},
            {OptionID.FXVolumeCheckBox,true},
            {OptionID.EnvironmentalVolumeCheckbox,true},
            // UI Setting
            {OptionID.GameGuideCheckBox,true},
            {OptionID.PartyInvitationCheckbox,true},
            {OptionID.ExchangeRequestCheckbox,true},
            {OptionID.PersonalMsgCheckbox,true},
            {OptionID.Unknown2005Checkbox,true},
            {OptionID.Unknown2006Checkbox,true},
            {OptionID.Unknown2007Checkbox,true},
            {OptionID.SystemUIAutoHideCheckbox,true},
            {OptionID.QuickPartyViewBuffStatusCheckbox,true},
            {OptionID.OwnNameCheckbox,true},
            {OptionID.OtherNameCheckbox,true},
            {OptionID.MonsterNamCheckbox,true},
            {OptionID.NPCNameCheckbox,true},
            {OptionID.GuildNameCheckbox,true},
            {OptionID.IsWindowMode,true},
            {OptionID.HighSettingIntroCheckbox,true},
            {OptionID.FortressWarMarkCheckbox,true},
            {OptionID.HideAbilityPetsCheckbox,false},
            {OptionID.HPWarningCheckbox,false},
            {OptionID.MPWarningCheckbox,false},
            {OptionID.SelfConditionCheckbox,false},
            {OptionID.COSConditionCheckbox,false},
            {OptionID.PartyMemberStatusCheckbox,false},
            {OptionID.MonsterConditionCheckbox,false},
            {OptionID.OpenTheGuideCheckbox,true},
            {OptionID.ActivateActionShortcutCheckbox,true},
            {OptionID.CameraRotationMethod1Checkbox,true},
            {OptionID.CameraRotationMethod2Checkbox,false},
            // Key mapping
            {OptionID.KeyCharacter,(uint)System.ConsoleKey.C},
            {OptionID.KeyInventory,(uint)System.ConsoleKey.I},
            {OptionID.KeySkill,(uint)System.ConsoleKey.S},
            {OptionID.KeyAction,(uint)System.ConsoleKey.A},
            {OptionID.KeyParty,(uint)System.ConsoleKey.P},
            {OptionID.KeyQuest,(uint)System.ConsoleKey.Q},
            {OptionID.KeyCommunity,(uint)System.ConsoleKey.U},
            {OptionID.KeyWorldMap,(uint)System.ConsoleKey.M},
            {OptionID.KeyBerserkerMode,(uint)System.ConsoleKey.Tab},
            {OptionID.KeyHelp,(uint)System.ConsoleKey.H},
            {OptionID.KeyViewDropItem,(uint)16}, // I can't remember
            {OptionID.KeyMouseQuickSlot,(uint)System.ConsoleKey.X},
            {OptionID.KeySitStand,(uint)System.ConsoleKey.N},
            {OptionID.KeyAutoPickup,(uint)System.ConsoleKey.Spacebar},
            {OptionID.KeyCOSInfo,(uint)System.ConsoleKey.Insert},
            {OptionID.KeyCOSRide,(uint)System.ConsoleKey.Home},
            {OptionID.KeyCOSRelease,(uint)System.ConsoleKey.PageUp},
            {OptionID.KeyCOSFollow,(uint)System.ConsoleKey.Delete},
            {OptionID.KeyCOSAttack,(uint)System.ConsoleKey.End},
            {OptionID.KeyCOSAIType,(uint)System.ConsoleKey.PageDown},
            {OptionID.KeyReplyWhisper,(uint)System.ConsoleKey.R},
            {OptionID.KeyAutoPotion,(uint)System.ConsoleKey.T},
            {OptionID.KeyCOSSelection,(uint)System.ConsoleKey.W},
            {OptionID.KeyPartyMatch,(uint)System.ConsoleKey.E},
            {OptionID.KeyAlchemy,(uint)System.ConsoleKey.Y},
            {OptionID.KeyTargetEnemy,(uint)0},
            {OptionID.KeyTargetRecent,(uint)System.ConsoleKey.B},
            {OptionID.KeyTargetSupport,(uint)0},
            {OptionID.KeyTargetSee,(uint)0},
            {OptionID.KeyAcademy,(uint)0},
            {OptionID.KeyHideFriends,(uint)0},
            {OptionID.KeyHideEnemies,(uint)0},
            {OptionID.isMouseShortcutSwapped,false}
        };
        /// <summary>
        /// All possible settings by ID
        /// </summary>
        public enum OptionID : uint
        {
            Graphic01_ShadowDetail = 1,
            Graphic01_BackgroundSightRange = 2,
            Graphic01_CharacterSightRange = 3,
            Graphic01_WaterReflection = 4,
            Graphic01_WaterDetail = 5,
            Graphic01_MetallicSheen = 6,
            Graphic01_LightEffect = 7,
            Graphic01_TextureFiltering = 8,
            Graphic01_TextureDetail = 9,
            Graphic01_LensFlare = 10,
            Graphic01_BloomEffect = 11,
            Graphic01_DynamicAnimation = 12,
            Graphic01_EffectQuallity = 13,
            Graphic01_Unknown14 = 14,
            Graphic01_Unknown15 = 15,
            Graphic02_ShadowDetail = 101,
            Graphic02_BackgroundSightRange = 102,
            Graphic02_CharacterSightRange = 103,
            Graphic02_WaterReflection = 104,
            Graphic02_WaterDetail = 105,
            Graphic02_MetallicSheen = 106,
            Graphic02_LightEffect = 107,
            Graphic02_TextureFiltering = 108,
            Graphic02_TextureDetail = 109,
            Graphic02_LensFlare = 110,
            Graphic02_BloomEffect = 111,
            Graphic02_DynamicAnimation = 112,
            Graphic02_EffectQuallity = 113,
            Graphic02_Unknown14 = 114,
            Graphic02_Unknown15 = 115,
            Graphic01_Unknown501 = 501,
            Graphic01_Brightness = 502,
            Graphic01_Width = 503,
            Graphic01_Height = 504,
            Graphic02_Unknown501 = 601,
            Graphic02_Brightness = 602,
            Graphic02_Width = 603,
            Graphic02_Height = 604,
            BackgroundVolumeSlider = 1001,
            FXVolumeSlider = 1002,
            EnvironmentalVolumeSlider = 1003,
            BackgroundVolumeCheckbox = 1004,
            FXVolumeCheckBox = 1005,
            EnvironmentalVolumeCheckbox = 1006,
            GameGuideCheckBox = 2001,
            PartyInvitationCheckbox = 2002,
            ExchangeRequestCheckbox = 2003,
            PersonalMsgCheckbox = 2004,
            Unknown2005Checkbox = 2005,
            Unknown2006Checkbox = 2006,
            Unknown2007Checkbox = 2007,
            SystemUIAutoHideCheckbox = 2008,
            QuickPartyViewBuffStatusCheckbox = 2009,
            OwnNameCheckbox = 2010,
            OtherNameCheckbox = 2011,
            MonsterNamCheckbox = 2012,
            NPCNameCheckbox = 2013,
            GuildNameCheckbox = 2014,
            IsWindowMode = 2015,
            HighSettingIntroCheckbox = 2016,
            FortressWarMarkCheckbox = 2017,
            HideAbilityPetsCheckbox = 2018,
            HPWarningCheckbox = 2019,
            MPWarningCheckbox = 2020,
            SelfConditionCheckbox = 2021,
            COSConditionCheckbox = 2022,
            PartyMemberStatusCheckbox = 2023,
            MonsterConditionCheckbox = 2024,
            OpenTheGuideCheckbox = 2025,
            ActivateActionShortcutCheckbox = 2026,
            CameraRotationMethod1Checkbox = 2027,
            CameraRotationMethod2Checkbox = 2028,
            KeyCharacter = 3001,
            KeyInventory = 3002,
            KeySkill = 3003,
            KeyAction = 3004,
            KeyParty = 3005,
            KeyQuest = 3006,
            KeyCommunity = 3007,
            KeyWorldMap = 3008,
            KeyBerserkerMode = 3009,
            KeyHelp = 3011,
            KeyViewDropItem = 3012,
            KeyMouseQuickSlot = 3013,
            KeySitStand = 3014,
            KeyAutoPickup = 3015,
            KeyCOSInfo = 3016,
            KeyCOSRide = 3017,
            KeyCOSRelease = 3018,
            KeyCOSFollow = 3019,
            KeyCOSAttack = 3020,
            KeyCOSAIType = 3021,
            KeyReplyWhisper = 3023,
            KeyAutoPotion = 3024,
            KeyCOSSelection = 3025,
            KeyPartyMatch = 3026,
            KeyAlchemy = 3027,
            KeyTargetEnemy = 3029,
            KeyTargetRecent = 3030,
            KeyTargetSupport = 3031,
            KeyTargetSee = 3032,
            KeyAcademy = 3033,
            KeyHideFriends = 3034,
            KeyHideEnemies = 3035,
            isMouseShortcutSwapped = 3101
        }
    }
}
