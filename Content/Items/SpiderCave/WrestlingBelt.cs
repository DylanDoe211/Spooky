using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Items.SpiderCave
{
	public class WrestlingBelt : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 24;
			Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 20);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.GetDamage(DamageClass.Generic) += 0.1f;
			player.GetAttackSpeed(DamageClass.Generic) += 0.1f;

			if (player.statLife <= (player.statLifeMax / 2))
			{
				player.statDefense += 10;
				player.GetDamage(DamageClass.Generic) += 0.1f;
			}
        }
    }
}
