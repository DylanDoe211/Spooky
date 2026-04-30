using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Fishing
{
	public class PumpkinBobber : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 14;
			Item.height = 30;
			Item.accessory = true;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 1);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.fishingSkill += 10;
			player.overrideFishingBobber = ModContent.ProjectileType<PumpkinBobberProj>();
		}
    }

	public class PumpkinBobberProj : ModProjectile
    {
        public override void SetDefaults()
        {
			Projectile.aiStyle = ProjAIStyleID.Bobber;
			AIType = ProjectileID.FishingBobber;
			Projectile.bobber = true;
			Projectile.width = 18;
			Projectile.height = 18;
			DrawOriginOffsetY = -6;
		}
    }
}
