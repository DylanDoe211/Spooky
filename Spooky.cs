using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

using Spooky.Core;
using Spooky.Content.Backgrounds;
using Spooky.Content.Backgrounds.Cemetery;
using Spooky.Content.Backgrounds.SpiderCave;
using Spooky.Content.Backgrounds.SpookyHell;
using Spooky.Content.Biomes;
using Spooky.Content.NPCs.Boss.Moco;
using Spooky.Content.NPCs.Friendly;
using Spooky.Content.NPCs.Tameable;
using Spooky.Content.Tiles.Cemetery;
using Spooky.Content.Tiles.Shipyard;
using Spooky.Content.Tiles.SpookyBiome;
using Spooky.Effects;

using SpiritReforged.Common.WorldGeneration.Ecotones;

namespace Spooky
{
	public class Spooky : Mod
	{
        internal static Spooky Instance;
        
        internal Mod subworldLibrary = null;
        internal Mod thoriumMod = null;
        internal Mod calamityMod = null;

		public static Effect vignetteEffect;
        public static Vignette vignetteShader;

        public static ModKeybind AccessoryHotkey { get; private set; }

        internal static Spooky mod;

        public Spooky()
		{
			mod = this;
            //MusicSkipsVolumeRemap = true; //disabled for now because it makes music TOO loud
		}

		public override object Call(params object[] args)
		{
			if (args is null)
			{
				Logger.Error("Call Error: Arguments are null.");
			}

			if (args.Length == 0)
			{
				Logger.Error("Call Error: Arguments are empty.");
			}

			if (args[0] is not string firstArg)
			{
				return null;
			}

			switch (firstArg)
			{
				case "BossDowned":
				{
					string text = args[1] as string;
					text = text.ToLower();
					return text switch
					{
						nameof(Flags.downedRotGourd) => Flags.downedRotGourd,
						nameof(Flags.downedSpookySpirit) => Flags.downedSpookySpirit,
						nameof(Flags.downedMoco) => Flags.downedMoco,
						nameof(Flags.downedDaffodil) => Flags.downedDaffodil,
						nameof(Flags.downedOldHunter) => Flags.downedOldHunter,
						nameof(Flags.downedOrroboro) => Flags.downedOrroboro,
						nameof(Flags.downedSpookFishron) => Flags.downedSpookFishron,
						nameof(Flags.downedBigBone) => Flags.downedBigBone,
						_ => throw new ArgumentException(text + " Is not a valid boss downed variable name"),
					};
				}
				default:
				{
					Logger.Error($"Call Error: Context '{firstArg}' is invalid.");
					return null;
				}
			}
		}

		public override void PostSetupContent()
		{
			if (ModLoader.HasMod("SpiritReforged"))
			{
				Setup();
			}
		}

		[JITWhenModsEnabled("SpiritReforged")]
		public void Setup()
		{
			EcotoneEdgeDefinitions.AddEdgeDefinition<SpookyGrass, SpookyDirt, SpookyStone, SpookyBiome>(mod, "SpookyForest", null, Color.OrangeRed, true);
			EcotoneEdgeDefinitions.AddEdgeDefinition<CemeteryDirt, CemeteryGrass, CemeteryStone, CemeteryBiome>(mod, "Cemetery", null, Color.Teal, true);
			EcotoneEdgeDefinitions.AddEdgeDefinition<BlackSand, BlackSandstone, BlackSandstoneMoss, ShipyardBiome>(mod, "Shipyard", null, Color.Gray, true);
		}

