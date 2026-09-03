using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Pets;

namespace Spooky.Content.Tiles.SpookyBiome.GourdBlocks
{
    public class GourdPetSeedTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.BreakableWhenPlacing[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(170, 132, 101));
            DustType = 288;
            HitSound = SoundID.Dig;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            float divide = 1000f;

            r = 141f / divide;
            g = 133f / divide;
            b = 89f / divide;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
		{
            int variant = Framing.GetTileSafely(i, j).TileFrameX / 36;
			switch (variant)
			{
				case 0:
				{
					yield return new Item(ModContent.ItemType<GourdPetSeedGreen>());
					break;
				}
                case 1:
				{
					yield return new Item(ModContent.ItemType<GourdPetSeedLime>());
					break;
				}
                case 2:
				{
					yield return new Item(ModContent.ItemType<GourdPetSeedOrange>());
					break;
				}
                case 3:
				{
					yield return new Item(ModContent.ItemType<GourdPetSeedRed>());
					break;
				}
                case 4:
				{
					yield return new Item(ModContent.ItemType<GourdPetSeedWhite>());
					break;
				}
                case 5:
				{
					yield return new Item(ModContent.ItemType<GourdPetSeedYellow>());
					break;
				}
            }
        }
    }
}