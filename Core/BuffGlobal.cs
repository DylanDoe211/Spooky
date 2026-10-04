
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Core
{
    public class BuffGlobal : GlobalBuff
	{
		public static bool[] IsSpookyDebuffForAchievement = BuffID.Sets.Factory.CreateBoolSet();
	}
}