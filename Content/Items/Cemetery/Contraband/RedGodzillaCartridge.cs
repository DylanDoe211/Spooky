using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Cemetery.Contraband
{
    public class RedGodzillaCartridge : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 44;
            Item.accessory = true;
            Item.rare = ItemRarityID.Lime;  
            Item.value = Item.buyPrice(gold: 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<RedGodzillaCartridgePlayer>().RedGodzillaCartridge = true;
        }
    }

    public class RedGodzillaCartridgePlayer : ModPlayer
    {
        public bool RedGodzillaCartridge = false;
        public int RedGodzillaCartridgeHits = 0;

        public override void ResetEffects()
        {
            RedGodzillaCartridge = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (RedGodzillaCartridge && !target.friendly && !target.dontTakeDamage && !NPCID.Sets.CountsAsCritter[target.type])
            {
                RedGodzillaCartridgeHits++;

                if (RedGodzillaCartridgeHits >= 25)
                {
                    RedGodzillaCartridgeHits = 0;

                    //dont spawn a red apparition if one already exists
                    if (Player.ownedProjectileCounts[ModContent.ProjectileType<RedFace>()] <= 0)
                    {
                        Vector2 SpawnPosition = target.Center + new Vector2(0, 85).RotatedByRandom(360);

                        Projectile.NewProjectile(target.GetSource_OnHit(target), SpawnPosition, Vector2.Zero, ModContent.ProjectileType<RedFace>(), damageDone * 5, hit.Knockback, Player.whoAmI, 0, target.whoAmI);
                    }
                }
            }
        }

        public override void PreUpdate()
        {
            if (!RedGodzillaCartridge)
            {
                RedGodzillaCartridgeHits = 0;
            }
        }
    }
}