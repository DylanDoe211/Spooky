using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Buffs.Minion;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientPygmyStaff : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 42;
            Item.mana = 30;
			Item.DamageType = DamageClass.Summon;
            Item.autoReuse = true;
			Item.noMelee = true;
			Item.width = 48;
			Item.height = 54;
            Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 45);
            Item.UseSound = SoundID.Item79;
            Item.buffType = ModContent.BuffType<GoblinoidBuff>();
			Item.shoot = ModContent.ProjectileType<SentientGoblinFlyMouth>();
            Item.shootSpeed = 4f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            player.AddBuff(Item.buffType, 2);

            var projectile = Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<SentientGoblinFlyMouth>(), damage, knockback, player.whoAmI);
            projectile.originalDamage = Item.damage;
            projectile = Projectile.NewProjectileDirect(source, position - new Vector2(0, 15), velocity, ModContent.ProjectileType<SentientGoblinSpit>(), damage, knockback, player.whoAmI);
            projectile.originalDamage = Item.damage;
            projectile = Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<SentientGoblinTiny>(), damage, knockback, player.whoAmI);
            projectile.originalDamage = Item.damage;
            projectile = Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<SentientGoblinFlyEye>(), damage, knockback, player.whoAmI);
            projectile.originalDamage = Item.damage;

			return false;
		}
    }
}