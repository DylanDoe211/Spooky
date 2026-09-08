using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Shipyard.Projectiles
{
    public class QueenConchSpawn : ModNPC
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 20;
            NPC.height = 20;
            NPC.npcSlots = 0f;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.behindTiles = true;
            NPC.dontCountMe = true;
			NPC.alpha = 255;
            NPC.aiStyle = -1;
		}

        public override void AI()
        {
			NPC.ai[0]++;

			if (Main.rand.NextBool(5))
			{
				Dust dust = Dust.NewDustPerfect(NPC.Center + new Vector2(Main.rand.Next(-60, 61), 10), ModContent.DustType<GlowyDust>(), Vector2.Zero);
				dust.color = Color.Cyan;
				dust.velocity.X = 0;
				dust.velocity.Y = Main.rand.NextFloat(-5f, -2f);
				dust.scale = 0.3f;
				dust.noGravity = true;
			}

			if (NPC.ai[0] >= 120)
			{
				/*
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					int NewNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-35, 36), (int)NPC.Center.Y + 40, ModContent.NPCType<QueenConch>());
					Main.npc[NewNPC].velocity.Y = Main.rand.Next(-5, -2);
					Main.npc[NewNPC].alpha = 255;

					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.SyncNPC, number: NewNPC);
					}
				}
				*/

				WorldGen.KillTile((int)NPC.Center.X / 16, (int)NPC.Center.Y / 16, fail: false);

				NPC.active = false;
				NPC.netUpdate = true;
			}
        }
	}
}