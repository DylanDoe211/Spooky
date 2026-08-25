using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameInput;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Cemetery.Contraband
{
    public class HerobrineAltar : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 36;
            Item.accessory = true;
            Item.rare = ItemRarityID.Cyan;  
            Item.value = Item.buyPrice(gold: 60);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<HerobrineAltarPlayer>().HerobrineAltar = true;
        }
    }

    public class HerobrineAltarPlayer : ModPlayer
    {
        public bool HerobrineAltar = false;

        public override void ResetEffects()
        {
            HerobrineAltar = false;
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
                if (HerobrineAltar && !Player.HasBuff(ModContent.BuffType<HerobrineAltarCooldown>()))
                {
                    SoundEngine.PlaySound(SoundID.Thunder with { Pitch = -0.5f }, Player.Center);

                    Screenshake.ShakeScreenWithIntensity(Player.Center, 10f, 100f);

                    Vector2 ShootSpeed = new Vector2(Player.Center.X, Player.Center.Y - Main.screenHeight) - Main.MouseWorld;
                    ShootSpeed.Normalize();
                    ShootSpeed *= -100f;

                    //each lighting bolt is set to deal 1 damage, the actual final damage is handled in the projectile itself
                    Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y - Main.screenHeight, ShootSpeed.X, ShootSpeed.Y,
                    ModContent.ProjectileType<HerobrineLightning>(), 1, 0f, Player.whoAmI, ShootSpeed.ToRotation(), 100);

                    Main.NewLightning();

                    Player.AddBuff(ModContent.BuffType<HerobrineAltarCooldown>(), 7200);
                }
            }
        }
    }
}