using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Boss.Daffodil.Projectiles
{
	public class DaffodilPollen : ModProjectile
	{
        Vector2 SavePlayerPos;

		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = false;
            Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 250;
            Projectile.penetrate = 1;
            Projectile.aiStyle = -1;
		}

		public override void AI()
		{
            Player player = Main.player[Player.FindClosest(Projectile.Center, Projectile.width, Projectile.height)];

            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.15f * (float)Projectile.direction;

            if (Projectile.ai[0] == 0)
            {
                SavePlayerPos = player.Center;
                Projectile.ai[0]++;
            }
            else
            {
                if (Projectile.Distance(SavePlayerPos) <= 20f)
                {
                    Projectile.tileCollide = true;
                }
            }

            if (Main.rand.NextBool(3))
            {
                int ProjDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<DaffodilPollenDust>());
                Main.dust[ProjDust].noGravity = true;
                Main.dust[ProjDust].scale = 0.5f;
                Main.dust[ProjDust].velocity /= 4f;
                Main.dust[ProjDust].velocity += Projectile.velocity / 2;
            }
        }

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.DoubleJump with { Pitch = -0.5f, Volume = 2f }, Projectile.Center);

            int Damage = Main.masterMode ? 135 : Main.expertMode ? 85 : 50;

            foreach (var player in Main.ActivePlayers)
			{
				if (!player.dead && player.Distance(Projectile.Center) <= 80f)
				{
                    player.Hurt(PlayerDeathReason.ByCustomReason(Language.GetText("Mods.Spooky.DeathReasons.PollenExplosion").ToNetworkText(player.name)), Damage + Main.rand.Next(-10, 30), 0);
                }
            }

            //spawn pollen clouds
            float maxAmount = 25;
            int currentAmount = 0;
            while (currentAmount <= maxAmount)
            {
                Vector2 velocity = new Vector2(Main.rand.NextFloat(2f, 12f), Main.rand.NextFloat(2f, 12f));
                Vector2 Bounds = new Vector2(Main.rand.NextFloat(2f, 12f), Main.rand.NextFloat(2f, 12f));
                float intensity = Main.rand.NextFloat(2f, 12f);

                Vector2 vector12 = Vector2.UnitX * 0f;
                vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
                vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
                
                int Smoke = Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.White * 0.5f, 1f);
                Main.dust[Smoke].noGravity = true;
                Main.dust[Smoke].position = Projectile.Center + vector12;
                Main.dust[Smoke].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity * 0.2f;

                currentAmount++;
            }
		}
	}
}