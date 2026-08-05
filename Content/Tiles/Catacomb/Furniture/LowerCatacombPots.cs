using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Items.Catacomb;
using Spooky.Content.Items.Slingshots.Ammo;
using Spooky.Content.Items.SpookyBiome;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
	public class LowerCatacombPots : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileCut[Type] = true;
			Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
			Main.tileSpelunker[Type] = true;
            TileID.Sets.BreakableWhenPlacing[Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
            AddMapEntry(new Color(149, 80, 81), Language.GetText("MapObject.Pot"));
            DustType = DustID.t_Lihzahrd;
		}

		public override IEnumerable<Item> GetItemDrops(int i, int j)
		{
			switch (Main.rand.Next(4))
			{
				//torches
				case 0:
				{
					yield return new Item(ModContent.ItemType<CatacombTorch2Item>(), Main.rand.Next(4, 13));
					break;
				}
				//healing or mana potions
				case 1:
				{
					int[] Potions = new int[] { ItemID.HealingPotion, ItemID.ManaPotion };
					yield return new Item(Main.rand.Next(Potions), Main.rand.Next(3, 7));
					break;
				}
				//random potions
				case 2:
				{
					int[] Potions = new int[] { ItemID.RegenerationPotion, ItemID.IronskinPotion, ItemID.SpelunkerPotion, ItemID.ThornsPotion, 
					ItemID.RecallPotion, ItemID.SwiftnessPotion, ItemID.TrapsightPotion, ItemID.PotionOfReturn };
					yield return new Item(Main.rand.Next(Potions));
					break;
				}
				//ammos
				case 3:
				{
					int[] Ammos = new int[] { ModContent.ItemType<OldWoodArrow>(), ModContent.ItemType<RustedBullet>(), ModContent.ItemType<MossyBoulder>() };
					yield return new Item(Main.rand.Next(Ammos), Main.rand.Next(10, 21));
					break;
				}
			}

			if (Main.rand.NextBool(3))
			{
				yield return new Item(ItemID.SilverCoin, Main.rand.Next(1, 11));
			}
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			if (WorldGen.gen)
			{
				return;
			}

			if (Main.netMode != NetmodeID.Server)
			{
				//chance to spawn a coin portal
				if (Main.rand.NextBool(750))
				{
					Projectile.NewProjectile(new EntitySource_TileBreak(i, j), (i * 16 + 16), (j * 16 + 16), 0.0f, -12f, ProjectileID.CoinPortal, 0, 0.0f, Main.myPlayer, 0.0f, 0.0f);
				}

				int x = i - (Main.tile[i, j].TileFrameX / 18);
				int y = j - (Main.tile[i, j].TileFrameY / 18);

				int spawnX = (x + 1) * 16;
				int spawnY = (y + 1) * 16 - 8;

				Vector2 gorePos = new Vector2(spawnX, spawnY);
				SoundEngine.PlaySound(SoundID.Shatter, gorePos);
				Vector2 goreVelocity = default(Vector2);
			}
		}
	}
}
