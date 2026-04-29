using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Buffs.Minion;
using Spooky.Content.Projectiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave
{
    public class WolfSpiderStaff : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 45;
			Item.mana = 20;
            Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.autoReuse = true; 
            Item.width = 44;
            Item.height = 78;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.Item76 with { Volume = 0.5f, Pitch = -0.5f };
            Item.buffType = ModContent.BuffType<WolfSpiderMinionBuff>();
			Item.shoot = ModContent.ProjectileType<WolfSpiderMinion>();
            Item.shootSpeed = 7f;
        }

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			player.AddBuff(Item.buffType, 2);

			var projectile  = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
			projectile.originalDamage = Item.damage;

			return false;
		}
    }
}