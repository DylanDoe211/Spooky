using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.NPCs.Shipyard.Projectiles
{
    public class HermitCrabGroundEffect : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {
			Projectile.width = 52;
            Projectile.height = 52;
			Projectile.friendly = false;
			Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 40;
            Projectile.alpha = 255;
		}

        public override bool? CanCutTiles()
        {
            return false;
        }

		public override bool? CanDamage()
		{
			return false;
		}
		
		public override void AI()
        {
			Point scanAreaStart = Projectile.TopLeft.ToTileCoordinates();
			Point scanAreaEnd = Projectile.BottomRight.ToTileCoordinates();
			_ = scanAreaStart.X / 2;
			_ = scanAreaEnd.X / 2;
			int explosionRange = Projectile.width / 2;
			Projectile.ai[0]++;
			if (Projectile.ai[0] % 10 == 0)
			{
				Projectile.CreateImpactExplosion(2, Projectile.Bottom, ref scanAreaStart, ref scanAreaEnd, explosionRange, out var causedShockwaves);
			}
		}
    }
}