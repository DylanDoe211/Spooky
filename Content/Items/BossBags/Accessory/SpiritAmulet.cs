using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.BossBags.Accessory
{
    public class SpiritAmulet : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.expert = true;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 10);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SpiritAmuletPlayer>().SpiritAmulet = true;
        }
    }

    public class SpiritAmuletPlayer : ModPlayer
    {
        public bool SpiritAmulet = false;

        public override void ResetEffects()
        {
            SpiritAmulet = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (SpiritAmulet && Main.rand.NextBool(10) && Player.ownedProjectileCounts[ModContent.ProjectileType<AmuletGhost>()] < 5 && !target.friendly && !target.dontTakeDamage && !NPCID.Sets.CountsAsCritter[target.type])
            {
                int RealDamage = damageDone < 20 ? 20 : damageDone;
				Projectile.NewProjectile(target.GetSource_OnHurt(Player), Player.Center, Vector2.Zero, ModContent.ProjectileType<AmuletGhost>(), RealDamage, 0, ai2: Main.rand.Next(0, 6));
            }
        }
    }
}