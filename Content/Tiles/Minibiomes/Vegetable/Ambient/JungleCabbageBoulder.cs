using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Minibiomes.Vegetable;

namespace Spooky.Content.Tiles.Minibiomes.Vegetable.Ambient
{
    public class JungleCabbageBoulder : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileCut[Type] = true;
			TileID.Sets.BreakableWhenPlacing[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
            AddMapEntry(new Color(52, 135, 46));
            DustType = DustID.Grass;
            HitSound = SoundID.Grass;
        }

        public override bool IsTileDangerous(int i, int j, Player player) => true;

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
            int SpawnX = i * 16 + 22;
            int SpawnY = j * 16 + 26;

            int damage = 70;
			damage /= 2;
            
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(new EntitySource_TileBreak(i * 16, j * 16), SpawnX, SpawnY, 0, -1, ModContent.ProjectileType<JungleCabbageBoulderProj>(), damage, 4.5f, Main.myPlayer);
            }
        }
    }
}