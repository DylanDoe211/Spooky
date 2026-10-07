using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
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
using Spooky.Content.Biomes;
using Spooky.Content.NPCs.Shipyard.Projectiles;

namespace Spooky.Content.NPCs.Shipyard
{
	public class QueenConch : ModNPC
	{
        int CurrentFrameX = 0; //0 = emerge from/go in shell  1 = idle animation  2 = wiggle animation, 3 = spin animation, 4 = spin animation but out of shell
        int SpinFramerate = 5;
        bool ResetFrameToZero = false;

        Vector2 SaveVelocity = Vector2.Zero;

        public enum AnimationState
		{
			EmergeFromShell, HideInShell, Idle, Wiggle, WiggleStop, Spin, Nothing
		}

		private AnimationState CurrentAnimation
        {
			get => (AnimationState)NPC.ai[3];
			set => NPC.ai[3] = (float)value;
		}

        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 13;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCGlobal.IsSpookyModMiniboss[Type] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 1000;
            NPC.damage = 35;
            NPC.defense = 15;
            NPC.width = 45;
			NPC.height = 45;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 50, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.Tink with { Pitch = -1.5f };
			NPC.DeathSound = SoundID.NPCDeath6;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ShipyardBiome>().Type };
        }

        //uses boss hp scaling so that it scales based on the amount of players
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance * bossAdjustment * 0.85f);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.QueenConch"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ShipyardBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 5;
            }

            NPC.frame.X = (int)(NPC.frame.Width * CurrentFrameX);

            NPC.frameCounter++;

            if (CurrentAnimation == AnimationState.EmergeFromShell)
			{
				if (NPC.frameCounter > 7)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}

				if (NPC.frame.Y >= frameHeight * 5)
				{
					NPC.frame.Y = 4 * frameHeight;
				}
            }
            else if (CurrentAnimation == AnimationState.HideInShell)
			{
				if (NPC.frameCounter > 7)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}

                if (NPC.frame.Y < frameHeight * 5)
				{
					NPC.frame.Y = 4 * frameHeight;
				}

				if (NPC.frame.Y >= frameHeight * 9)
				{
					NPC.frame.Y = 8 * frameHeight;
				}
            }
            else if (CurrentAnimation == AnimationState.Idle)
			{
				if (NPC.frameCounter > 5)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}

				if (NPC.frame.Y >= frameHeight * 4)
				{
					NPC.frame.Y = 0 * frameHeight;
				}
            }
            else if (CurrentAnimation == AnimationState.Wiggle)
			{
				if (NPC.frameCounter > 5)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}

				if (NPC.frame.Y >= frameHeight * 10)
				{
					NPC.frame.Y = 5 * frameHeight;
				}
            }
            else if (CurrentAnimation == AnimationState.WiggleStop)
			{
				if (NPC.frameCounter > 5)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}

				if (NPC.frame.Y >= frameHeight * 13)
				{
					NPC.frame.Y = 12 * frameHeight;
				}
            }
            else if (CurrentAnimation == AnimationState.Spin)
			{
				if (NPC.frameCounter > SpinFramerate)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}

				if (NPC.frame.Y >= frameHeight * 7)
				{
					NPC.frame.Y = 0 * frameHeight;
				}
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            
            return false;
        }

        public override void AI()
		{
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];

            if (NPC.alpha == 255)
            {
                NPC.alpha = 0;
            }

            switch ((int)NPC.ai[0])
            {
                //idle floating
                case 0:
                {
                    NPC.localAI[0]++;
                    if (NPC.localAI[0] == 1)
                    {
                        CurrentFrameX = 1;
                        CurrentAnimation = AnimationState.Idle;
                    }

                    if (NPC.localAI[0] <= 300)
                    {
                        Vector2 RotateTowards = player.Center - NPC.Center;
                        float RotateDirection = (float)Math.Atan2(RotateTowards.Y, RotateTowards.X) + 4.71f;
                        float RotateSpeed = 0.05f;
                        NPC.rotation = NPC.rotation.AngleTowards(RotateDirection - MathHelper.TwoPi, RotateSpeed);

                        NPC.ai[1] += 0.005f;

                        float theta = MathHelper.PiOver2 * MathF.Sin(NPC.ai[1] * 12) * 0.4f;
                        Vector2 GoTo = player.Center + new Vector2(0, -280).RotatedBy(theta);

                        Vector2 desiredVelocity = NPC.DirectionTo(GoTo) * 12;
                        NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
                    }
                    else
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[1] = 0;
                        NPC.ai[0] = Main.rand.NextBool() ? 1 : 2;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //move in an arc above player and shoot slime while doing spin out of shell animation
                case 1:
                {
                    NPC.localAI[0]++;
                    if (NPC.localAI[0] == 1)
                    {
                        CurrentFrameX = 2;
                        CurrentAnimation = AnimationState.Wiggle;

                        ResetFrameToZero = true;
                    }

                    if (NPC.localAI[0] <= 180)
                    {
                        Vector2 RotateTowards = player.Center - NPC.Center;
                        float RotateDirection = (float)Math.Atan2(RotateTowards.Y, RotateTowards.X) + 4.71f;
                        float RotateSpeed = 0.05f;
                        NPC.rotation = NPC.rotation.AngleTowards(RotateDirection - MathHelper.TwoPi, RotateSpeed);

                        NPC.ai[1] += 0.005f;

                        float theta = MathHelper.PiOver2 * MathF.Sin(NPC.ai[1] * 6) * 0.5f;
                        Vector2 GoTo = player.Center + new Vector2(0, -330).RotatedBy(theta);

                        Vector2 desiredVelocity = NPC.DirectionTo(GoTo) * 12;
                        NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);

                        //fire off ectoplasm down
                        if (NPC.localAI[0] % 15 == 0)
                        {
                            Vector2 ShootSpeed = player.Center - NPC.Center;
                            ShootSpeed.Normalize();
                            ShootSpeed *= 5f;

                            Vector2 position = NPC.Center;
                            Vector2 Offset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 15f;

                            if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
                            {
                                position += Offset;
                            }

                            NPCGlobalHelper.ShootHostileProjectile(NPC, position, new Vector2(ShootSpeed.X, 0), ModContent.ProjectileType<QueenConchSludge>(), NPC.damage, 4.5f);
                        }
                    }
                    else
                    {
                        CurrentAnimation = AnimationState.WiggleStop;

                        float RotateSpeed = 0.05f;
                        NPC.rotation = NPC.rotation.AngleTowards(0f, RotateSpeed);

                        NPC.velocity *= 0.96f;
                    }

                    if (NPC.localAI[0] >= 260)
                    {
                        CurrentFrameX = 1;
                        CurrentAnimation = AnimationState.Idle;
                    }

                    if (NPC.localAI[0] >= 300)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[1] = 0;
                        NPC.ai[0] = 3;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //move in an arc above player and shoot bubbles all over the place while spinning
                case 2:
                {
                    NPC.localAI[0]++;
                    if (NPC.localAI[0] == 1)
                    {
                        CurrentFrameX = 4;
                        CurrentAnimation = AnimationState.Spin;

                        ResetFrameToZero = true;
                    }

                    if (NPC.localAI[0] <= 180)
                    {
                        Vector2 RotateTowards = player.Center - NPC.Center;
                        float RotateDirection = (float)Math.Atan2(RotateTowards.Y, RotateTowards.X) + 4.71f;
                        float RotateSpeed = 0.05f;
                        NPC.rotation = NPC.rotation.AngleTowards(RotateDirection - MathHelper.TwoPi, RotateSpeed);

                        NPC.ai[1] += 0.005f;

                        float theta = MathHelper.PiOver2 * MathF.Sin(NPC.ai[1] * 6) * 0.75f;
                        Vector2 GoTo = player.Center + new Vector2(0, -330).RotatedBy(theta);

                        Vector2 desiredVelocity = NPC.DirectionTo(GoTo) * 12;
                        NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);

                        //fire off bubbles
                        if (NPC.localAI[0] % 15 == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item111 with { Volume = 0.5f }, NPC.Center);

                            Vector2 newVelocity = new Vector2(0, Main.rand.Next(6, 13)).RotatedByRandom(MathHelper.ToRadians(12));
                            NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, newVelocity, ModContent.ProjectileType<QueenConchBubble>(), NPC.damage, 4.5f);
                        }
                    }
                    else
                    {
                        CurrentFrameX = 1;
                        CurrentAnimation = AnimationState.Idle;

                        float RotateSpeed = 0.05f;
                        NPC.rotation = NPC.rotation.AngleTowards(0f, RotateSpeed);

                        NPC.velocity *= 0.96f;
                    }

                    if (NPC.localAI[0] >= 240)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[1] = 0;
                        NPC.ai[0] = 3;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //slam down attack where it spins and slams the ground, then spew out bubbles when emerging
                case 3:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[1] == 0)
                    {
                        //go inside shell animation
                        if (NPC.localAI[0] == 1)
                        {
                            CurrentFrameX = 0;
                            CurrentAnimation = AnimationState.HideInShell;
                        }

                        //spin animation
                        if (NPC.localAI[0] == 50)
                        {
                            CurrentFrameX = 3;
                            CurrentAnimation = AnimationState.Spin;

                            ResetFrameToZero = true;
                        }

                        if (NPC.localAI[0] < 50)
                        {
                            NPC.rotation += (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) * 0.01f;
                            NPC.velocity *= 0.96f;
                        }

                        //spin while hovering above the player, speed up animation
                        if (NPC.localAI[0] >= 50 && NPC.localAI[0] < 180)
                        {
                            Vector2 RotateTowards = player.Center - NPC.Center;
                            float RotateDirection = (float)Math.Atan2(RotateTowards.Y, RotateTowards.X) - 3.14f;
                            float RotateSpeed = 0.05f;
                            NPC.rotation = NPC.rotation.AngleTowards(RotateDirection - MathHelper.TwoPi, RotateSpeed);

                            Vector2 desiredVelocity = NPC.DirectionTo(player.Center - new Vector2(0, 320)) * 10f;
                            NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);

                            if (NPC.localAI[0] % 30 == 0 && SpinFramerate > 3)
                            {
                                SpinFramerate--;
                            }
                        }

                        //charge at player
                        if (NPC.localAI[0] == 180)
                        {
                            Vector2 ChargeDirection = player.Center - NPC.Center;
                            ChargeDirection.Normalize();
                            ChargeDirection *= 25;
                            NPC.velocity.X = ChargeDirection.X;
                            NPC.velocity.Y = 25f;

                            NPC.netUpdate = true;
                        }
                    
                        //handle stuff when it collides with the ground
                        if (NPC.localAI[0] > 180)
                        {
                            if (NPC.Center.Y >= player.Center.Y - 100 && NPCGlobalHelper.IsCollidingWithFloor(NPC, true))
                            {
                                CurrentAnimation = AnimationState.Nothing;
                                ResetFrameToZero = true;

                                NPC.velocity = Vector2.Zero;

                                Screenshake.ShakeScreenWithIntensity(NPC.Center, 7f, 350f);

                                SoundEngine.PlaySound(SoundID.Tink with { Pitch = -1f }, NPC.Center);
                                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, NPC.Center);

                                NPC.localAI[1]++;
                                NPC.netUpdate = true;
                            }
                            else
                            {
                            }
                        }
                    }
                    //once the actual slam is completed, emerge from shell and shoot some bubbles
                    else
                    {
                        NPC.localAI[1]++;
                        if (NPC.localAI[1] == 60)
                        {
                            CurrentFrameX = 0;
                            CurrentAnimation = AnimationState.EmergeFromShell;

                            ResetFrameToZero = true;
                        }

                        if (NPC.localAI[1] == 75)
                        {
                            SoundEngine.PlaySound(SoundID.Item111 with { Volume = 0.5f }, NPC.Center);

                            for (int numProjs = 0; numProjs < 8; numProjs++)
							{
								Vector2 newVelocity = new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 0)).RotatedByRandom(MathHelper.ToRadians(45));
                                NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, newVelocity, ModContent.ProjectileType<QueenConchBubble>(), NPC.damage, 4.5f);
                            }
                        }

                        if (NPC.localAI[1] == 85)
                        {
                            CurrentFrameX = 1;
                            CurrentAnimation = AnimationState.Idle;

                            ResetFrameToZero = true;
                        }

                        if (NPC.localAI[1] >= 150)
                        {
                            SpinFramerate = 5;
                            
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.ai[0] = 0;

                            NPC.netUpdate = true;
                        }
                    }

                    break;
                }
            }

            if (ResetFrameToZero)
            {
                NPC.frame.Y = 0;
                ResetFrameToZero = false;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
            }
        }
    }
}