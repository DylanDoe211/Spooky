using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Projectiles.Shipyard
{
    public class PirateHookScythe : ModProjectile
    {
		private static Asset<Texture2D> ProjTexture;

		public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 50;
			Projectile.penetrate = 2;
        }

        public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 projDrawOrigin = new(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
			
			for (int i = 0; i < 360; i += 90)
            {
				Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Cyan);

                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 3f), Main.rand.NextFloat(1f, 3f)).RotatedBy(MathHelper.ToRadians(i));

				Main.EntitySpriteDraw(ProjTexture.Value, vector + circular, rectangle, color, Projectile.rotation, projDrawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}

			return false;
		}

        public override void AI()
        {
            Projectile.rotation += 0.5f * (float)Projectile.direction;

			Projectile.velocity *= 0.95f;

			if (Projectile.timeLeft <= 25)
			{
				Projectile.alpha += 10;
			}
        }
    }
}