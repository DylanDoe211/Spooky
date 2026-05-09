using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpiderCave;
 
namespace Spooky.Content.Items.SpiderCave
{
	public class MagicBeanBag : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 60;
			Item.mana = 20;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.width = 28;
			Item.height = 32;
			Item.useTime = 35;
			Item.useAnimation = 35;
			Item.useStyle = ItemUseStyleID.RaiseLamp;
			Item.knockBack = 4;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 20);
			Item.UseSound = SoundID.Item28 with { Volume = 0.5f, Pitch = -0.65f };
			Item.shoot = ModContent.ProjectileType<MagicBean>();
			Item.shootSpeed = 8f;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(25));

			Projectile.NewProjectile(source, position, newVelocity, type, damage, knockback, player.whoAmI);

			return false;
		}
	}
}
