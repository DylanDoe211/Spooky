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
    public class SeaSlugHead : ModNPC
    {
        private bool segmentsSpawned;

        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/SeaSlugBestiary",
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
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
            segmentsSpawned = reader.ReadBoolean();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 200;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 26;
            NPC.height = 28;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
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
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ShipyardBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 origin = new Vector2(NPCTexture.Width() * 0.5f, NPCTexture.Height() / Main.npcFrameCount[NPC.type] * 0.5f);
            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - Main.screenPosition, NPC.frame, Color.White * 0.65f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            return false;
        }
        
        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;

            //Create the worm itself
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!segmentsSpawned)
                {
                    NPC.realLife = NPC.whoAmI;
                    int latestNPC = NPC.whoAmI;

                    for (int numSegments = 0; numSegments < 6; numSegments++)
                    {
                        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                        ModContent.NPCType<SeaSlugBody>(), NPC.whoAmI, 0, latestNPC);
                        Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                        Main.npc[latestNPC].realLife = NPC.whoAmI;
                        Main.npc[latestNPC].ai[2] = numSegments / 2;
                        Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                        if (numSegments == 0) Main.npc[latestNPC].ai[0] = 1;
                        if (numSegments == 3) Main.npc[latestNPC].ai[0] = 2;
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

            //chase movement
            Vector2 GoTo = player.Center;
            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 1f, 7f);
            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
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
            NPC.dontCountMe = true;
            NPC.HitSound = SoundID.NPCHit25;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);
            Fin1Texture ??= ModContent.Request<Texture2D>(Texture + "FinBig");
            Fin2Texture ??= ModContent.Request<Texture2D>(Texture + "FinSmall");

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 origin = new Vector2(NPCTexture.Width() * 0.5f, NPCTexture.Height() * 0.5f);
            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - Main.screenPosition, NPC.frame, Color.White * 0.65f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);

            if (NPC.ai[0] == 1)
            {
                Vector2 FinVector1 = Utils.RotatedBy(new Vector2(6f, 0f), NPC.rotation, default);
                Vector2 FinVector2 = Utils.RotatedBy(new Vector2(-6f, 0f), NPC.rotation, default);

                Main.EntitySpriteDraw(Fin1Texture.Value, NPC.Center - Main.screenPosition + FinVector1, null, Color.White * 0.65f,
			    NPC.rotation, new Vector2(0f, Fin1Texture.Height() / 2), 1f, SpriteEffects.None, 0f);

			    Main.EntitySpriteDraw(Fin1Texture.Value, NPC.Center - Main.screenPosition + FinVector2, null, Color.White * 0.65f,
			    NPC.rotation, new Vector2(Fin1Texture.Width(), Fin1Texture.Height() / 2), 1f, SpriteEffects.FlipHorizontally, 0f);
            }
            if (NPC.ai[0] == 2)
            {
                Vector2 FinVector1 = Utils.RotatedBy(new Vector2(4f, 0f), NPC.rotation, default);
                Vector2 FinVector2 = Utils.RotatedBy(new Vector2(-4f, 0f), NPC.rotation, default);

                Main.EntitySpriteDraw(Fin2Texture.Value, NPC.Center - Main.screenPosition + FinVector1, null, Color.White * 0.65f,
			    NPC.rotation, new Vector2(0f, Fin2Texture.Height() / 2), 1f, SpriteEffects.None, 0f);

			    Main.EntitySpriteDraw(Fin2Texture.Value, NPC.Center - Main.screenPosition + FinVector2, null, Color.White * 0.65f,
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
                NPC.active = false;
            }

			NPC SegmentParent = Main.npc[(int)NPC.ai[1]];

			Vector2 SegmentCenter = SegmentParent.Center + SegmentParent.velocity - NPC.Center;

			if (SegmentParent.rotation != NPC.rotation)
			{
				float angle = MathHelper.WrapAngle(SegmentParent.rotation - NPC.rotation);
				SegmentCenter = SegmentCenter.RotatedBy(angle * 0.1f);
			}

			NPC.rotation = SegmentCenter.ToRotation() + 1.57f;

			//how far each segment should be from each other
			if (SegmentCenter != Vector2.Zero)
			{
                float Dist = SegmentParent.type == ModContent.NPCType<SeaSlugHead>() ? 20f : 12f;
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

    public class SeaSlugTail : SeaSlugBody
    {
        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

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

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 origin = new Vector2(NPCTexture.Width() * 0.5f, NPCTexture.Height() * 0.5f);
            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - Main.screenPosition, NPC.frame, Color.White * 0.65f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);

            return false;
        }
    }
}