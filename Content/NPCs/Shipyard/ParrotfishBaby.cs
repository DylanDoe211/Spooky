using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Shipyard
{
	public class ParrotfishBaby : ModNPC
	{
        public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;

        Vector2 GoToPosition;

        private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 4;
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
            //vector2
            writer.WriteVector2(GoToPosition);

            //ints
            writer.Write(MoveSpeedX);
            writer.Write(MoveSpeedY);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //vector2
            GoToPosition = reader.ReadVector2();

            //ints
            MoveSpeedX = reader.ReadInt32();
            MoveSpeedY = reader.ReadInt32();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 60;
            NPC.damage = 0;
			NPC.defense = 0;
			NPC.width = 56;
			NPC.height = 38;
            NPC.npcSlots = 1f;
            NPC.noGravity = true;
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
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ParrotfishBaby"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ShipyardBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

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
                    NPC.GetAlpha(Color.DodgerBlue * 0.65f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
                }
            }

            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, drawColor * 0.9f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            return false;
        }
        
        public override void FindFrame(int frameHeight)
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

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
		{
			NPC Parent = Main.npc[(int)NPC.ai[1]];
			if (Parent.ai[0] == 0)
            {
                Parent.ai[0]++;
            }
		}

		public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
		{
			NPC Parent = Main.npc[(int)NPC.ai[1]];
			if (Parent.ai[0] == 0)
            {
                Parent.ai[0]++;
            }
		}

        public override void AI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[1]];

            NPC.rotation = NPC.velocity.Y * (NPC.spriteDirection == 1 ? 0.03f : -0.03f);

            NPC.spriteDirection = NPC.velocity.X < 0 ? -1 : 1;

            if (Parent.active && Parent.type == ModContent.NPCType<Parrotfish>())
            {
                NPC.localAI[0]++;

                //randomly go to a position around the parent npc
                if (NPC.localAI[0] == 1 || NPC.localAI[0] % 10 == 0)
                {
                    GoToPosition = new Vector2(Main.rand.Next(-125, 126), Main.rand.Next(-80, 81));
                    NPC.netUpdate = true;
                }

                Vector2 GoTo = Parent.Center + GoToPosition;

                Vector2 desiredVelocity = NPC.DirectionTo(GoTo) * 2;
                NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
            }
            else
            {
                float MaxVelocityX = 1f;
                float MaxVelocityY = 1.5f;
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
                for (int TilePosY = PosY; TilePosY < PosY + 5; TilePosY++)
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
                    NPC.velocity.Y += 0.045f;
                }
                else
                {
                    NPC.velocity.Y -= 0.045f;
                }

                NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -MaxVelocityY, MaxVelocityY);
            }
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

                    int newDust = Dust.NewDust(NPC.Center, 1, 1, ModContent.DustType<GlowyDust>(), 0f, 0f, 0, Color.DodgerBlue, 0.2f);
                    Main.dust[newDust].noGravity = true;
                    Main.dust[newDust].position = NPC.Center + vector12;
                    Main.dust[newDust].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;

                    currentAmount++;
                }
            }
        }
	}
}