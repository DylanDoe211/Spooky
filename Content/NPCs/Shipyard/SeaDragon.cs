using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Shipyard
{
	public class SeaDragon1 : ModNPC
	{
        int SaveDirection;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
				Velocity = 1f
			};
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 20;
            NPC.damage = 0;
			NPC.defense = 0;
			NPC.width = 36;
			NPC.height = 36;
            NPC.npcSlots = 0.5f;
            NPC.noGravity = true;
            NPC.chaseable = false;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = 16;
			AIType = NPCID.Pupfish;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ShipyardBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.SeaDragon"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ShipyardBiome>().ModBiomeBestiaryInfoElement)
			});
		}
        
        public override void FindFrame(int frameHeight)
		{
            NPC.frameCounter++;
            if (NPC.frameCounter > 7 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
		}

        public override void AI()
        {
            NPC.spriteDirection = NPC.direction;

            if (NPC.localAI[1] == 0)
            {
                NPC.localAI[1] = Main.rand.Next(300, 541);
                NPC.netUpdate = true;
            }

            NPC.localAI[0]++;
            if (NPC.localAI[0] < NPC.localAI[1])
            {
                NPC.spriteDirection = NPC.direction;

                SaveDirection = NPC.direction;
            }
            if (NPC.localAI[0] >= NPC.localAI[1])
            {
                NPC.spriteDirection = SaveDirection;

                NPC.aiStyle = 0;
                NPC.velocity *= 0.985f;
            }
            if (NPC.localAI[0] >= NPC.localAI[1] + 140)
            {
                NPC.aiStyle = 16;
                NPC.localAI[1] = Main.rand.Next(300, 541);
                NPC.localAI[0] = 0;
                NPC.netUpdate = true;
            }
        }
	}

    public class SeaDragon2 : SeaDragon1
	{
        public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
                NPC BestiaryParent = new();
                BestiaryParent.SetDefaults(ModContent.NPCType<SeaDragon1>());
                Main.BestiaryTracker.Kills.RegisterKill(BestiaryParent);
            }
        }
    }

    public class SeaDragon3 : SeaDragon1
	{
        public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
                NPC BestiaryParent = new();
                BestiaryParent.SetDefaults(ModContent.NPCType<SeaDragon1>());
                Main.BestiaryTracker.Kills.RegisterKill(BestiaryParent);
            }
        }
    }

    public class SeaDragon4 : SeaDragon1
	{
        public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
                NPC BestiaryParent = new();
                BestiaryParent.SetDefaults(ModContent.NPCType<SeaDragon1>());
                Main.BestiaryTracker.Kills.RegisterKill(BestiaryParent);
            }
        }
    }
}