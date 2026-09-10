using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Buffs
{
	public class PiratePotionBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = false;
			Main.persistentBuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<PiratePotionPlayer>().PiratePotionBuff = true;
		}
	}

	public class PiratePotionPlayer : ModPlayer
    {
        public bool PiratePotionBuff = false;

		public override void ResetEffects()
        {
            PiratePotionBuff = false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (PiratePotionBuff && Main.rand.NextBool(70))
			{
				SoundEngine.PlaySound(SoundID.Coins, target.Center);

				int Coin = Item.NewItem(target.GetSource_OnHit(target), target.Center, ItemID.GoldCoin);
				if (Main.netMode == NetmodeID.Server && Coin > 0)
				{
					NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Coin, 1f);
				}

				if (Main.netMode != NetmodeID.Server)
				{
					for (int numGores = 1; numGores <= 10; numGores++)
					{
						Gore.NewGore(target.GetSource_OnHit(target), target.Center, new Vector2(0, -3).RotatedByRandom(360), GoreID.ShadowMimicCoins);
					}
				}
			}
		}
	}
}
