using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Catacomb
{
	public class FlowerPotShoeFlower : ModProjectile
	{
        public override string Texture => "Spooky/Content/NPCs/Boss/BigBone/Projectiles/BouncingFlower";

        private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> TrailTexture;

        public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

        public override void SetDefaults()
		{
			Projectile.width = 66;
            Projectile.height = 68;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.timeLeft = 240;
            Projectile.penetrate = 3;
            Projectile.aiStyle = -1;
		}

        public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);
			TrailTexture ??= ModContent.Request<Texture2D>(Texture + "Trail");

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

			Color color1 = Color.White;
			Color color2 = Color.White;

			switch (Projectile.frame)
			{
				case 0:
				{
					color1 = new Color(120, 205, 65);
			 		color2 = color1 * 0.5f;
					break;
				}
				case 1:
				{
					color1 = new Color(142, 92, 208);
			 		color2 = color1 * 0.5f;
					break;
				}
				case 2:
				{
					color1 = new Color(251, 46, 51);
			 		color2 = color1 * 0.5f;
					break;
				}
				case 3:
				{
					color1 = new Color(250, 177, 70);
			 		color2 = color1 * 0.5f;
					break;
				}
			}

			for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
			{
				float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1.1f;
				Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Color.Lerp(color1, color2, oldPos / (float)Projectile.oldPos.Length) * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(TrailTexture.Value, drawPos, rectangle, Projectile.GetAlpha(color), Projectile.oldRot[oldPos], drawOrigin, scale, SpriteEffects.None, 0);
			}

			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}

		public override void AI()
		{
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

            Projectile.ai[0]++;
            if (Projectile.ai[0] > 15)
            {
                Projectile.tileCollide = true;
            }
            else
            {
                Projectile.tileCollide = false;
            }

            int foundTarget = HomeOnTarget();
            if (foundTarget != -1)
            {
                NPC target = Main.npc[foundTarget];
                Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 25;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                Projectile.tileCollide = false;
            }
            else
            {
                Projectile.tileCollide = true;
            }
		}

        private int HomeOnTarget()
        {
            const float homingMaximumRangeInPixels = 800;

            int selectedTarget = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (target.CanBeChasedBy(Projectile))
                {
                    float distance = Projectile.Distance(target.Center);
                    if (distance <= homingMaximumRangeInPixels && (selectedTarget == -1 || Projectile.Distance(Main.npc[selectedTarget].Center) > distance))
                    {
                        selectedTarget = i;
                    }
                }
            }

            return selectedTarget;
        }

        public override void OnKill(int timeLeft)
		{
        	for (int numDusts = 0; numDusts < 25; numDusts++)
			{                                                                                  
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.YellowTorch, 0f, -2f, 0, default(Color), 1.5f);
				Main.dust[newDust].noGravity = true;
				Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                
				if (Main.dust[newDust].position != Projectile.Center)
				{
					Main.dust[newDust].velocity = Projectile.DirectionTo(Main.dust[newDust].position) * 2f;
				}
			}
		}
	}
}