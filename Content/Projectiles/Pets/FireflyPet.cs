using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Pets
{
	public class FireflyPet : ModProjectile
	{
        private static Asset<Texture2D> NPCTexture;
        private static Asset<Texture2D> GlowTexture;
        private static Asset<Texture2D> SpotlightTexture;

        public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 6;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.LightPet[Projectile.type] = true;
        }

		public override void SetDefaults()
		{
			Projectile.width = 34;
            Projectile.height = 34;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
		}

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
		{
            Player player = Main.player[Projectile.owner];

            NPCTexture ??= ModContent.Request<Texture2D>(Texture);
			GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");
            SpotlightTexture ??= ModContent.Request<Texture2D>("Spooky/Effects/LightCone");

            Vector2 frameOrigin = new Vector2(SpotlightTexture.Width() / 2f, 0f);
            Vector2 drawPos = Projectile.Center + new Vector2(5 * Projectile.spriteDirection, 3) - Main.screenPosition + frameOrigin + new Vector2(-SpotlightTexture.Width() / 2, Projectile.gfxOffY + 4);

            Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(new Color(255, 255, 255, 0));

			Main.EntitySpriteDraw(SpotlightTexture.Value, drawPos, null, color, Projectile.rotation, frameOrigin, new Vector2(0.5f, 0.35f), SpriteEffects.None, 0);

			var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Vector2 drawOrigin = new(GlowTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, GlowTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, GlowTexture.Width(), GlowTexture.Height() / Main.projFrames[Projectile.type]);
			
            Main.EntitySpriteDraw(NPCTexture.Value, vector, rectangle, lightColor, 0f, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(GlowTexture.Value, vector, rectangle, Projectile.GetAlpha(Color.White), 0f, drawOrigin, Projectile.scale, effects, 0);

            return false;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
            
			if (player.dead)
            {
				player.GetModPlayer<SpookyPlayer>().FireflyPet = false;
            }

			if (player.GetModPlayer<SpookyPlayer>().FireflyPet)
            {
				Projectile.timeLeft = 2;
            }

            Projectile.frameCounter++;
			if (Projectile.frameCounter >= 2)
			{
				Projectile.frameCounter = 0;
				
                Projectile.frame++;
				if (Projectile.frame >= 6)
				{
					Projectile.frame = 0;
				}
			}

            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            Projectile.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;
            
            Projectile.spriteDirection = -player.direction;
            
            Lighting.AddLight(Projectile.Center, 1f, 1f, 1f);
            
            if (Projectile.Distance(player.Center) >= 1000f)
            {
                Projectile.Center = player.Center;
            }

			Vector2 GoTo = new Vector2(player.Center.X, player.Center.Y - 80);

            if (Projectile.Distance(GoTo) > 10f)
            {
                float vel = MathHelper.Clamp(Projectile.Distance(GoTo) / 6, 15, 100);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(GoTo) * vel, 0.08f);
            }
            else
            {
                Projectile.velocity *= 0.875f;
            }
		}
	}
}