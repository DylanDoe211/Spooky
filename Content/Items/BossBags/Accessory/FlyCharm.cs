using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Buffs;
using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.BossBags.Accessory
{
    [LegacyName("PumpkinCore")]
    public class FlyCharm : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.expert = true;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 10);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<FlyCharmPlayer>().FlyAmulet = true;
        }
    }

    public class FlyCharmPlayer : ModPlayer
    {
        public bool FlyAmulet = false;
        public int FlySpawnTimer = 0;

        public override void ResetEffects()
        {
            FlyAmulet = false;
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (FlyAmulet)
            {
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<SwarmFly>()] > 0)
                {
                    foreach (var Proj in Main.ActiveProjectiles)
				    {
                        if (Proj.owner == Player.whoAmI && Proj.type == ModContent.ProjectileType<SwarmFly>()) 
                        {
                            Proj.Kill();
                        }
                    }
                }
            }
        }

        public override void PreUpdate()
        {
            if (FlyAmulet)
            {
                //add the fly buff if the player has any flies around them
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<SwarmFly>()] > 0)
                {
                    Player.AddBuff(ModContent.BuffType<FlyBuff>(), 2);
                }

                //spawn flies
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<SwarmFly>()] < 10)
                {
                    FlySpawnTimer++;
                    if (FlySpawnTimer >= 240)
                    {
                        Vector2 randomVelocity = Vector2.UnitY.RotatedByRandom(1.5f) * new Vector2(5f, 3f);

                        Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, randomVelocity.X, 
                        randomVelocity.Y, ModContent.ProjectileType<SwarmFly>(), 0, 0f, Player.whoAmI);

                        FlySpawnTimer = 0;
                    }
                }
            }
            else
            {
                FlySpawnTimer = 0;
            }
        }
    }
}