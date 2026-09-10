using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Shipyard
{
	public class Frogfish1 : ModNPC
	{
        int AISwitchTimer = 0;

        private static Asset<Texture2D> GlowTexture;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 6;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/FrogfishBestiary"
            };
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 50;
            NPC.damage = 20;
			NPC.defense = 0;
			NPC.width = 70;
			NPC.height = 45;
            NPC.npcSlots = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
            NPC.noGravity = false;
            NPC.chaseable = false;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = 0;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ShipyardBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Frogfish"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ShipyardBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Shipyard/FrogfishGlow");

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY - 4), 
            NPC.frame, Color.White * 0.1f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }
        
        public override void FindFrame(int frameHeight)
		{
            if (NPC.velocity.X != 0)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > 6)
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
            NPC.spriteDirection = NPC.direction;

            if (!NPC.wet)
            {
                AISwitchTimer = 0;

                NPC.noGravity = true;
                NPC.aiStyle = 16;
                AIType = NPCID.Piranha;
            }
            else
            {
                Player player = Main.player[NPC.target];
                bool HasLineOfSight = Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height);
                if (!player.wet || !HasLineOfSight)
                {
                    AISwitchTimer++;

                    if (AISwitchTimer >= 600)
                    {
                        if (NPC.aiStyle == 16)
                        {
                            NPC.noGravity = false;
                            NPC.rotation = 0;
                            NPC.aiStyle = 0;
                        }
                        else
                        {
                            NPC.noGravity = true;
                            NPC.aiStyle = 16;
                            AIType = NPCID.Piranha;
                        }

                        AISwitchTimer = 0;
                        NPC.netUpdate = true;
                    }
                }
                else
                {
                    NPC.noGravity = true;
                    NPC.aiStyle = 16;
                    AIType = NPCID.Piranha;
                }
            }
        }
	}

    public class Frogfish2 : Frogfish1
	{
        public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}
    }
}