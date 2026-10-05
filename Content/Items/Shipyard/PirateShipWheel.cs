using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Shipyard;

namespace Spooky.Content.Items.Shipyard
{
    public class PirateShipWheel : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 35;
            Item.mana = 5;
			Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 54;
			Item.height = 54;
            Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 2);
            Item.shoot = ModContent.ProjectileType<PirateShipWheelProj>();
			Item.shootSpeed = 0f;
        }

        public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<PirateShipWheelProj>()] < 1;
		}
		
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position.X, position.Y, 0, 0, ModContent.ProjectileType<PirateShipWheelProj>(), damage, knockback, player.whoAmI);
			Projectile.NewProjectile(source, position.X, position.Y, 0, 0, ModContent.ProjectileType<PirateShipWheelVortex>(), damage, knockback, player.whoAmI);

			return false;
		}
    }
}