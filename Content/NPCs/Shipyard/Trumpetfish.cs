using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Shipyard
{
    public class TrumpetfishHead : ModNPC
    {
        Vector2 SavePosition;

        private bool segmentsSpawned;

        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                //CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/TrumpetfishBestiary",
                Position = new Vector2(0f, 35f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 0f
            };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //bools
            writer.Write(segmentsSpawned);

            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
            segmentsSpawned = reader.ReadBoolean();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 350;
            NPC.damage = 20;
            NPC.defense = 10;
            NPC.width = 26;
            NPC.height = 26;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.behindTiles = true;
            NPC.HitSound = SoundID.NPCHit25;
			NPC.DeathSound = SoundID.NPCDeath28;
            NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ShipyardBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Trumpetfish"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ShipyardBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 origin = new Vector2(NPCTexture.Width() * 0.5f, NPCTexture.Height() / Main.npcFrameCount[NPC.type] * 0.5f);
            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            return false;
        }

        public override void FindFrame(int frameHeight)
		{
			NPC.frame.Y = (int)NPC.ai[3] * frameHeight;
		}

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
		{
			if (NPC.ai[2] == 0)
            {
                NPC.ai[2] = 180;
            }
		}

		public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
		{
			if (NPC.ai[2] == 0)
            {
                NPC.ai[2] = 180;
            }
		}
        
        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.velocity.X > 0 ? -1 : 1;

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;

            //create the worm itself
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!segmentsSpawned)
                {
                    NPC.ai[3] = Main.rand.Next(0, 2);

                    NPC.spriteDirection = Main.rand.NextBool() ? -1 : 1;

                    NPC.realLife = NPC.whoAmI;
                    int latestNPC = NPC.whoAmI;

                    int maxSegments = Main.rand.Next(4, 11);
                    for (int numSegments = 0; numSegments < maxSegments; numSegments++)
                    {
                        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                        ModContent.NPCType<TrumpetfishBody>(), NPC.whoAmI, 0, latestNPC);
                        Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                        Main.npc[latestNPC].realLife = NPC.whoAmI;
                        Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                        Main.npc[latestNPC].ai[2] = NPC.ai[3];
                        NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);
                    }
                    
                    latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<TrumpetfishTail>(), NPC.whoAmI, 0, latestNPC);                   
                    Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                    Main.npc[latestNPC].realLife = NPC.whoAmI;
                    Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                    Main.npc[latestNPC].ai[2] = NPC.ai[3];
                    NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);

                    segmentsSpawned = true;
                    NPC.netUpdate = true;
                }
            }

            if (NPC.ai[2] == 0)
            {
                if (NPC.ai[0] == 0)
                {
                    bool HasLineOfSight = NPC.Distance(player.Center) <= 350f && Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height);

                    if (HasLineOfSight && NPC.Distance(player.Center) <= 150f)
                    {
                        SoundEngine.PlaySound(SoundID.Zombie56 with { Volume = 3f, Pitch = 1.5f }, NPC.Center);

                        NPC.ai[0]++;
                        NPC.netUpdate = true;
                    }

                    Vector2 GoTo = HasLineOfSight ? player.Center : (NPC.Center + new Vector2(50 * -NPC.spriteDirection, 0));
                    
                    Vector2 desiredVelocity = NPC.DirectionTo(GoTo) * 1.5f;
                    NPC.velocity.X = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20).X;
                }
                else
                {
                    NPC.ai[1]++;
                    if (NPC.ai[1] < 200)
                    {
                        if (NPC.Distance(player.Center) >= 50f)
                        {
                            Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 10f;
                            NPC.velocity.X = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20).X;
                        }
                    }

                    if (NPC.ai[1] >= 200)
                    {
                        NPC.ai[2] = 180;
                        NPC.netUpdate = true;
                    }
                }
            }
            else
            {
                NPC.ai[2]--;

                Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * -5f;
                NPC.velocity.X = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20).X;

                if (NPC.ai[2] <= 0)
                {
                    NPC.ai[0] = 0;
                    NPC.ai[1] = 0;
                    NPC.ai[2] = 0;
                    NPC.netUpdate = true;
                }
            }

            bool GoUp = false;
            int PosX = (int)(NPC.Center.X / 16f);
            int PosY = (int)(NPC.Center.Y / 16f);
            for (int TilePosY = PosY; TilePosY < PosY + 4; TilePosY++)
            {
                if (!WorldGen.InWorld(PosX, TilePosY, 10))
                {
                    continue;
                }
                if (WorldGen.SolidOrSlopedTile(PosX, TilePosY))
                {
                    NPC.velocity.Y *= 0.98f;
                    GoUp = true; 
                    NPC.netUpdate = true;
                    break;
                }
            }

            bool FasterY = NPC.ai[0] > 0 || NPC.ai[2] > 0;
            
            if (!GoUp)
            {
                NPC.velocity.Y += FasterY ? 0.3f : 0.035f;
            }
            else
            {
                NPC.velocity.Y -= FasterY ? 0.3f : 0.035f;
            }
        }
        
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.2f;
            return null;
        }
    }

    public class TrumpetfishBody : ModNPC
    {
        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
		{
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 200;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 26;
            NPC.height = 26;
            NPC.npcSlots = 0f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.behindTiles = true;
            NPC.dontCountMe = true;
            NPC.HitSound = SoundID.NPCHit25;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 origin = new Vector2(NPCTexture.Width() * 0.5f, NPCTexture.Height() / Main.npcFrameCount[NPC.type] * 0.5f);
            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            return false;
        }

        public override void FindFrame(int frameHeight)
		{
			NPC.frame.Y = (int)NPC.ai[2] * frameHeight;
		}

        public override bool PreAI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            NPC.spriteDirection = Parent.spriteDirection;

            //kill segment if the head doesnt exist
			if (!Parent.active || Parent.type != ModContent.NPCType<TrumpetfishHead>())
            {
                NPC.active = false;
            }

			NPC SegmentParent = Main.npc[(int)NPC.ai[1]];

			Vector2 SegmentCenter = SegmentParent.Center + SegmentParent.velocity - NPC.Center;

			if (SegmentParent.rotation != NPC.rotation)
			{
				float angle = MathHelper.WrapAngle(SegmentParent.rotation - NPC.rotation);
				SegmentCenter = SegmentCenter.RotatedBy(angle * 0.12f);
			}

			NPC.rotation = SegmentCenter.ToRotation() + 1.57f;

			//how far each segment should be from each other
			if (SegmentCenter != Vector2.Zero)
			{
                float Dist = SegmentParent.type == ModContent.NPCType<TrumpetfishHead>() ? 22f : 14f;
				NPC.Center = SegmentParent.Center - SegmentCenter.SafeNormalize(Vector2.Zero) * Dist;
			}

			return false;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override bool CheckActive()
        {
            return false;
        }
    }

    public class TrumpetfishTail : TrumpetfishBody
    {
        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 origin = new Vector2(NPCTexture.Width() * 0.5f, NPCTexture.Height() / Main.npcFrameCount[NPC.type] * 0.5f);
            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            return false;
        }

        public override bool PreAI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            NPC.spriteDirection = Parent.spriteDirection;

            //kill segment if the head doesnt exist
			if (!Parent.active || Parent.type != ModContent.NPCType<TrumpetfishHead>())
            {
                NPC.active = false;
            }

			NPC SegmentParent = Main.npc[(int)NPC.ai[1]];

			Vector2 SegmentCenter = SegmentParent.Center + SegmentParent.velocity - NPC.Center;

			if (SegmentParent.rotation != NPC.rotation)
			{
				float angle = MathHelper.WrapAngle(SegmentParent.rotation - NPC.rotation);
				SegmentCenter = SegmentCenter.RotatedBy(angle * 0.12f);
			}

			NPC.rotation = SegmentCenter.ToRotation() + 1.57f;

			//how far each segment should be from each other
			if (SegmentCenter != Vector2.Zero)
			{
                float Dist = 26f;
				NPC.Center = SegmentParent.Center - SegmentCenter.SafeNormalize(Vector2.Zero) * Dist;
			}

			return false;
        }
    }
}