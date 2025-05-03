using System;
using System.Collections.Generic;
using System.IO;

namespace SRO.Data.Setting
{
    /// <summary>
    /// SROptionSet.dat file format.
    /// <para>
    /// <see cref="https://github.com/DummkopfOfHachtenduden/SilkroadDoc/wiki/SROptionSet"/>
    /// </para>
    /// </summary>
    public class SROptionSet
    {
        #region Private Members
        private Dictionary<ID, object> Options { get; } = new Dictionary<ID, object>();
        #endregion

        #region Public Properties
        public uint Version { get; set; } = 13;
        public byte unkByte01 { get; set; } = 15;
        public uint unkUInt01 { get; set; } = 39;
        #endregion

        #region Constructor
        public SROptionSet()
        {
            // Set default options
            foreach (var kv in Map)
                Options[kv.Key] = kv.Value;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get option value by id
        /// </summary>
        public object Get(ID id) => Options.TryGetValue(id, out var value) ? value : null;
        public object GetDefault(ID id) => Map[id];
        public void Set(ID id, object value)
        {
            // Make sure the casting type is right
            if (Map[id].GetType() != value.GetType())
                throw new InvalidCastException();
            // Set new value
            Options[id] = value;
        }
        /// <summary>
        /// Loads a SROptionSet.dat file format
        /// </summary>
        public void Load(Stream stream)
        {
            using (var br = new BinaryReader(stream))
            {
                Version = br.ReadUInt32();
                unkByte01 = br.ReadByte();
                unkUInt01 = br.ReadUInt32();
                // Try to read until EOF
                var eof = br.BaseStream.Length;
                while (br.BaseStream.Position < eof)
                {
                    var id = (ID)br.ReadUInt16();
                    var unk = br.ReadUInt16(); // Always 0, could be Name.Length
                    // Find ID
                    if (!Map.TryGetValue(id, out var type))
                        throw new ArgumentException("Option ID not found ("+id+")");
                    // Casting object
                    object value = null;
                    if (type is bool)
                        value = br.ReadBoolean();
                    if (type is byte)
                        value = br.ReadByte();
                    else if (type is ushort)
                        value = br.ReadUInt16();
                    else if (type is uint)
                        value = br.ReadUInt32();
                    // Set value
                    Options[id] = value;
                }
            }
        }
        /// <summary>
        /// Saves a SROptionSet.dat file format
        /// </summary>
        public void Save(Stream stream)
        {
            using (var bw = new BinaryWriter(stream))
            {
                bw.Write(Version);
                bw.Write(unkByte01);
                bw.Write(unkUInt01);
                // Save all options by default or by modifications
                foreach (var kv in Map)
                {
                    // ID
                    bw.Write((ushort)kv.Key);
                    bw.Write(default(ushort));
                    // Get value or Default
                    if (!Options.TryGetValue(kv.Key, out var value))
                        value = kv.Value;
                    // Casting object
                    if (value is bool _bool)
                        bw.Write(_bool);
                    else if (value is byte _byte)
                        bw.Write(_byte);
                    else if (value is ushort _ushort)
                        bw.Write(_ushort);
                    else if (value is uint _uint)
                        bw.Write(_uint);
                }
            }
        }
        #endregion

        #region Public & Private Helpers

        /// <summary>
        /// ID from each option available in the file
        /// </summary>
        public enum ID : ushort
        {
            Video_Graphic1_ShadowDetail = 1,
            Video_Graphic1_BackgroundSightRange = 2,
            Video_Graphic1_CharacterSightRange = 3,
            Video_Graphic1_WaterReflection = 4,
            Video_Graphic1_WaterDetail = 5,
            Video_Graphic1_MetallicSheen = 6,
            Video_Graphic1_LightEffect = 7,
            Video_Graphic1_TextureFiltering = 8,
            Video_Graphic1_TextureDetail = 9,
            Video_Graphic1_LensFlare = 10,
            Video_Graphic1_BloomEffect = 11,
            Video_Graphic1_DynamicAnimation = 12,
            Video_Graphic1_EffectQuallity = 13,
            Video_Graphic1__ = 14,
            Video_Graphic1___ = 15,

            Video_Graphic2_ShadowDetail = 101,
            Video_Graphic2_BackgroundSightRange = 102,
            Video_Graphic2_CharacterSightRange = 103,
            Video_Graphic2_WaterReflection = 104,
            Video_Graphic2_WaterDetail = 105,
            Video_Graphic2_MetallicSheen = 106,
            Video_Graphic2_LightEffect = 107,
            Video_Graphic2_TextureFiltering = 108,
            Video_Graphic2_TextureDetail = 109,
            Video_Graphic2_LensFlare = 110,
            Video_Graphic2_BloomEffect = 111,
            Video_Graphic2_DynamicAnimation = 112,
            Video_Graphic2_EffectQuallity = 113,
            Video_Graphic2__ = 114,
            Video_Graphic2___ = 115,

            Video_Graphic1_Type = 501,
            Video_Graphic1_Brightness = 502,
            Video_Graphic1_ResolutionWidth = 503,
            Video_Graphic1_ResolutionHeight = 504,

            Video_Graphic2_Type = 601,
            Video_Graphic2_Brightness = 602,
            Video_Graphic2_ResolutionWidth = 603,
            Video_Graphic2_ResolutionHeight = 604,

            Audio_BackgroundVolume_BackgroundVolumeSlider = 1001,
            Audio_FXVolume_FXVolumeSlider = 1002,
            Audio_EnvironmentalVolume_EnvironmentalVolumeSlider = 1003,
            Audio_BackgroundVolume_BackgroundVolumeCheckbox = 1004,
            Audio_FXVolume_FXVolumeCheckBox = 1005,
            Audio_EnvironmentalVolume_EnvironmentalVolumeCheckbox = 1006,

            Setting_Community_GameGuideCheckBox = 2001,
            Setting_Community_PartyInvitationCheckbox = 2002,
            Setting_Community_ExchangeRequestCheckbox = 2003,
            Setting_Community_PersonalMsgCheckbox = 2004,
            Setting__ = 2005,
            Setting___ = 2006,
            Setting____ = 2007,
            Setting_Display_SystemUIAutoHideCheckbox = 2008,
            Setting_Display_QuickPartyViewBuffStatusCheckbox = 2009,
            Setting_IndicateName_OwnNameCheckbox = 2010,
            Setting_IndicateName_OtherNameCheckbox = 2011,
            Setting_IndicateName_MonsterNameCheckbox = 2012,
            Setting_IndicateName_NPCNameCheckbox = 2013,
            Setting_IndicateName_GuildNameCheckbox = 2014,
            Video_WindowMode_isWindowMode = 2015,
            Setting_Others_HighSettingIntroCheckbox = 2016,
            Setting_Display_FortressWarMarkCheckbox = 2017,
            Setting_Display_HideAbilityPetsCheckbox = 2018,
            Setting_Others_HPWarningCheckbox = 2019,
            Setting_Others_MPWarningCheckbox = 2020,
            Setting_Display_SelfConditionCheckbox = 2021,
            Setting_Display_COSConditionCheckbox = 2022,
            Setting_Display_PartyMemberStatusCheckbox = 2023,
            Setting_Display_MonsterConditionCheckbox = 2024,
            Setting_Display_OpenTheGuideCheckbox = 2025,
            Setting_Display_ActivateActionShortcutCheckbox = 2026,
            Setting_Others_CameraRotationMethod1Checkbox = 2027,
            Setting_Others_CameraRotationMethod2Checkbox = 2028,

            KeyMap_CustomKey_KeyCharacter = 3001,
            KeyMap_CustomKey_KeyInventory = 3002,
            KeyMap_CustomKey_KeySkill = 3003,
            KeyMap_CustomKey_KeyAction = 3004,
            KeyMap_CustomKey_KeyParty = 3005,
            KeyMap_CustomKey_KeyQuest = 3006,
            KeyMap_CustomKey_KeyCommunity = 3007,
            KeyMap_CustomKey_KeyWorldMap = 3008,
            KeyMap_CustomKey_KeyBerserkerMode = 3009,
            KeyMap_CustomKey_KeyHelp = 3011,
            KeyMap_CustomKey_KeyViewDropItem = 3012,
            KeyMap_CustomKey_KeyMouseQuickSlot = 3013,
            KeyMap_CustomKey_KeySitStand = 3014,
            KeyMap_CustomKey_KeyAutoPickup = 3015,
            KeyMap_CustomKey_KeyCOSInfo = 3016,
            KeyMap_CustomKey_KeyCOSRide = 3017,
            KeyMap_CustomKey_KeyCOSRelease = 3018,
            KeyMap_CustomKey_KeyCOSFollow = 3019,
            KeyMap_CustomKey_KeyCOSAttack = 3020,
            KeyMap_CustomKey_KeyCOSAIType = 3021,
            KeyMap_CustomKey_KeyReplyWhisper = 3023,
            KeyMap_CustomKey_KeyAutoPotion = 3024,
            KeyMap_CustomKey_KeyCOSSelection = 3025,
            KeyMap_CustomKey_KeyPartyMatch = 3026,
            KeyMap_CustomKey_KeyAlchemy = 3027,
            KeyMap_CustomKey_KeyTargetEnemy = 3029,
            KeyMap_CustomKey_KeyTargetRecent = 3030,
            KeyMap_CustomKey_KeyTargetSupport = 3031,
            KeyMap_CustomKey_KeyTargetSee = 3032,
            KeyMap_CustomKey_KeyAcademy = 3033,
            KeyMap_CustomKey_KeyHideFriends = 3034,
            KeyMap_CustomKey_KeyHideEnemies = 3035,

            KeyMap_IsMouseShortcutSwapped = 3101,
        }
        /// <summary>
        /// Map for all the options that can be changed ingame.
        /// <para>
        /// ID = Type/Value (Default)
        /// </para>
        /// </summary>
        private static readonly Dictionary<ID, object> Map = new Dictionary<ID, object>()
        {
            { ID.Video_Graphic1_ShadowDetail, (ushort)258 },
            { ID.Video_Graphic1_BackgroundSightRange, (ushort)516 },
            { ID.Video_Graphic1_CharacterSightRange, (ushort)516 },
            { ID.Video_Graphic1_WaterReflection, (ushort)1 },
            { ID.Video_Graphic1_WaterDetail, (ushort)258 },
            { ID.Video_Graphic1_MetallicSheen, (ushort)257 },
            { ID.Video_Graphic1_LightEffect, (ushort)257 },
            { ID.Video_Graphic1_TextureFiltering, (ushort)257 },
            { ID.Video_Graphic1_TextureDetail, (ushort)258 },
            { ID.Video_Graphic1_LensFlare, (ushort)257 },
            { ID.Video_Graphic1_BloomEffect, (ushort)257 },
            { ID.Video_Graphic1_DynamicAnimation, (ushort)257 },
            { ID.Video_Graphic1_EffectQuallity, (ushort)514 },
            { ID.Video_Graphic1__, (ushort)6425 },
            { ID.Video_Graphic1___, (ushort)0 },
            { ID.Video_Graphic1_Type, (byte)SilkCfg.Graphic.High },
            { ID.Video_Graphic1_Brightness, (byte)SilkCfg.Brightness.Normal },
            { ID.Video_Graphic1_ResolutionWidth, (uint)1024 },
            { ID.Video_Graphic1_ResolutionHeight, (uint)768 },

            { ID.Video_Graphic2_ShadowDetail, (ushort)258 },
            { ID.Video_Graphic2_BackgroundSightRange, (ushort)516 },
            { ID.Video_Graphic2_CharacterSightRange, (ushort)516 },
            { ID.Video_Graphic2_WaterReflection, (ushort)1 },
            { ID.Video_Graphic2_WaterDetail, (ushort)258 },
            { ID.Video_Graphic2_MetallicSheen, (ushort)257 },
            { ID.Video_Graphic2_LightEffect, (ushort)257 },
            { ID.Video_Graphic2_TextureFiltering, (ushort)257 },
            { ID.Video_Graphic2_TextureDetail, (ushort)258 },
            { ID.Video_Graphic2_LensFlare, (ushort)257 },
            { ID.Video_Graphic2_BloomEffect, (ushort)257 },
            { ID.Video_Graphic2_DynamicAnimation, (ushort)257 },
            { ID.Video_Graphic2_EffectQuallity, (ushort)514 },
            { ID.Video_Graphic2__, (ushort)37008 },
            { ID.Video_Graphic2___, (ushort)0 },
            { ID.Video_Graphic2_Type, (byte)SilkCfg.Graphic.High },
            { ID.Video_Graphic2_Brightness, (byte)SilkCfg.Brightness.Normal },
            { ID.Video_Graphic2_ResolutionWidth, (uint)1024 },
            { ID.Video_Graphic2_ResolutionHeight, (uint)768 },

            { ID.Audio_BackgroundVolume_BackgroundVolumeSlider, (uint)30 },
            { ID.Audio_FXVolume_FXVolumeSlider, (uint)50 },
            { ID.Audio_EnvironmentalVolume_EnvironmentalVolumeSlider, (uint)50 },
            { ID.Audio_BackgroundVolume_BackgroundVolumeCheckbox, false },
            { ID.Audio_FXVolume_FXVolumeCheckBox, false },
            { ID.Audio_EnvironmentalVolume_EnvironmentalVolumeCheckbox, false },

            { ID.Setting_Community_GameGuideCheckBox, true },
            { ID.Setting_Community_PartyInvitationCheckbox, true },
            { ID.Setting_Community_ExchangeRequestCheckbox, true },
            { ID.Setting_Community_PersonalMsgCheckbox, true },
            { ID.Setting__, true },
            { ID.Setting___, true },
            { ID.Setting____, true },
            { ID.Setting_Display_SystemUIAutoHideCheckbox, true },
            { ID.Setting_Display_QuickPartyViewBuffStatusCheckbox, true },
            { ID.Setting_IndicateName_OwnNameCheckbox, true },
            { ID.Setting_IndicateName_OtherNameCheckbox, true },
            { ID.Setting_IndicateName_MonsterNameCheckbox, true },
            { ID.Setting_IndicateName_NPCNameCheckbox, true },
            { ID.Setting_IndicateName_GuildNameCheckbox, true },
            { ID.Video_WindowMode_isWindowMode, true },
            { ID.Setting_Others_HighSettingIntroCheckbox, true },
            { ID.Setting_Display_FortressWarMarkCheckbox, true },
            { ID.Setting_Display_HideAbilityPetsCheckbox, false },
            { ID.Setting_Others_HPWarningCheckbox, false },
            { ID.Setting_Others_MPWarningCheckbox, false },
            { ID.Setting_Display_SelfConditionCheckbox, false },
            { ID.Setting_Display_COSConditionCheckbox, false },
            { ID.Setting_Display_PartyMemberStatusCheckbox, false },
            { ID.Setting_Display_MonsterConditionCheckbox, false },
            { ID.Setting_Display_OpenTheGuideCheckbox, true },
            { ID.Setting_Display_ActivateActionShortcutCheckbox, false },
            { ID.Setting_Others_CameraRotationMethod1Checkbox, true },
            { ID.Setting_Others_CameraRotationMethod2Checkbox, false },

            { ID.KeyMap_CustomKey_KeyCharacter, (uint)ConsoleKey.C },
            { ID.KeyMap_CustomKey_KeyInventory, (uint)ConsoleKey.I },
            { ID.KeyMap_CustomKey_KeySkill, (uint)ConsoleKey.S },
            { ID.KeyMap_CustomKey_KeyAction, (uint)ConsoleKey.A },
            { ID.KeyMap_CustomKey_KeyParty, (uint)ConsoleKey.P },
            { ID.KeyMap_CustomKey_KeyQuest, (uint)ConsoleKey.Q },
            { ID.KeyMap_CustomKey_KeyCommunity, (uint)ConsoleKey.U },
            { ID.KeyMap_CustomKey_KeyWorldMap, (uint)ConsoleKey.M },
            { ID.KeyMap_CustomKey_KeyBerserkerMode, (uint)ConsoleKey.Tab },
            { ID.KeyMap_CustomKey_KeyHelp, (uint)ConsoleKey.H },
            { ID.KeyMap_CustomKey_KeyViewDropItem, (uint)ConsoleKey.Z },
            { ID.KeyMap_CustomKey_KeyMouseQuickSlot, (uint)ConsoleKey.X },
            { ID.KeyMap_CustomKey_KeySitStand, (uint)ConsoleKey.N },
            { ID.KeyMap_CustomKey_KeyAutoPickup, (uint)ConsoleKey.G },
            { ID.KeyMap_CustomKey_KeyCOSInfo, (uint)ConsoleKey.Insert },
            { ID.KeyMap_CustomKey_KeyCOSRide, (uint)ConsoleKey.Home },
            { ID.KeyMap_CustomKey_KeyCOSRelease, (uint)ConsoleKey.PageUp },
            { ID.KeyMap_CustomKey_KeyCOSFollow, (uint)ConsoleKey.Delete },
            { ID.KeyMap_CustomKey_KeyCOSAttack, (uint)ConsoleKey.End },
            { ID.KeyMap_CustomKey_KeyCOSAIType, (uint)ConsoleKey.PageDown },
            { ID.KeyMap_CustomKey_KeyReplyWhisper, (uint)ConsoleKey.R },
            { ID.KeyMap_CustomKey_KeyAutoPotion, (uint)ConsoleKey.T },
            { ID.KeyMap_CustomKey_KeyCOSSelection, (uint)ConsoleKey.W },
            { ID.KeyMap_CustomKey_KeyPartyMatch, (uint)ConsoleKey.E },
            { ID.KeyMap_CustomKey_KeyAlchemy, (uint)ConsoleKey.Y },
            { ID.KeyMap_CustomKey_KeyTargetEnemy, default(uint) },
            { ID.KeyMap_CustomKey_KeyTargetRecent, default(uint) },
            { ID.KeyMap_CustomKey_KeyTargetSupport, default(uint) },
            { ID.KeyMap_CustomKey_KeyTargetSee, default(uint) },
            { ID.KeyMap_CustomKey_KeyAcademy, (uint)ConsoleKey.L },
            { ID.KeyMap_CustomKey_KeyHideFriends, default(uint) },
            { ID.KeyMap_CustomKey_KeyHideEnemies, default(uint) },

            { ID.KeyMap_IsMouseShortcutSwapped, false },
        };
        #endregion
    }
}
