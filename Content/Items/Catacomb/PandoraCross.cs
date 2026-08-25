using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameInput;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Items.Catacomb
{
    public class PandoraCross : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<PandoraCuffs>();
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;  
            Item.value = Item.buyPrice(gold: 20);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<PandoraCrossPlayer>().PandoraCross = true;
        }
    }

    public class PandoraCrossPlayer : ModPlayer
    {
        public bool PandoraCross = false;
        public int CrossSoundTimer = 0;

        public static readonly SoundStyle CrossBassSound = new("Spooky/Content/Sounds/CrossBass", SoundType.Sound) { Volume = 0.7f };

        public override void ResetEffects()
        {
            PandoraCross = false;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            //do not allow hotkeys to do anything if you are dead
            if (Player.dead)
            {
                return;
            }

            //handle everything when the accessory hotkey is pressed
            if (Spooky.AccessoryHotkey.JustPressed && Main.myPlayer == Player.whoAmI)
            {
                if (PandoraCross && !Player.HasBuff(ModContent.BuffType<PandoraCrossCooldown>()))
                {
                    SoundEngine.PlaySound(CrossBassSound, Player.Center);

                    CrossSoundTimer = 300;
                    Player.AddBuff(ModContent.BuffType<PandoraCrossCooldown>(), 2400);
                }
            }
        }

        public override void PreUpdate()
        {
            if (CrossSoundTimer > 0)
            {
                CrossSoundTimer--;

                if (CrossSoundTimer % 12 == 2)
                {
                    Projectile.NewProjectile(null, Player.Center, Vector2.Zero, ModContent.ProjectileType<PandoraCrossSound>(), 50, 0f, Player.whoAmI);
                }
            }
        }
    }
}