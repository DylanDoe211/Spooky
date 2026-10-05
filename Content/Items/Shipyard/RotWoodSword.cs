using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.Shipyard;
 
namespace Spooky.Content.Items.Shipyard
{
    public class RotWoodSword : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Melee;
            Item.autoReuse = false;         
            Item.width = 44;
            Item.height = 46;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.White;
            Item.value = Item.buyPrice(copper: 20);
            Item.UseSound = SoundID.Item1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RotWoodItem>(), 7)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}