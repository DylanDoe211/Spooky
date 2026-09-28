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
    public class SeaSlugHead : ModNPC
    {
        Vector2 SavePosition;

        private bool segmentsSpawned;

        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.CountsAsCritter[NPC.type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/SeaSlugBestiary",
                Position = new Vector2(0f, 35f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 18f
            };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //vector2
            writer.WriteVector2(SavePosition);

            //bools
            writer.Write(segmentsSpawned);

            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //vector2
			SavePosition = reader.ReadVector2();

            //bools
            segmentsSpawned = reader.ReadBoolean();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 100;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 26;
            NPC.height = 26;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.chaseable = false;
            NPC.HitSound = SoundID.NPCHit25;
			NPC.DeathSound = SoundID.NPCDeath28;
            NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ShipyardBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.SeaSlug"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new BestiaryBackgroundOverlay("Spooky/Content/Biomes/ShipyardBiomeNight_Background", Color.White)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            //draw aura
            if (!NPC.IsABestiaryIconDummy)
			{
                for (int i = 0; i < 3; i++)
                {
                    Vector2 offset = i switch
                    {
                        1 => new(2, 0),
                        2 => new(0, -2),
                        _ => new(-2, 0)
                    };

                    Main.EntitySpriteDraw(DrawUtils.ColorSolid(NPCTexture.Value, Color.White), NPC.Center + offset.RotatedBy(NPC.rotation) - screenPos, NPC.frame, 
                    NPC.GetAlpha(Color.Cyan * 0.65f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0f);
                }
            }

            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, 
            NPC.frame, drawColor * 0.9f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0f);

            return false;
        }
        
        public override void AI()
        {
            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;

            //Create the worm itself
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!segmentsSpawned)
                {
                    NPC.realLife = NPC.whoAmI;
                    int latestNPC = NPC.whoAmI;

                    for (int numSegments = 0; numSegments < 9; numSegments++)
                    {
                        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                        ModContent.NPCType<SeaSlugBody>(), NPC.whoAmI, 0, latestNPC);
                        Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                        Main.npc[latestNPC].realLife = NPC.whoAmI;
                        Main.npc[latestNPC].ai[2] = numSegments / 3;
                        Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                        if (numSegments == 1) Main.npc[latestNPC].ai[0] = 1;
                        if (numSegments == 5) Main.npc[latestNPC].ai[0] = 2;
                        NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);
                    }
                    
                    latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<SeaSlugTail>(), NPC.whoAmI, 0, latestNPC);                   
                    Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                    Main.npc[latestNPC].realLife = NPC.whoAmI;
                    Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);

                    segmentsSpawned = true;
                    NPC.netUpdate = true;
                }
            }

            //movement
            NPC.ai[1]++;
            if (NPC.ai[1] <= 580)
            {
                if (NPC.ai[0] == 0)
                {
                    NPC.velocity.X = Main.rand.NextBool() ? -0.5f : 0.5f;

                    NPC.ai[0]++;
                    NPC.netUpdate = true;
                }

                float MaxVelocityX = 2f;
                if (NPC.velocity.X < 0)
                {
                    NPC.velocity.X -= 0.1f;
                }
                if (NPC.velocity.X > 0)
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
                for (int TilePosY = PosY; TilePosY < PosY + 8; TilePosY++)
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
                    NPC.velocity.Y -= 0.05f;
                }

                //limit npc y-velocity
                if (NPC.velocity.Y > 1.5f)
                {
                    NPC.velocity.Y = 1.5f;
                }
                if (NPC.velocity.Y < -2.5f)
                {
                    NPC.velocity.Y = -2.5f;
                }
            }

            if (NPC.ai[1] > 590 && NPC.ai[1] <= 600)
            {
                SavePosition = NPC.Center - new Vector2(0, 20);
                NPC.velocity.Y = 0;
            }

            if (NPC.ai[1] > 600 && NPC.ai[1] <= 645)
            {
                double angle = NPC.DirectionTo(SavePosition).ToRotation() - NPC.velocity.ToRotation();
                while (angle > Math.PI)
                {
                    angle -= 2.0 * Math.PI;
                }
                while (angle < -Math.PI)
                {
                    angle += 2.0 * Math.PI;
                }

                if (Math.Abs(angle) > Math.PI / 2)
                {
                    NPC.ai[2] = Math.Sign(angle);
                    NPC.velocity = Vector2.Normalize(NPC.velocity) * 2f;
                }

                NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(4f) * NPC.ai[2]);
            }

            if (NPC.ai[1] >= 660)
            {
                NPC.ai[1] = 0;
                NPC.ai[2] = 0;
                NPC.netUpdate = true;
            }
        }

        public override bool CheckDead()
        {
            for (int numDusts = 0; numDusts < 3; numDusts++)
            {
                int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.2f);
                Main.dust[dustGore].color = Color.Cyan;
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

    public class SeaSlugBody : ModNPC
    {
        private static Asset<Texture2D> NPCTexture;
        private static Asset<Texture2D> Fin1Texture;
        private static Asset<Texture2D> Fin2Texture;

        public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;

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
            NPC.chaseable = false;
            NPC.dontCountMe = true;
            NPC.HitSound = SoundID.NPCHit25;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);
            Fin1Texture ??= ModContent.Request<Texture2D>(Texture + "FinBig");
            Fin2Texture ??= ModContent.Request<Texture2D>(Texture + "FinSmall");

            //draw aura
            if (!NPC.IsABestiaryIconDummy)
			{
                for (int i = 0; i < 2; i++)
                {
                    Vector2 offset = i switch
                    {
                        1 => new(2, 0),
                        _ => new(-2, 0)
                    };

                    Main.EntitySpriteDraw(DrawUtils.ColorSolid(NPCTexture.Value, Color.White), NPC.Center + offset.RotatedBy(NPC.rotation) - screenPos, NPC.frame, 
                    NPC.GetAlpha(Color.Cyan * 0.65f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0f);
                }
            }

            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, 
            NPC.frame, drawColor * 0.9f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0f);

            //bigger fin texture
            if (NPC.ai[0] == 1)
            {
                Vector2 FinVector1 = Utils.RotatedBy(new Vector2(12f, 0f), NPC.rotation, default);
                Vector2 FinVector2 = Utils.RotatedBy(new Vector2(-12f, 0f), NPC.rotation, default);

                //big fin auras
                for (int i = 0; i < 4; i++)
                {
                    Vector2 offset = i switch
                    {
                        1 => new(0, -2),
                        2 => new(2, 0),
                        3 => new(0, 2),
                        _ => new(-2, 0)
                    };

                    Main.EntitySpriteDraw(DrawUtils.ColorSolid(Fin1Texture.Value, Color.White), NPC.Center + offset - screenPos + FinVector1, null, 
                    NPC.GetAlpha(Color.Cyan * 0.65f), NPC.rotation, new Vector2(0f, Fin1Texture.Height() / 2), 1f, SpriteEffects.None, 0f);
                    
                    Main.EntitySpriteDraw(DrawUtils.ColorSolid(Fin1Texture.Value, Color.White), NPC.Center + offset - screenPos + FinVector2, null, 
                    NPC.GetAlpha(Color.Cyan * 0.65f), NPC.rotation, new Vector2(Fin1Texture.Width(), Fin1Texture.Height() / 2), 1f, SpriteEffects.FlipHorizontally, 0f);
                }

                Main.EntitySpriteDraw(Fin1Texture.Value, NPC.Center - screenPos + FinVector1, null, drawColor * 0.9f,
			    NPC.rotation, new Vector2(0f, Fin1Texture.Height() / 2), 1f, SpriteEffects.None, 0f);

			    Main.EntitySpriteDraw(Fin1Texture.Value, NPC.Center - screenPos + FinVector2, null, drawColor * 0.9f,
			    NPC.rotation, new Vector2(Fin1Texture.Width(), Fin1Texture.Height() / 2), 1f, SpriteEffects.FlipHorizontally, 0f);
            }
            //smaller fin texture
            if (NPC.ai[0] == 2)
            {
                Vector2 FinVector1 = Utils.RotatedBy(new Vector2(8f, 0f), NPC.rotation, default);
                Vector2 FinVector2 = Utils.RotatedBy(new Vector2(-8f, 0f), NPC.rotation, default);

                //smaller fin aura
                for (int i = 0; i < 4; i++)
                {
                    Vector2 offset = i switch
                    {
                        1 => new(0, -2),
                        2 => new(2, 0),
                        3 => new(0, 2),
                        _ => new(-2, 0)
                    };

                    Main.EntitySpriteDraw(DrawUtils.ColorSolid(Fin2Texture.Value, Color.White), NPC.Center + offset.RotatedBy(NPC.rotation) - screenPos + FinVector1, null,
			        NPC.GetAlpha(Color.Cyan * 0.65f), NPC.rotation, new Vector2(0f, Fin2Texture.Height() / 2), 1f, SpriteEffects.None, 0f);
                    
                    Main.EntitySpriteDraw(DrawUtils.ColorSolid(Fin2Texture.Value, Color.White), NPC.Center + offset.RotatedBy(NPC.rotation) - screenPos + FinVector2, null,
			        NPC.GetAlpha(Color.Cyan * 0.65f), NPC.rotation, new Vector2(Fin2Texture.Width(), Fin2Texture.Height() / 2), 1f, SpriteEffects.FlipHorizontally, 0f);
                }

                Main.EntitySpriteDraw(Fin2Texture.Value, NPC.Center - screenPos + FinVector1, null, drawColor * 0.9f,
			    NPC.rotation, new Vector2(0f, Fin2Texture.Height() / 2), 1f, SpriteEffects.None, 0f);

			    Main.EntitySpriteDraw(Fin2Texture.Value, NPC.Center - screenPos + FinVector2, null, drawColor * 0.9f,
			    NPC.rotation, new Vector2(Fin2Texture.Width(), Fin2Texture.Height() / 2), 1f, SpriteEffects.FlipHorizontally, 0f);
            }

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
			if (!Parent.active || Parent.type != ModContent.NPCType<SeaSlugHead>())
            {
                SpawnGores();
                NPC.active = false;
            }

			NPC SegmentParent = Main.npc[(int)NPC.ai[1]];

			Vector2 SegmentCenter = SegmentParent.Center + SegmentParent.velocity - NPC.Center;

			if (SegmentParent.rotation != NPC.rotation)
			{
				float angle = MathHelper.WrapAngle(SegmentParent.rotation - NPC.rotation);
				SegmentCenter = SegmentCenter.RotatedBy(angle * 0.5f);
			}

			NPC.rotation = SegmentCenter.ToRotation() + 1.57f;

			//how far each segment should be from each other
			if (SegmentCenter != Vector2.Zero)
			{
                float Dist = SegmentParent.type == ModContent.NPCType<SeaSlugHead>() ? 21f : 14f;
				NPC.Center = SegmentParent.Center - SegmentCenter.SafeNormalize(Vector2.Zero) * Dist;
			}

			return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
                SpawnGores();
            }
        }

        public void SpawnGores()
        {
            for (int numDusts = 0; numDusts < 3; numDusts++)
            {
                int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.2f);
                Main.dust[dustGore].color = Color.Cyan;
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

    public class SeaSlugTail : SeaSlugBody
    {
        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void FindFrame(int frameHeight)
		{
			NPC.frame.Y = 0;
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            //draw aura
            if (!NPC.IsABestiaryIconDummy)
			{
                for (int i = 0; i < 3; i++)
                {
                    Vector2 offset = i switch
                    {
                        1 => new(2, 0),
                        2 => new(0, 2),
                        _ => new(-2, 0)
                    };

                    Main.EntitySpriteDraw(DrawUtils.ColorSolid(NPCTexture.Value, Color.White), NPC.Center + offset.RotatedBy(NPC.rotation) - screenPos, NPC.frame, 
                    NPC.GetAlpha(Color.Cyan * 0.65f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0f);
                }
            }

            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, 
            NPC.frame, drawColor * 0.9f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}