using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class BrainyBeam : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults() 
		{
			ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2400;
		}
		public override void SetDefaults() 
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.timeLeft = 90;
			Projectile.penetrate = -1;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}
		
		public override bool PreDraw(ref Color lightColor)
		{
			bool dust = false;
			if(Projectile.alpha < 5)
			{
				dust = true;
			}
			Player player  = Main.player[Projectile.owner];
			Vector2 endPoint = new Vector2(Projectile.ai[0], Projectile.ai[1]);
			Vector2 unit = endPoint - Projectile.Center;
			float length = unit.Length();
			unit.Normalize();
			lightColor = new Color(255, 255, 255) * ((255 - Projectile.alpha) / 255f);
			for (float Distance = 0; Distance <= length; Distance += 3.5f)
			{
				Vector2 drawPos = Projectile.Center + unit * Distance - Main.screenPosition;
				Vector2 position = Projectile.Center + unit * Distance;
				int i = (int)(position.X / 16);
				int j =	(int)(position.Y / 16);
				if (WorldGen.SolidOrSlopedTile(i, j))
                {
                    break;
				}

				if (Distance >= 10)
				{
                    Color beamColor = Color.HotPink;

					float alphaMult = (255 - Projectile.alpha) / 255f;
					Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
					float rotation = (float)Math.Atan2(unit.Y, unit.X);
					float otherMult = 1f - Distance / length;
                    float helix = otherMult * alphaMult * 0.8f * (float)Math.Sin(MathHelper.ToRadians(Projectile.localAI[0] * -8 + Distance));

					Main.EntitySpriteDraw(texture, drawPos, null, beamColor.MultiplyRGBA(lightColor), rotation, texture.Size() / 2, new Vector2(1f, 1f + (helix / 2)), unit.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
					Lighting.AddLight(position, (255 - Projectile.alpha) * 0.3f / 255f, (255 - Projectile.alpha) * 0.3f / 255f, (255 - Projectile.alpha) * 0.3f / 255f);
				}
			}

			return false;
		}

		public override bool? CanDamage()
        {
            return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

		public override void AI() 
		{
			Projectile.alpha += 20;
			if (Projectile.alpha > 250) 
            {
				Projectile.Kill();
			}

            Projectile.localAI[0]++;
		}
    }
}
     
          






