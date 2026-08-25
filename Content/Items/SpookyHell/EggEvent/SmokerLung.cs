using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameInput;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.EggEvent
{
    public class SmokerLung : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<StonedKidney>();
        }

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 52;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 30);
        }

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<SmokerLungPlayer>().SmokerLung = true;
		}
	}

    public class SmokerLungPlayer : ModPlayer
    {
		public bool SmokerLung = false;

        public override void ResetEffects()
        {
			SmokerLung = false;
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
				if (SmokerLung && !Player.HasBuff(ModContent.BuffType<SmokerLungCooldown>()))
				{
					SoundEngine.PlaySound(SoundID.NPCHit27 with { Pitch = -1f }, Player.Center);

					Projectile.NewProjectile(null, Player.Center, Vector2.Zero, ModContent.ProjectileType<CoughSmokeCloud>(), 50, 0f, Player.whoAmI);

					Player.AddBuff(ModContent.BuffType<SmokerLungCooldown>(), 3600);
				}
            }
        }
    }
}