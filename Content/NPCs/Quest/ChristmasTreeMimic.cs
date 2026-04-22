using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Quest;
using Spooky.Content.NPCs.Quest.Projectiles;

namespace Spooky.Content.NPCs.Quest
{
	public class ChristmasTreeMimic : ModNPC
	{
		private static Asset<Texture2D> GlowTexture;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 6;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
				Position = new Vector2(15f, -20f),
				PortraitPositionXOverride = 0f,
				PortraitPositionYOverride = 0f,
				Velocity = -1
            };

			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			//floats
			writer.Write(NPC.localAI[0]);
			writer.Write(NPC.localAI[1]);
			writer.Write(NPC.localAI[2]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			//floats
			NPC.localAI[0] = reader.ReadSingle();
			NPC.localAI[1] = reader.ReadSingle();
			NPC.localAI[2] = reader.ReadSingle();
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 3000;
            NPC.damage = 55;
            NPC.defense = 0;
			NPC.width = 88;
			NPC.height = 152;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
			NPC.value = Item.buyPrice(0, 0, 10, 0);
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit7;
			NPC.DeathSound = SoundID.NPCDeath39 with { Pitch = -0.65f };
            NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ChristmasTreeMimic"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

		public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

			if (NPC.velocity.X != 0)
			{
				if (NPC.frameCounter > 8 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 6)
				{
					NPC.frame.Y = 1 * frameHeight;
				}
			}
			else
			{
				NPC.frame.Y = 0 * frameHeight;
			}
		}

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction = NPC.velocity.X <= 0 ? -1 : 1;

			switch ((int)NPC.ai[0])
			{
				//standing still
				case 0:
				{
					NPC.noGravity = false;
					NPC.noTileCollide = false;

					if (NPC.life < NPC.lifeMax)
					{
						NPC.ai[0]++;
						NPC.netUpdate = true;
					}

					break;
				}
				
				//move at player
				case 1:
				{
					NPC.noGravity = true;
					NPC.noTileCollide = true;

					int CollideWidth = 80;
					int CollideHeight = 20;
					Vector2 NPCCollisionPos = new Vector2(NPC.Center.X - 40, NPC.position.Y + (float)NPC.height - 20);

					bool IncreaseFallSpeed = false;
					bool HasLineOfSight = Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height);

					MoveBackAndFourth(player.Center, 5f, 0.65f, 150);

					if (NPC.position.X < player.position.X && NPC.position.X + (float)NPC.width > player.position.X + (float)player.width &&
					NPC.position.Y + (float)NPC.height < player.position.Y + (float)player.height - 16f)
					{
						IncreaseFallSpeed = true;
					}
					if (IncreaseFallSpeed)
					{
						NPC.velocity.Y += 0.5f;
					}
					else if (Collision.SolidCollision(NPCCollisionPos, CollideWidth, CollideHeight) && !HasLineOfSight && NPC.Bottom.Y < player.Top.Y)
					{
						Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 10;
						NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
					}
					else if (Collision.SolidCollision(NPCCollisionPos, CollideWidth, CollideHeight))
					{
						if (NPC.velocity.Y > 0f)
						{
							NPC.velocity.Y = 0f;
						}
						if (NPC.velocity.Y > -0.2)
						{
							NPC.velocity.Y -= 0.025f;
						}
						else
						{
							NPC.velocity.Y -= 0.2f;
						}
						if (NPC.velocity.Y < -10f)
						{
							NPC.velocity.Y = -10f;
						}
					}
					else
					{
						if (NPC.velocity.Y < 0f)
						{
							NPC.velocity.Y = 0f;
						}
						if (NPC.velocity.Y < 0.1)
						{
							NPC.velocity.Y += 0.025f;
						}
						else
						{
							NPC.velocity.Y += 0.25f;
						}
					}
					if (NPC.velocity.Y > 10f)
					{
						NPC.velocity.Y = 10f;
					}

					break;
				}

				//shoot stars out of the star on top
				case 2:
				{	
					break;
				}
			}
		}

		public void MoveBackAndFourth(Vector2 Center, float MaxSpeed, float Acceleration, int Distance)
        {
            //prevents the pet from getting stuck on sloped tiles
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

            Vector2 center2 = NPC.Center;
            Vector2 vector48 = Center - center2;
            float CenterDistance = vector48.Length();

            if (CenterDistance > Distance)
            {
                if (Center.X - NPC.position.X > 0f)
                {
                    NPC.velocity.X += Acceleration;
                    if (NPC.velocity.X > MaxSpeed)
                    {
                        NPC.velocity.X = MaxSpeed;
                    }
                }
                else
                {
                    NPC.velocity.X -= Acceleration;
                    if (NPC.velocity.X < -MaxSpeed)
                    {
                        NPC.velocity.X = -MaxSpeed;
                    }
                }
            }
            else
            {
                if (NPC.velocity.X >= 0)
                {
                    NPC.velocity.X += Acceleration;
                    if (NPC.velocity.X > MaxSpeed)
                    {
                        NPC.velocity.X = MaxSpeed;
                    }
                }
                else
                {
                    NPC.velocity.X -= Acceleration;
                    if (NPC.velocity.X < -MaxSpeed)
                    {
                        NPC.velocity.X = -MaxSpeed;
                    }
                }
            }
        }

		public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OldHunterTinsel>()));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
				for (int numGores = 1; numGores <= 6; numGores++)
                {
					if (Main.netMode != NetmodeID.Server) 
					{
						//Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ChristmasTreeMimicGore" + numGores).Type);
					}
				}
            }
        }
	}
}