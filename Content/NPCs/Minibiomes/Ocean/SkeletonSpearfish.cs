using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Minibiomes.Ocean;

namespace Spooky.Content.NPCs.Minibiomes.Ocean
{
	public class SkeletonSpearfish : ModNPC
	{
		Vector2 SavePosition = Vector2.Zero;

		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 8;
			NPCID.Sets.CountsAsCritter[Type] = true;
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 40;
			NPC.damage = 0;
			NPC.defense = 10;
			NPC.width = 100;
			NPC.height = 48;
			NPC.npcSlots = 0.5f;
			NPC.knockBackResist = 0.35f;
			NPC.noGravity = true;
			NPC.chaseable = false;
            NPC.dontTakeDamageFromHostiles = false;
			NPC.HitSound = SoundID.DD2_SkeletonHurt;
			NPC.DeathSound = SoundID.DD2_SkeletonHurt;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ZombieOceanBiome>().Type };
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.SkeletonSpearfish"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ZombieOceanBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			//draw aura
			Vector2 drawOrigin = new(NPCTexture.Width() * 0.5f, NPC.height * 0.5f);

            var effects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

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

					Main.EntitySpriteDraw(DrawUtils.ColorSolid(NPCTexture.Value, Color.White), NPC.Center + offset - screenPos, NPC.frame, 
					NPC.GetAlpha(Color.Lime * 0.5f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
				}
			}

            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            return false;
		}

		public override void FindFrame(int frameHeight)
		{
            NPC.frameCounter++;
			if (NPC.frameCounter > 9 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 8)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
		}

		public override void AI()
		{
			if (NPC.velocity.X > 0f)
			{
				NPC.direction = 1;
			}
			if (NPC.velocity.X < 0f)
			{
				NPC.direction = -1;
			}

			NPC.rotation = NPC.velocity.Y * (NPC.direction == 1 ? 0.05f : -0.05f);

			if (SavePosition == Vector2.Zero)
			{
				NPC.ai[0] = Main.rand.Next(-400, 400);
				SavePosition = NPC.Center;
			}

			SkeletonFish.FishSwimmingAI(NPC, SavePosition, 220, 60, 7f, 2f, 0.02f, 0.05f);
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FishboneChunk>(), 1, 1, 2));
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/SkeletonSpearfishGore" + numGores).Type);
                    }
                }
            }
        }
	}
}