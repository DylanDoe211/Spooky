using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class MetalFist : ModProjectile
    {
        private static Asset<Texture2D> ChainTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 36;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10000;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

			ChainTexture ??= ModContent.Request<Texture2D>(Texture + "Chain");

			Vector2 drawOrigin = new Vector2(0, ChainTexture.Height() / 2);
			Vector2 myCenter = Projectile.Center;
			Vector2 p0 = player.MountedCenter;
			Vector2 p1 = player.MountedCenter;
			Vector2 p2 = myCenter;
			Vector2 p3 = myCenter;

			int segments = 25;

			for (int i = 0; i < segments; i++)
			{
				float t = i / (float)segments;
				Vector2 drawPos2 = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
				t = (i + 1) / (float)segments;
				Vector2 drawPosNext = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
				Vector2 toNext = drawPosNext - drawPos2;
				float rotation = toNext.ToRotation();
				float distance = toNext.Length();

				Color color = Lighting.GetColor((int)drawPos2.X / 16, (int)(drawPos2.Y / 16));

				Main.spriteBatch.Draw(ChainTexture.Value, drawPos2 - Main.screenPosition, null, color, rotation, drawOrigin, Projectile.scale * new Vector2((distance + 4) / (float)ChainTexture.Width(), 1), SpriteEffects.None, 0f);
			}
            
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<MetalFistStick>(), Projectile.damage, Projectile.knockBack, Projectile.owner,
            ai0: Projectile.rotation, ai1: target.whoAmI);

            Projectile.ai[0] = 20;
            Projectile.alpha = 255;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.ai[0] = 20;

			return false;
		}

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            player.itemRotation = Projectile.rotation;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

            Vector2 vectorTowardsPlayer = Projectile.DirectionTo(player.MountedCenter).SafeNormalize(Vector2.Zero);
			Projectile.rotation = vectorTowardsPlayer.ToRotation() + MathHelper.PiOver2;

            Projectile.ai[0]++;
            if (Projectile.ai[0] > 2)
            {
                player.direction = Projectile.Center.X < player.MountedCenter.X ? -1 : 1;
            }
            if (Projectile.ai[0] >= 20)
            {
                Projectile.tileCollide = false;

                Vector2 ReturnSpeed = player.MountedCenter - Projectile.Center;
                ReturnSpeed.Normalize();
                ReturnSpeed *= 35;

                Projectile.velocity = ReturnSpeed;

                if (Projectile.Hitbox.Intersects(player.Hitbox))
                {
                    Projectile.Kill();
                }
            }
        }
    }
}