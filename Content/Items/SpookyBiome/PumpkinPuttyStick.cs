using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Buffs.Minion;
using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome
{
    public class PumpkinPuttyStick : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 55;
			Item.mana = 20;
            Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.autoReuse = true; 
            Item.width = 38;
            Item.height = 38;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.Item44;
            Item.buffType = ModContent.BuffType<TinyPumpPuttyBuff>();
			Item.shoot = ModContent.ProjectileType<TinyPumpPutty>();
            Item.shootSpeed = 8f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            player.AddBuff(Item.buffType, 2);

			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
			projectile.originalDamage = Item.damage;

			return false;
		}
    }
}