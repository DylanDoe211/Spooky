using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

using Spooky.Core;
using Spooky.Content.Buffs;

namespace Spooky.Content.Items.Catacomb
{
    public class SkullAmulet : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;  
            Item.value = Item.buyPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SkullAmuletPlayer>().SkullAmulet = true;
        }
    }

    public class SkullAmuletPlayer : ModPlayer
    {
        public bool SkullAmulet = false;
        public int SkullFrenzyCharge = 0;

        public override void ResetEffects()
        {
            SkullAmulet = false;
        }

        public override void PreUpdate()
        {
            if (!SkullAmulet)
            {
                SkullFrenzyCharge = 0;
            }

            if (SkullFrenzyCharge >= 10)
            {
                Player.AddBuff(ModContent.BuffType<SkullFrenzyBuff>(), 600);

                SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton with { Volume = SoundID.DD2_DarkMageSummonSkeleton.Volume * 3.5f }, Player.Center);

                for (int numDust = 0; numDust < 45; numDust++)
                {
                    int newDust = Dust.NewDust(Player.position, Player.width, Player.height, DustID.KryptonMoss, 0f, 0f, 100, default, 1.5f);
                    Main.dust[newDust].velocity.X *= Main.rand.Next(-12, 12);
                    Main.dust[newDust].velocity.Y *= Main.rand.Next(-12, 12);
                    Main.dust[newDust].noGravity = true;
                }

                SkullFrenzyCharge = 0;
            }
        }

        public override void PostUpdate()
		{
			if (SkullFrenzyCharge > 0)
			{
				Player.GetDamage(DamageClass.Generic) += (0.02f * SkullFrenzyCharge);
			}
        }
    }
}