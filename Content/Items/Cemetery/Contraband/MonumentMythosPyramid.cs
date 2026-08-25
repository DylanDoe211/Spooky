using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Items.Cemetery.Contraband
{
    public class MonumentMythosPyramid : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 46;
            Item.accessory = true;
            Item.rare = ItemRarityID.Cyan;  
            Item.value = Item.buyPrice(gold: 60);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!player.HasBuff(ModContent.BuffType<MonumentMythosCooldown>()))
            {
                player.GetModPlayer<MonumentMythosPyramidPlayer>().MonumentMythosPyramid = true;
                player.endurance += 0.35f;
            }
        }
    }

    public class MonumentMythosPyramidPlayer : ModPlayer
    {
        public bool MonumentMythosPyramid = false;
        public int MythosPyarmidHits = 0;

        public override void ResetEffects()
        {
            MonumentMythosPyramid = false;
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (Player.HasBuff(ModContent.BuffType<MonumentMythosShatter>()))
            {
                info.Damage *= 2;
            }

            if (MonumentMythosPyramid)
            {
                MythosPyarmidHits++;
                if (MythosPyarmidHits == 5)
                {
                    SoundEngine.PlaySound(SoundID.Shatter, Player.Center);
                    
                    Player.AddBuff(ModContent.BuffType<MonumentMythosShatter>(), 600);
                    Player.AddBuff(ModContent.BuffType<MonumentMythosCooldown>(), 3600);

                    for (int numGores = 1; numGores <= 4; numGores++)
                    {
                        if (Main.netMode != NetmodeID.Server)
                        {
                            Gore.NewGore(Player.GetSource_OnHurt(info.DamageSource), Player.Center, Player.velocity, ModContent.Find<ModGore>("Spooky/GizaGlassGore" + numGores).Type);
                        }
                    }

                    MythosPyarmidHits = 0;
                }
            }
        }
    }
}