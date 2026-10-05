using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.Shipyard;
 
namespace Spooky.Content.Items.Shipyard
{
    public class RotWoodHammer : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 5;
            Item.hammer = 25;
            Item.DamageType = DamageClass.Melee;
            Item.autoReuse = true;
            Item.width = 40;
            Item.height = 42;
            Item.useTime = 25;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.White;
            Item.value = Item.buyPrice(copper: 20);
            Item.UseSound = SoundID.Item1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RotWoodItem>(), 8)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}