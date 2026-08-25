using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;

using Spooky.Core;
using Spooky.Content.Buffs;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Items.BossBags.Accessory
{
    public class OrroboroEmbryo : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 42;
            Item.expert = true;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 25);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.GetModPlayer<OrroboroEmbryoPlayer>().OrroboroEmbyro = true;
        }
    }

    public class OrroboroEmbryoPlayer : ModPlayer
    {
        public bool OrroboroEmbyro = false;

        public override void ResetEffects()
        {
            OrroboroEmbyro = false;
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            bool ShouldPlayerDie = true;

            if (Player.statLife <= 0)
			{
                if (OrroboroEmbyro && !Player.HasBuff(ModContent.BuffType<EmbryoCooldown>()))
                {
                    SoundEngine.PlaySound(SoundID.Item103, Player.Center);
                    Player.AddBuff(ModContent.BuffType<EmbryoRevival>(), 300);
                    Player.AddBuff(ModContent.BuffType<EmbryoCooldown>(), 18000);
                    Player.immuneTime += 60;
                    Player.statLife = 1;
                    ShouldPlayerDie = false;
                }
            }

            return ShouldPlayerDie;
        }
    }
}