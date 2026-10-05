using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.Enums;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Collections.Generic;

using Spooky.Content.Items.Shipyard;
using Spooky.Content.Projectiles.Shipyard;

namespace Spooky.Content.Tiles.Shipyard.Furniture
{
	public class GiantPirateChest : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
			Main.tileSpelunker[Type] = true;
			Main.tileShine2[Type] = true;
			Main.tileOreFinderPriority[Type] = 500;
			TileID.Sets.HasOutlines[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.Origin = new Point16(1, 2);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.RandomStyleRange = 2;
			TileObjectData.addTile(Type);
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(216, 167, 51), name);
			DustType = DustID.Ash;
		}

		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return true;
        }

		public override bool CanKillTile(int i, int j, ref bool blockDamaged)
		{
			return false;
		}

		public override bool CanExplode(int i, int j)
		{
			return false;
		}

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
		{
			int x = i;
			int y = j;
			while (Main.tile[x, y].TileType == Type) x--;
			x++;
			while (Main.tile[x, y].TileType == Type) y--;
			y++;

			int SpawnX = (x * 16 + 24);
			int SpawnY = (y * 16 + 30);

			var existingProjectile = Main.projectile.Where(n => n.active && n.Hitbox.Intersects(new Rectangle(SpawnX - 2, SpawnY - 2, 2, 2)) && n.type == ModContent.ProjectileType<PirateChestProj>()).FirstOrDefault();
			var existingNPC = Main.npc.Where(n => n.active && n.Hitbox.Intersects(new Rectangle(SpawnX - 2, SpawnY - 2, 2, 2)) && n.type == ModContent.NPCType<PirateChestVisuals>()).FirstOrDefault();
			if (existingProjectile == default && existingNPC == default && Main.tile[i, j].TileFrameY > 36)
			{
				if (Main.tile[i, j - 1].TileType != Type && Main.tile[i, j + 1].TileType == Type)
				{
					Main.tile[i, j].TileFrameY = 0;
				}
				else if (Main.tile[i, j - 1].TileType == Type && Main.tile[i, j + 1].TileType == Type)
				{
					Main.tile[i, j].TileFrameY = 18;
				}
				else if (Main.tile[i, j - 1].TileType == Type && Main.tile[i, j + 2].TileType != Type)
				{
					Main.tile[i, j].TileFrameY = 36;
				}
			}
		}

		public override bool RightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;

			int x = i;
			int y = j;
			while (Main.tile[x, y].TileType == Type) x--;
			x++;
			while (Main.tile[x, y].TileType == Type) y--;
			y++;

			int SpawnX = (x * 16 + 24);
			int SpawnY = (y * 16 + 30);

			var existingProjectile = Main.projectile.Where(n => n.active && n.Hitbox.Intersects(new Rectangle(SpawnX - 2, SpawnY - 2, 2, 2)) && n.type == ModContent.ProjectileType<PirateChestProj>()).FirstOrDefault();
			var existingNPC = Main.npc.Where(n => n.active && n.Hitbox.Intersects(new Rectangle(SpawnX - 2, SpawnY - 2, 2, 2)) && n.type == ModContent.NPCType<PirateChestVisuals>()).FirstOrDefault();
			if (existingProjectile != default || existingNPC != default || Main.tile[i, j].TileFrameY > 36)
			{
				return false;
			}

			Projectile.NewProjectile(new EntitySource_TileInteraction(player, x * 16, y * 16), SpawnX, SpawnY, 0, 0, ModContent.ProjectileType<PirateChestProj>(), 0, 0, player.whoAmI);

			return true;
		}

		public override IEnumerable<Item> GetItemDrops(int i, int j)
		{
			int[] MainItem = new int[] { ModContent.ItemType<PirateHook>(), ModContent.ItemType<PirateBlunderbuss>(), ModContent.ItemType<PirateShipWheel>(), ModContent.ItemType<PirateInkPen>() };
			yield return new Item(Main.rand.Next(MainItem));

			yield return new Item(ModContent.ItemType<PiratePotion>());

			yield return new Item(ItemID.GoldCoin, Main.rand.Next(1, 4));
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			if (WorldGen.gen)
			{
				return;
			}

			if (Main.netMode != NetmodeID.Server)
			{
				int x = i - (Main.tile[i, j].TileFrameX / 18);
				int y = j - (Main.tile[i, j].TileFrameY / 18);

				int spawnX = (x + 1) * 16;
				int spawnY = (y + 1) * 16 - 8;

				Vector2 gorePos = new Vector2(spawnX, spawnY);
				SoundEngine.PlaySound(SoundID.Shatter, gorePos);
				Vector2 goreVelocity = default(Vector2);

				for (int numGores = 1; numGores <= 6; numGores++)
				{
					Gore.NewGore(WorldGen.GetItemSource_FromTileBreak(x, y), gorePos, goreVelocity, ModContent.Find<ModGore>("Spooky/GiantPirateChestGore" + numGores).Type);
				}
			}
		}
	}
}