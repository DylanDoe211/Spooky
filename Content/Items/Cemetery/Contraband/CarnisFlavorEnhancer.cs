using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Cemetery.Contraband
{
    public class CarnisFlavorEnhancer : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 40;
            Item.accessory = true;
            Item.rare = ItemRarityID.Orange;  
            Item.value = Item.buyPrice(gold: 30);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CarnisFlavorEnhancerPlayer>().CarnisFlavorEnhancer = true;
        }
    }

    public class CarnisFlavorEnhancerPlayer : ModPlayer
    {
        public bool CarnisFlavorEnhancer = false;
        public int CarnisSporeSpawnTimer = 0;

        public override void ResetEffects()
        {
            CarnisFlavorEnhancer = false;
        }

        public override void PreUpdate()
        {
            if (CarnisFlavorEnhancer)
            {
                CarnisSporeSpawnTimer++;

                if (SpookyPlayer.PlayerSpeedToMPH(Player) >= 10)
                {
                    CarnisSporeSpawnTimer++;

                    if (CarnisSporeSpawnTimer >= 30)
                    {
                        Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, 0, 0, ModContent.ProjectileType<FoodEnhancerSpore>(), 0, 0f, Player.whoAmI);
                        CarnisSporeSpawnTimer = 0;
                    }
                }
                else
                {
                    CarnisSporeSpawnTimer = 0;
                }
            }
            else
            {
                CarnisSporeSpawnTimer = 0;
            }
        }
    }
}