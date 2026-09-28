using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Shipyard
{
	public class Batfish : ModNPC
	{
        bool HasJumped = false;
        int SaveDirection = 0;

        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(HasJumped);
            writer.Write(SaveDirection);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            HasJumped = reader.ReadBoolean();
            SaveDirection = reader.ReadInt32();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 35;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 34;
            NPC.height = 34;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath6;
            NPC.aiStyle = 66;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ShipyardBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Batfish"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ShipyardBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //draw aura
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
                    NPC.GetAlpha(Color.Gray * 0.65f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
                }
            }

            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, drawColor * 0.9f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            return false;
        }

        public override void FindFrame(int frameHeight)
		{
            if (NPC.velocity.Y > 0)
            {
                NPC.frame.Y = 4 * frameHeight;
            }
            else if (NPC.velocity.Y < 0)
            {
                NPC.frame.Y = 3 * frameHeight;
            }
            else
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > 7)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 3)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
		}

        public override void AI()
		{
			NPC.spriteDirection = SaveDirection;

            JumpToTarget(50, 50, 0);
		}

        public void JumpToTarget(int JumpHeight, int TimeBeforeNextJump, int DelayBeforeNextJump)
        {
            NPC.ai[0]++;

            //set where the it should be jumping towards
            Vector2 JumpTo = new Vector2(NPC.Center.X + Main.rand.Next(-100, 101), NPC.Center.Y - JumpHeight);

            //set velocity and speed
            Vector2 velocity = JumpTo - NPC.Center;
            velocity.Normalize();

            int JumpSpeed = Main.rand.Next(13, 18);

            float speed = MathHelper.Clamp(velocity.Length() / 36, 10, JumpSpeed);

            NPC.velocity.X *= NPC.velocity.Y <= 0 ? 0.98f : 0.95f;

            //actual jumping
            if (NPC.ai[0] >= TimeBeforeNextJump)
            {
                NPC.ai[1]++;

                if (NPC.velocity == Vector2.Zero && !HasJumped)
                {
                    if (NPC.ai[1] == 1)
                    {
                        velocity.Y -= 0.25f;
                        
                        HasJumped = true;
                    }
                }
                
                if (NPC.ai[1] == 2 && HasJumped)
                {
                    NPC.velocity = velocity * speed;

                    if (NPC.velocity.X < 0)
                    {
                        SaveDirection = -1;
                    }
                    else
                    {
                        SaveDirection = 1;
                    }
                }
            }

            //loop ai
            if (NPC.ai[0] >= TimeBeforeNextJump + 100)
            {
                HasJumped = false;

                NPC.ai[0] = DelayBeforeNextJump;
                NPC.ai[1] = 0;
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

                    int newDust = Dust.NewDust(NPC.Center, 1, 1, ModContent.DustType<GlowyDust>(), 0f, 0f, 0, Color.Gray, 0.2f);
                    Main.dust[newDust].noGravity = true;
                    Main.dust[newDust].position = NPC.Center + vector12;
                    Main.dust[newDust].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;

                    currentAmount++;
                }
            }
		}
    }
}