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

namespace Spooky.Content.NPCs.Shipyard
{
	public class HermitCrab : ModNPC
	{
		int CurrentFrameX = 0; //0 = walking  1 = snipping, 2 = digging

		public enum AnimationState
		{
			Thrown, Walking, Digging
		}

		private AnimationState CurrentAnimation
        {
			get => (AnimationState)NPC.ai[3];
			set => NPC.ai[3] = (float)value;
		}

		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 5;
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
            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //floats
            SpeedModifier = reader.ReadSingle();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 850;
            NPC.damage = 25;
            NPC.defense = 10;
			NPC.width = 32;
			NPC.height = 32;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit29 with { Pitch = -1.5f };
			NPC.DeathSound = SoundID.NPCDeath31 with { Pitch = -1f };
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
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.CorklidQueen"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderWarBiome>().ModBiomeBestiaryInfoElement),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			return false;
		}

		public override void FindFrame(int frameHeight)
        {
			if (Main.netMode != NetmodeID.Server)
			{
				NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 3;
			}

			NPC.frame.X = (int)(NPC.frame.Width * CurrentFrameX);

			//go into the ground, use Xframe 0 and plays animation in reverse
			if (CurrentAnimation == AnimationState.Thrown)
			{
				NPC.frameCounter++;
				if (NPC.frameCounter > 4)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 4)
				{
					NPC.frame.Y = 0 * frameHeight;
				}
			}
			//emerge out of the ground, use Xframe 0 and plays animation normally
			else if (CurrentAnimation == AnimationState.Walking)
			{
				NPC.frameCounter++;
				if (NPC.frameCounter > 4)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 4)
				{
					NPC.frame.Y = 0 * frameHeight;
				}
			}
			//same as above, but it stops at the frame right before its real body comes out of the ground for attacks
			else if (CurrentAnimation == AnimationState.Digging)
			{
				NPC.frameCounter++;
				if (NPC.frameCounter > 4)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 4)
				{
					NPC.frame.Y = 0 * frameHeight;
				}
			}
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return CurrentAnimation == AnimationState.Digging;
        }

		public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

			switch ((int)NPC.ai[0])
			{
				//walk at player
				case 0:
				{
					float SpeedModifier = 2.5f;
					bool SlowDown = false;

					if (Math.Abs(NPC.Center.X - player.Center.X) < 50f)
					{
						SlowDown = true;
					}
					if (SlowDown)
					{
						NPC.velocity.X *= 0.9f;
						if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
						{
							NPC.velocity.X = 0f;
						}

						if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
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
						else if ((!Collision.SolidCollision(NPC.position, NPC.width, NPC.height) && !HasLineOfSight) ||
						(Collision.SolidCollision(NPC.position, NPC.width, NPC.height) && !HasLineOfSight && NPC.Bottom.Y < player.Top.Y))
						{
							Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 10;
							NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
						}
						else if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
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

				//shoot up small rockets that come back down above player
				case 1:
				{
					break;
				}

				//spawn giant homing nuke bomb
				case 2:
				{
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