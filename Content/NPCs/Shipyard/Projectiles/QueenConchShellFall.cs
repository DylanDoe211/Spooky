using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Shipyard.Projectiles
{
    public class QueenConchShellFall : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 45;
            Projectile.height = 45;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 1200;
			Projectile.penetrate = -1;
        }

        public override void AI()       
        {
            Projectile.velocity.Y = Projectile.velocity.Y + 0.25f;

            if (Projectile.ai[1] == 0)
            {
                Projectile.rotation = Projectile.ai[0];
                Projectile.ai[1]++;
            }
        }
    }
}