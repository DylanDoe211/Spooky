using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Achievements;
using Spooky.Content.Items.Quest;
using Spooky.Content.Items.Slingshots;
using Spooky.Content.Items.SpiderCave;
using Spooky.Content.Items.SpiderCave.Misc;

namespace Spooky.Content.UserInterfaces
{
    public class OldHunterDialogueChoiceUI
    {
        public static int OldHunter = -1;
        public static bool UIOpen = false;

        public static Mod Mod = Spooky.mod;

        public static Vector2 modifier = new(-200, -75);
        public static readonly Vector2 UITopLeft = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);

        public static readonly SoundStyle TalkSound = new("Spooky/Content/Sounds/TalkSounds/OldHunterTalk", SoundType.Sound) { Volume = 3f, PitchVariance = 0.5f };

        private static Asset<Texture2D> BarTexture;
		private static Asset<Texture2D> BarHoverTexture;
        private static Asset<Texture2D> UITexture;

        public static void Draw(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;

			Mod Mod = Spooky.mod;

			//dont draw at all if the UI isnt open
			if (!UIOpen)
            {
                OldHunter = -1;
                return;
            }

            //stop the UI from being open if the player is doing other stuff
            if (player.chest != -1 || player.sign != -1 || player.talkNPC == -1 || !InRangeOfNPC() || Main.InGuideCraftMenu)
            {
                UIOpen = false;
                return;
            }

			if (player.controlInv)
			{
				if (player.talkNPC > -1)
				{
					player.SetTalkNPC(-1);
					Main.npcChatText = string.Empty;
				}
				OldHunter = -1;
				UIOpen = false;
			}

			Main.LocalPlayer.mouseInterface = true;
			Main.LocalPlayer.GetModPlayer<SpookyPlayer>().DisablePlayerControls = true;

            Main.instance.CameraModifiers.Add(new CameraPanning(Main.npc[OldHunter].Center, 20));

            Vector2 UIBoxScale = Vector2.One * Main.UIScale;

			string Choice1 = Language.GetTextValue("Mods.Spooky.UI.OldHunterDialogueChoice.RareItemChoice");
			string Choice2 = Language.GetTextValue("Mods.Spooky.UI.OldHunterDialogueChoice.RematchChoice");

            BarTexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/OldHunterDialogueChoiceUIBar");
			BarHoverTexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/OldHunterDialogueChoiceUIBarHover");
            UITexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/DialogueUIOldHunter");

            Vector2 UITopLeft1 = UITopLeft + new Vector2(0, -150 + (BarTexture.Height() / 2 * UIBoxScale.Y));
            Vector2 UITopLeft2 = UITopLeft + new Vector2(0, -100 + (BarTexture.Height() / 2 * UIBoxScale.Y) * 2);

			Vector2 RematchBarPosition = Flags.downedOldHunter ? UITopLeft2 : UITopLeft1;

			//top 2 UI bars shouldnt be accessible until after you beat the old hunter
			if (Flags.downedOldHunter)
			{
				//spriteBatch.Draw(BarTexture.Value, UITopLeft1, null, Color.White, 0f, BarTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);
				spriteBatch.Draw(BarTexture.Value, UITopLeft1, null, Color.White, 0f, BarTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);
			}
            spriteBatch.Draw(BarTexture.Value, RematchBarPosition, null, Color.White, 0f, BarTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);

            Color textColor1 = Color.White;
            Color textColor2 = Color.White;

			//dialogue and rewards for rare enemy items
			if (IsMouseOverUI(UITopLeft1, BarTexture.Value, UIBoxScale) && Flags.downedOldHunter)
            {
				spriteBatch.Draw(BarHoverTexture.Value, UITopLeft1, null, Color.White, 0f, BarHoverTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);

                textColor1 = Color.Gold;
                player.mouseInterface = true;

                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
					//pre spider war
					if (player.HasItem(ModContent.ItemType<ArchdukeBounty>()) && !Flags.downedSpiderWar)
					{
						DialogueChain chain = new();
						chain.Add(new(UITexture.Value, Main.npc[OldHunter],
						Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.SpiderWar1"),
						Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PlayerSpiderWar1"),
						TalkSound, 2f, 0f, modifier, NPCID: Main.npc[OldHunter].type))
						.Add(new(UITexture.Value, Main.npc[OldHunter],
						Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.SpiderWar2"),
						Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PlayerSpiderWar2"),
						TalkSound, 2f, 0f, modifier, NPCID: Main.npc[OldHunter].type))
						.Add(new(UITexture.Value, Main.npc[OldHunter],
						Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.SpiderWar3"),
						Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PlayerSpiderWar3"),
						TalkSound, 2f, 0f, modifier, NPCID: Main.npc[OldHunter].type))
						.Add(new(UITexture.Value, Main.npc[OldHunter],
						Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.SpiderWar4"),
						Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PlayerSpiderWar4"),
						TalkSound, 2f, 0f, modifier, NPCID: Main.npc[OldHunter].type))
						.Add(new(UITexture.Value, Main.npc[OldHunter], null, null, TalkSound, 2f, 0f, modifier, true));
						chain.OnPlayerResponseTrigger += PlayerResponse;
						chain.OnEndTrigger += EndDialogue;
						DialogueUI.Visible = true;
						DialogueUI.Add(chain);
					}
					//post spider war
					else if (Flags.downedSpiderWar && !Flags.OldHunterQuestEnd)
					{
						DialogueChain chain = new();
						chain.Add(new(UITexture.Value, Main.npc[OldHunter],
						Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PostSpiderWar1"),
						Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PlayerPostSpiderWar1"),
						TalkSound, 2f, 0f, modifier, NPCID: Main.npc[OldHunter].type))
						.Add(new(UITexture.Value, Main.npc[OldHunter],
						Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PostSpiderWar2"),
						Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PlayerPostSpiderWar2"),
						TalkSound, 2f, 0f, modifier, NPCID: Main.npc[OldHunter].type))
						.Add(new(UITexture.Value, Main.npc[OldHunter],
						Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PostSpiderWar3"),
						Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PlayerPostSpiderWar3"),
						TalkSound, 2f, 0f, modifier, NPCID: Main.npc[OldHunter].type))
						.Add(new(UITexture.Value, Main.npc[OldHunter],
						Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PostSpiderWar4"),
						Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PlayerPostSpiderWar4"),
						TalkSound, 2f, 0f, modifier, NPCID: Main.npc[OldHunter].type))
						.Add(new(UITexture.Value, Main.npc[OldHunter],
						Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PostSpiderWar5"),
						Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PlayerPostSpiderWar5"),
						TalkSound, 2f, 0f, modifier, NPCID: Main.npc[OldHunter].type))
						.Add(new(UITexture.Value, Main.npc[OldHunter],
						Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PostSpiderWar6"),
						Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PlayerPostSpiderWar6"),
						TalkSound, 2f, 0f, modifier, NPCID: Main.npc[OldHunter].type))
						.Add(new(UITexture.Value, Main.npc[OldHunter], null, null, TalkSound, 2f, 0f, modifier, true));
						chain.OnPlayerResponseTrigger += PlayerResponse;
						chain.OnEndTrigger += EndDialoguePostWar;
						DialogueUI.Visible = true;
						DialogueUI.Add(chain);
					}
					//normal dialogue
					else
					{
						/*
						dialogue order:
						1 = Tick
						2 = Harold Ghost
						3 = Chud Mushroom
						4 = Triplets
						5 = Putty Amalgam
						6 = Potato Gator
						7 = Christmas Tree
						8 = Tar Pig
						*/

						List<int> DialogueChoices = new List<int>()
						{
							1, 2, 3, 4, 5, 6, 7, 8
						};
						List<int> DialogueRewardChoices = new List<int>()
						{
							1, 2, 3, 4, 5, 6, 7, 8
						};

						//remove each specific dialogue choice if the corresponding quest is done already
						if (Flags.OldHunterQuest1)
						{
							DialogueChoices.RemoveAll(x => x == 1);
							DialogueRewardChoices.RemoveAll(x => x == 1);
						}
						if (Flags.OldHunterQuest2)
						{
							DialogueChoices.RemoveAll(x => x == 2);
							DialogueRewardChoices.RemoveAll(x => x == 2);
						}
						if (Flags.OldHunterQuest3)
						{
							DialogueChoices.RemoveAll(x => x == 3);
							DialogueRewardChoices.RemoveAll(x => x == 3);
						}
						if (Flags.OldHunterQuest4)
						{
							DialogueChoices.RemoveAll(x => x == 4);
							DialogueRewardChoices.RemoveAll(x => x == 4);
						}
						if (Flags.OldHunterQuest5)
						{
							DialogueChoices.RemoveAll(x => x == 5);
							DialogueRewardChoices.RemoveAll(x => x == 5);
						}
						if (Flags.OldHunterQuest6)
						{
							DialogueChoices.RemoveAll(x => x == 6);
							DialogueRewardChoices.RemoveAll(x => x == 6);
						}
						if (Flags.OldHunterQuest7)
						{
							DialogueChoices.RemoveAll(x => x == 7);
							DialogueRewardChoices.RemoveAll(x => x == 7);
						}
						if (Flags.OldHunterQuest8)
						{
							DialogueChoices.RemoveAll(x => x == 8);
							DialogueRewardChoices.RemoveAll(x => x == 8);
						}

						//complete quests if you have the item for said quest
						int CurrentCompletedQuest = 0;

						if (player.HasItem(ModContent.ItemType<OldHunterSac>()))
						{
							CurrentCompletedQuest = 1;
						}
						else if (player.HasItem(ModContent.ItemType<OldHunterCloth>()))
						{
							CurrentCompletedQuest = 2;
						}
						else if (player.HasItem(ModContent.ItemType<OldHunterMushroom>()))
						{
							CurrentCompletedQuest = 3;
						}
						else if (player.HasItem(ModContent.ItemType<OldHunterEyes>()))
						{
							CurrentCompletedQuest = 4;
						}
						else if (player.HasItem(ModContent.ItemType<OldHunterPutty>()))
						{
							CurrentCompletedQuest = 5;
						}
						else if (player.HasItem(ModContent.ItemType<OldHunterPotatoes>()))
						{
							CurrentCompletedQuest = 6;
						}
						else if (player.HasItem(ModContent.ItemType<OldHunterTinsel>()))
						{
							CurrentCompletedQuest = 7;
						}
						else if (player.HasItem(ModContent.ItemType<OldHunterPigBone>()))
						{
							CurrentCompletedQuest = 8;
						}

						//if no quest is completed then display quest dialogue
						if (CurrentCompletedQuest <= 0)
						{
							if (DialogueChoices.Count > 0)
							{
								int Choice = Main.rand.Next(DialogueChoices);

								DialogueChain chain = new();
								chain.Add(new(UITexture.Value, Main.npc[OldHunter],
								Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.Quest" + Choice + "-1"),
								Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PlayerQuest" + Choice + "-1"),
								TalkSound, 2f, 0f, modifier, NPCID: Main.npc[OldHunter].type))
								.Add(new(UITexture.Value, Main.npc[OldHunter],
								Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.Quest" + Choice + "-2"),
								Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PlayerQuest" + Choice + "-2"),
								TalkSound, 2f, 0f, modifier, NPCID: Main.npc[OldHunter].type))
								.Add(new(UITexture.Value, Main.npc[OldHunter], null, null, TalkSound, 2f, 0f, modifier, true));
								chain.OnPlayerResponseTrigger += PlayerResponse;
								chain.OnEndTrigger += EndDialogue;
								DialogueUI.Visible = true;
								DialogueUI.Add(chain);
							}
						}
						//display quest completed dialogue
						else
						{
							if (DialogueRewardChoices.Contains(CurrentCompletedQuest))
							{
								DialogueChain chain = new();
								chain.Add(new(UITexture.Value, Main.npc[OldHunter],
								Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.QuestComplete" + CurrentCompletedQuest + "-1"),
								Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PlayerQuestComplete" + CurrentCompletedQuest + "-1"),
								TalkSound, 2f, 0f, modifier, NPCID: Main.npc[OldHunter].type))
								.Add(new(UITexture.Value, Main.npc[OldHunter],
								Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.QuestComplete" + CurrentCompletedQuest + "-2"),
								Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PlayerQuestComplete" + CurrentCompletedQuest + "-2"),
								TalkSound, 2f, 0f, modifier, NPCID: Main.npc[OldHunter].type))
								.Add(new(UITexture.Value, Main.npc[OldHunter],
								Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.QuestComplete" + CurrentCompletedQuest + "-3"),
								Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PlayerQuestComplete" + CurrentCompletedQuest + "-3"),
								TalkSound, 2f, 0f, modifier, NPCID: Main.npc[OldHunter].type))
								.Add(new(UITexture.Value, Main.npc[OldHunter], null, null, TalkSound, 2f, 0f, modifier, true));
								chain.OnPlayerResponseTrigger += PlayerResponse;
								chain.OnEndTrigger += EndDialogueBountyComplete;
								DialogueUI.Visible = true;
								DialogueUI.Add(chain);
							}
							else
							{
								DialogueChain chain = new();
								chain.Add(new(UITexture.Value, Main.npc[OldHunter], null, null, TalkSound, 2f, 0f, modifier, true));
								chain.OnPlayerResponseTrigger += PlayerResponse;
								chain.OnEndTrigger += EndDialogueBountyComplete;
								DialogueUI.Visible = true;
								DialogueUI.Add(chain);
							}
						}
					}

					UIOpen = false;
                }
            }

