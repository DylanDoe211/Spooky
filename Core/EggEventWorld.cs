using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Spooky.Content.Biomes;
using Spooky.Content.NPCs.EggEvent;
using Spooky.Content.NPCs.EggEvent.Projectiles;

namespace Spooky.Core
{
	public class EggEventWorld : ModSystem
	{
		public static int EventTimeLeft = 0;
		public static int EventTimeLeftUI = 0;
		public static int EnemySpawnTimer = 0;
		public static bool EggEventActive = false;
		public static bool HasSpawnedBiojetter1 = false;
		public static bool HasSpawnedBiojetter2 = false;
		public static bool HasSpawnedBolster1 = false;
		public static bool HasSpawnedBolster2 = false;
		public static bool HasSpawnedBolster3 = false;

		public override void NetSend(BinaryWriter writer)
		{
			writer.Write(EventTimeLeft);
			writer.Write(EventTimeLeftUI);
			writer.Write(EnemySpawnTimer);

			writer.WriteFlags(EggEventActive, HasSpawnedBiojetter1, HasSpawnedBiojetter2, HasSpawnedBolster1, HasSpawnedBolster2, HasSpawnedBolster3);
		}

		public override void NetReceive(BinaryReader reader)
		{
			EventTimeLeft = reader.ReadInt32();
			EventTimeLeftUI = reader.ReadInt32();
			EnemySpawnTimer = reader.ReadInt32();

			reader.ReadFlags(out EggEventActive, out HasSpawnedBiojetter1, out HasSpawnedBiojetter2, out HasSpawnedBolster1, out HasSpawnedBolster2, out HasSpawnedBolster3);
		}

		public override void OnWorldLoad()
		{
			EventTimeLeft = 0;
			EventTimeLeftUI = 0;
			EggEventActive = false;
			HasSpawnedBiojetter1 = false;
			HasSpawnedBiojetter2 = false;
			HasSpawnedBolster1 = false;
			HasSpawnedBolster2 = false;
			HasSpawnedBolster3 = false;
		}

		//select a random player in the event if they are in the eye valley and arent dead/inactive
		public static int GetRandomPlayerInEvent()
		{
			int[] list = new int[] { };

			foreach (Player player in Main.ActivePlayers)
			{
				if (!player.dead && !player.ghost && player.InModBiome(ModContent.GetInstance<SpookyHellBiome>()))
				{
					list = list.Append(player.whoAmI).ToArray();
				}
			}

			return list[Main.rand.Next(0, list.Length)];
		}

		public bool AnyPlayersInBiome()
		{
			foreach (Player player in Main.ActivePlayers)
			{
				int playerInBiomeCount = 0;

				if (!player.dead && !player.ghost && player.InModBiome(ModContent.GetInstance<SpookyHellBiome>()))
				{
					playerInBiomeCount++;
				}

				if (playerInBiomeCount >= 1)
				{
					return true;
				}
			}

			return false;
		}

		//get the total number of active egg incursion enemies
		public int EventActiveNPCCount()
		{
			int NpcCount = 0;

			foreach (NPC Enemy in Main.ActiveNPCs)
			{
				int[] EventNPCs = new int[] { ModContent.NPCType<Biojetter>(), ModContent.NPCType<CoughLungs>(), ModContent.NPCType<CruxBat>(), ModContent.NPCType<EarWorm>(), ModContent.NPCType<EarWormFalling>(),
				ModContent.NPCType<ExplodingAppendix>(), ModContent.NPCType<GooSlug>(), ModContent.NPCType<HoppingHeart>(), ModContent.NPCType<HoverBrain>(), ModContent.NPCType<TongueBiter>() };

				if (EventNPCs.Contains(Enemy.type))
				{
					NpcCount++;
				}
				else
				{
					continue;
				}
			}

			return NpcCount;
		}

		//get the total number of active ear worms since they are spawned in manuallys
		public int EarWormCount()
		{
			int NpcCount = 0;

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC Enemy = Main.npc[i];

				if (Enemy.active && Enemy.type == ModContent.NPCType<EarWorm>())
				{
					NpcCount++;
				}
				else
				{
					continue;
				}
			}

			return NpcCount;
		}

		//spawn an enemy based on the type inputted
		public static void SpawnEnemy(int BiomassType, int Type)
		{
			switch (BiomassType)
			{
				case 0:
				{
					//Types:
					//0 = GooSlug
					//1 = CruxBat
					//2 = Earworm
					//3 = Biojetter
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Player player = Main.player[GetRandomPlayerInEvent()];

						int Biomass = NPC.NewNPC(null, (int)(player.Center.X + Main.rand.Next(-600, 600)), 
						(int)(Flags.EggPosition.Y + Main.rand.Next(100, 150)), ModContent.NPCType<GiantBiomassPurple>(), ai2: Type);
						Main.npc[Biomass].netUpdate = true;

						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.SyncNPC, number: Biomass);
						}
					}