		public override void Load()
        {
            Instance = this;
            
            ModLoader.TryGetMod("SubworldLibrary", out subworldLibrary);
            ModLoader.TryGetMod("ThoriumMod", out thoriumMod);
            ModLoader.TryGetMod("CalamityMod", out calamityMod);

			AccessoryHotkey = KeybindLoader.RegisterKeybind(this, "AccessoryHotkey", "E");

            if (Main.netMode != NetmodeID.Server)
			{
                Filters.Scene["Spooky:CemeterySky"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(0f, 135f, 35f).UseOpacity(0.001f), EffectPriority.VeryHigh);
                SkyManager.Instance["Spooky:CemeterySky"] = new CemeterySky();

                Filters.Scene["Spooky:ShipyardSky"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(149f, 131f, 217f).UseOpacity(0.0005f), EffectPriority.VeryHigh);
                SkyManager.Instance["Spooky:ShipyardSky"] = new CemeterySky();

                Filters.Scene["Spooky:RaveyardSky"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryHigh);
                SkyManager.Instance["Spooky:RaveyardSky"] = new RaveyardSky();

                Filters.Scene["Spooky:SpookyForestTint"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(255f, 116f, 23f).UseOpacity(0.001f), EffectPriority.VeryHigh);

                Filters.Scene["Spooky:HallucinationEffect"] = new Filter(new SpookyScreenShader("FilterMoonLordShake").UseIntensity(0.5f), EffectPriority.VeryHigh);

                Filters.Scene["Spooky:SpookFishron"] = new Filter(new FishronScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryHigh);
                SkyManager.Instance["Spooky:SpookFishron"] = new FishronSky();

				vignetteEffect = ModContent.Request<Effect>("Spooky/Effects/Vignette", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
				vignetteShader = new Vignette(vignetteEffect, "MainPS");
				Filters.Scene["Spooky:Vignette"] = new Filter(vignetteShader, (EffectPriority)100);
            }

            SpiderCaveBG.Load();
            SpookyHellBG.Load();
        }

        public override void Unload()
        {
            subworldLibrary = null;
            thoriumMod = null;
            calamityMod = null;

            AccessoryHotkey = null;
			mod = null;
		}

        public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			SpookyMessageType messageType = (SpookyMessageType)reader.ReadByte();
			switch (messageType)
			{
                case SpookyMessageType.SpawnMoco:
                {
                    NPC.NewNPC(null, Flags.MocoSpawnX, Flags.MocoSpawnY, ModContent.NPCType<MocoSpawner>());
					break;
                }
                case SpookyMessageType.SpawnDaffodil:
                {
                    Flags.SpawnDaffodil = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.SpawnBigBone:
                {
                    Flags.SpawnBigBone = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
				}
                case SpookyMessageType.SpawnOldHunter:
                {
                    Flags.SpawnOldHunter = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
				}
                case SpookyMessageType.SpawnOrroboro:
                {
                    Flags.SpawnOrroboro = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
				}
				case SpookyMessageType.SpawnTurkey:
                {
                    int Turkey = NPC.NewNPC(null, Flags.TurkeySpawnX, Flags.TurkeySpawnY, ModContent.NPCType<Turkey>());
                    Main.npc[Turkey].GetGlobalNPC<NPCGlobal>().NPCTamed = true;
					break;
                }
                case SpookyMessageType.EggIncursionStart:
                {
                    EggEventWorld.EventTimeLeftUI = 21600;
                    EggEventWorld.EggEventActive = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.EggIncursionTimeReduce:
                {
                    EggEventWorld.EventTimeLeft += 720;
                    EggEventWorld.EventTimeLeftUI -= 720;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.CatacombKey1:
                {
                    Flags.CatacombKey1 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.CatacombKey2:
                {
                    Flags.CatacombKey2 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.CatacombKey3:
                {
                    Flags.CatacombKey3 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.BountyAccepted1:
                {
                    Flags.BountyInProgress1 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.BountyAccepted2:
                {
                    Flags.BountyInProgress2 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.BountyAccepted3:
                {
                    Flags.BountyInProgress3 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.BountyAccepted4:
                {
                    Flags.BountyInProgress4 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.Bounty1Complete:
                {
                    Flags.LittleEyeBounty1 = true;
                    Flags.BountyInProgress1 = false;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.Bounty2Complete:
                {
                    Flags.LittleEyeBounty2 = true;
                    Flags.BountyInProgress2 = false;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.Bounty3Complete:
                {
                    Flags.LittleEyeBounty3 = true;
                    Flags.BountyInProgress3 = false;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.Bounty4Complete:
                {
                    Flags.LittleEyeBounty4 = true;
                    Flags.BountyInProgress4 = false;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.BountyIntro:
                {
                    Flags.BountyIntro = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.PokedLittleEye:
                {
                    Flags.PokedLittleEye = true;
                    Flags.AlreadyPokedLittleEye = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.KrampusQuestGiven:
                {
                    Flags.KrampusQuestGiven = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.KrampusQuestlineDone:
                {
                    Flags.KrampusQuestlineDone = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.KrampusDailyQuestDone:
                {
                    Flags.KrampusDailyQuestDone = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.KrampusDailyQuestReset:
                {
                    Flags.KrampusDailyQuest = false;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.DrawKrampusMapIconReset:
                {
                    Flags.DrawKrampusMapIcon = false;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.SpawnMushGnome:
                {
                    int[] Gnomes = new int[] { ModContent.NPCType<MushGnome1>(), ModContent.NPCType<MushGnome2>(), ModContent.NPCType<MushGnome3>(), ModContent.NPCType<MushGnome4>() };
                    int Gnome = NPC.NewNPC(null, Flags.MushGnomeSpawnX, Flags.MushGnomeSpawnY, Main.rand.Next(Gnomes));
                    Main.npc[Gnome].velocity.X = Main.rand.NextBool() ? -1 : 1;
                    break;
                }
                case SpookyMessageType.SpawnGhostAmbush:
                {
                    Flags.SpawnGhostAmbush = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.OldHunterQuest1Complete:
                {
                    Flags.OldHunterQuest1 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.OldHunterQuest2Complete:
                {
                    Flags.OldHunterQuest2 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.OldHunterQuest3Complete:
                {
                    Flags.OldHunterQuest3 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.OldHunterQuest4Complete:
                {
                    Flags.OldHunterQuest4 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.OldHunterQuest5Complete:
                {
                    Flags.OldHunterQuest5 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.OldHunterQuest6Complete:
                {
                    Flags.OldHunterQuest6 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.OldHunterQuest7Complete:
                {
                    Flags.OldHunterQuest7 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.OldHunterQuest8Complete:
                {
                    Flags.OldHunterQuest8 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.OldHunterDefeatDialogue:
                {
                    Flags.OldHunterDefeatDialogue = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.OldHunterQuestEnd:
                {
                    Flags.OldHunterQuestEnd = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.StartSporeEvent:
                {
                    Flags.SporeEventHappening = true;
                    Flags.SporeEventTimeLeft = 54000; //15 real-life minutes
                    Flags.SporeFogIntensity = 0.5f;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
				//should never occur I think?
				default:
                {
					Logger.Warn("Spooky Mod: Unknown Message type: " + messageType);
					break;
                }
			}
		}
    }

    enum SpookyMessageType : byte
    {
        SpawnMoco,
        SpawnOrroboro,
        SpawnDaffodil,
        SpawnBigBone,
        SpawnOldHunter,
		SpawnTurkey,
        EggIncursionStart,
        EggIncursionTimeReduce,
        CatacombKey1,
        CatacombKey2,
        CatacombKey3,
        BountyAccepted1,
        BountyAccepted2,
        BountyAccepted3,
        BountyAccepted4,
        Bounty1Complete,
        Bounty2Complete,
        Bounty3Complete,
        Bounty4Complete,
        BountyIntro,
        PokedLittleEye,
        KrampusQuestGiven,
        KrampusQuestlineDone,
        KrampusDailyQuestDone,
        KrampusDailyQuestReset,
        DrawKrampusMapIconReset,
        SpawnMushGnome,
        SpawnGhostAmbush,
        OldHunterQuest1Complete,
        OldHunterQuest2Complete,
        OldHunterQuest3Complete,
        OldHunterQuest4Complete,
        OldHunterQuest5Complete,
        OldHunterQuest6Complete,
        OldHunterQuest7Complete,
        OldHunterQuest8Complete,
        OldHunterDefeatDialogue,
        OldHunterQuestEnd,
        StartSporeEvent,
	}
}