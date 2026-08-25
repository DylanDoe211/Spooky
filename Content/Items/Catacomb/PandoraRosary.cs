using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Items.Catacomb
{
    public class PandoraRosary : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<PandoraChalice>();
        }

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 38;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;  
            Item.value = Item.buyPrice(gold: 20);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<PandoraRosaryPlayer>().PandoraRosary = true;
        }
    }

    public class PandoraRosaryPlayer : ModPlayer
    {
        public bool PandoraRosary = false;
        public int RosaryHandTimer = 0;

        public override void ResetEffects()
        {
            PandoraRosary = false;
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (PandoraRosary && !Player.HasBuff(ModContent.BuffType<PandoraHandCooldown>()))
            {
                Player.AddBuff(ModContent.BuffType<PandoraHandCooldown>(), 720);

                for (int i = 0; i <= Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].type == ModContent.ProjectileType<PandoraRosaryHand>() && Main.projectile[i].owner == Player.whoAmI)
                    {
                        Main.projectile[i].ai[0] = 1;
                    }
                }
            }
        }

        public override void PreUpdate()
        {
            if (PandoraRosary && !Player.HasBuff(ModContent.BuffType<PandoraHandCooldown>()) && Player.ownedProjectileCounts[ModContent.ProjectileType<PandoraRosaryHand>()] < 5)
            {
                RosaryHandTimer++;

                if (RosaryHandTimer >= 325)
                {
                    Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, 0, 0,
                    ModContent.ProjectileType<PandoraRosaryHand>(), 0, 0f, Player.whoAmI, 0f, Main.rand.Next(0, 360));

                    RosaryHandTimer = 0;
                }
            }
            else
            {
                RosaryHandTimer = 0;
            }
        }
    }
}