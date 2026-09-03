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
	public class BarreleyeFish : ModNPC
	{
        private static Asset<Texture2D> GlowTexture;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
				Velocity = 1f
			};
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 100;
            NPC.damage = 0;
			NPC.defense = 0;
			NPC.width = 102;
			NPC.height = 46;
            NPC.noGravity = true;
            NPC.chaseable = false;
            NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ShipyardBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.BarreleyeFish"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ShipyardBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White * 0.5f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }
        
        public override void FindFrame(int frameHeight)
		{
            NPC.frameCounter++;
            if (NPC.frameCounter > 5)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 6)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
		}

        public override void AI()
        {
            NPC.spriteDirection = NPC.direction = NPC.velocity.X < 0 ? -1 : 1;

            if (NPC.ai[0] == 0)
            {
                NPC.velocity.X = Main.rand.NextBool() ? -0.5f : 0.5f;

                NPC.ai[0]++;
                NPC.netUpdate = true;
            }

            float MaxVelocityX = 0.5f;
            if (NPC.direction == -1 && NPC.velocity.X > -MaxVelocityX)
            {
                NPC.velocity.X -= 0.1f;
            }
            else if (NPC.direction == 1 && NPC.velocity.X < MaxVelocityX)
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

            bool GoUp = false;
            int PosX = (int)(NPC.Center.X / 16f);
            int PosY = (int)((NPC.position.Y + (float)NPC.height) / 16f);
            for (int TilePosY = PosY; TilePosY < PosY + 10; TilePosY++)
            {
                if (!WorldGen.InWorld(PosX, TilePosY, 10))
                {
                    continue;
                }
                if (WorldGen.SolidOrSlopedTile(PosX, TilePosY) || Main.tile[PosX, TilePosY].LiquidAmount > 0)
                {
                    GoUp = true; 
                    NPC.netUpdate = true;
                    break;
                }
            }
            
            if (!GoUp)
            {
                NPC.velocity.Y += 0.025f;
            }
            else
            {
                NPC.velocity.Y -= 0.025f;
            }

            //limit npc y-velocity
            if (NPC.velocity.Y > 1f)
            {
                NPC.velocity.Y = 1f;
            }
            if (NPC.velocity.Y < -1f)
            {
                NPC.velocity.Y = -1f;
            }
        }
	}
}