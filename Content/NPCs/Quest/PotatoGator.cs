using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Quest;

namespace Spooky.Content.NPCs.Quest
{
	public class PotatoGator : ModNPC
	{
		bool IsInsideOfBlocks = false;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 5;
			NPCGlobal.IsSpookyModMiniboss[Type] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Position = new Vector2(35f, 5f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 12f
            };

			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			//floats
			writer.Write(NPC.localAI[0]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			//floats
			NPC.localAI[0] = reader.ReadSingle();
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 2500;
            NPC.damage = 55;
            NPC.defense = 0;
			NPC.width = 140;
			NPC.height = 68;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
			NPC.value = Item.buyPrice(0, 0, 10, 0);
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.behindTiles = true;
			NPC.HitSound = SoundID.NPCHit11;
			NPC.DeathSound = SoundID.NPCDeath5 with { Pitch = -0.65f };
            NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.VegetableBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PotatoGator"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.VegetableBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void FindFrame(int frameHeight)
        {
			if (IsInsideOfBlocks)
			{
				NPC.frame.Y = 0 * frameHeight;
			}
			else
			{
				NPC.frameCounter++;

				if (NPC.frameCounter > 3)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 5)
				{
					NPC.frame.Y = 1 * frameHeight;
				}
			}
		}

        public override void AI()
        {
			NPC.TargetClosest(false);
			Player player = Main.player[NPC.target];

			NPC.rotation = NPC.velocity.Y * (0.04f * NPC.direction);

			if (NPC.velocity.X < 0)
			{
				NPC.spriteDirection = -1;
			}
			else
			{
				NPC.spriteDirection = 1;
			}

			bool IsSolidTile = true;
			Point TilePoint = NPC.Center.ToTileCoordinates();
			IsSolidTile = WorldGen.SolidTile2(TilePoint.X, TilePoint.Y);
			bool CanTargetPlayer = false;
			Vector2 TargetPosition = NPC.targetRect.Center.ToVector2();
			if (player.velocity.Y > -0.1f && !player.dead && NPC.Distance(TargetPosition) > 150f)
			{
				CanTargetPlayer = true;
			}

			if (NPC.localAI[0] == -1f && !IsSolidTile)
			{
				NPC.localAI[0] = 20f;
			}
			if (NPC.localAI[0] > 0f)
			{
				NPC.localAI[0]--;
			}
			
			if (IsSolidTile)
			{
				IsInsideOfBlocks = true;

				if (NPC.soundDelay == 0)
				{
					float DelayForDigSound = NPC.Distance(TargetPosition) / 40f;
					if (DelayForDigSound < 10f)
					{
						DelayForDigSound = 10f;
					}
					if (DelayForDigSound > 20f)
					{
						DelayForDigSound = 20f;
					}
					
					NPC.soundDelay = (int)DelayForDigSound;
					SoundEngine.PlaySound(SoundID.WormDig with { Volume = 0.35f, Pitch = -0.5f }, NPC.Center);
				}

				if (NPC.ai[2] < 30f)
				{
					NPC.ai[2]++;
				}

				if (CanTargetPlayer)
				{
					NPC.TargetClosest();
					NPC.velocity.X += (float)NPC.direction * 0.15f;
					NPC.velocity.Y += (float)NPC.directionY * 0.15f;

					if (NPC.velocity.X > 5f)
					{
						NPC.velocity.X = 5f;
					}
					if (NPC.velocity.X < -5f)
					{
						NPC.velocity.X = -5f;
					}
					if (NPC.velocity.Y > 3f)
					{
						NPC.velocity.Y = 3f;
					}
					if (NPC.velocity.Y < -3f)
					{
						NPC.velocity.Y = -3f;
					}

					Vector2 NPCPosWithVelocity = NPC.Center + NPC.velocity.SafeNormalize(Vector2.Zero) * NPC.Size.Length() / 2f + NPC.velocity;
					TilePoint = NPCPosWithVelocity.ToTileCoordinates();
					bool SolidTileWithNewPoint = WorldGen.SolidTile2(TilePoint.X, TilePoint.Y);

					//jump out of the ground and growl
					if (!SolidTileWithNewPoint && Math.Sign(NPC.velocity.X) == NPC.direction && NPC.Distance(TargetPosition) < 400f && (NPC.ai[2] >= 30f || NPC.ai[2] < 0f))
					{
						if (NPC.localAI[0] == 0f)
						{
							SoundEngine.PlaySound(SoundID.Zombie39 with { Pitch = -0.5f }, NPC.Center);
							NPC.localAI[0] = -1f;
						}

						NPC.ai[2] = -30f;
						Vector2 GoAbovePlayer = NPC.DirectionTo(TargetPosition + new Vector2(0f, -120f));
						NPC.velocity = GoAbovePlayer * 15f;
					}
				}
				else
				{
					if (NPC.collideX)
					{
						NPC.velocity.X *= -1f;
						NPC.direction *= -1;
						NPC.netUpdate = true;
					}
					if (NPC.collideY)
					{
						NPC.netUpdate = true;
						NPC.velocity.Y *= -1f;
						NPC.directionY = Math.Sign(NPC.velocity.Y);
						NPC.ai[0] = NPC.directionY;
					}
					float num1584 = 6f;
					NPC.velocity.X += (float)NPC.direction * 0.1f;

					if (NPC.velocity.X < 0f - num1584 || NPC.velocity.X > num1584)
					{
						NPC.velocity.X *= 0.95f;
					}

					if (IsSolidTile)
					{
						NPC.ai[0] = -1f;
					}
					else
					{
						NPC.ai[0] = 1f;
					}

					float MaxVerticalVel = 0.06f;
					float VerticalVelIncrease = 0.01f;

					if (NPC.ai[0] == -1f)
					{
						NPC.velocity.Y -= VerticalVelIncrease;
						if (NPC.velocity.Y < 0f - MaxVerticalVel)
						{
							NPC.ai[0] = 1f;
						}
					}
					else
					{
						NPC.velocity.Y += VerticalVelIncrease;
						if (NPC.velocity.Y > MaxVerticalVel)
						{
							NPC.ai[0] = -1f;
						}
					}
					if (NPC.velocity.Y > 0.4f || NPC.velocity.Y < -0.4f)
					{
						NPC.velocity.Y *= 0.9f;
					}
				}
			}
			else
			{
				IsInsideOfBlocks = false;

				if (NPC.velocity.Y == 0f)
				{
					if (CanTargetPlayer)
					{
						NPC.TargetClosest();
					}

					NPC.velocity.X += (float)NPC.direction * 0.1f;

					if (NPC.velocity.X < -1f || NPC.velocity.X > 1f)
					{
						NPC.velocity.X *= 0.95f;
					}
				}

				NPC.velocity.Y += 0.5f;

				if (NPC.velocity.Y > 10f)
				{
					NPC.velocity.Y = 10f;
				}
				
				NPC.ai[0] = 1f;
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OldHunterPotatoes>()));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
				for (int numGores = 1; numGores <= 7; numGores++)
                {
					if (Main.netMode != NetmodeID.Server) 
					{
						Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PotatoGatorGore" + numGores).Type);
					}
				}
            }
        }
	}
}