			//old hunter boss rematch
			if (IsMouseOverUI(RematchBarPosition, BarTexture.Value, UIBoxScale))
            {
				spriteBatch.Draw(BarHoverTexture.Value, RematchBarPosition, null, Color.White, 0f, BarHoverTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);

                textColor2 = Color.Gold;
                player.mouseInterface = true;

                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
					if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        ModPacket packet = Mod.GetPacket();
                        packet.Write((byte)SpookyMessageType.SpawnOldHunter);
                        packet.Send();
                    }
                    else
                    {
                        Flags.SpawnOldHunter = true;
                    }

                    if (player.talkNPC > -1)
					{
						player.SetTalkNPC(-1);
						Main.npcChatText = string.Empty;
					}
					UIOpen = false;
                }
            }

			if (Flags.downedOldHunter)
			{
				DrawTextDescription(spriteBatch, UITopLeft1 + new Vector2(-52f, -10f), Choice1, textColor1);
			}
			DrawTextDescription(spriteBatch, RematchBarPosition + new Vector2(-52f, -10f), Choice2, textColor2);
        }

        public static bool InRangeOfNPC()
        {
            if (!Main.npc.IndexInRange(OldHunter) || !Main.npc[OldHunter].active)
            {
                return false;
            }

            Rectangle validTalkArea = Utils.CenteredRectangle(Main.LocalPlayer.Center, new Vector2(Player.tileRangeX * 3f, Player.tileRangeY * 2f) * 16f);
            
            return validTalkArea.Intersects(Main.npc[OldHunter].Hitbox);
        }

		public static void DrawTextDescription(SpriteBatch spriteBatch, Vector2 TextTopLeft, string Text, Color color)
		{
			Vector2 scale = new Vector2(0.9f, 0.925f) * MathHelper.Clamp(Main.screenHeight / 1440f, 0.825f, 1f) * Main.UIScale;

			foreach (string TextLine in Utils.WordwrapString(Text, FontAssets.MouseText.Value, 700, 16, out _))
			{
				if (string.IsNullOrEmpty(TextLine))
				{
					continue;
				}

				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, TextLine, TextTopLeft, color, 0f, Vector2.Zero, scale);
				TextTopLeft.Y += Main.UIScale * 16f;
			}
        }

        //check if the mouse is hovering over a specific button
        public static bool IsMouseOverUI(Vector2 TopLeft, Texture2D texture, Vector2 backgroundScale)
        {
            Rectangle backgroundArea = new Rectangle((int)TopLeft.X - (int)(texture.Width * backgroundScale.X) / 2, 
            (int)TopLeft.Y - (int)(texture.Height * backgroundScale.Y) / 2, 
            (int)(texture.Width * backgroundScale.X), (int)(texture.Height * backgroundScale.Y));

            if (backgroundArea.Contains(Main.mouseX, Main.mouseY))
			{
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void PlayerResponse(Dialogue dialogue, string Text, int ID)
		{
			Dialogue newDialogue = new(ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/DialogueUIPlayer").Value, Main.LocalPlayer,
			Text, null, SoundID.Item1, 2f, 0f, default, NotPlayer: false);
			DialogueUI.Visible = true;
			DialogueUI.Add(newDialogue);
		}

        public static void EndDialogue(Dialogue dialogue, int ID)
		{
			DialogueUI.Visible = false;
		}

		public static void EndDialogueBountyComplete(Dialogue dialogue, int ID)
		{
			GiveRewardAndSetComplete();

			DialogueUI.Visible = false;
		}

		public static void EndDialoguePostWar(Dialogue dialogue, int ID)
		{
			if (Main.netMode != NetmodeID.SinglePlayer)
			{
				ModPacket packet = Mod.GetPacket();
				packet.Write((byte)SpookyMessageType.OldHunterQuestEnd);
				packet.Send();
			}
			else
			{
				Flags.OldHunterQuestEnd = true;
			}

			DialogueUI.Visible = false;
		}

        public static void SpawnItem(int Type, int Amount)
        {
            int newItem = Item.NewItem(Main.LocalPlayer.GetSource_DropAsItem(), Main.LocalPlayer.Hitbox, Type, Amount);
			NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
        }

		public static void GiveRewardAndSetComplete()
		{
			Player player = Main.LocalPlayer;

			if (player.ConsumeItem(ModContent.ItemType<OldHunterSac>()))
			{
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					ModPacket packet = Mod.GetPacket();
					packet.Write((byte)SpookyMessageType.OldHunterQuest1Complete);
					packet.Send();
				}
				else
				{
					Flags.OldHunterQuest1 = true;
				}
			}
			else if (player.ConsumeItem(ModContent.ItemType<OldHunterCloth>()))
			{
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					ModPacket packet = Mod.GetPacket();
					packet.Write((byte)SpookyMessageType.OldHunterQuest2Complete);
					packet.Send();
				}
				else
				{
					Flags.OldHunterQuest2 = true;
				}
			}
			else if (player.ConsumeItem(ModContent.ItemType<OldHunterMushroom>()))
			{
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					ModPacket packet = Mod.GetPacket();
					packet.Write((byte)SpookyMessageType.OldHunterQuest3Complete);
					packet.Send();
				}
				else
				{
					Flags.OldHunterQuest3 = true;
				}
			}
			else if (player.ConsumeItem(ModContent.ItemType<OldHunterEyes>()))
			{
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					ModPacket packet = Mod.GetPacket();
					packet.Write((byte)SpookyMessageType.OldHunterQuest4Complete);
					packet.Send();
				}
				else
				{
					Flags.OldHunterQuest4 = true;
				}
			}
			else if (player.ConsumeItem(ModContent.ItemType<OldHunterPutty>()))
			{
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					ModPacket packet = Mod.GetPacket();
					packet.Write((byte)SpookyMessageType.OldHunterQuest5Complete);
					packet.Send();
				}
				else
				{
					Flags.OldHunterQuest5 = true;
				}
			}	
			else if (player.ConsumeItem(ModContent.ItemType<OldHunterPotatoes>()))
			{
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					ModPacket packet = Mod.GetPacket();
					packet.Write((byte)SpookyMessageType.OldHunterQuest6Complete);
					packet.Send();
				}
				else
				{
					Flags.OldHunterQuest6 = true;
				}
			}
			else if (player.ConsumeItem(ModContent.ItemType<OldHunterTinsel>()))
			{
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					ModPacket packet = Mod.GetPacket();
					packet.Write((byte)SpookyMessageType.OldHunterQuest7Complete);
					packet.Send();
				}
				else
				{
					Flags.OldHunterQuest7 = true;
				}
			}
			else if (player.ConsumeItem(ModContent.ItemType<OldHunterPigBone>()))
			{
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					ModPacket packet = Mod.GetPacket();
					packet.Write((byte)SpookyMessageType.OldHunterQuest8Complete);
					packet.Send();
				}
				else
				{
					Flags.OldHunterQuest8 = true;
				}
			}

			if (Flags.OldHunterQuest1 && Flags.OldHunterQuest2 && Flags.OldHunterQuest3 && Flags.OldHunterQuest4 &&
			Flags.OldHunterQuest5 && Flags.OldHunterQuest6 && Flags.OldHunterQuest7 && Flags.OldHunterQuest8)
			{
				ModContent.GetInstance<MiscAchievementOldHunterQuest>().OldHunterQuestCondition.Complete();
			}

			//actual unique weapons
			int[] MainItem = new int[] { ModContent.ItemType<ProSlingshot>(), ModContent.ItemType<MagicBeanBag>(), ModContent.ItemType<MetalFistBox>(), 
			ModContent.ItemType<PossessedCrown>(), ModContent.ItemType<TrackingCrossbow>(), ModContent.ItemType<WrestlingBelt>() };
			SpawnItem(Main.rand.Next(MainItem), 1);

			//give souls of night or light
			int[] Souls = new int[] { ItemID.SoulofNight, ItemID.SoulofLight };
			SpawnItem(Main.rand.Next(Souls), Main.rand.Next(8, 16));

			//give random tier 2 or 3 hardmode bars 
			int[] Bars = new int[] { ItemID.MythrilBar, ItemID.OrichalcumBar, ItemID.AdamantiteBar, ItemID.TitaniumBar };
			SpawnItem(Main.rand.Next(Bars), Main.rand.Next(12, 26));

			//give potions
			int[] Potions = new int[] { ItemID.BattlePotion, ItemID.CalmingPotion, ItemID.EndurancePotion, ItemID.LuckPotionGreater, ItemID.IronskinPotion,
			ItemID.LifeforcePotion, ItemID.MagicPowerPotion, ItemID.RegenerationPotion, ItemID.SummoningPotion, ItemID.WrathPotion };
			SpawnItem(Main.rand.Next(Potions), Main.rand.Next(2, 6));

			//coins
			SpawnItem(ItemID.GoldCoin, Main.rand.Next(5, 11));
		}
    }
}