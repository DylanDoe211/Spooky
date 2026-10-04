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

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Shipyard
{
	public class BarreleyeFish : ModNPC
	{
        private static Asset<Texture2D> NPCTexture;
        private static Asset<Texture2D> GlowTexture;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Position = new Vector2(25f, 0f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 0f
            };
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 100;
            NPC.damage = 0;
			NPC.defense = 0;
			NPC.width = 102;
			NPC.height = 46;
            NPC.npcSlots = 0.5f;
            NPC.noGravity = true;
            NPC.chaseable = false;
            NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ShipyardBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.BarreleyeFish"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new BestiaryBackgroundOverlay("Spooky/Content/Biomes/ShipyardBiomeNight_Background", Color.White)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

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
                    NPC.GetAlpha(Color.Aqua * 0.65f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
                }
            }

            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, drawColor * 0.9f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White * 0.5f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            return false;
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
            if (NPC.ai[1] == 0)
            {
                NPC.spriteDirection = Main.rand.NextBool() ? -1 : 1;

                NPC.ai[1]++;
                NPC.netUpdate = true;
            }
            else
            {
                NPC.spriteDirection = NPC.velocity.X < 0 ? -1 : 1;
            }

            float MaxVelocityX = 0.5f;
            float MaxVelocityY = 1f;
            if (NPC.spriteDirection == -1 && NPC.velocity.X > -MaxVelocityX)
            {
                NPC.velocity.X -= 0.1f;
            }
            else if (NPC.spriteDirection == 1 && NPC.velocity.X < MaxVelocityX)
            {
                NPC.velocity.X += 0.1f;
            }

            NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -MaxVelocityX, MaxVelocityX);

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

            NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -MaxVelocityY, MaxVelocityY);
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
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

                    int newDust = Dust.NewDust(NPC.Center, 1, 1, ModContent.DustType<GlowyDust>(), 0f, 0f, 0, Color.Aqua, 0.2f);
                    Main.dust[newDust].noGravity = true;
                    Main.dust[newDust].position = NPC.Center + vector12;
                    Main.dust[newDust].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;

                    currentAmount++;
                }
            }
        }
	}
}