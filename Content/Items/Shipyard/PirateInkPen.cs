using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Buffs.Minion;
using Spooky.Content.Projectiles.Shipyard;

namespace Spooky.Content.Items.Shipyard
{
	public class PirateInkPen : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 10;
			Item.mana = 20;
			Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.autoReuse = true;       
			Item.width = 42;           
			Item.height = 40;         
			Item.useTime = 35;         
			Item.useAnimation = 35;         
			Item.useStyle = ItemUseStyleID.Swing;          
			Item.knockBack = 1;
			Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.Item78;     
			Item.buffType = ModContent.BuffType<InkFishBuff>();
			Item.shoot = ModContent.ProjectileType<InkFishHead>();
			Item.shootSpeed = 3f;
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
