using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpiderCave;
 
namespace Spooky.Content.Items.SpiderCave
{
	public class MetalFistBox : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 40;
			Item.DamageType = DamageClass.Melee;
			Item.noMelee = true;
			Item.autoReuse = false;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 68;           
			Item.height = 22;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 20);
			Item.UseSound = SoundID.Item10;
			Item.shoot = ModContent.ProjectileType<MetalFist>();
			Item.shootSpeed = 25f;
		}

		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[Item.shoot] < 1;
		}
	}
}
