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

using Spooky.Content.Achievements;
using Spooky.Content.Biomes;
using Spooky.Content.NPCs.SpiderCave.SpiderWar;

namespace Spooky.Core
{
	public class SpiderWarWorld : ModSystem
	{
		public static int SpiderWarPoints = 0;
		public static int SpiderWarWave = 0;
		public static int SpiderWarMaxPoints = 1;
		
		public static float SpiderWarDisplayPoints = 0f;

		public static bool SpiderWarActive = false;

		public override void NetSend(BinaryWriter writer)
		{
			writer.Write(SpiderWarPoints);
			writer.Write(SpiderWarWave);
			writer.Write(SpiderWarMaxPoints);

			writer.WriteFlags(SpiderWarActive);
		}

		public override void NetReceive(BinaryReader reader)
		{
			SpiderWarPoints = reader.ReadInt32();
			SpiderWarWave = reader.ReadInt32();
			SpiderWarMaxPoints = reader.ReadInt32();

			reader.ReadFlags(out SpiderWarActive);
		}

		public override void OnWorldLoad()
		{
			SpiderWarPoints = 0;
			SpiderWarWave = 0;
			SpiderWarMaxPoints = 1;
			SpiderWarDisplayPoints = 0f;
			SpiderWarActive = false;
		}

