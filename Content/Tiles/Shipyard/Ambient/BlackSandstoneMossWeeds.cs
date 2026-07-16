using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.Shipyard.Ambient
{
    public class BlackSandstoneMossWeeds : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;
            TileID.Sets.SwaysInWindBasic[Type] = true;
            TileID.Sets.IgnoredByGrowingSaplings[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.AnchorValidTiles = new[] { ModContent.TileType<BlackSandstoneMoss>() };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(54, 199, 191));
            DustType = ModContent.DustType<ShipyardMossDust>();
            HitSound = SoundID.Grass;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			float divide = 1500f;

			r = 54f / divide;
			g = 199f / divide;
			b = 191f / divide;
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }
    }
}