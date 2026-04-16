using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Projectiles.SpiderCave;
using Spooky.Content.Tiles.SpiderCave;
 
namespace Spooky.Content.Items.SpiderCave.OldHunter
{
	public class TrackingCrossbow : ModItem
	{
		public static readonly SoundStyle UseSound = new("Spooky/Content/Sounds/CrossbowCharge", SoundType.Sound);

		public override void SetDefaults()
		{
			Item.damage = 35;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = false;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 68;           
			Item.height = 22;
			Item.useTime = 75;
			Item.useAnimation = 75;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 20);
			Item.UseSound = UseSound;
			Item.shoot = ModContent.ProjectileType<TrackingCrossbowProj>();
			Item.useAmmo = AmmoID.Arrow;
			Item.shootSpeed = 10f;
		}

		public override bool CanConsumeAmmo(Item ammo, Player player)
		{
			return player.ItemUsesThisAnimation != 0;
		}

		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[Item.shoot] < 1;
		}
		
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position.X, position.Y, 0, 0, Item.shoot, damage, knockback, player.whoAmI);

			return false;
		}
	}
}
