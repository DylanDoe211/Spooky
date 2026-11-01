using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Tiles.Catacomb.Ambient
{
    public class CatacombBoulder2 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.AnchorTop = AnchorData.Empty;
			TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
			TileObjectData.newTile.AnchorLeft = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.AnchorRight = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
            AddMapEntry(new Color(77, 42, 27));
            DustType = -1;
        }

        public override bool IsTileDangerous(int i, int j, Player player) => true;

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
            int SpawnX = i * 16 + 16;
            int SpawnY = j * 16 + 16;

            int damage = 100;
			damage /= 2;

            int variant = frameX / 36;
            
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int Type = variant == 3 ? ModContent.ProjectileType<CatacombBoulderProj2Gold>() : ModContent.ProjectileType<CatacombBoulderProj2>();
                Projectile.NewProjectile(new EntitySource_TileBreak(i * 16, j * 16), SpawnX, SpawnY, 0, 0, Type, damage, 4.5f, Main.myPlayer, ai2: variant);
            }
        }
    }
}