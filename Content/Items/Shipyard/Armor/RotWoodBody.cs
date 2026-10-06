using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.Shipyard;

namespace Spooky.Content.Items.Shipyard.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class RotWoodBody : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 2;
			Item.width = 34;
			Item.height = 20;
			Item.rare = ItemRarityID.White;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RotWoodItem>(), 30)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}