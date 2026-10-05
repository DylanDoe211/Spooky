using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Shipyard;

namespace Spooky.Content.Items.Shipyard
{
    public class PirateHook : ModItem
    {
        int numUses = -1;

        public override void SetDefaults()
        {
            Item.damage = 20;
			Item.DamageType = DamageClass.Melee;
			Item.noUseGraphic = true;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.channel = true;
            Item.width = 58;
            Item.height = 48;
            Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 2);
            Item.UseSound = SoundID.Item7;
            Item.shoot = ModContent.ProjectileType<PirateHookProj>();
            Item.shootSpeed = 12f;
        }

        public override bool MeleePrefix() 
		{
			return true;
		}

        public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<PirateHookProj>()] <= 0;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			numUses++;
			if (numUses > 1)
            {
                numUses = 0;
            }

			Projectile.NewProjectileDirect(source, position + (velocity * 20) + (velocity.RotatedBy(-1.57f * player.direction) * 20), Vector2.Zero, type, damage, knockback, player.whoAmI, numUses == 0 ? 0 : 1);
			
			return false;
		}
    }
}