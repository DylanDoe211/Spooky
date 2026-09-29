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

using Spooky.Core;
using Spooky.Content.Dusts;

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
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/TrumpetfishBestiary",
                Position = new Vector2(-20f, 35f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 20f
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
            NPC.lifeMax = 200;
            NPC.damage = 20;
            NPC.defense = 5;
            NPC.width = 26;
            NPC.height = 26;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.behindTiles = true;
            NPC.HitSound = SoundID.NPCHit25;
			NPC.DeathSound = SoundID.NPCDeath6;
            NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ShipyardBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Trumpetfish"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
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
                Color AuraColor = NPC.ai[3] == 0 ? Color.White : Color.DarkGray;

                for (int i = 0; i < 3; i++)
                {
                    Vector2 offset = i switch
                    {
                        1 => new(2, 0),
                        2 => new(0, -2),
                        _ => new(-2, 0)
                    };

                    Main.EntitySpriteDraw(DrawUtils.ColorSolid(NPCTexture.Value, Color.White), NPC.Center + offset.RotatedBy(NPC.rotation) - screenPos, NPC.frame, 
                    NPC.GetAlpha(AuraColor * 0.65f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
                }
            }

            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, 
            NPC.frame, drawColor * 0.9f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

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

            //passive behavior
            if (NPC.ai[2] == 0)
            {
                //when not aggressive, float towards the player if they are close enough or just move in a set direction
                if (NPC.ai[0] == 0)
                {
                    //if the player is too close and has line of sight, become hostile
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
                //while aggressive quickly move towards the player
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
            //fleeing behavior
            else
            {
                NPC.ai[2]--;

                NPC.velocity.X = NPC.Center.X < player.Center.X ? -5 : 5;

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

            //limit npc y-velocity
            float MaxVelocityY = FasterY ? 3f : 1.8f;
            if (NPC.velocity.Y > MaxVelocityY)
            {
                NPC.velocity.Y = MaxVelocityY;
            }
            if (NPC.velocity.Y < -MaxVelocityY)
            {
                NPC.velocity.Y = -MaxVelocityY;
            }
        }

        public override bool CheckDead()
        {
            for (int numDusts = 0; numDusts < 3; numDusts++)
            {
                int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.2f);
                Main.dust[dustGore].color = NPC.ai[2] == 0 ? Color.White : Color.DarkGray;
                Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-2f, 2f);
                Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-2f, 2f);
                Main.dust[dustGore].noGravity = true;
            }

            return true;
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

            //draw aura
            if (!NPC.IsABestiaryIconDummy)
			{
                Color AuraColor = NPC.ai[2] == 0 ? Color.White : Color.DarkGray;

                for (int i = 0; i < 2; i++)
                {
                    Vector2 offset = i switch
                    {
                        1 => new(2, 0),
                        _ => new(-2, 0)
                    };

                    Main.EntitySpriteDraw(DrawUtils.ColorSolid(NPCTexture.Value, Color.White), NPC.Center + offset.RotatedBy(NPC.rotation) - screenPos, NPC.frame, 
                    NPC.GetAlpha(AuraColor * 0.65f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
                }
            }

            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, 
            NPC.frame, drawColor * 0.9f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            return false;
        }

        public override void FindFrame(int frameHeight)
		{
			NPC.frame.Y = (int)NPC.ai[2] * frameHeight;
		}

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
		{
            NPC Parent = Main.npc[(int)NPC.ai[3]];
			if (Parent.ai[2] == 0)
            {
                Parent.ai[2] = 180;
            }
		}

		public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
		{
            NPC Parent = Main.npc[(int)NPC.ai[3]];
			if (Parent.ai[2] == 0)
            {
                Parent.ai[2] = 180;
            }
		}

        public override bool PreAI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            NPC.spriteDirection = Parent.spriteDirection;

            //kill segment if the head doesnt exist
			if (!Parent.active || Parent.type != ModContent.NPCType<TrumpetfishHead>())
            {
                SpawnGores(NPC);
                NPC.active = false;
            }

			NPC SegmentParent = Main.npc[(int)NPC.ai[1]];

			Vector2 SegmentCenter = SegmentParent.Center + SegmentParent.velocity - NPC.Center;

			if (SegmentParent.rotation != NPC.rotation)
			{
				float angle = MathHelper.WrapAngle(SegmentParent.rotation - NPC.rotation);
				SegmentCenter = SegmentCenter.RotatedBy(angle * 0.2f);
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

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
                SpawnGores(NPC);
            }
        }

        public void SpawnGores(NPC NPC)
        {
            for (int numDusts = 0; numDusts < 3; numDusts++)
            {
                int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.2f);
                Main.dust[dustGore].color = NPC.ai[2] == 0 ? Color.White : Color.DarkGray;
                Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-2f, 2f);
                Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-2f, 2f);
                Main.dust[dustGore].noGravity = true;
            }
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

            //draw aura
            if (!NPC.IsABestiaryIconDummy)
			{
                Color AuraColor = NPC.ai[2] == 0 ? Color.White : Color.DarkGray;

                for (int i = 0; i < 3; i++)
                {
                    Vector2 offset = i switch
                    {
                        1 => new(2, 0),
                        2 => new(0, 2),
                        _ => new(-2, 0)
                    };

                    Main.EntitySpriteDraw(DrawUtils.ColorSolid(NPCTexture.Value, Color.White), NPC.Center + offset.RotatedBy(NPC.rotation) - screenPos, NPC.frame, 
                    NPC.GetAlpha(AuraColor * 0.65f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
                }
            }

            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, 
            NPC.frame, drawColor * 0.9f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            return false;
        }

        public override bool PreAI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            NPC.spriteDirection = Parent.spriteDirection;

            //kill segment if the head doesnt exist
			if (!Parent.active || Parent.type != ModContent.NPCType<TrumpetfishHead>())
            {
                SpawnGores(NPC);
                NPC.active = false;
            }

			NPC SegmentParent = Main.npc[(int)NPC.ai[1]];

			Vector2 SegmentCenter = SegmentParent.Center + SegmentParent.velocity - NPC.Center;

			if (SegmentParent.rotation != NPC.rotation)
			{
				float angle = MathHelper.WrapAngle(SegmentParent.rotation - NPC.rotation);
				SegmentCenter = SegmentCenter.RotatedBy(angle * 0.2f);
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