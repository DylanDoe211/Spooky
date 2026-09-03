using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Shipyard
{
	public class GhostJelly1 : ModNPC
	{
        public static List<int> JellyTypes = new()
		{
			ModContent.NPCType<GhostJelly1>(), 
            ModContent.NPCType<GhostJelly2>(), 
            ModContent.NPCType<GhostJelly3>(), 
            ModContent.NPCType<GhostJelly4>()
		};

        private static Asset<Texture2D> GlowTexture;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 5;
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 70;
            NPC.damage = 35;
			NPC.defense = 0;
			NPC.width = 30;
			NPC.height = 66;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit25;
			NPC.DeathSound = SoundID.NPCDeath28;
            NPC.alpha = 80;
			NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ShipyardBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.GhostJelly"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ShipyardBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Cap");

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White * 0.5f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }
        
        public override void FindFrame(int frameHeight)
		{
            NPC.frameCounter++;
            int FrameCounter = NPC.velocity.Y >= 0 ? 10 : 3;
            if (NPC.frameCounter > FrameCounter)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 5)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
		}

        public override void AI()
        {
            NPC.spriteDirection = NPC.direction = NPC.velocity.X < 0 ? -1 : 1;

            NPC.rotation = NPC.velocity.X * 0.07f;

            Lighting.AddLight(NPC.Center, new Color(116, 244, 212).ToVector3() * 0.5f);

            if (NPC.ai[1] == 0)
            {
                NPC.ai[1] = Main.rand.NextBool() ? -1 : 1;
                NPC.netUpdate = true;
            }
            else
            {
                if (NPC.ai[2] == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int numJellies = -1; numJellies <= 1; numJellies += 2)
                        {
                            int NewEnemy = NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X + (30 * numJellies), (int)NPC.Center.Y + (NPC.height / 2), Main.rand.Next(JellyTypes), ai1: NPC.ai[1], ai2: 1);
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, number: NewEnemy);
                            }
                        }
                    }

                    NPC.ai[2]++;
                    NPC.netUpdate = true;
                }
            }

            float MaxVelocityX = 0.5f;
            if (NPC.ai[1] == -1 && NPC.velocity.X > -MaxVelocityX)
            {
                NPC.velocity.X -= 0.1f;
            }
            else if (NPC.ai[1] == 1 && NPC.velocity.X < MaxVelocityX)
            {
                NPC.velocity.X += 0.1f;
            }

            if (NPC.velocity.X < -MaxVelocityX)
            {
                NPC.velocity.X = -MaxVelocityX;
            }
            if (NPC.velocity.X > MaxVelocityX)
            {
                NPC.velocity.X = MaxVelocityX;
            }

            int PosX = (int)(NPC.Center.X / 16f);
            int PosY = (int)((NPC.position.Y + (float)NPC.height) / 16f);
            for (int TilePosY = PosY; TilePosY < PosY + 5; TilePosY++)
            {
                if (!WorldGen.InWorld(PosX, TilePosY, 10))
                {
                    continue;
                }
                if (NPC.ai[0] == 0 && (WorldGen.SolidOrSlopedTile(PosX, TilePosY) || Main.tile[PosX, TilePosY].LiquidAmount > 0))
                {
                    NPC.ai[0] = Main.rand.Next(15, 46);
                    NPC.netUpdate = true;
                    break;
                }
            }
            
            if (NPC.ai[0] <= 0)
            {
                NPC.velocity.Y += 0.025f;
            }
            else
            {
                NPC.velocity.Y -= 0.1f;
                NPC.ai[0]--;
            }

            //limit npc y-velocity
            if (NPC.velocity.Y > 1f)
            {
                NPC.velocity.Y = 1f;
            }
            if (NPC.velocity.Y < -4f)
            {
                NPC.velocity.Y = -4f;
            }
        }
	}

    public class GhostJelly2 : GhostJelly1
	{
        private static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Cap");

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 5), 
            NPC.frame, Color.White * 0.5f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
                NPC BestiaryParent = new();
                BestiaryParent.SetDefaults(ModContent.NPCType<GhostJelly1>());
                Main.BestiaryTracker.Kills.RegisterKill(BestiaryParent);
            }
        }
    }

    public class GhostJelly3 : GhostJelly1
	{
        private static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Cap");

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White * 0.5f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
                NPC BestiaryParent = new();
                BestiaryParent.SetDefaults(ModContent.NPCType<GhostJelly1>());
                Main.BestiaryTracker.Kills.RegisterKill(BestiaryParent);
            }
        }
    }

    public class GhostJelly4 : GhostJelly1
	{
        private static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Cap");

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 5), 
            NPC.frame, Color.White * 0.5f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
                NPC BestiaryParent = new();
                BestiaryParent.SetDefaults(ModContent.NPCType<GhostJelly1>());
                Main.BestiaryTracker.Kills.RegisterKill(BestiaryParent);
            }
        }
    }
}