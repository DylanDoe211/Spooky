using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Shipyard
{
    public class BlackSandWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(43, 43, 43));
            DustType = DustID.Ash;
        }
    }

    public class BlackSandWallSafe : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/Shipyard/BlackSandWall";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(43, 43, 43));
            DustType = DustID.Ash;
        }
	}
}