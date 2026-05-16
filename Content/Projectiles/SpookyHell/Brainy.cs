using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class Brainy : ModProjectile
    {
        float OrbiterDistance = 1.2f;
		float OrbiterRotation = 0f;
        float ExtraRotation = 0f;

        private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> OrbTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }
        
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 52;
            Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
            Projectile.minionSlots = 0;
            Projectile.aiStyle = -1;
        }

        //shout-out Flye for this orbiter draw code: https://github.com/flye-name/ebonian-mod/blob/1e460b0b4b889f2554e9d3441242edd5257a9eb6/Content/NPCs/Overworld/Asteroid/AsteroidHerder.cs
		public float CircleDividedEqually(float i, float max)
		{
			return 2f * MathF.PI / max * i;
		}

        public int GetPlayerMinionCount()
        {
            int MinionCount = 0;

            foreach (var Proj in Main.ActiveProjectiles)
            {
                if (Proj.minion && Proj.owner == Projectile.owner && Proj.type != ModContent.ProjectileType<Brainy>()) 
                {
                    MinionCount++;
                }
            }

            return MinionCount;
        }

        public override bool PreDraw(ref Color lightColor)
		{
			DrawBrainy(false, lightColor);

			return false;
        }

        public void DrawBrainy(bool SpawnGore, Color lightColor)
		{
            float rotation = Main.GlobalTimeWrappedHourly + Projectile.whoAmI * 5 + MathHelper.ToRadians(ExtraRotation);

            OrbiterRotation = Utils.AngleLerp(OrbiterRotation, -MathF.Sin(rotation * 0.05f) * 3, 0.2f);

            if (!SpawnGore)
			{
                ProjTexture ??= ModContent.Request<Texture2D>(Texture);

                DrawEyeOrbiters(false, rotation, lightColor, false);

                Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
                Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
                Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

                var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                
                Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

                DrawEyeOrbiters(true, rotation, lightColor, false);
            }
            else
            {
                DrawEyeOrbiters(false, rotation, lightColor, true);
            }
        }

        public void DrawEyeOrbiters(bool after, float rotOff, Color color, bool SpawnGore)
        {
            if (!SpawnGore)
            {
                OrbTexture ??= ModContent.Request<Texture2D>(Texture + "Eye");
            }

            int TotalEyes = GetPlayerMinionCount();
            for (int i = after ? TotalEyes : 0; after ? i > 0 : i < TotalEyes; i += (after ? -1 : 1))
            {
                float angle = CircleDividedEqually(i, TotalEyes) + rotOff;
                float progress = (MathF.Sin(rotOff * 0.1f) + 1) * 0.5f;
                float distscale = MathHelper.Lerp(0.5f, 0.65f, progress) * 1.25f * OrbiterDistance;
                Vector2 angleVec = angle.ToRotationVector2() * 50;
                float perspectiveScale = MathHelper.Lerp(MathF.Sin(rotOff * 0.4f), 0.75f, 0f);
                Vector2 offset = new Vector2(angleVec.X, angleVec.Y * 0.25f) * distscale;
                float scale = MathHelper.Lerp(MathHelper.Lerp(0.5f + MathF.Abs(perspectiveScale * 0.25f), 1, MathHelper.Clamp(MathF.Sin(angle), 0, 1f)), 1, 0f);
                bool shouldDraw = after ? scale > 0.78f : scale <= 0.78f;

                if (shouldDraw && !SpawnGore)
                {
                    Main.EntitySpriteDraw(OrbTexture.Value, Projectile.Center + offset.RotatedBy(OrbiterRotation + MathF.Sin(rotOff * 0.05f) * 3) - Main.screenPosition, null,
                    color, angle, OrbTexture.Size() / 2, scale, SpriteEffects.None, 0f);
                }
                if (SpawnGore)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center + offset.RotatedBy(OrbiterRotation + MathF.Sin(rotOff * 0.05f) * 3), 
                        Vector2.Zero, ModContent.Find<ModGore>("Spooky/BrainEyeGore").Type, scale);
                    }
                }
            }
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

            Player player = Main.player[Projectile.owner];

			if (player.dead || !player.active) 
            {
				player.ClearBuff(ModContent.BuffType<BrainyBuff>());
			}

			if (player.HasBuff(ModContent.BuffType<BrainyBuff>())) 
            {
				Projectile.timeLeft = 2;
			}

            Projectile.spriteDirection = player.direction;

            Projectile.ai[2]++;
            Vector2 velocityOffset = -player.velocity * 7;
            Vector2 destination = new Vector2(player.Center.X + velocityOffset.X, player.Center.Y + velocityOffset.Y - 85 + (float)Math.Sin(Projectile.ai[2] / 30) * 30);
            
            if (Projectile.Distance(destination) >= 15)
            {
                float vel = MathHelper.Clamp(Projectile.Distance(destination) / 12, 10, 15);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(destination) * vel, 0.08f);
            }
            else
            {
                Projectile.velocity *= 0.9f;
            }

            if (Projectile.Distance(player.Center) > 1200f)
            {
                Projectile.Center = player.Center;
            }

            //actual minion exploding ai
            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 3510 && Projectile.ai[0] < 3600)
            {
                Projectile.localAI[0]++;
				Projectile.localAI[1] += 0.12f;
				ExtraRotation += MathHelper.Lerp(0, Projectile.localAI[1], MathHelper.Clamp(Projectile.localAI[0] / 60f, 0, 1));
            }

            if (Projectile.ai[0] >= 3600)
            {
                DrawBrainy(true, Color.White);

                foreach (var Proj in Main.ActiveProjectiles)
				{
                    if (Proj.minion && Proj.owner == Projectile.owner && Proj.type != ModContent.ProjectileType<Brainy>()) 
                    {
                        SoundEngine.PlaySound(SoundID.Item96, Proj.Center);

                        LaunchLaser(Projectile.Center, Proj.Center);

                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Proj.Center, Vector2.Zero, 
                        ModContent.ProjectileType<BrainyExplosion>(), Projectile.damage + Proj.damage, 0f, Projectile.owner);

                        float maxAmount = 35;
                        int currentAmount = 0;
                        while (currentAmount <= maxAmount)
                        {
                            Vector2 velocity = new Vector2(Main.rand.NextFloat(8f, 18f), Main.rand.NextFloat(8f, 18f));
                            Vector2 Bounds = new Vector2(Main.rand.NextFloat(8f, 18f), Main.rand.NextFloat(8f, 18f));
                            float intensity = Main.rand.NextFloat(8f, 18f);

                            Vector2 vector12 = Vector2.UnitX * 0f;
                            vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
                            vector12 = vector12.RotatedBy(velocity.ToRotation(), default);

                            int newDust = Dust.NewDust(Projectile.Center, 1, 1, 182, 0f, 0f, 100, default, 2f);
                            Main.dust[newDust].noGravity = true;
                            Main.dust[newDust].position = Projectile.Center + vector12;
                            Main.dust[newDust].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;

                            currentAmount++;
                        }

                        Proj.Kill();
                    }
                }

				Projectile.localAI[0] = 0;
				Projectile.localAI[1] = 0;
				ExtraRotation = 0;

				Projectile.ai[0] = 0;
            }
        }

        private void LaunchLaser(Vector2 fromArea, Vector2 toArea)
		{
			Vector2 direction = fromArea - toArea;
			direction.Normalize();
			direction *= fromArea - toArea;

			Projectile.NewProjectile(Projectile.GetSource_FromThis(), fromArea, Vector2.Zero,
			ModContent.ProjectileType<BrainyBeam>(), 0, 0f, Projectile.owner, ai0: toArea.X - direction.X, ai1: toArea.Y - direction.Y);
		}
    }
}