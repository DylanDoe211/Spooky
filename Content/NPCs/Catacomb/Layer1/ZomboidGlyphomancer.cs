using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Items.Catacomb;
using Spooky.Content.Items.Food;
using Spooky.Content.NPCs.Catacomb.Layer1.Projectiles;

namespace Spooky.Content.NPCs.Catacomb.Layer1
{
    public class ZomboidGlyphomancer : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 150;
            NPC.damage = 20;
            NPC.defense = 0;
            NPC.width = 52;
			NPC.height = 60;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 3, 0);
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = 3;
            AIType = NPCID.Crab;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ZomboidGlyphomancer"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.localAI[0] == 0)
            {
                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 5)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }

                //frame when falling/jumping
                if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }
            }
            //attacking frames
            else
            {
				if (NPC.frame.Y < frameHeight * 7)
				{
					NPC.frame.Y = 6 * frameHeight;
				}

				if (NPC.frameCounter > 6)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 9)
				{
					NPC.frame.Y = 8 * frameHeight;
				}
			}
        }
        
        public override void AI()
		{
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            switch ((int)NPC.localAI[0])
            {
                case 0:
                {
					bool HasLineOfSight = Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height);
                    if ((player.Distance(NPC.Center) <= 420f && HasLineOfSight) || NPC.localAI[1] >= 100)
                    {
                        NPC.localAI[1]++;
                    }

                    NPC.aiStyle = 3;
                    AIType = NPCID.Crab;

                    //start actually attacking
                    if (NPC.localAI[1] == 150)
                    {
                        NPC.localAI[1] = 0;
                        NPC.localAI[0]++;
                        NPC.netUpdate = true;
                    }

                    break;
                }

                //projectile attack
                case 1:
                {
					NPC.aiStyle = 0;
					NPC.velocity.X *= 0.5f;

                    NPC.localAI[1]++;
					if (NPC.localAI[1] == 1)
					{
						SoundEngine.PlaySound(SoundID.Item175 with { Pitch = -0.5f }, NPC.Center);
					}
                    if (NPC.localAI[1] == 30)
                    {
						SoundEngine.PlaySound(SoundID.Item20, NPC.Center);

                        NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center + new Vector2(15 * NPC.direction, -10), 
						Vector2.Zero, ModContent.ProjectileType<GlyphomancerBolt>(), NPC.damage, 4.5f, ai2: NPC.direction);
                    }

                    if (NPC.localAI[1] >= 150)
                    {
                        NPC.localAI[1] = 0;
                        NPC.localAI[0] = 0;
                        NPC.netUpdate = true;
                    }

                    break;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SkullAmulet>(), 12));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrankenMarshmallow>(), 100));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 5; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ZomboidGlyphomancerGore" + numGores).Type);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ZomboidNecromancerCloth" + Main.rand.Next(2, 4)).Type);
                    }
                }
            }
        }
    }
}