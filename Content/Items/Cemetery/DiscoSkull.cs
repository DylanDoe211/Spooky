using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Projectiles.Cemetery;
 
namespace Spooky.Content.Items.Cemetery
{
	public class DiscoSkull : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 75;
			Item.mana = 5;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.useTurn = false;
			Item.autoReuse = true;
			Item.channel = true;
			Item.width = 40;
            Item.height = 44;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0;
			Item.rare = ItemRarityID.Yellow;
			Item.value = Item.buyPrice(platinum: 1);
			Item.UseSound = SoundID.Item117;
			Item.shoot = ModContent.ProjectileType<DiscoSkullHoldout>();
			Item.shootSpeed = 0f;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-4, 0);
		}

		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<DiscoSkullHoldout>()] < 1;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position.X, position.Y, 0, 0, ModContent.ProjectileType<DiscoSkullHoldout>(), damage, knockback, player.whoAmI);

			return false;
		}
	}
}
