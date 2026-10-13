using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientVenusMagnum : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 40;
			Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
			Item.autoReuse = false;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 58;
			Item.height = 42;
            Item.useTime = 120;
			Item.useAnimation = 120;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 35);
            Item.shoot = ModContent.ProjectileType<SentientVenusMagnumProj>();
			Item.useAmmo = AmmoID.Bullet;
			Item.shootSpeed = 15f;
        }

        public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<SentientVenusMagnumProj>()] < 1;
		}
		
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position.X, position.Y, 0, 0, ModContent.ProjectileType<SentientVenusMagnumProj>(), damage, knockback, player.whoAmI);

			return false;
		}
    }
}