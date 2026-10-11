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
	public class Parrotfish : ModNPC
	{
        public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;

        private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Position = new Vector2(28f, 0f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 0f
            };
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
            //ints
            writer.Write(MoveSpeedX);
            writer.Write(MoveSpeedY);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //ints
            MoveSpeedX = reader.ReadInt32();
            MoveSpeedY = reader.ReadInt32();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 180;
            NPC.damage = 30;
			NPC.defense = 0;
			NPC.width = 104;
			NPC.height = 60;
            NPC.npcSlots = 1f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
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
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Parrotfish"),
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
			if (NPC.ai[0] == 0)
            {
				NPC.ai[0]++;
				Spooky.ManuallySyncNPCAI(NPC.whoAmI);
			}
		}

		public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
		{
			if (NPC.ai[0] == 0)
            {
                NPC.ai[0]++;
				Spooky.ManuallySyncNPCAI(NPC.whoAmI);
			}
		}

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return NPC.ai[0] == 1;
		}

        public override void AI()
        {
            NPC.rotation = NPC.velocity.Y * (NPC.spriteDirection == 1 ? 0.03f : -0.03f);

			if (NPC.ai[2] == 0)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					for (int numFish = 0; numFish < 3; numFish++)
					{
						int NewParrotfish = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<ParrotfishBaby>(), ai1: NPC.whoAmI);

						if (Main.netMode != NetmodeID.SinglePlayer)
						{
							NetMessage.SendData(MessageID.SyncNPC, number: NewParrotfish);
						}
					}
				}

				NPC.ai[2]++;
				NPC.netUpdate = true;
			}

            switch ((int)NPC.ai[0])
            {
                //slowly move around
                case 0:
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

                    float MaxVelocityX = 0.68f;
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

                    break;
                }

                //chase the player
                case 1:     
                {
                    NPC.TargetClosest(true);
                    Player player = Main.player[NPC.target];

                    NPC.spriteDirection = NPC.direction;

                    int MaxSpeed = 4;

                    //flies to players X position
                    if (NPC.Center.X >= player.Center.X && MoveSpeedX >= -MaxSpeed) 
                    {
                        MoveSpeedX--;
                    }
                    else if (NPC.Center.X <= player.Center.X && MoveSpeedX <= MaxSpeed)
                    {
                        MoveSpeedX++;
                    }

                    NPC.velocity.X += MoveSpeedX * 0.01f;
                    NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -MaxSpeed, MaxSpeed);
                    
                    //flies to players Y position
                    if (NPC.Center.Y >= player.Center.Y - 20 && MoveSpeedY >= -MaxSpeed * 0.5f)
                    {
                        MoveSpeedY--;
                    }
                    else if (NPC.Center.Y <= player.Center.Y - 20 && MoveSpeedY <= MaxSpeed * 0.5f)
                    {
                        MoveSpeedY++;
                    }

                    NPC.velocity.Y += MoveSpeedY * 0.1f;
                    NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -MaxSpeed * 0.5f, MaxSpeed * 0.5f);

                    break;
                }
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