					break;
				}

				case 1:
				{
					//Types:
					//0 = HoppingHeart
					//1 = TongueBiter
					//2 = ExplodingAppendix
					//3 = CoughLungs
					//4 = HoverBrain
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Player player = Main.player[GetRandomPlayerInEvent()];

						int Biomass = NPC.NewNPC(null, (int)(player.Center.X + Main.rand.Next(-600, 600)), 
						(int)(Flags.EggPosition.Y + Main.rand.Next(100, 150)), ModContent.NPCType<GiantBiomassRed>(), ai2: Type);
						Main.npc[Biomass].netUpdate = true;

						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.SyncNPC, number: Biomass);
						}
					}

					break;
				}

				case 2:
				{
					//spawn stomach enemy
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Player player = Main.player[GetRandomPlayerInEvent()];

						int Biomass = NPC.NewNPC(null, (int)(player.Center.X + Main.rand.Next(-600, 600)), 
						(int)(Flags.EggPosition.Y + Main.rand.Next(100, 150)), ModContent.NPCType<GiantBiomassOrange>());
						Main.npc[Biomass].netUpdate = true;

						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.SyncNPC, number: Biomass);
						}
					}

					break;
				}
			}
		}

		public override void PostUpdateEverything()
		{
			if (!EggEventActive)
			{
				EventTimeLeft = 0;
				EventTimeLeftUI = 0;
			}

			if (EggEventActive)
			{
				//end the event and reset everything if you die, or if you leave the valley of eyes
				if (!AnyPlayersInBiome())
				{
					EggEventActive = false;
					EventTimeLeft = 0;
					EventTimeLeftUI = 0;
					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.WorldData);
					}

					return;
				}

				if (EventTimeLeft < 21600)
				{
					//increment both timers
					//the timer for the UI gets decreased so that the actual time displayed on the UI bar is counting down and not up
					EventTimeLeft++;
					EventTimeLeftUI--;

					//timeLeft converts the time left to actual seconds, goes up to 360 seconds (or 6 minutes)
					//60 = 1 minute in
					//120 = 2 minutes in
					//180 = 3 minutes in
					//240 = 4 minutes in
					//300 = 5 minutes in
					//360 = 6 minutes in
					float timeLeft = EventTimeLeft / 60;

					int ChanceToSpawnEnemy = 300;
					if (timeLeft >= 60) ChanceToSpawnEnemy = 300;
					if (timeLeft >= 120) ChanceToSpawnEnemy = 250;
					if (timeLeft >= 180) ChanceToSpawnEnemy = 200;
					if (timeLeft >= 240) ChanceToSpawnEnemy = 150;

					if (EventTimeLeft == 3600 || EventTimeLeft == 7200 || EventTimeLeft == 10800 || EventTimeLeft == 14400 || EventTimeLeft == 18000)
					{
						SpawnEnemy(2, 0);
					}

					//spawn a biojetter a little before 3 minutes and a little after 4 minutes
					if (!HasSpawnedBiojetter1 && timeLeft >= 150)
					{
						SpawnEnemy(0, 3);

						HasSpawnedBiojetter1 = true;

						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.WorldData);
						}
					}
					if (!HasSpawnedBiojetter2 && timeLeft >= 280)
					{
						SpawnEnemy(0, 3);

						HasSpawnedBiojetter2 = true;

						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.WorldData);
						}
					}

					//spawn bolsters at 3 minutes, 4 minutes, and 5 minutes in
					if (!HasSpawnedBolster1 && timeLeft >= 180)
					{
						SpawnEnemy(1, 5);

						HasSpawnedBolster1 = true;

						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.WorldData);
						}
					}
					if (!HasSpawnedBolster2 && timeLeft >= 240)
					{
						SpawnEnemy(1, 5);

						HasSpawnedBolster2 = true;

						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.WorldData);
						}
					}
					if (!HasSpawnedBolster3 && timeLeft >= 300)
					{
						SpawnEnemy(1, 5);

						HasSpawnedBolster3 = true;

						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.WorldData);
						}
					}

					//if theres no enemies for too long, then manually spawn a bunch of them
					if (EventActiveNPCCount() <= 1)
					{
						EnemySpawnTimer++;
						if (EnemySpawnTimer >= 240)
						{
							for (int numEnemies = 0; numEnemies <= 5; numEnemies++)
							{
								if (timeLeft < 60)
								{
									int BiomassType = Main.rand.Next(0, 2);
									SpawnEnemy(BiomassType, BiomassType == 0 ? 0 : Main.rand.Next(0, 2));
								}
								if (timeLeft >= 60 && timeLeft < 180)
								{
									int BiomassType = Main.rand.Next(0, 2);
									SpawnEnemy(BiomassType, BiomassType == 0 ? Main.rand.Next(0, 2) : Main.rand.Next(0, 3));
								}
								if (timeLeft >= 180)
								{
									int BiomassType = Main.rand.Next(0, 2);
									SpawnEnemy(BiomassType, BiomassType == 0 ? Main.rand.Next(0, 3) : Main.rand.Next(0, 5));
								}
							}

							EnemySpawnTimer = -60;
						}
					}

					//randomly spawn enemies throughout the event
					if (EventActiveNPCCount() < 20 && Main.rand.NextBool(ChanceToSpawnEnemy))
					{
						if (timeLeft < 60)
						{
							int BiomassType = Main.rand.Next(0, 2);
							SpawnEnemy(BiomassType, BiomassType == 0 ? 0 : Main.rand.Next(0, 2));
						}
						if (timeLeft >= 60 && timeLeft < 180)
						{
							int BiomassType = Main.rand.Next(0, 2);
							SpawnEnemy(BiomassType, BiomassType == 0 ? Main.rand.Next(0, 2) : Main.rand.Next(0, 3));
						}
						if (timeLeft >= 180)
						{
							int BiomassType = Main.rand.Next(0, 2);
							SpawnEnemy(BiomassType, BiomassType == 0 ? Main.rand.Next(0, 3) : Main.rand.Next(0, 5));
						}
					}
				}
				else
				{
					EventTimeLeft = 21600;
					EventTimeLeftUI = 0;
				}
			}
		}
	}
}