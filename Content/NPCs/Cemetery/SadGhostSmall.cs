using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Cemetery
{
    public class SadGhostSmall : ModNPC
    {
        public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;

        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 35;
            NPC.damage = 15;
            NPC.defense = 0;
            NPC.width = 38;
			NPC.height = 34;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit54 with { Pitch = 1f };
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.aiStyle = 22;
			AIType = NPCID.Wraith;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CemeteryBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.SadGhostSmall"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //draw aura
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            //draw aura
            for (int i = 0; i < 360; i += 90)
            {
                Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Lerp(Color.White, Color.Blue, i / 30));

                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 2f), 0).RotatedBy(MathHelper.ToRadians(i));

                spriteBatch.Draw(NPCTexture.Value, NPC.Center + circular - screenPos + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 1.075f, effects, 0f);
            }

            return true;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
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

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<SadDebuff>(), 300);
        }

        public override void AI()
		{
            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.05f;
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

                    int newDust = Dust.NewDust(NPC.Center, 1, 1, ModContent.DustType<GlowyDust>(), 0f, 0f, 0, Color.CornflowerBlue, 0.25f);
                    Main.dust[newDust].noGravity = true;
                    Main.dust[newDust].position = NPC.Center + vector12;
                    Main.dust[newDust].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;

                    currentAmount++;
                }
            }
        }
    }
}