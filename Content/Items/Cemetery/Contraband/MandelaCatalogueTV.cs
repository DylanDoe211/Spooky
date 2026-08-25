using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameInput;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Cemetery.Contraband
{
    public class MandelaCatalogueTV : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 44;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;  
            Item.value = Item.buyPrice(gold: 20);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<MandelaCatalogueTVPlayer>().MandelaCatalogueTV = true;
        }
    }

    public class MandelaCatalogueTVPlayer : ModPlayer
    {
        public bool MandelaCatalogueTV = false;

        public override void ResetEffects()
        {
            MandelaCatalogueTV = false;
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
                if (MandelaCatalogueTV && !Player.HasBuff(ModContent.BuffType<AlternateCooldown>()))
                {
                    Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, 0f, 0f, ModContent.ProjectileType<Alternate>(), 0, 0f, Player.whoAmI, 0f, 0f);
                    Player.AddBuff(ModContent.BuffType<AlternateCooldown>(), 3600);
                }
            }
        }
    }
}