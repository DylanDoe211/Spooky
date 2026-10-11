using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.DataStructures;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

using Spooky.Core;
using Spooky.Effects;
using Spooky.Content.Backgrounds;
using Spooky.Content.Backgrounds.Cemetery;
using Spooky.Content.Backgrounds.Shipyard;
using Spooky.Content.Backgrounds.SpiderCave;
using Spooky.Content.Backgrounds.SpookyHell;
using Spooky.Content.Biomes;
using Spooky.Content.NPCs.Boss.Moco;
using Spooky.Content.NPCs.Friendly;
using Spooky.Content.NPCs.Tameable;
using Spooky.Content.Projectiles.Shipyard;
using Spooky.Content.Tiles.Catacomb.Furniture;
using Spooky.Content.Tiles.Cemetery;
using Spooky.Content.Tiles.Minibiomes.Christmas.Furniture;
using Spooky.Content.Tiles.Minibiomes.Desert.Furniture;
using Spooky.Content.Tiles.Minibiomes.Ocean.Furniture;
using Spooky.Content.Tiles.Minibiomes.Vegetable.Furniture;
using Spooky.Content.Tiles.NoseTemple.Furniture;
using Spooky.Content.Tiles.Shipyard;
using Spooky.Content.Tiles.Shipyard.Furniture;
using Spooky.Content.Tiles.SpiderCave.Furniture;
using Spooky.Content.Tiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome.Furniture;
using Spooky.Content.UserInterfaces.LittleEyeQuests;

using SpiritReforged.Common.WorldGeneration.Ecotones;

namespace Spooky
{
	public class Spooky : Mod
	{
		internal static Spooky Instance;

