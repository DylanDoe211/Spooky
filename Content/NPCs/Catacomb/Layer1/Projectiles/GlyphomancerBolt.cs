using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.NPCs.Catacomb.Layer1.Projectiles
{
    public class GlyphomancerBolt : ModProjectile
    {
		public override string Texture => "Spooky/Content/Projectiles/Blank";
		
        public override void SetDefaults()
        {
			Projectile.width = 12;                  			 
            Projectile.height = 12; 
			Projectile.friendly = false;
			Projectile.hostile = true;                               			  		
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 600;
            Projectile.alpha = 255;
		}

		public override bool? CanDamage()
		{
			return false;
		}

		public override void AI()
		{
			Player target = Main.player[Player.FindClosest(Projectile.Center, Projectile.width, Projectile.height)];

			if (Projectile.ai[0] == 0)
			{
				for (int numHands = 0; numHands < 3; numHands++)
				{
					int distance = 360 / 3;
					Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlyphomancerHand>(), Projectile.damage, Projectile.knockBack, Main.myPlayer,
					ai0: numHands * distance, ai1: Projectile.whoAmI, ai2: Projectile.ai[2]);
				}

				Projectile.ai[0]++;
			}

			Projectile.ai[1]++;
			if (Projectile.ai[1] == 120)
			{
				SoundEngine.PlaySound(SoundID.Item90 with { Pitch = 1.1f }, Projectile.Center);

				double Velocity = Math.Atan2(target.position.Y - Projectile.position.Y, target.position.X - Projectile.position.X);
				Projectile.velocity = new Vector2((float)Math.Cos(Velocity), (float)Math.Sin(Velocity)) * 4;

				Projectile.ai[1]++;

				Projectile.netUpdate = true;
			}
		}
    }
}
     
          






