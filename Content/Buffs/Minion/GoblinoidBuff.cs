using Terraria;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Buffs.Minion
{
	public class GoblinoidBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<SentientGoblinFlyMouth>()] > 0 ||
			player.ownedProjectileCounts[ModContent.ProjectileType<SentientGoblinSpit>()] > 0 || 
			player.ownedProjectileCounts[ModContent.ProjectileType<SentientGoblinTiny>()] > 0) //||
			//player.ownedProjectileCounts[ModContent.ProjectileType<SentientGoblinTiny>()] > 0) 
			{
				player.buffTime[buffIndex] = 18000;
			}
			else
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}
}