		internal Mod subworldLibrary = null;
		internal Mod thoriumMod = null;
		internal Mod calamityMod = null;
		internal Mod spiritReforged = null;

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
						nameof(Flags.downedDunkleosteus) => Flags.downedDunkleosteus,
						_ => throw new ArgumentException(text + " Is not a valid boss downed variable name"),
					};
				}
				case "EventDowned":
				{
					string text = args[1] as string;
					return text switch
					{
						nameof(Flags.downedPandoraBox) => Flags.downedPandoraBox,
						nameof(Flags.downedEggEvent) => Flags.downedEggEvent,
						nameof(Flags.downedSpiderWar) => Flags.downedSpiderWar,
						nameof(Flags.downedSporeEvent) => Flags.downedSporeEvent,
						_ => throw new ArgumentException(text + " Is not a valid event downed variable name"),
					};
				}
				case "BiomePositions":
				{
					string text = args[1] as string;
					return text switch
					{
						nameof(Flags.SpiderGrottoCenter) => Flags.SpiderGrottoCenter.ToPoint16(),
						nameof(Flags.EyeValleyCenter) => Flags.EyeValleyCenter.ToPoint16(),
						nameof(Flags.SpookyBiomeCenter) => Flags.SpookyBiomeCenter.ToPoint16(),
						nameof(Flags.ZombieOceanTopLeft) => Flags.ZombieOceanTopLeft.ToPoint16(),
						nameof(Flags.ZombieOceanBottomRight) => Flags.ZombieOceanBottomRight.ToPoint16(),
						nameof(Flags.CatacombUpperTopLeft) => Flags.CatacombUpperTopLeft.ToPoint16(),
						nameof(Flags.CatacombUpperBottomRight) => Flags.CatacombUpperBottomRight.ToPoint16(),
						nameof(Flags.CatacombLowerTopLeft) => Flags.CatacombLowerTopLeft.ToPoint16(),
						nameof(Flags.CatacombLowerBottomRight) => Flags.CatacombLowerBottomRight.ToPoint16(),
						nameof(Flags.NoseTempleLeftmostPosition) => Flags.NoseTempleLeftmostPosition.ToPoint16(),
						nameof(Flags.NoseTempleRightmostPosition) => Flags.NoseTempleRightmostPosition.ToPoint16(),
						_ => throw new ArgumentException(text + " Is not a valid biome position variable name"),
					};
				}
				case "EyeQuest":
				{
					return LittleEyeCrossmod.Call(args[1..]);
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
				SetupSpiritReforgedCrossmod();
			}
		}

		[JITWhenModsEnabled("SpiritReforged")]
		public void SetupSpiritReforgedCrossmod()
		{
			EcotoneEdgeDefinitions.AddEdgeDefinition<SpookyGrass, SpookyDirt, SpookyStone, SpookyBiome>(mod, "SpookyForest", null, Color.OrangeRed, true);
			EcotoneEdgeDefinitions.AddEdgeDefinition<CemeteryDirt, CemeteryGrass, CemeteryStone, CemeteryBiome>(mod, "Cemetery", null, Color.Teal, true);
			EcotoneEdgeDefinitions.AddEdgeDefinition<BlackSand, BlackSandGrass, BlackSandstone, ShipyardBiome>(mod, "Shipyard", null, Color.Gray, true);

			//add all spooky mod pots to the spirit reforged potstiary
			//shipyard shell
			spiritReforged.Call("ManualAddRecord", ModContent.TileType<ShipyardPots>(), new int[] { 0, 1, 2, 3, 4 }, "SpookyShipyardPot", (byte)1, () => false, 
			(Action<int, Point16, ILoot>)ShipyardPots.LootTable, Language.GetText("Mods.Spooky.Tiles.SpiritPotstiary.ShipyardShellPotDescription"), 
			Language.GetText("Mods.Spooky.Tiles.SpiritPotstiary.ShipyardShellPotName"));
			//shipyard crate
			spiritReforged.Call("ManualAddRecord", ModContent.TileType<ShipyardPotsWood>(), new int[] { 0, 1, 2 }, "SpookyShipyardCratePot", (byte)1, () => false, 
			(Action<int, Point16, ILoot>)ShipyardPots.LootTable, Language.GetText("Mods.Spooky.Tiles.SpiritPotstiary.ShipyardCratePotDescription"), 
			Language.GetText("Mods.Spooky.Tiles.SpiritPotstiary.ShipyardCratePotName"));
			//upper catacomb
			spiritReforged.Call("ManualAddRecord", ModContent.TileType<UpperCatacombPots>(), new int[] { 0, 1, 2 }, "SpookyUpperCatacombsPot", (byte)1, () => false, 
			(Action<int, Point16, ILoot>)UpperCatacombPots.LootTable, Language.GetText("Mods.Spooky.Tiles.SpiritPotstiary.CatacombPotUpperDescription"), 
			Language.GetText("Mods.Spooky.Tiles.SpiritPotstiary.CatacombPotUpperName"));
			//lower catacomb
			spiritReforged.Call("ManualAddRecord", ModContent.TileType<LowerCatacombPots>(), new int[] { 0, 1, 2 }, "SpookyLowerCatacombsPot", (byte)1, () => false, 
			(Action<int, Point16, ILoot>)LowerCatacombPots.LootTable, Language.GetText("Mods.Spooky.Tiles.SpiritPotstiary.CatacombPotLowerDescription"), 
			Language.GetText("Mods.Spooky.Tiles.SpiritPotstiary.CatacombPotLowerName"));
			//christmas presents
			spiritReforged.Call("ManualAddRecord", ModContent.TileType<ChristmasPresentPots>(), new int[] { 0, 1, 2 }, "SpookyChristmasPots", (byte)1, () => false, 
			(Action<int, Point16, ILoot>)ChristmasPresentPots.LootTable, Language.GetText("Mods.Spooky.Tiles.SpiritPotstiary.ChristmasPotDescription"), 
			Language.GetText("Mods.Spooky.Tiles.SpiritPotstiary.ChristmasPotName"));
			//tar pits
			spiritReforged.Call("ManualAddRecord", ModContent.TileType<TarPitsPots>(), new int[] { 0, 1, 2 }, "SpookyTarPitsPot", (byte)1, () => false, 
			(Action<int, Point16, ILoot>)TarPitsPots.LootTable, Language.GetText("Mods.Spooky.Tiles.SpiritPotstiary.TarPitsPotDescription"), 
			Language.GetText("Mods.Spooky.Tiles.SpiritPotstiary.TarPitsPotName"));
			//rotten depths
			spiritReforged.Call("ManualAddRecord", ModContent.TileType<OceanPots>(), new int[] { 0, 1, 2 }, "SpookyOceanPot", (byte)1, () => false, 
			(Action<int, Point16, ILoot>)OceanPots.LootTable, Language.GetText("Mods.Spooky.Tiles.SpiritPotstiary.OceanPotDescription"), 
			Language.GetText("Mods.Spooky.Tiles.SpiritPotstiary.OceanPotName"));
			//fetid farms
			spiritReforged.Call("ManualAddRecord", ModContent.TileType<FarmsPots>(), new int[] { 0, 1, 2 }, "SpookyFarmsPot", (byte)1, () => false, 
			(Action<int, Point16, ILoot>)FarmsPots.LootTable, Language.GetText("Mods.Spooky.Tiles.SpiritPotstiary.FarmsPotDescription"), 
			Language.GetText("Mods.Spooky.Tiles.SpiritPotstiary.FarmsPotName"));
			//nose temple
			spiritReforged.Call("ManualAddRecord", ModContent.TileType<NoseTemplePots>(), new int[] { 0, 1, 2 }, "SpookyNoseTemplePot", (byte)1, () => false, 
			(Action<int, Point16, ILoot>)NoseTemplePots.LootTable, Language.GetText("Mods.Spooky.Tiles.SpiritPotstiary.NoseTemplePotDescription"), 
			Language.GetText("Mods.Spooky.Tiles.SpiritPotstiary.NoseTemplePotName"));
			//spider cave
			spiritReforged.Call("ManualAddRecord", ModContent.TileType<SpiderCavePots>(), new int[] { 0, 1, 2 }, "SpookySpiderCavePot", (byte)1, () => false, 
			(Action<int, Point16, ILoot>)SpiderCavePots.LootTable, Language.GetText("Mods.Spooky.Tiles.SpiritPotstiary.SpiderCavePotDescription"), 
			Language.GetText("Mods.Spooky.Tiles.SpiritPotstiary.SpiderCavePotName"));
			//spooky biome
			spiritReforged.Call("ManualAddRecord", ModContent.TileType<SpookyBiomePots>(), new int[] { 0, 1, 2 }, "SpookyBiomePot", (byte)1, () => false, 
			(Action<int, Point16, ILoot>)SpookyBiomePots.LootTable, Language.GetText("Mods.Spooky.Tiles.SpiritPotstiary.SpookyBiomePotDescription"), 
			Language.GetText("Mods.Spooky.Tiles.SpiritPotstiary.SpookyBiomePotName"));
		}

		public override void Load()
		{
			Instance = this;

			ModLoader.TryGetMod("SubworldLibrary", out subworldLibrary);
			ModLoader.TryGetMod("ThoriumMod", out thoriumMod);
			ModLoader.TryGetMod("CalamityMod", out calamityMod);
			ModLoader.TryGetMod("SpiritReforged", out spiritReforged);

			AccessoryHotkey = KeybindLoader.RegisterKeybind(this, "AccessoryHotkey", "E");

			if (Main.netMode != NetmodeID.Server)
			{
				Filters.Scene["Spooky:CemeterySky"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(0f, 135f, 35f).UseOpacity(0.001f), EffectPriority.VeryHigh);
				SkyManager.Instance["Spooky:CemeterySky"] = new CemeterySky();

				Filters.Scene["Spooky:ShipyardSky"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryHigh);
				SkyManager.Instance["Spooky:ShipyardSky"] = new ShipyardSky();

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
			spiritReforged = null;

			AccessoryHotkey = null;
			mod = null;
		}

		//method to send a packet manually syncing all npcs ai and locaAI values
		public static void ManuallySyncNPCAI(int npcWhoAmI)
		{
			if (Main.netMode != NetmodeID.SinglePlayer)
			{
				ModPacket packet = mod.GetPacket();
				packet.Write((byte)SpookyMessageType.ManuallySyncNPCAI);
				packet.Write((short)npcWhoAmI);
				packet.Write(Main.npc[npcWhoAmI].ai[0]);
				packet.Write(Main.npc[npcWhoAmI].ai[1]);
				packet.Write(Main.npc[npcWhoAmI].ai[2]);
				packet.Write(Main.npc[npcWhoAmI].ai[3]);
				packet.Write(Main.npc[npcWhoAmI].localAI[0]);
				packet.Write(Main.npc[npcWhoAmI].localAI[1]);
				packet.Write(Main.npc[npcWhoAmI].localAI[2]);
				packet.Write(Main.npc[npcWhoAmI].localAI[3]);
				packet.Send();
			}
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			SpookyMessageType messageType = (SpookyMessageType)reader.ReadByte();
			switch (messageType)
			{
				case SpookyMessageType.ManuallySyncNPCAI:
				{
					int npcID = reader.ReadInt16();
					Main.npc[npcID].ai[0] = reader.ReadSingle();
					Main.npc[npcID].ai[1] = reader.ReadSingle();
					Main.npc[npcID].ai[2] = reader.ReadSingle();
					Main.npc[npcID].ai[3] = reader.ReadSingle();
					Main.npc[npcID].localAI[0] = reader.ReadSingle();
					Main.npc[npcID].localAI[1] = reader.ReadSingle();
					Main.npc[npcID].localAI[2] = reader.ReadSingle();
					Main.npc[npcID].localAI[3] = reader.ReadSingle();
					break;
				}
				case SpookyMessageType.SpawnMoco:
				{
					NPC.NewNPC(null, (int)Flags.MocoSpawn.X, (int)Flags.MocoSpawn.Y, ModContent.NPCType<MocoSpawner>());
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
					int Turkey = NPC.NewNPC(null, (int)Flags.TurkeySpawn.X, (int)Flags.TurkeySpawn.Y, ModContent.NPCType<Turkey>());
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
					int Gnome = NPC.NewNPC(null, (int)Flags.MushGnomeSpawn.X, (int)Flags.MushGnomeSpawn.Y, Main.rand.Next(Gnomes));
					Main.npc[Gnome].velocity.X = Main.rand.NextBool() ? -1 : 1;
					break;
				}
				case SpookyMessageType.SpawnGhostAmbush:
				{
					Flags.SpawnGhostAmbush = true;
					NetMessage.SendData(MessageID.WorldData);
					break;
				}
				case SpookyMessageType.SpawnQueenConch:
				{
					Flags.SpawnQueenConch = true;
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
				case SpookyMessageType.OpenPirateChest:
				{
					NPC.NewNPC(null, (int)Flags.PirateChestSpawn.X, (int)Flags.PirateChestSpawn.Y, ModContent.NPCType<PirateChestVisuals>());
					break;
				}
				default:
				{
					Logger.Warn("Spooky Mod: Unknown Message type: " + messageType);
					break;
				}
			}
		}
	}
}

enum SpookyMessageType : byte
{
	ManuallySyncNPCAI,
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
	SpawnQueenConch,
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
	OpenPirateChest,
}