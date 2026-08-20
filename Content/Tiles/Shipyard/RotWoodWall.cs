using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Shipyard
{
    public class RotWoodWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
			Main.wallHouse[Type] = true;
            AddMapEntry(new Color(23, 25, 32));
            DustType = DustID.Ash;
        }
    }
}