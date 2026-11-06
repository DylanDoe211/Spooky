using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Enums;
using Terraria.Audio;
using Terraria.GameContent.ObjectInteractions;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Achievements;
using Spooky.Content.Items.SpookyBiome.Misc;

namespace Spooky.Content.Tiles.SpookyBiome.Furniture
{
	public class SpookyBiomeChest : ModTile
	{
		public override void SetStaticDefaults() 
		{
			Main.tileSpelunker[Type] = true;
			Main.tileContainer[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileOreFinderPriority[Type] = 500;
			TileID.Sets.HasOutlines[Type] = true;
			TileID.Sets.BasicChest[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			AdjTiles = new int[] { TileID.Containers };
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.AnchorInvalidTiles = [TileID.MagicalIceBlock, TileID.Boulder, TileID.BouncyBoulder, TileID.LifeCrystalBoulder, TileID.RollingCactus];
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);
            AddMapEntry(Color.Orange, this.GetLocalization("MapEntry0"), MapChestName);
            AddMapEntry(Color.Orange, this.GetLocalization("MapEntry1"), MapChestName);
            DustType = DustID.Grass;
			HitSound = SoundID.Dig;
		}

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int style = TileObjectData.GetTileStyle(tile);
            if (style == 0)
            {
                yield return new Item(ModContent.ItemType<SpookyBiomeChestItem>());
            }
            if (style == 1)
            {
                yield return new Item(ModContent.ItemType<SpookyBiomeChestItem>());
            }
        }

        private static Point GetMultiTilePos(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			int left = i;
			int top = j;
			if (tile.TileFrameX % 36 != 0)
				left--;
			if (tile.TileFrameY != 0)
				top--;

			return new Point(left, top);
		}

		public static string MapChestName(string name, int i, int j)
		{
			(i, j) = (GetMultiTilePos(i, j).X, GetMultiTilePos(i, j).Y);

			int chest = Chest.FindChest(i, j);
			if (chest < 0)
				return Language.GetTextValue("LegacyChestType.0");

			if (Main.chest[chest].name == string.Empty)
				return name;

			return name + ": " + Main.chest[chest].name;
		}

		public override LocalizedText DefaultContainerName(int frameX, int frameY) => this.GetLocalization("MapEntry0");

		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

		public override void NumDust(int i, int j, bool fail, ref int num) => num = 1;

		public override void KillMultiTile(int i, int j, int frameX, int frameY) => Chest.DestroyChest(i, j);

        public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameX / 36);

        public override bool IsLockedChest(int i, int j) => Main.tile[i, j].TileFrameX / 36 == 1;

		public override bool UnlockChest(int i, int j, ref short frameXAdjustment, ref int dustType, ref bool manual) 
		{
			return true;
		}

		public override bool RightClick(int i, int j)
		{
			(i, j) = (GetMultiTilePos(i, j).X, GetMultiTilePos(i, j).Y);

			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			Main.mouseRightRelease = false;

			int left = i;
			int top = j;
			if (tile.TileFrameX % 36 != 0) 
			{
				left--;
			}

			if (tile.TileFrameY != 0) 
			{
				top--;
			}
			bool isLocked = IsLockedChest(left, top);
			if (isLocked)
			{
				if (NPC.downedPlantBoss)
				{
					int key = ModContent.ItemType<SpookyBiomeKey>();

					if (player.ConsumeItem(key) && Chest.Unlock(left, top))
					{
						if (Main.netMode == NetmodeID.MultiplayerClient) 
						{
							NetMessage.SendData(MessageID.LockAndUnlock, -1, -1, null, player.whoAmI, 1f, left, top);
						}
					}
				}
			}
			else 
			{
				player.CloseSign();
				player.SetTalkNPC(-1);
				Main.npcChatCornerItem = 0;
				Main.npcChatText = string.Empty;

				if (Main.editChest)
				{
					SoundEngine.PlaySound(SoundID.MenuTick);
					Main.editChest = false;
					Main.npcChatText = string.Empty;
				}
				if (player.editedChestName)
				{
					NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f);
					player.editedChestName = false;
				}
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					if (i == player.chestX && j == player.chestY && player.chest >= 0)
					{
						player.chest = -1;
						Recipe.FindRecipes();
						SoundEngine.PlaySound(SoundID.MenuClose);
					}
					else
					{
						NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, i, j);
						Main.stackSplit = 600;
					}
				}
				else
				{
					int chest = Chest.FindChest(i, j);
					if (chest >= 0)
					{
						Main.stackSplit = 600;
						if (chest == player.chest)
						{
							player.chest = -1;
							SoundEngine.PlaySound(SoundID.MenuClose);
						}
						else
						{
							SoundEngine.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
							player.OpenChest(i, j, chest);
						}

						Recipe.FindRecipes();
					}
				}
			}
			return true;
		}

		public override void MouseOver(int i, int j)
		{
			(i, j) = (GetMultiTilePos(i, j).X, GetMultiTilePos(i, j).Y);

			Player player = Main.LocalPlayer;
			int chest = Chest.FindChest(i, j);
			bool isLocked = IsLockedChest(i, j);
			player.cursorItemIconID = -1;

			if (chest < 0)
			{
				player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
			}
			else
			{
				Tile tile = Framing.GetTileSafely(i, j);
				string defaultName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY);

				player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : defaultName;
				if (player.cursorItemIconText == defaultName)
				{
					player.cursorItemIconID = isLocked ? ModContent.ItemType<SpookyBiomeKey>() : ModContent.ItemType<SpookyBiomeChestItem>();
					player.cursorItemIconText = string.Empty;
				}
			}

			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
		}

		public override void MouseOverFar(int i, int j)
		{
			MouseOver(i, j);

			Player player = Main.LocalPlayer;
			if (player.cursorItemIconText == string.Empty)
			{
				player.cursorItemIconEnabled = false;
				player.cursorItemIconID = 0;
			}
		}
	}
}