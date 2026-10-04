using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Shipyard.Projectiles
{
    public class QueenConchSludge : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 14;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
			Projectile.extraUpdates = 10;
            Projectile.timeLeft = 1800;
			Projectile.penetrate = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void AI()       
        {
			float num129 = 0.01f;
			float num130 = 0.15f;

			Projectile.scale -= num129;
			if (Projectile.scale <= 0f)
			{
				Projectile.Kill();
			}
			if (Projectile.ai[0] > 3f)
			{
				Projectile.velocity.Y += num130;
				for (int num131 = 0; num131 < 1; num131++)
				{
					for (int num132 = 0; num132 < 3; num132++)
					{
						float num133 = Projectile.velocity.X / 3f * (float)num132;
						float num134 = Projectile.velocity.Y / 3f * (float)num132;
						int num135 = 6;
						int num136 = Dust.NewDust(new Vector2(Projectile.position.X + (float)num135, Projectile.position.Y + (float)num135), 
                        Projectile.width - num135 * 2, Projectile.height - num135 * 2, 160, 0f, 0f, 100, default(Color), 1.2f);

						Main.dust[num136].noGravity = true;
						Dust dust2 = Main.dust[num136];
						dust2.velocity *= 0.3f;
						dust2 = Main.dust[num136];
						dust2.velocity += Projectile.velocity * 0.5f;
						Main.dust[num136].position.X -= num133;
						Main.dust[num136].position.Y -= num134;
					}
					if (Main.rand.Next(8) == 0)
					{
						int num137 = 6;
						int num138 = Dust.NewDust(new Vector2(Projectile.position.X + (float)num137, Projectile.position.Y + (float)num137), 
                        Projectile.width - num137 * 2, Projectile.height - num137 * 2, 160, 0f, 0f, 100, default(Color), 0.75f);

						Dust dust2 = Main.dust[num138];
						dust2.velocity *= 0.5f;
						dust2 = Main.dust[num138];
						dust2.velocity += Projectile.velocity * 0.5f;
					}
				}
			}
			else
			{
				Projectile.ai[0] += 1f;
			}
        }

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item126, Projectile.Center);
		}
    }
}