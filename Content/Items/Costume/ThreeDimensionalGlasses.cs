using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class ThreeDimensionalGlasses : ModItem, ISpecialArmorDraw
	{
		public string HeadFlippedTexture => "Spooky/Content/Items/Costume/ThreeDimensionalGlasses_Head_Flipped";

		public override void SetStaticDefaults()
		{
			ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
		}

		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 26;
			Item.vanity = true;
			Item.rare = ItemRarityID.Blue;
		}
	}
}