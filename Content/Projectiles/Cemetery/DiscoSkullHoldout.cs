using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Spooky.Core;
using System;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Projectiles.Cemetery
{
	public class DiscoSkullHoldout : ModProjectile
	{
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        private static Asset<Texture2D> SpotlightTexture;

        float[] Rotations = new float[22];
		float[] RotationMults = new float[22];
		float[] Scales = new float[22];
		float[] ScaleIncrease = new float[22];
		bool[] FlipRotation = new bool[22];

		bool runOnce = true;

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.netImportant = true;
			Projectile.timeLeft = 5;
			Projectile.penetrate = -1;
			Projectile.alpha = 255;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) 
		{
			Rectangle bigHitbox = new Rectangle((int)Projectile.Center.X - 350, (int)Projectile.Center.Y - 350, 700, 700);
			if (targetHitbox.Intersects(bigHitbox))
			{
				return true;
			}
			return false;
		}

		public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
		{
            if (runOnce)
            {
                return false;
            }

			SpotlightTexture ??= ModContent.Request<Texture2D>("Spooky/Effects/LightConeSmall");

			Vector2 frameOrigin = new Vector2(SpotlightTexture.Width() / 2f, 0f);
            Vector2 drawPos = Projectile.Center - Main.screenPosition + frameOrigin + new Vector2(-SpotlightTexture.Width() / 2, Projectile.gfxOffY + 4);

			for (int i = 0; i < Rotations.Length; i++)
			{
				float Rotation = Main.GlobalTimeWrappedHourly * RotationMults[i];

				float fade = Main.GameUpdateCount % 60 / 60f;
				int index = (int)(Main.GameUpdateCount / 60 % 3);
				Color[] BeamColors = new Color[]
                {
                    new Color(18, 148, 0, 0),
                    new Color(148, 80, 0, 0),
                    new Color(80, 0, 148, 0)
                };

				float RealRotation = MathF.Cos(Rotation) + Rotations[i];

				if (FlipRotation[i])
				{
					RealRotation = MathF.Sin(Rotation) + Rotations[i];
				}

				if (ScaleIncrease[i] < Scales[i])
				{
					ScaleIncrease[i] += (Scales[i] / 50);
				}

				//spotlight drawning
				Main.EntitySpriteDraw(SpotlightTexture.Value, drawPos, null, Color.Lerp(BeamColors[index], BeamColors[(index + 1) % 3], fade), RealRotation * Projectile.spriteDirection, frameOrigin, ScaleIncrease[i], SpriteEffects.None, 0);
			}

			return false;
		}

		public override void AI()
		{
            Player player = Main.player[Projectile.owner];

            if (!player.active || player.dead || player.noItems || player.CCed) 
            {
                Projectile.Kill();
            }

            if (!player.CheckMana(ItemGlobal.ActiveItem(player), ItemGlobal.ActiveItem(player).mana, false, false))
			{
				Projectile.Kill();
			}
			
			if (runOnce)
			{
                for (int i = 0; i < Rotations.Length; i++)
				{
                    Rotations[i] = i * 30;
					RotationMults[i] = Main.rand.NextFloat(0.25f, 0.75f);
					Scales[i] = i < 8 ? 1.5f : Main.rand.NextFloat(0.5f, 1.65f);
					FlipRotation[i] = Main.rand.NextBool();
				}

                runOnce = false;
            }

			Lighting.AddLight(Projectile.Center, Main.DiscoColor.R / 450f, Main.DiscoColor.G / 450f, Main.DiscoColor.B / 450f);

            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 ProjDirection = Main.MouseWorld - player.position;
                ProjDirection.Normalize();
                Projectile.ai[0] = ProjDirection.X;
				Projectile.ai[1] = ProjDirection.Y;
				Projectile.netUpdate = true;
            }

            Vector2 direction = new Vector2(Projectile.ai[0], Projectile.ai[1]);

            Projectile.direction = Projectile.spriteDirection = direction.X > 0 ? 1 : -1;

            Projectile.position = player.MountedCenter - Projectile.Size / 2 + new Vector2((Projectile.direction == -1 ? -15 : 15), 0);

			if (player.channel)
            {
                Projectile.timeLeft = 2;

                player.itemRotation = Projectile.rotation;

                player.bodyFrame.Y = player.bodyFrame.Height * 3;

                Projectile.ai[2]++;

                //consume mana
                if (Projectile.ai[2] >= ItemGlobal.ActiveItem(player).useTime && player.CheckMana(ItemGlobal.ActiveItem(player), ItemGlobal.ActiveItem(player).mana, false, false))
                {
                    player.statMana -= ItemGlobal.ActiveItem(player).mana;
                    Projectile.ai[2] = 0;
                }

                if (direction.X > 0) 
                {
					player.direction = 1;
				}
				else
                {
					player.direction = -1;
				}
            }
            else
            {
				Projectile.active = false;
            }

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }
	}
}