using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.NPCs.Quest.Projectiles
{ 
    public class EyeWizardOrb : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
			Projectile.width = 28;
            Projectile.height = 30;
			Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1000;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
            Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            for (int i = 0; i < 4; i++)
			{
				Vector2 offset = i switch
				{
					1 => new(0, -2),
					2 => new(2, 0),
					3 => new(0, 2),
					_ => new(-2, 0)
				};

				Main.EntitySpriteDraw(DrawUtils.ColorSolid(ProjTexture.Value, Color.White), Projectile.Center + offset - Main.screenPosition, rectangle, 
				Projectile.GetAlpha(Color.Crimson * 0.6f), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
			}

            return true;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }
		
		public override void AI()
        {
            Player target = Main.player[Player.FindClosest(Projectile.Center, Projectile.width, Projectile.height)];

            Projectile.velocity *= 0.95f;

            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 60 && Projectile.ai[0] < 200 && Projectile.ai[0] % Projectile.ai[1] == 0)
            {
                SoundEngine.PlaySound(SoundID.Item42, Projectile.Center);

                Vector2 ShootSpeed = target.Center - Projectile.Center;
                ShootSpeed.Normalize();
                ShootSpeed *= 10f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, ShootSpeed, ModContent.ProjectileType<BouncingEye>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                }
            }

            if (Projectile.ai[0] >= 230)
            {
                Projectile.Kill();
            }
		}

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);

            for (int numGores = 1; numGores <= 4; numGores++)
			{
				if (Main.netMode != NetmodeID.Server) 
				{
					Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, -2)), ModContent.Find<ModGore>("Spooky/EyeWizardOrbGore" + numGores).Type);
				}
			}
        }
    }
}
     
          






