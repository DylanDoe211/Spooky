using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Shipyard
{
	public class GiantSnail : ModNPC
	{
        public int CurrentBehavior = 0;
		public int GoBackInShellTimer = 0;
        public int JumpOutOfShellDelay = 0;
        public int SaveDirection = 0;

        private static Asset<Texture2D> ShellTexture;
        private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
				Velocity = 1f,
                Position = new Vector2(20f, 0f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 0f
			};
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
            //ints
            writer.Write(CurrentBehavior);
            writer.Write(GoBackInShellTimer);
            writer.Write(JumpOutOfShellDelay);
            writer.Write(SaveDirection);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //ints
            CurrentBehavior = reader.ReadInt32();
            GoBackInShellTimer = reader.ReadInt32();
            JumpOutOfShellDelay = reader.ReadInt32();
            SaveDirection = reader.ReadInt32();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 50;
            NPC.damage = 0;
			NPC.defense = 0;
			NPC.width = 44;
			NPC.height = 44;
            NPC.npcSlots = 0.5f;
            NPC.noGravity = false;
            NPC.chaseable = false;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.aiStyle = 0;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ShipyardBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.GiantSnail"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ShipyardBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //draw aura
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);
            ShellTexture ??= ModContent.Request<Texture2D>(Texture + "Shell");

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            //draw aura
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

                    Main.EntitySpriteDraw(DrawUtils.ColorSolid(NPCTexture.Value, Color.White), NPC.Center + offset - screenPos + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, 
                    NPC.GetAlpha(Color.Tan * 0.65f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
                }
            }

            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, drawColor * 0.9f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            Main.EntitySpriteDraw(ShellTexture.Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            return false;
        }
        
        public override void FindFrame(int frameHeight)
		{
            if (NPC.IsABestiaryIconDummy)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > 5)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }

                if (NPC.frame.Y < frameHeight * 5)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }

                if (NPC.frame.Y >= frameHeight * 9)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }
            }
            else
            {
                if (CurrentBehavior == 0)
                {
                    NPC.frameCounter++;
                    if (NPC.frameCounter > 5)
                    {
                        NPC.frame.Y = NPC.frame.Y - frameHeight;
                        NPC.frameCounter = 0;
                    }

                    if (NPC.frame.Y <= frameHeight * 0)
                    {
                        NPC.frame.Y = 0 * frameHeight;
                    }
                }
                else
                {
                    if (NPC.velocity.X == 0)
                    {
                        NPC.frame.Y = 4 * frameHeight;
                    }
                    else
                    {
                        NPC.frameCounter++;
                        if (NPC.frameCounter > 5)
                        {
                            NPC.frame.Y = NPC.frame.Y + frameHeight;
                            NPC.frameCounter = 0;
                        }

                        if (NPC.frame.Y >= frameHeight * 9)
                        {
                            NPC.frame.Y = 5 * frameHeight;
                        }
                    }
                }
            }
		}

        public override void AI()
        {
            if (SaveDirection == 0)
            {
                SaveDirection = Main.rand.NextBool() ? -1 : 1;
                NPC.netUpdate = true;
            }

            if (NPCGlobalHelper.IsCollidingWithFloor(NPC, true))
            {
                if (CurrentBehavior == 0)
                {
                    NPC.spriteDirection = SaveDirection;
                }
                else
                {
                    NPC.spriteDirection = NPC.direction;
                }

                NPC.rotation = 0;
            }
            else
            {
                if (NPC.velocity.X != 0)
                {
                    NPC.spriteDirection = NPC.direction = NPC.velocity.X < 0 ? -1 : 1;
                }

                //NPC.rotation += (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) * 0.025f * (NPC.velocity.X < 0 ? -1 : 1);
                NPC.rotation = NPC.velocity.Y * (NPC.direction == 1 ? 0.04f : -0.04f);
            }

            if (CurrentBehavior == 0)
            {
                NPC.aiStyle = 0;

                if (JumpOutOfShellDelay <= 0)
                {
                    foreach (Player player in Main.ActivePlayers)
                    {
                        if (player.Distance(NPC.Center) <= 200f)
                        {
                            SoundEngine.PlaySound(SoundID.NPCDeath1 with { Pitch = -0.5f, Volume = 0.5f }, NPC.Center);

                            NPC.velocity.X = player.Center.X > NPC.Center.X ? -2 : 2;
                            NPC.velocity.Y = -6;

                            CurrentBehavior = 1;
                            NPC.netUpdate = true;

                            break;
                        }
                    }
                }
                else
                {
                    JumpOutOfShellDelay--;
                }
            }
            else
            {
                NPC.aiStyle = 66;
			    AIType = NPCID.Buggy;

                if (NPC.velocity.X == 0)
                {
                    GoBackInShellTimer++;
                }
                else
                {
                    if (GoBackInShellTimer >= 120)
                    {
                        SaveDirection = NPC.direction;
                        CurrentBehavior = 0;
                        GoBackInShellTimer = 0;
                        JumpOutOfShellDelay = 240;
                        NPC.netUpdate = true;
                    }
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/GiantSnailShellGore").Type);
                }

                float maxAmount = 20;
                int currentAmount = 0;
                while (currentAmount <= maxAmount)
                {
                    Vector2 velocity = new Vector2(Main.rand.NextFloat(1f, 3f), Main.rand.NextFloat(1f, 3f));
                    Vector2 Bounds = new Vector2(Main.rand.NextFloat(1f, 3f), Main.rand.NextFloat(1f, 3f));
                    float intensity = Main.rand.NextFloat(1f, 3f);

                    Vector2 vector12 = Vector2.UnitX * 0f;
                    vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
                    vector12 = vector12.RotatedBy(velocity.ToRotation(), default);

                    int newDust = Dust.NewDust(NPC.Center, 1, 1, ModContent.DustType<GlowyDust>(), 0f, 0f, 0, Color.Tan, 0.2f);
                    Main.dust[newDust].noGravity = true;
                    Main.dust[newDust].position = NPC.Center + vector12;
                    Main.dust[newDust].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;

                    currentAmount++;
                }
            }
        }
	}
}