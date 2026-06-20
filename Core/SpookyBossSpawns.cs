using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Chat;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Biomes;
using Spooky.Content.NPCs.Boss.BigBone;
using Spooky.Content.NPCs.Boss.Daffodil;
using Spooky.Content.NPCs.Boss.OldHunter;
using Spooky.Content.NPCs.Boss.Orroboro;
using Spooky.Content.NPCs.Cemetery.Projectiles;
using Spooky.Content.NPCs.EggEvent;
using Spooky.Content.NPCs.Friendly;
using Spooky.Content.NPCs.Minibiomes.Ocean;
using Spooky.Content.NPCs.NoseCult;
using Spooky.Content.NPCs.PandoraBox;

namespace Spooky.Core
{
    public class SpookyBossSpawns : ModSystem
    {
        public override void PostUpdateEverything()
        {
            if (Main.gameMenu)
            {
                return;
            }

            //spawn bosses with already existing npcs in the world (for multiplayer purposes)
            //spawn daffodil eye on her body
            if (Flags.SpawnDaffodil)
            {
                foreach (var npc in Main.ActiveNPCs)
			    {
                    if (npc != null && npc.type == ModContent.NPCType<DaffodilBody>())
                    {
                        //spawn message
                        if (!NPC.AnyNPCs(ModContent.NPCType<DaffodilEye>()))
                        {
                            string text = Language.GetTextValue("Mods.Spooky.EventsAndBosses.DaffodilSpawn");

                            if (Main.netMode == NetmodeID.Server)
                            {
                                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), new Color(171, 64, 255));
                            }
                            else if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                Main.NewText(text, 171, 64, 255);
                            }
                            
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int Daffodil = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X - 7, (int)npc.Center.Y + 29, 
                                ModContent.NPCType<DaffodilEye>(), ai0: Main.rand.NextBool(20) && Flags.downedDaffodil ? -4 : -1, ai1: npc.whoAmI);

                                if (Main.netMode == NetmodeID.Server)
                                {
                                    NetMessage.SendData(MessageID.SyncNPC, number: Daffodil);
                                }
                            }
                        }

                        break;
                    }
                }

                Flags.SpawnDaffodil = false;

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

            //spawn big bone from his flower pot
            if (Flags.SpawnBigBone)
            {
                foreach (var npc in Main.ActiveNPCs)
			    {
                    if (npc != null && npc.type == ModContent.NPCType<BigFlowerPot>())
                    {
                        //spawn message
                        if (!NPC.AnyNPCs(ModContent.NPCType<BigBone>()))
                        {
                            string text = Language.GetTextValue("Mods.Spooky.EventsAndBosses.BigBoneSpawn");

                            if (Main.netMode == NetmodeID.Server)
                            {
                                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), new Color(171, 64, 255));
                            }
                            else if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                Main.NewText(text, 171, 64, 255);
                            }
                            
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int BigBone = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<BigBone>(), ai3: npc.whoAmI);

                                if (Main.netMode == NetmodeID.Server)
                                {
                                    NetMessage.SendData(MessageID.SyncNPC, number: BigBone);
                                }
                            }
                        }

                        break;
                    }
                }

                Flags.SpawnBigBone = false;

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

            //spawn old hunter from either the corpse or friendly npc
            if (Flags.SpawnOldHunter)
            {
                foreach (var npc in Main.ActiveNPCs)
			    {
                    if (npc != null && (npc.type == ModContent.NPCType<OldHunterDead>() || npc.type == ModContent.NPCType<OldHunter>()))
                    {
                        //spawn message
                        if (!NPC.AnyNPCs(ModContent.NPCType<OldHunterBoss>()))
                        {
                            string text = Language.GetTextValue("Mods.Spooky.EventsAndBosses.OldHunterSpawn");

                            if (Main.netMode == NetmodeID.Server)
                            {
                                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), new Color(171, 64, 255));
                            }
                            else if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                Main.NewText(text, 171, 64, 255);
                            }
                            
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int OldHunter = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y + (npc.height / 2), 
                                ModContent.NPCType<OldHunterBoss>(), ai0: -1);
                                Main.npc[OldHunter].alpha = 255;

                                if (Main.netMode == NetmodeID.Server)
                                {
                                    NetMessage.SendData(MessageID.SyncNPC, number: OldHunter);
                                }
                            }
                        }

                        npc.active = false;
                        npc.netUpdate = true;

                        break;
                    }
                }

                Flags.SpawnOldHunter = false;

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

            //spawn orroboro at the egg
            if (Flags.SpawnOrroboro)
            {
                foreach (var npc in Main.ActiveNPCs)
			    {
                    if (npc != null && npc.type == ModContent.NPCType<OrroboroEgg>())
                    {
                        //spawn message
                        if (!NPC.AnyNPCs(ModContent.NPCType<OrroHeadP1>()) && !NPC.AnyNPCs(ModContent.NPCType<OrroHead>()) && !NPC.AnyNPCs(ModContent.NPCType<BoroHead>()))
                        {
                            string text = Language.GetTextValue("Mods.Spooky.EventsAndBosses.OrroboroSpawn");

                            if (Main.netMode == NetmodeID.Server)
                            {
                                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), new Color(171, 64, 255));
                            }
                            else if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                Main.NewText(text, 171, 64, 255);
                            }
                            
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int OrroBoro = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<OrroHeadP1>(), ai0: -1);

                                if (Main.netMode == NetmodeID.Server)
                                {
                                    NetMessage.SendData(MessageID.SyncNPC, number: OrroBoro);
                                }
                            }
                        }

                        break;
                    }
                }

                Flags.SpawnOrroboro = false;

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

            //mist ghost ambush
            if (Flags.SpawnGhostAmbush)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int GhostSpawner = NPC.NewNPC(null,  Flags.GhostAmbushSpawnX, Flags.GhostAmbushSpawnY, ModContent.NPCType<MistGhostSpawn>(), ai2: Flags.RaveyardHappening ? 1 : 0);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: GhostSpawner);
                    }
                }

                Flags.SpawnGhostAmbush = false;

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
            }
        }
    }
}