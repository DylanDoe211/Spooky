using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.EggEvent.Projectiles
{
    public class BrainBeam : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
			Projectile.width = 12;
            Projectile.height = 12;
			Projectile.friendly = false;
			Projectile.hostile = true;                               			  		
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;  
            Projectile.penetrate = 1;
			Projectile.timeLeft = 300;
		}

        public override Color? GetAlpha(Color lightColor)
		{
            return Color.White * (1f - (Projectile.alpha / 255f));
        }
        
        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 vector = new Vector2(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 position = Projectile.oldPos[i] - Main.screenPosition + vector + new Vector2(0f, Projectile.gfxOffY);
                Color color = Color.MediumPurple * ((float)(Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length);
                Rectangle value = new Rectangle(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.spriteBatch.Draw(ProjTexture.Value, position, value, color, Projectile.rotation, vector, Projectile.scale, SpriteEffects.None, 0f);
            }

            return true;
        }

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Confused, 60);
        }

		public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            Projectile.ai[0]++;
            if (Projectile.ai[0] % 3 == 0)
            {
                int DustEffect = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<BrainConfusionDust>(), 0f, 0f, 100, Color.White, 0.85f);
                Main.dust[DustEffect].velocity = Vector2.Zero;
            }
		}
    }
}
     
          






