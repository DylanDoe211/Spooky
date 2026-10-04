using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Shipyard
{
	public class PirateShipWheelProj : ModProjectile
	{
		public override void SetDefaults()
		{
            Projectile.width = 54;
            Projectile.height = 54;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 5;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
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

            if (!player.CheckMana(ItemGlobal.ActiveItem(player), ItemGlobal.ActiveItem(player).mana, false, false))
			{
				Projectile.Kill();
			}

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
            Projectile.rotation += 0.2f * Projectile.direction;

            Projectile.position = player.MountedCenter - Projectile.Size / 2 + new Vector2((Projectile.direction == -1 ? -15 : 15), 0);

			if (player.channel)
            {
                Projectile.timeLeft = 2;

                player.itemRotation = Projectile.rotation;

                player.bodyFrame.Y = player.bodyFrame.Height * 3;

                Projectile.ai[2]++;
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
				Projectile.Kill();
            }

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }
	}
}