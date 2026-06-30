using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Spooky.Content.NPCs.Boss.Daffodil.Projectiles
{
	public class Corpsebloom : ModProjectile
	{
		private static Asset<Texture2D> ProjTexture;

		public static readonly SoundStyle GrowSound = new("Spooky/Content/Sounds/BigBone/PlantGrow", SoundType.Sound) { Pitch = -0.35f };
		public static readonly SoundStyle KillSound = new("Spooky/Content/Sounds/BigBone/PlantDestroy", SoundType.Sound) { Pitch = -0.35f, Volume = 0.5f };
		public static readonly SoundStyle FlySound = new("Spooky/Content/Sounds/FlyBuzzing", SoundType.Sound) { Volume = 2f, PitchVariance = 0.75f };

		public override void SetDefaults()
		{
			Projectile.width = 60;
			Projectile.height = 54;
            Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 600;
			Projectile.penetrate = -1;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

			Color color = Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16));

			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(color), 0f, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}

		public override bool? CanDamage()
		{
			return false;
		}

		public override void AI()
		{
			Projectile.ai[0]++;
			if (Projectile.ai[0] == 2)
			{
				SoundEngine.PlaySound(GrowSound, Projectile.Center);
			}

			if (Projectile.timeLeft >= 240)
			{
				if (Projectile.ai[0] % 45 == 0)
				{
					Vector2 FlyPosition = (Vector2.One * new Vector2((float)Projectile.width / 2f, (float)Projectile.height / 2f) * 15f).RotatedBy(Main.rand.NextFloat(0f, 361f)) + Projectile.Center;

					SoundEngine.PlaySound(FlySound, FlyPosition);
				}
				if (Projectile.ai[0] % 4 == 0)
				{
					Vector2 FlyPosition = (Vector2.One * new Vector2((float)Projectile.width / 2f, (float)Projectile.height / 2f) * 15f).RotatedBy(Main.rand.NextFloat(0f, 361f)) + Projectile.Center;
					Vector2 velocity = FlyPosition - Projectile.Center;

					Projectile.NewProjectile(Projectile.GetSource_FromAI(), FlyPosition + velocity, Vector2.Normalize(velocity) * -7f,
					ModContent.ProjectileType<DaffodilFly>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
				}
			}
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Grass, Projectile.Center);
			SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);

			for (int numGores = 1; numGores <= 3; numGores++)
			{
				if (Main.netMode != NetmodeID.Server) 
				{
					//Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, ModContent.Find<ModGore>("Spooky/ThornBallGore" + numGores).Type);
				}
			}
		}
	}
}