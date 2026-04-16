using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class MiteDen : ModProjectile
    {
        float addedStretch = 0f;
		float stretchRecoil = 0f;

        bool isAttacking = false;

        private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> GlowTexture1;
        private static Asset<Texture2D> GlowTexture2;

        public override void SetStaticDefaults()
		{
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 30;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.sentry = true;
            Projectile.netImportant = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
        }

        public override bool PreDraw(ref Color lightColor)
		{
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            GlowTexture1 ??= ModContent.Request<Texture2D>(Texture + "Glow1");
            GlowTexture2 ??= ModContent.Request<Texture2D>(Texture + "Glow2");

            float stretch = 0f;

			stretch = Math.Abs(stretch) - addedStretch;

			//limit how much it can stretch
			if (stretch > 0.5f)
			{
				stretch = 0.5f;
			}

			//limit how much it can squish
			if (stretch < -0.5f)
			{
				stretch = -0.5f;
			}

			Vector2 scaleStretch = new Vector2(1f + stretch, 1f - stretch);

            Vector2 drawOrigin = new(GlowTexture1.Width() * 0.5f, Projectile.height);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y + 15) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, GlowTexture1.Height() / Main.projFrames[Projectile.type] * Projectile.frame, GlowTexture1.Width(), GlowTexture1.Height() / Main.projFrames[Projectile.type]);

            float time = Main.GameUpdateCount * 0.01f;

            float intensity1 = 0.7f;
            intensity1 *= (float)MathF.Sin(8f + time);

            float intensity2 = 0.7f;
            intensity2 *= (float)MathF.Cos(8f + time);
			
            Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, lightColor, Projectile.rotation, drawOrigin, scaleStretch, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(GlowTexture1.Value, vector, rectangle, Color.White * intensity1, Projectile.rotation, drawOrigin, scaleStretch, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(GlowTexture2.Value, vector, rectangle, Color.White * intensity2, Projectile.rotation, drawOrigin, scaleStretch, SpriteEffects.None, 0);

            return false;
        }

        public override bool? CanDamage()
		{
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			fallThrough = false;
			return true;
		}

        public override void AI()
        {
            //fall down constantly
            Projectile.velocity.Y++;
            if (Projectile.velocity.Y > 20f)
            {
                Projectile.velocity.Y = 20f;
            }

            //stretch stuff
			if (stretchRecoil > 0)
			{
				stretchRecoil -= 0.05f;
			}
			else
			{
				stretchRecoil = 0;
			}

			addedStretch = -stretchRecoil;

            //target an enemy
            for (int i = 0; i < Main.maxNPCs; i++)
            {
				NPC Target = Projectile.OwnerMinionAttackTargetNPC;
                if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type] && Vector2.Distance(Projectile.Center, Target.Center) <= 750f)
                {
					AttackingAI(Target);

					break;
				}
                else
                {
                    isAttacking = false;
                }

				NPC NPC = Main.npc[i];
                if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Projectile.Center, NPC.Center) <= 750f)
                {
					AttackingAI(NPC);

					break;
				}
                else
                {
                    isAttacking = false;
                }
            }
        }

        public void AttackingAI(NPC target)
		{
            isAttacking = true;

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 80)
            {
                SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);

                stretchRecoil = 0.65f;

                for (int numMites = 0; numMites < 3; numMites++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 ProjectilePosition = Projectile.Center - new Vector2(0, 20);
                        Vector2 ShootSpeed = Projectile.Center - ProjectilePosition;
                        ShootSpeed.Normalize();
                        ShootSpeed *= Main.rand.NextFloat(10f, 20f);

                        Vector2 newVelocity = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(25));

                        int newMite = Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, newVelocity, ModContent.ProjectileType<MiteProjectile>(), Projectile.damage, 0, Projectile.owner);
                        Main.projectile[newMite].DamageType = DamageClass.Summon;
                        Main.projectile[newMite].ai[0] = Main.rand.Next(0, 8);
                        Main.projectile[newMite].ai[2] = 2;
                        Main.projectile[newMite].localAI[0] = 1;
                        Main.projectile[newMite].penetrate = 2;
                    }
                }

                Projectile.ai[0] = 0;
                Projectile.netUpdate = true;
            }
		}
    }
}