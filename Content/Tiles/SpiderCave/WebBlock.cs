using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.SpiderCave
{
	public class WebBlock : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            TileID.Sets.GeneralPlacementTiles[Type] = false;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(215, 208, 187));
			HitSound = SoundID.NPCHit11;
            DustType = DustID.Web;
			MineResist = 0.4f;
		}

        public override bool HasWalkDust()
        {
			return true;
        }

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            bool isPlayerNear = WorldGen.PlayerLOS(i, j);

            if (!Main.gamePaused && Main.instance.IsActive && !WorldGen.SolidTile(i, j + 1) && isPlayerNear)
            {
                if (Main.rand.NextBool(600))
                {
                    int newDust = Dust.NewDust(new Vector2((i) * 16, (j + 1) * 16), 5, 5, ModContent.DustType<CobwebParticle>());
                    Main.dust[newDust].velocity.Y += 0.09f;
                }
            }
        }
    }
}
