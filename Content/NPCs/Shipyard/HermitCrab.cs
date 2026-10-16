using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;
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
using Spooky.Content.NPCs.Shipyard.Projectiles;

namespace Spooky.Content.NPCs.Shipyard
{
	public class HermitCrab : ModNPC
	{
		int CurrentFrameX = 0; //0 = walking  1 = snipping, 2 = digging
		int DigInFloorCount = 0;
		bool SlowDown = false;

		Vector2 SaveGroundPosition = Vector2.Zero;

		public enum AnimationState
		{
			Idle, Thrown, Walking, SnippingClaw, Digging
		}

		private AnimationState CurrentAnimation
        {
			get => (AnimationState)NPC.ai[3];
			set => NPC.ai[3] = (float)value;
		}

		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 7;
			NPCID.Sets.ShouldBeCountedAsBoss[NPC.type] = true;

			/*
			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/CorklidQueenBestiary",
				Position = new Vector2(35f, 35f),
				PortraitPositionXOverride = 0f,
              	PortraitPositionYOverride = 12f
            };
			*/
		}

		public override void SendExtraAI(BinaryWriter writer)
        {
			//vector2
			writer.WriteVector2(SaveGroundPosition);

            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
			writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			//vector2
			SaveGroundPosition = reader.ReadVector2();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
			NPC.localAI[3] = reader.ReadSingle();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 720;
            NPC.damage = 30;
            NPC.defense = 0;
			NPC.width = 52;
			NPC.height = 52;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit25;
			NPC.DeathSound = SoundID.NPCDeath6;
            NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ShipyardBiome>().Type };
		}

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance * bossAdjustment);
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.HermitCrab"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ShipyardBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			if (!NPC.IsABestiaryIconDummy)
			{
                for (int i = 0; i < 4; i++)
                {
                    Vector2 offset = i switch
                    {
                        1 => new(0, -2),
                        2 => new(2, 0),
                        3 => new(0, 2),
                        _ => new(-2, 0)
                    };

                    Main.EntitySpriteDraw(DrawUtils.ColorSolid(NPCTexture.Value, Color.White), NPC.Center + offset - screenPos + new Vector2(0, NPC.gfxOffY + 2), NPC.frame,
                    NPC.GetAlpha(new Color(211, 118, 255) * 0.65f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
                }
            }

			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY + 2), NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			return false;
		}

		public override void FindFrame(int frameHeight)
        {
			if (Main.netMode != NetmodeID.Server)
			{
				NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 3;
			}

			NPC.frame.X = (int)(NPC.frame.Width * CurrentFrameX);

			if (CurrentAnimation == AnimationState.Idle)
			{
				NPC.frame.Y = 0 * frameHeight;
			}
			if (CurrentAnimation == AnimationState.Thrown)
			{
				NPC.frame.Y = 5 * frameHeight;
			}
			if (CurrentAnimation == AnimationState.Walking)
			{
				if (NPC.velocity.X == 0)
				{
					NPC.frame.Y = 0 * frameHeight;
				}
				else
				{
					NPC.frameCounter++;
					if (NPC.frameCounter > 4)
					{
						NPC.frame.Y = NPC.frame.Y + frameHeight;
						NPC.frameCounter = 0;
					}
					if (NPC.frame.Y >= frameHeight * 7)
					{
						NPC.frame.Y = 2 * frameHeight;
					}
				}
			}
			else if (CurrentAnimation == AnimationState.SnippingClaw || CurrentAnimation == AnimationState.Digging)
			{
				NPC.frameCounter++;
				if (NPC.frameCounter > 3)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 5)
				{
					NPC.frame.Y = 0 * frameHeight;
				}
			}
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return NPC.localAI[3] >= 40;
        }

		public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

			switch ((int)NPC.ai[0])
			{
				//being thrown
				case -1:
				{
					if (NPC.alpha == 255)
					{
						NPC.alpha = 0;
					}

					CurrentFrameX = 2;
					CurrentAnimation = AnimationState.Thrown;

					NPC.rotation += (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) * 0.025f * NPC.direction;

					NPC.velocity.Y += 0.3f;
					if (NPC.velocity.Y > 1f)
					{
						NPC.velocity.Y += 0.6f;
					}

					if (NPCGlobalHelper.IsCollidingWithFloor(NPC) && NPC.localAI[0] == 0)
					{
						NPC.velocity.Y = -NPC.velocity.Y * 0.2f;

						NPC.localAI[0]++;
						NPC.netUpdate = true;
					}

					if (NPC.localAI[0] > 0)
					{
						NPC.localAI[0]++;
						if (NPC.localAI[0] >= 25)
						{
							NPC.ai[0]++;
							NPC.netUpdate = true;
						}
					}

					break;
				}

				//walk at player
				case 0:
				{
					CurrentFrameX = 0;
					CurrentAnimation = AnimationState.Walking;

					NPC.rotation = 0;

					float SpeedModifier = 2f;

					if (NPC.Distance(player.Center) >= 300)
					{
						NPC.localAI[1]++;
						if (NPC.localAI[1] >= 180)
						{
							NPC.localAI[0] = 0;
							NPC.localAI[1] = 0;
							NPC.ai[0]++;
							NPC.netUpdate = true;
						}
					}

					if (Math.Abs(NPC.Center.X - player.Center.X) < 45f)
					{
						SlowDown = true;
						if (NPC.velocity.X == 0)
						{
							if (NPC.Distance(player.Center) <= 35f)
							{
								CurrentFrameX = 1;
								CurrentAnimation = AnimationState.SnippingClaw;

								NPC.localAI[0]++;
								if (NPC.localAI[0] % 20 == 0)
								{
									NPCGlobalHelper.ShootHostileProjectile(NPC, player.Center, Vector2.Zero, ModContent.ProjectileType<HermitCrabSnip>(), NPC.damage, 0f);
								}
							}
							else
							{
								CurrentFrameX = 0;
								CurrentAnimation = AnimationState.Idle;
							}
						}
						else
						{
							CurrentFrameX = 0;
							CurrentAnimation = AnimationState.Walking;
						}
					}
					else
					{
						SlowDown = false;
					}

					if (SlowDown)
					{
						NPC.velocity.X *= 0.9f;
						if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
						{
							NPC.velocity.X = 0f;
						}

						if (Collision.SolidCollision(new Vector2(NPC.position.X, NPC.Center.Y), NPC.width, NPC.height / 2))
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
					}
					else
					{
						if (NPC.direction > 0)
						{
							NPC.velocity.X = (NPC.velocity.X * 20f + SpeedModifier) / 21f;
						}
						if (NPC.direction < 0)
						{
							NPC.velocity.X = (NPC.velocity.X * 20f - SpeedModifier) / 21f;
						}

						bool IncreaseFallSpeed = false;
						bool HasLineOfSight = Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height);

						if (NPC.position.X < player.position.X && NPC.position.X + (float)NPC.width > player.position.X + (float)player.width &&
						NPC.position.Y + (float)NPC.height < player.position.Y + (float)player.height - 16f)
						{
							IncreaseFallSpeed = true;
						}
						if (IncreaseFallSpeed)
						{
							NPC.velocity.Y += 0.5f;
						}
						else if (Collision.SolidCollision(new Vector2(NPC.position.X, NPC.Center.Y), NPC.width, NPC.height / 2))
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
					}

					break;
				}

				//dig in ground and jump out elsewhere
				case 1:
				{
					if (NPC.localAI[1] == 0)
					{
						NPC.localAI[0]++;
						if (NPC.localAI[0] == 1)
						{
							SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack with { Pitch = 1.2f }, NPC.Center);

							CurrentFrameX = 2;
							CurrentAnimation = AnimationState.Thrown;

							NPC.velocity.Y = -5f;
						}

						if (NPC.localAI[0] < 30)
						{
							NPC.rotation += (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) * 0.025f * NPC.direction;
						}

						if (NPC.localAI[0] >= 30)
						{
							CurrentAnimation = AnimationState.Digging;

							NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) - 1.57f;

							NPC.behindTiles = true;

							NPC.velocity.Y += 0.4f;
							if (NPC.velocity.Y > 2f)
							{
								NPC.velocity.Y += 0.8f;
							}

							if (NPCGlobalHelper.IsCollidingWithFloor(NPC))
							{
								int NpcCenterY = (int)(NPC.Center.Y / 16);
								for (int TileCheckY = NpcCenterY; TileCheckY < NpcCenterY + 8; TileCheckY++)
								{
									Tile tile = Main.tile[(int)NPC.Center.X / 16, TileCheckY];
									if (tile.HasTile && Main.tileSolid[tile.TileType])
									{
										DigInFloorCount++;
									}
								}

								if (DigInFloorCount > 6)
								{
									NPC.velocity = Vector2.Zero;
									NPC.rotation = 0;
									NPC.localAI[1]++;
									NPC.netUpdate = true;
								}
							}
						}
					}
					else
					{
						NPC.localAI[1]++;
						if (NPC.localAI[1] < 60)
						{
							if (NPC.localAI[1] % 10 == 0)
							{
								SoundEngine.PlaySound(SoundID.Dig, NPC.Center);
								NPC.velocity.Y = 12f;
								NPC.alpha += 51;
							}
							else
							{
								NPC.velocity = Vector2.Zero;
							}
						}
						else
						{
							if (SaveGroundPosition == Vector2.Zero)
							{
								Vector2 center = player.Center - new Vector2(Main.rand.Next(-50, 51), 50);
								int numtries = 0;
								int x = (int)(center.X / 16);
								int y = (int)(center.Y / 16);
								while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && 
								Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y)) 
								{
									y++;
									center.Y = y * 16;
								}
								while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 10) 
								{
									numtries++;
									y--;
									center.Y = y * 16;
								}

								SaveGroundPosition = center;
								NPC.netUpdate = true;
							}
							else
							{
								NPC.localAI[2]++;
								if (NPC.localAI[2] >= 30)
								{
									NPC.localAI[3]++;
									if (NPC.localAI[3] == 1)
									{
										NPCGlobalHelper.ShootHostileProjectile(NPC, SaveGroundPosition + new Vector2(0, 15), Vector2.Zero, ModContent.ProjectileType<HermitCrabGroundEffect>(), 0, 0);
									}
									if (NPC.localAI[3] == 40)
									{
										CurrentFrameX = 2;
										CurrentAnimation = AnimationState.Thrown;

										NPC.behindTiles = false;
										NPC.position.X = (SaveGroundPosition.X - (float)(NPC.width / 2)) + 8f;
										NPC.position.Y = (SaveGroundPosition.Y - (float)NPC.height) + 18f;
										NPC.velocity.X = 0f;
										NPC.velocity.Y = -9f;
										NPC.alpha = 0;

										SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact with { Pitch = 1.2f, Volume = 0.5f }, NPC.Center);
										Screenshake.ShakeScreenWithIntensity(NPC.Center, 2f, 300f);

										NPC.netUpdate = true;
									}
									if (NPC.localAI[3] > 40)
									{
										NPC.rotation += (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) * 0.035f * NPC.direction;
									
										NPC.velocity.Y += 0.4f;

										if (NPC.velocity.Y >= 0 && NPCGlobalHelper.IsCollidingWithFloor(NPC))
										{
											SaveGroundPosition = Vector2.Zero;
											NPC.velocity = Vector2.Zero;

											DigInFloorCount = 0;
											NPC.localAI[0] = 0;
											NPC.localAI[1] = 0;
											NPC.localAI[2] = 0;
											NPC.localAI[3] = 0;
											NPC.ai[0] = 0;

											NPC.netUpdate = true;
										}
									}
								}
							}
						}
					}

					break;
				}
			}
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0)
            {
			
            }
        }
	}
}