using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Catacomb.Layer1
{
	public class BoneWorm1 : ModNPC
	{
		private static Asset<Texture2D> NPCTexture;
		private static Asset<Texture2D> TailTexture;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.TrailCacheLength[NPC.type] = 1;
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 100;
            NPC.damage = 20;
            NPC.defense = 5;
            NPC.width = 52;
			NPC.height = 40;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 0, 75);
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
			NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.aiStyle = 3;
            AIType = NPCID.GoblinScout;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.BoneWorm1"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome>().ModBiomeBestiaryInfoElement)
			});
        }

		public override void FindFrame(int frameHeight)
        {
            //running animation
            NPC.frameCounter++;
            if (NPC.frameCounter > 2)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 3)
            {
                NPC.frame.Y = 0 * frameHeight;
            }

            //frame when falling/jumping
            if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);
			TailTexture ??= ModContent.Request<Texture2D>(Texture + "Tail");

			if (NPC.IsABestiaryIconDummy)
			{
				NPC.oldPos[0] = NPC.Center + Vector2.UnitX * 50f;
			}

			Rectangle frame = NPC.frame;
			Vector2 val = Utils.Size(frame) / 2f;
			SpriteEffects effects = SpriteEffects.None;

			float MaxDist = 24f;

			Utils.AngleTo(NPC.oldPos[0], NPC.Center - MaxDist * Utils.ToRotationVector2(NPC.rotation));

			//draw body
			if (NPC.oldPos[0].X - NPC.Center.X > 0f)
			{
				effects = SpriteEffects.None;
			}
			else
			{
				effects = SpriteEffects.FlipHorizontally;
			}

			Utils.Distance(NPC.oldPos[0], NPC.Center);

			spriteBatch.Draw(TailTexture.Value, NPC.oldPos[0] - screenPos, NPC.frame, drawColor, 0f, val, NPC.scale, effects, 0f);

			//draw head
			effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, 0f, val, NPC.scale, effects, 0f);

			return false;
		}

		public override void AI()
		{
			NPC.spriteDirection = NPC.direction;

			float MaxDist = 24f;
			float MaxDistDirection = NPC.Center.X - MaxDist * (float)NPC.spriteDirection;
			if (Math.Abs(NPC.oldPos[0].X - MaxDistDirection) > MaxDist * NPC.scale)
			{
				NPC.oldPos[0].X = MaxDistDirection + (float)Math.Sign(NPC.oldPos[0].X - MaxDistDirection) * MaxDist;
			}
			NPC.oldPos[0].Y = MathHelper.Lerp(NPC.oldPos[0].Y, NPC.Center.Y, 0.3f);
			if (Math.Abs(NPC.oldPos[0].Y - NPC.Center.Y) > 10f)
			{
				NPC.oldPos[0].Y = NPC.Center.Y + (float)(Math.Sign(NPC.oldPos[0].Y - NPC.Center.Y) * 10);
			}
		}

		public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                if (Main.netMode != NetmodeID.Server)
                {
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/SkeletoidGore1").Type);

					for (int repeats = 0; repeats < 2; repeats++)
					{
						for (int numGores = 1; numGores <= 3; numGores++)
						{
							Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BoneWorm1Gore" + numGores).Type);
						}
						for (int numGores = 1; numGores <= 3; numGores++)
						{
							Gore.NewGore(NPC.GetSource_Death(), NPC.oldPos[0], NPC.velocity, ModContent.Find<ModGore>("Spooky/BoneWorm1Gore" + numGores).Type);
						}
					}
                }
            }
        }
	}

	public class BoneWorm2 : BoneWorm1
	{
		private static Asset<Texture2D> NPCTexture;
		private static Asset<Texture2D> TailTexture;

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.BoneWorm2"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome>().ModBiomeBestiaryInfoElement)
			});
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);
			TailTexture ??= ModContent.Request<Texture2D>(Texture + "Tail");

			if (NPC.IsABestiaryIconDummy)
			{
				NPC.oldPos[0] = NPC.Center + Vector2.UnitX * 50f;
			}

			Rectangle frame = NPC.frame;
			Vector2 val = Utils.Size(frame) / 2f;
			SpriteEffects effects = SpriteEffects.None;

			float MaxDist = 24f;

			Utils.AngleTo(NPC.oldPos[0], NPC.Center - MaxDist * Utils.ToRotationVector2(NPC.rotation));

			//draw body
			if (NPC.oldPos[0].X - NPC.Center.X > 0f)
			{
				effects = SpriteEffects.None;
			}
			else
			{
				effects = SpriteEffects.FlipHorizontally;
			}

			Utils.Distance(NPC.oldPos[0], NPC.Center);

			spriteBatch.Draw(TailTexture.Value, NPC.oldPos[0] - screenPos, NPC.frame, drawColor, 0f, val, NPC.scale, effects, 0f);

			//draw head
			effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, 0f, val, NPC.scale, effects, 0f);

			return false;
		}

		public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                if (Main.netMode != NetmodeID.Server)
                {
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/SkeletoidFlowerGore1").Type);

					for (int repeats = 0; repeats < 2; repeats++)
					{
						for (int numGores = 1; numGores <= 3; numGores++)
						{
							Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BoneWorm2Gore" + numGores).Type);
						}
						for (int numGores = 1; numGores <= 3; numGores++)
						{
							Gore.NewGore(NPC.GetSource_Death(), NPC.oldPos[0], NPC.velocity, ModContent.Find<ModGore>("Spooky/BoneWorm2Gore" + numGores).Type);
						}
					}
                }
            }
        }
	}
}