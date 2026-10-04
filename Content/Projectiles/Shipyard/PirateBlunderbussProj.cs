using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Shipyard
{
	public class PirateBlunderbussProj : ModProjectile
	{
        int playerCenterOffset = 10;

        private static Asset<Texture2D> ChainTexture;

        public static readonly SoundStyle ReloadSound = new("Spooky/Content/Sounds/ScarecrowReload", SoundType.Sound);

		public override void SetDefaults()
		{
            Projectile.width = 26;
            Projectile.height = 78;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
		}

        public override bool PreDraw(ref Color lightColor)
		{
			Player player = Main.player[Projectile.owner];

			ChainTexture ??= ModContent.Request<Texture2D>(Texture + "Chain");

            bool flip = false;
            if (player.direction == -1)
            {
                flip = true;
            }

            Vector2 drawOrigin = new Vector2(0, ChainTexture.Height() / 2);
            Vector2 myCenter = Projectile.Center - new Vector2(5 * (flip ? -1 : 1), -5).RotatedBy(Projectile.rotation);
            Vector2 p0 = player.MountedCenter - new Vector2(14 * (flip ? -1 : 1), 3).RotatedBy(player.fullRotation);
            Vector2 p1 = player.MountedCenter - new Vector2(14 * (flip ? -1 : 1), 3).RotatedBy(player.fullRotation);
            Vector2 p2 = myCenter - new Vector2(12 * (flip ? -1 : 1), 10).RotatedBy(Projectile.rotation);
            Vector2 p3 = myCenter;

            int segments = 10;

            for (int i = 0; i < segments; i++)
            {
                float t = i / (float)segments;
                Vector2 drawPos2 = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
                t = (i + 1) / (float)segments;
                Vector2 drawPosNext = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
                Vector2 toNext = (drawPosNext - drawPos2);
                float rotation = toNext.ToRotation();
                float distance = toNext.Length();

                lightColor = Lighting.GetColor((int)drawPos2.X / 16, (int)(drawPos2.Y / 16));

                Main.spriteBatch.Draw(ChainTexture.Value, drawPos2 - Main.screenPosition, null, Projectile.GetAlpha(lightColor), rotation, drawOrigin, Projectile.scale * new Vector2((distance + 4) / (float)ChainTexture.Width(), 1), SpriteEffects.None, 0f);
            }

			return true;
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
            Player player = Main.player[Projectile.owner];

            if (!player.active || player.dead || player.noItems || player.CCed) 
            {
                Projectile.Kill();
            }

            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 ProjDirection = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y - playerCenterOffset);
                ProjDirection.Normalize();
                Projectile.ai[0] = ProjDirection.X;
				Projectile.ai[1] = ProjDirection.Y;
                Projectile.netUpdate = true;
            }

            Vector2 direction = new Vector2(Projectile.ai[0], Projectile.ai[1]);

            Projectile.direction = Projectile.spriteDirection = direction.X > 0 ? 1 : -1;

            if (Projectile.direction >= 0)
            {
                Projectile.rotation = direction.ToRotation() - 1.57f * (float)Projectile.direction;
            }
            else
            {
                Projectile.rotation = direction.ToRotation() + 1.57f * (float)Projectile.direction;
            }

            player.itemRotation = Projectile.rotation;

            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

            Projectile.position = new Vector2(player.MountedCenter.X - Projectile.width / 2, player.MountedCenter.Y - playerCenterOffset - Projectile.height / 2);

            if (direction.X > 0) 
            {
                player.direction = 1;
            }
            else 
            {
                player.direction = -1;
            }

			if (Projectile.ai[2] == 0)
            {
                Projectile.timeLeft = 20;

                Projectile.localAI[0]++;
                if (Projectile.localAI[0] == ItemGlobal.ActiveItem(player).useTime / 2)
                {
                    SoundEngine.PlaySound(ReloadSound, Projectile.Center);
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, player.itemRotation);
                }
                else
                {
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);
                }

                if (Projectile.localAI[0] >= ItemGlobal.ActiveItem(player).useTime)
                {
                    Projectile.ai[2]++;
                    Projectile.localAI[0] = 0;
                }
			}
			else 
            {
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

                if (Projectile.timeLeft >= 19)
                {
                    SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.Item45, Projectile.Center);

                    player.PickAmmo(ItemGlobal.ActiveItem(player), out _, out _, out _, out _, out _);

                    if (Projectile.owner == Main.myPlayer)
				    {
                        Vector2 ShootSpeed = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y - playerCenterOffset);
                        ShootSpeed.Normalize();
                        ShootSpeed *= ItemGlobal.ActiveItem(player).shootSpeed;

                        Vector2 position = Projectile.Center;
                        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 45f;
                        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                        {
                            position += muzzleOffset;
                        }

                        for (int numDusts = 0; numDusts < 12; numDusts++)
						{
                            Vector2 newVelocity = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(45));

							Dust dust = Dust.NewDustPerfect(position, DustID.DungeonSpirit, newVelocity * Main.rand.NextFloat(0.1f, 0.25f), default, default, 2f);
							dust.noGravity = true;
							dust.velocity += player.velocity;

                            newVelocity = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(80));

                            dust = Dust.NewDustPerfect(position, DustID.Smoke, newVelocity * Main.rand.NextFloat(0.01f, 0.065f), default, default, 2f);
                            dust.noGravity = true;
							dust.velocity += player.velocity;
						}

                        for (int numProjectiles = 0; numProjectiles <= 3; numProjectiles++)
                        {
                            Vector2 newVelocity = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(25));

                            int Bullet = Projectile.NewProjectile(Projectile.GetSource_FromAI(), position.X, position.Y, newVelocity.X, newVelocity.Y, 
                            ModContent.ProjectileType<PirateBlunderbussBullet>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                    }
                }

                if (Projectile.timeLeft < 5)
                {
                    if (!player.channel || !player.HasAmmo(ItemGlobal.ActiveItem(player)))
                    {
                        Projectile.Kill();
                    }
                    else
                    {
                        Projectile.ai[2] = 0;
                    }
                }
			}

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }
	}
}