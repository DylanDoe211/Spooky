using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Items.Catacomb
{
    public class CrossCharm : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 44;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;  
            Item.value = Item.buyPrice(gold: 15);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            if (!player.HasBuff(ModContent.BuffType<CrossCooldown>()))
            {
                player.GetModPlayer<CrossCharmPlayer>().CrossCharmShield = true;
                player.statDefense += 15;
            }
        }
    }

    public class CrossCharmPlayer : ModPlayer
    {
        public bool CrossCharmShield = false;

        public override void ResetEffects()
        {
            CrossCharmShield = false;
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (CrossCharmShield && !Player.HasBuff(ModContent.BuffType<CrossCooldown>()))
            {
                Player.AddBuff(ModContent.BuffType<CrossCooldown>(), 600);

                for (int numDust = 0; numDust < 20; numDust++)
                {
                    int dustEffect = Dust.NewDust(Player.position, Player.width, Player.height, DustID.OrangeTorch, 0f, 0f, 100, default, 2f);
                    Main.dust[dustEffect].velocity *= 3f;
                    Main.dust[dustEffect].noGravity = true;

                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[dustEffect].scale = 0.5f;
                        Main.dust[dustEffect].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
            }
        }
    }
}