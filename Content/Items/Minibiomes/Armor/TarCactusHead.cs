using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Minibiomes.Desert;
using Spooky.Content.Tiles.Minibiomes.Desert;

namespace Spooky.Content.Items.Minibiomes.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class TarCactusHead : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 3;
			Item.width = 22;
			Item.height = 16;
			Item.rare = ItemRarityID.Blue;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<TarCactusBody>() && legs.type == ModContent.ItemType<TarCactusLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.TarCactusArmor");
			player.GetModPlayer<TarCactusArmorPlayer>().TarCactusSet = true;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<TarPitCactusBlockItem>(), 20)
            .AddRecipeGroup(RecipeGroupID.IronBar, 12)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}

	public class TarCactusArmorPlayer : ModPlayer
    {
        public bool TarCactusSet = false;

		public override void ResetEffects()
        {
            TarCactusSet = false;
		}

		public override void OnHurt(Player.HurtInfo info)
        {
			if (TarCactusSet)
            {
				int MinDamage = 20;
				float Damage = info.Damage < MinDamage ? MinDamage : info.Damage;

                float maxAmount = 12;
				int currentAmount = 0;
                while (currentAmount <= maxAmount)
				{
					Vector2 velocity = new Vector2(Main.rand.NextFloat(2f, 25f), Main.rand.NextFloat(2f, 25f));
                    Vector2 Bounds = new Vector2(Main.rand.NextFloat(2f, 25f), Main.rand.NextFloat(2f, 25f));
                    float intensity = Main.rand.NextFloat(2f, 25f);

					Vector2 vector12 = Vector2.UnitX * 0f;
					vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
					vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
					Vector2 ShootVelocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;

					Projectile.NewProjectile(Player.GetSource_OnHurt(info.DamageSource), Player.Center, ShootVelocity, ModContent.ProjectileType<CactusNeedle>(), (int)Damage, 4.5f, Player.whoAmI);

					currentAmount++;
				}
            }
		}
	}
}