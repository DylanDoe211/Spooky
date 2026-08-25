using Terraria;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Items.Cemetery.Armor;

namespace Spooky.Content.Buffs.Minion
{
	public class PumpkinHeadBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.GetModPlayer<HorsemanArmorPlayer>().HorsemanSet)
			{
				player.buffTime[buffIndex] = 2;
			}
			else
			{
				player.buffTime[buffIndex] = 0;
			}
		}
	}
}
