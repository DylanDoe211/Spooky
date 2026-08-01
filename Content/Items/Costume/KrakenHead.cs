using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class KrakenHead : ModItem
	{
		private static Asset<Texture2D> GlowTexture;

		public override void Load()
		{
			GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow");
		}

		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 26;
			Item.vanity = true;
			Item.rare = ItemRarityID.Quest;
			Item.value = Item.buyPrice(gold: 10);
		}

		public override bool ModifyEquipTextureDraw(ref PlayerDrawSet drawInfo, ref DrawData drawData, EquipTexture equipTexture, string methodName)
		{
			drawInfo.DrawDataCache.Add(drawData);
			drawInfo.DrawDataCache.Add(drawData with { color = Color.White, texture = GlowTexture.Value });
			return false;
		}
	}
}