using Terraria;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.Pets;

namespace Spooky.Content.Buffs.Pets
{
	public class FireflyPetBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			Main.lightPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.buffTime[buffIndex] = 2;

			player.GetModPlayer<SpookyPlayer>().FireflyPet = true;

			if (player.ownedProjectileCounts[ModContent.ProjectileType<FireflyPet>()] < 1 && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center.X, player.Center.Y - 10, 0f, 0f, ModContent.ProjectileType<FireflyPet>(), 0, 0f, player.whoAmI);
			}
		}
	}
}
