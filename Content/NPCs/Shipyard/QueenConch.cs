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
        bool ResetFrameToZero = false;

        Vector2 SaveVelocity = Vector2.Zero;

        public enum AnimationState
		{
			EmergeFromShell, HideInShell, Idle, Wiggle, WiggleStop, Spin
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
            NPC.HitSound = SoundID.NPCHit48 with { Pitch = -0.5f };
            NPC.DeathSound = SoundID.NPCDeath1;
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
				if (NPC.frameCounter > 5)
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
                //jump out of ground and land
                case 0:
                {
                    NPC.localAI[0]++;
                    if (NPC.localAI[0] == 2)
                    {
                        CurrentFrameX = 3;
                        CurrentAnimation = AnimationState.Spin;

                        NPC.velocity = new Vector2(Main.rand.NextBool() ? Main.rand.Next(-6, -3) : Main.rand.Next(2, 7), -10);

                        NPC.netUpdate = true;
                    }

                    if (NPC.localAI[0] >= 2 && NPC.localAI[1] == 0)
                    {
                        NPC.rotation = NPC.velocity.Y * (NPC.spriteDirection == -1 ? 0.05f : -0.05f);
                        NPC.spriteDirection = NPC.velocity.X > 0 ? -1 : 1;
                    }
                    
                    if (NPC.localAI[0] >= 20 && NPC.localAI[1] == 0)
                    {
                        if (!NPCGlobalHelper.IsCollidingWithFloor(NPC, false))
                        {
                            NPC.velocity.Y += 0.45f;
                            NPC.velocity.X *= 0.985f;
                        }
                        else
                        {
                            CurrentFrameX = 0;
                            CurrentAnimation = AnimationState.EmergeFromShell;

                            NPC.velocity = Vector2.Zero;

                            ResetFrameToZero = true;

                            NPC.localAI[1]++;
                        }
                    }

                    if (NPC.localAI[1] > 0)
                    {
                        NPC.localAI[1]++;
                        if (NPC.localAI[1] == 40)
                        {
                            CurrentFrameX = 1;
                            CurrentAnimation = AnimationState.Idle;
                            ResetFrameToZero = true;
                        }

                        if (NPC.localAI[1] >= 120)
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }
                    }

                    break;
                }

                //move in an arc above player and shoot slime while doing spin out of shell animation
                case 1:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] >= 0)
                    {
                        Vector2 RotateTowards = player.Center - NPC.Center;

                        float RotateDirection = (float)Math.Atan2(RotateTowards.Y, RotateTowards.X) + 4.71f;
                        float RotateSpeed = 0.05f;

                        NPC.rotation = NPC.rotation.AngleTowards(RotateDirection - MathHelper.TwoPi, RotateSpeed);

                        NPC.ai[1] += 0.005f;

                        float theta = MathHelper.PiOver2 * MathF.Sin(NPC.ai[1] * 4) * 0.5f;
                        Vector2 GoTo = player.Center + new Vector2(0, -280).RotatedBy(theta);

                        if (NPC.Distance(GoTo) >= 10f)
                        {
                            Vector2 desiredVelocity = NPC.DirectionTo(GoTo) * 15;
                            NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
                        }
                        else
                        {
                            NPC.velocity *= 0.9f;
                        }
                    }

                    break;
                }

                //slam down attack where it spins and slams the ground, then spew out bubbles when emerging
                case 2:
                {
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