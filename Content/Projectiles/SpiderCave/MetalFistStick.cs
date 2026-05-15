using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class MetalFistStick : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/SpiderCave/MetalFist";

        public bool IsStickingToTarget = true;

        float FlashOpacity = 0f;

        private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> FlashTexture;

        public override void SetDefaults()
        {
			Projectile.width = 36;
            Projectile.height = 42;     
			Projectile.friendly = true;                              			  		
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            FlashTexture ??= ModContent.Request<Texture2D>(Texture + "Flash");

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            if (FlashOpacity > 0f)
            {
                Main.EntitySpriteDraw(FlashTexture.Value, vector, rectangle, Color.White * FlashOpacity, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
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
            Projectile.rotation = Projectile.ai[0];

            if (FlashOpacity > 0f)
            {
                FlashOpacity -= 0.025f;
            }

            if (Projectile.timeLeft > 60)
            {
                if (Projectile.timeLeft % 30 == 0)
                {
                    //SoundEngine.PlaySound(BeepSound, Projectile.Center);
                    FlashOpacity = 1f;
                }
            }
            else
            {
                if (Projectile.timeLeft % 15 == 0)
                {
                    //SoundEngine.PlaySound(BeepSound, Projectile.Center);
                    FlashOpacity = 1f;
                }
            }

            Vector2 position = Projectile.Center + Vector2.Normalize(Projectile.velocity);
			Dust dust = Dust.NewDustPerfect(position, 133, Vector2.Zero);
			dust.noGravity = true;
            dust.noLight = true;

            int npcTarget = (int)Projectile.ai[1];
            if (npcTarget < 0 || npcTarget >= 200)
            {
                Projectile.Kill();
            }
            else if (Main.npc[npcTarget].active && !Main.npc[npcTarget].dontTakeDamage) 
            {
                Projectile.Center = Main.npc[npcTarget].Center - Projectile.velocity * 2f;
                Projectile.gfxOffY = Main.npc[npcTarget].gfxOffY;
            }
            else 
            {
                Projectile.Kill();
            }
		}

        public override void OnKill(int timeLeft)
		{
            Player player = Main.player[Projectile.owner];

            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot with { Volume = 0.75f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with { Volume = 0.75f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item74 with { Volume = 0.75f, Pitch = -0.5f }, Projectile.Center);

            float maxAmount = 20;
            int currentAmount = 0;
            while (currentAmount <= maxAmount)
            {
                Vector2 velocity = new Vector2(Main.rand.NextFloat(6f, 15f), Main.rand.NextFloat(6f, 15f));
                Vector2 Bounds = new Vector2(Main.rand.NextFloat(6f, 15f), Main.rand.NextFloat(6f, 15f));
                float intensity = Main.rand.NextFloat(6f, 15f);

                Vector2 vector12 = Vector2.UnitX * 0f;
                vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
                vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
                int newDust = Dust.NewDust(Projectile.Center, 0, 0, DustID.Torch, 0f, 0f, 100, default, 3f);
                Main.dust[newDust].noGravity = true;
                Main.dust[newDust].position = Projectile.Center + vector12;
                Main.dust[newDust].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
                currentAmount++;
            }

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.Distance(Projectile.Center) <= 300f && !npc.friendly && !npc.dontTakeDamage)
                {
                    player.ApplyDamageToNPC(npc, Projectile.damage, Projectile.knockBack, 0, false, null, true);
                }
            }
        }
    }
}