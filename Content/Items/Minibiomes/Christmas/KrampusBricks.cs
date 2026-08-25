using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameInput;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Projectiles.Minibiomes.Christmas;

namespace Spooky.Content.Items.Minibiomes.Christmas
{
    public class KrampusBricks : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 44;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;  
            Item.value = Item.buyPrice(gold: 1);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<KrampusBricksPlayer>().KrampusBricks = true;
        }
    }

    public class KrampusBricksPlayer : ModPlayer
    {
		public bool KrampusBricks = false;

		public override void ResetEffects()
        {
			KrampusBricks = false;
		}

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            //do not allow hotkeys to do anything if you are dead
            if (Player.dead)
            {
                return;
            }

            if (Spooky.AccessoryHotkey.JustPressed && Main.myPlayer == Player.whoAmI)
            {
				if (KrampusBricks && !Player.HasBuff(ModContent.BuffType<KrampusBricksCooldown>()))
				{
                    for (int repeats = 0; repeats <= 1; repeats++)
                    {
                        for (int numProjectiles = -3; numProjectiles <= 3; numProjectiles++)
                        {
                            Projectile.NewProjectile(null, Player.Center, new Vector2(numProjectiles * 2, Main.rand.NextFloat(-15f, -7f)),
                            ModContent.ProjectileType<KrampusBricksProj>(), 25, 0f, Player.whoAmI, ai1: Main.rand.Next(0, 4));
                        }
                    }

					Player.AddBuff(ModContent.BuffType<KrampusBricksCooldown>(), 900);
				}
            }
        }
	}
}