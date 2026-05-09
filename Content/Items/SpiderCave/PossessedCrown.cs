using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Buffs.Minion;
using Spooky.Content.Projectiles.SpiderCave;
 
namespace Spooky.Content.Items.SpiderCave
{
    public class PossessedCrown : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.mana = 20;
            Item.DamageType = DamageClass.Summon;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.width = 30;
            Item.height = 28;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 20);
            Item.UseSound = SoundID.Item78;
            Item.buffType = ModContent.BuffType<PossessedCrownBuff>();
			Item.shoot = ModContent.ProjectileType<PossessedCrownProj>();
            Item.shootSpeed = 0f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            player.AddBuff(Item.buffType, 2);

            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            projectile.ai[1] = player.ownedProjectileCounts[type]; //projectile count for the minion position
			projectile.originalDamage = Item.damage;

			return false;
		}
    }
}