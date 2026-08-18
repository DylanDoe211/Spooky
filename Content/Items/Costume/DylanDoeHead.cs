using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class DylanDoeHead : ModItem
	{
		private static Asset<Texture2D> FlipTexture;

		public override void Load()
		{
			FlipTexture = ModContent.Request<Texture2D>("Spooky/Content/Items/Costume/DylanDoeHead_Head_Flipped");
		}

		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 30;
			Item.vanity = true;
			Item.rare = ItemRarityID.Quest;
			Item.value = Item.buyPrice(gold: 10);
		}

		public override bool ModifyEquipTextureDraw(ref PlayerDrawSet drawInfo, ref DrawData drawData, EquipTexture equipTexture, string methodName)
		{
			if (drawInfo.drawPlayer.direction == -1)
			{
				drawInfo.DrawDataCache.Add(drawData with { texture = FlipTexture.Value });
			}
			else
			{
				drawInfo.DrawDataCache.Add(drawData);
			}
			
			return false;
		}
	}
}