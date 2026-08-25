using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Items.BossBags.Accessory;

namespace Spooky.Content.Items.Minibiomes.Christmas
{
    public class KrampusResolution : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 40;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;  
            Item.value = Item.buyPrice(gold: 1);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<KrampusResolutionPlayer>().KrampusResolution = true;
        }
    }

    public class KrampusResolutionPlayer : ModPlayer
    {
		public bool KrampusResolution = false;
        public int KrampusResolutionTimer = 0;

		public override void ResetEffects()
        {
			KrampusResolution = false;
		}
        
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            bool ShouldPlayerDie = true;

            if (Player.statLife <= 0)
			{
                if (KrampusResolution && KrampusResolutionTimer <= 0 && 
                (!Player.GetModPlayer<OrroboroEmbryoPlayer>().OrroboroEmbyro || Player.HasBuff(ModContent.BuffType<EmbryoCooldown>())))
                {
                    KrampusResolutionTimer = 300;
                    Player.immuneTime += 300;
                    Player.statLife = 1;
                    ShouldPlayerDie = false;
                }
            }

            return ShouldPlayerDie;
        }

        public override void PreUpdate()
        {
            if (KrampusResolutionTimer > 0)
            {
				KrampusResolutionTimer--;
                if (KrampusResolutionTimer == 1)
                {
					Player.KillMe(PlayerDeathReason.ByCustomReason(Language.GetText("Mods.Spooky.DeathReasons.KrampusResolution").ToNetworkText(Player.name)), 10, 0, false);    
                }
            }
			if (Player.dead)
			{
				KrampusResolutionTimer = 0;
			}
        }
    }
}