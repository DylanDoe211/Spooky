using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;
using Spooky.Content.Projectiles.SpookyBiome;
 
namespace Spooky.Content.Items.SpookyBiome
{
	public class ScarecrowShotgun : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 42;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = true; 
			Item.width = 106;           
			Item.height = 30;
			Item.useTime = 60;         
			Item.useAnimation = 60;
			Item.useStyle = ItemUseStyleID.Shoot;         
			Item.knockBack = 5;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 20);
			Item.UseSound = SoundID.Item36;
			Item.shoot = ProjectileID.PurificationPowder;
			Item.useAmmo = AmmoID.Bullet;
			Item.shootSpeed = 13f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 35f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

			int[] DustTypes = new int[] { DustID.YellowTorch, DustID.PurpleTorch, DustID.GreenTorch, DustID.RedTorch };

			for (int numDusts = 0; numDusts < 12; numDusts++)
			{
				Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(80));

				Dust dust = Dust.NewDustPerfect(position, Main.rand.Next(DustTypes), newVelocity * Main.rand.NextFloat(0.25f, 1f), default, default, 3f);
				dust.noGravity = true;
				dust.velocity += player.velocity;

				newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(80));

				dust = Dust.NewDustPerfect(position, DustID.Smoke, newVelocity * Main.rand.NextFloat(0.1f, 0.35f), default, default, 1.5f);
				dust.noGravity = true;
				dust.velocity += player.velocity;
			}

            for (int numProjectiles = 0; numProjectiles < 4; numProjectiles++)
            {
				Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(25));

			    Projectile.NewProjectile(source, position, newVelocity, ModContent.ProjectileType<ScarecrowShotgunSoul>(), damage, knockback, player.whoAmI, ai2: numProjectiles);
            }

			return false;
		}
	}
}