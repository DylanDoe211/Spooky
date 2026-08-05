using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Items.Food
{
	public class Cranberry : ModItem
	{
		public override void SetStaticDefaults() 
        {
			Item.ResearchUnlockCount = 5;

            ItemID.Sets.IsFood[Type] = true;

			Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));

			ItemID.Sets.FoodParticleColors[Item.type] = new Color[1] 
            {
				new Color(212, 49, 64)
			};
		}

		public override void SetDefaults() 
        {
			Item.DefaultToFood(28, 28, BuffID.WellFed, 18000);
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
		}
	}
}