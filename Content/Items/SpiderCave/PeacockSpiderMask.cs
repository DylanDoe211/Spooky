using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Items.SpiderCave
{
	public class PeacockSpiderMask : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 50;
			Item.height = 52;
			Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 3);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.GetModPlayer<PeacockSpiderMaskPlayer>().PeacockSpiderMask = true;
        }
    }

	public class PeacockSpiderMaskPlayer : ModPlayer
    {
		public bool PeacockSpiderMask = false;

		public override void ResetEffects()
        {
			PeacockSpiderMask = false;
		}

		public override void OnHurt(Player.HurtInfo info)
        {
			if (PeacockSpiderMask)
			{
				foreach (var NPC in Main.ActiveNPCs)
				{
                    if (!NPC.friendly && !NPC.immortal && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && NPC.Distance(Player.Center) <= 600)
                    {
                        NPC.AddBuff(ModContent.BuffType<PeacockSpiderMaskDebuff>(), 300);
                    }
				}
			}
		}
	}
}
