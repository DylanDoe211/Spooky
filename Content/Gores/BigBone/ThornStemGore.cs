using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;

namespace Spooky.Content.Gores.BigBone
{
	public class ThornStemGore1 : ModGore
	{
		public override void OnSpawn(Gore gore, IEntitySource source)
		{
			ChildSafety.SafeGore[gore.type] = true;
			gore.timeLeft = 0;
		}
	}

	public class ThornStemGore2 : ThornStemGore1
	{
	}
}