		public bool AnyPlayersInBiome()
		{
			foreach (Player player in Main.ActivePlayers)
			{
				int playerInBiomeCount = 0;

				if (!player.dead && !player.ghost && player.InModBiome(ModContent.GetInstance<SpiderCaveBiome>()))
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
		public static int EventActiveNPCCount()
		{
			int NpcCount = 0;

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC Enemy = Main.npc[i];

				int[] EventNPCs = new int[] { ModContent.NPCType<CamelColonel>(), ModContent.NPCType<CorklidQueen>(), 
				ModContent.NPCType<EmperorMortar>(), ModContent.NPCType<EmpressJoro>(), ModContent.NPCType<OgreKing>() };

				if (Enemy.active && EventNPCs.Contains(Enemy.type))
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

		//get the total number of active egg incursion enemies
		public static int EventActiveSpotlightCount()
		{
			int NpcCount = 0;

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC Enemy = Main.npc[i];
				if (Enemy.active && Enemy.type == ModContent.NPCType<SpotlightSpiderFloor>())
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
		public static void SpawnEnemy(Player player, int Type)
		{
			//if the player is null (which is done on multiplayer clients) then dont do anything at all
			if (player == null)
			{
				return;
			}

            if (Main.netMode != NetmodeID.MultiplayerClient)
		    {
			    NPC.SpawnOnPlayer(Player.FindClosest(player.Center - new Vector2(-50, -50), 100, 100), Type);
            }
		}

		public override void PostUpdateEverything()
		{
			if (!SpiderWarActive)
			{
				SpiderWarPoints = 0;
				SpiderWarWave = 0;
				SpiderWarMaxPoints = 1;
				SpiderWarDisplayPoints = 0f;
			}

			if (SpiderWarActive)
			{
				if (SpiderWarWave >= 10 && !Flags.downedSpiderWar)
				{
					Flags.downedSpiderWar = true;

					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.WorldData);
					}
				}

				//end the event and reset everything if all players die, or if all players leave the spider grotto
				if (!AnyPlayersInBiome())
				{
					SpiderWarPoints = 0;
					SpiderWarDisplayPoints = 0f;
					SpiderWarWave = 0;
					SpiderWarActive = false;

					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.WorldData);
					}
				}
				else if (SpiderWarWave >= 10 && SpiderWarPoints >= 20)
				{
					SpiderWarPoints = 0;
					SpiderWarDisplayPoints = 0f;
					SpiderWarWave = 0;
					SpiderWarActive = false;

					if (Main.netMode != NetmodeID.Server)
                    {
						ModContent.GetInstance<EventAchievementSpiderWarEnd>().SpiderWarEndCondition.Complete();
					}

					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.WorldData);
					}
				}
				else
				{
					if (SpiderWarPoints >= SpiderWarMaxPoints)
					{
						SpiderWarPoints = SpiderWarMaxPoints;
					}

					if (SpiderWarDisplayPoints < SpiderWarPoints)
					{
						SpiderWarDisplayPoints += 0.025f;
					}
					if (SpiderWarDisplayPoints >= SpiderWarMaxPoints)
					{
						SpiderWarWave++;
						SpiderWarPoints = 0;
						SpiderWarDisplayPoints = 0f;

						Color TextColor = new Color(171, 64, 255);

						switch (SpiderWarWave)
						{
							case 0:
							{
								SpiderWarMaxPoints = 1;
								break;
							}
							case 1:
							{
								SpiderWarMaxPoints = 1;
								Main.NewText(Language.GetTextValue("Mods.Spooky.EventsAndBosses.SpiderWarWave") + " " + SpiderWarWave + ": " + 
								Language.GetTextValue("Mods.Spooky.NPCs.OgreKing.DisplayName"), TextColor);
								break;
							}
							case 2:
							{
								SpiderWarMaxPoints = 1;
								Main.NewText(Language.GetTextValue("Mods.Spooky.EventsAndBosses.SpiderWarWave") + " " + SpiderWarWave + ": " + 
								Language.GetTextValue("Mods.Spooky.NPCs.EmperorMortar.DisplayName"), TextColor);
								break;
							}
							case 3:
							{
								SpiderWarMaxPoints = 1;
								Main.NewText(Language.GetTextValue("Mods.Spooky.EventsAndBosses.SpiderWarWave") + " " + SpiderWarWave + ": " + 
								Language.GetTextValue("Mods.Spooky.NPCs.CorklidQueen.DisplayName"), TextColor);
								break;
							}
							case 4:
							{
								SpiderWarMaxPoints = 2;
								Main.NewText(Language.GetTextValue("Mods.Spooky.EventsAndBosses.SpiderWarWave") + " " + SpiderWarWave + ": " + 
								Language.GetTextValue("Mods.Spooky.NPCs.OgreKing.DisplayName") + ", " + Language.GetTextValue("Mods.Spooky.NPCs.EmperorMortar.DisplayName"), TextColor);
								break;
							}
							case 5:
							{
								SpiderWarMaxPoints = 2;
								Main.NewText(Language.GetTextValue("Mods.Spooky.EventsAndBosses.SpiderWarWave") + " " + SpiderWarWave + ": " + 
								Language.GetTextValue("Mods.Spooky.NPCs.CamelColonel.DisplayName"), TextColor);
								break;
							}
							case 6:
							{
								SpiderWarMaxPoints = 2;
								Main.NewText(Language.GetTextValue("Mods.Spooky.EventsAndBosses.SpiderWarWave") + " " + SpiderWarWave + ": " + 
								Language.GetTextValue("Mods.Spooky.NPCs.EmpressJoro.DisplayName"), TextColor);
								break;
							}
							case 7:
							{
								SpiderWarMaxPoints = 3;
								Main.NewText(Language.GetTextValue("Mods.Spooky.EventsAndBosses.SpiderWarWave") + " " + SpiderWarWave + ": " + 
								Language.GetTextValue("Mods.Spooky.NPCs.CamelColonel.DisplayName") + ", " + Language.GetTextValue("Mods.Spooky.NPCs.EmpressJoro.DisplayName"), TextColor);
								break;
							}
							case 8:
							{
								SpiderWarMaxPoints = 5;
								Main.NewText(Language.GetTextValue("Mods.Spooky.EventsAndBosses.SpiderWarWave") + " " + SpiderWarWave + ": " + 
								Language.GetTextValue("Mods.Spooky.NPCs.OgreKing.DisplayName") + ", " + Language.GetTextValue("Mods.Spooky.NPCs.EmperorMortar.DisplayName") + ", " +
								Language.GetTextValue("Mods.Spooky.NPCs.CamelColonel.DisplayName"), TextColor);
								break;
							}
							case 9:
							{
								SpiderWarMaxPoints = 5;
								Main.NewText(Language.GetTextValue("Mods.Spooky.EventsAndBosses.SpiderWarWave") + " " + SpiderWarWave + ": " + 
								Language.GetTextValue("Mods.Spooky.NPCs.EmperorMortar.DisplayName") + ", " + Language.GetTextValue("Mods.Spooky.NPCs.CorklidQueen.DisplayName") + ", " +
								Language.GetTextValue("Mods.Spooky.NPCs.EmpressJoro.DisplayName"), TextColor);
								break;
							}
							case 10:
							{
								SpiderWarMaxPoints = int.MaxValue;
								Main.NewText(Language.GetTextValue("Mods.Spooky.EventsAndBosses.SpiderWarFinalWave") + ": " + 
								Language.GetTextValue("Mods.Spooky.NPCs.OgreKing.DisplayName") + ", " + Language.GetTextValue("Mods.Spooky.NPCs.EmperorMortar.DisplayName") + ", " +
								Language.GetTextValue("Mods.Spooky.NPCs.CorklidQueen.DisplayName") + ", " + Language.GetTextValue("Mods.Spooky.NPCs.CamelColonel.DisplayName") + ", " +
								Language.GetTextValue("Mods.Spooky.NPCs.EmpressJoro.DisplayName"), TextColor);

								if (Main.netMode != NetmodeID.Server)
                    			{
									ModContent.GetInstance<EventAchievementSpiderWar>().SpiderWarCondition.Complete();
								}
								
								break;
							}
						}
					}
				}
			}
		}
	}
}