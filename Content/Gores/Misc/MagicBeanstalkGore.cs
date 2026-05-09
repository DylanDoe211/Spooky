using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;

namespace Spooky.Content.Gores.Misc
{
	public class MagicBeanstalkTopGore : ModGore
	{
		public override void OnSpawn(Gore gore, IEntitySource source)
		{
			ChildSafety.SafeGore[gore.type] = true;
			gore.timeLeft = 0;
		}
	}

	public class MagicBeanstalkStemGore : MagicBeanstalkTopGore
	{
	}
}