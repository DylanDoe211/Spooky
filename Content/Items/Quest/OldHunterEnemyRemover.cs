using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
 
namespace Spooky.Content.Items.Quest
{
	public class OldHunterEnemyRemover : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 26;
			Item.maxStack = 1;
			Item.rare = ItemRarityID.Quest;
		}
	}
}