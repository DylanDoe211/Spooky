using Terraria;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Buffs.Debuff
{
	public class PeacockSpiderMaskDebuff : ModBuff
	{
        public override string Texture => "Spooky/Content/Buffs/Debuff/DebuffPlaceholder";

        public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
			BuffGlobal.IsSpookyDebuffForAchievement[Type] = true;
		}
    }
}
