using Terraria;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Items.Cemetery.Contraband;

namespace Spooky.Content.Buffs
{
	public class SlendermanPageBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			//set page delay so pages cant drop for another 20 seconds
            player.GetModPlayer<SlendermanPagePlayer>().SlendermanPageDelay = 1200;
		}
	}
}
