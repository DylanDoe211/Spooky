using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.Shipyard;

namespace Spooky.Content.Items.Shipyard.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class RotWoodLegs : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 1;
			Item.width = 22;
			Item.height = 18;
			Item.rare = ItemRarityID.White;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RotWoodItem>(), 25)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}