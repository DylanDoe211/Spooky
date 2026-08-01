using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class ThreeDimensionalGlasses : ModItem
	{
		private static Asset<Texture2D> FlipTexture;

		public override void Load()
		{
			FlipTexture = ModContent.Request<Texture2D>("Spooky/Content/Items/Costume/ThreeDimensionalGlasses_Head_Flipped");
		}

		public override void SetStaticDefaults()
		{
			ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
		}

		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 26;
			Item.vanity = true;
			Item.rare = ItemRarityID.Blue;
		}

		public override bool ModifyEquipTextureDraw(ref PlayerDrawSet drawInfo, ref DrawData drawData, EquipTexture equipTexture, string methodName)
		{
			drawInfo.DrawDataCache.Add(drawData);
			if (drawInfo.drawPlayer.direction == -1)
				drawInfo.DrawDataCache.Add(drawData with { texture = FlipTexture.Value });
			return false;
		}
	}
}