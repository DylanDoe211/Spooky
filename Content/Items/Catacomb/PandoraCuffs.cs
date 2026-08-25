using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Items.Catacomb
{
    public class PandoraCuffs : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<PandoraRosary>();
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 46;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;  
            Item.value = Item.buyPrice(gold: 20);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<PandoraCuffsPlayer>().PandoraCuffs = true;
        }
    }

    public class PandoraCuffsPlayer : ModPlayer
    {
        public bool PandoraCuffs = false;
        public int PandoraCuffTimer = 0;

        public override void ResetEffects()
        {
            PandoraCuffs = false;
        }

        public override void PreUpdate()
        {
            if (PandoraCuffs && Player.ownedProjectileCounts[ModContent.ProjectileType<PandoraCuffProj>()] < 1)
            {
                foreach (var NPC in Main.ActiveNPCs)
                {
                    if (!NPC.friendly && !NPC.immortal && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Player.Center, NPC.Center) <= 450f)
                    {
                        PandoraCuffTimer++;
                        if (PandoraCuffTimer == 900)
                        {
                            //prioritize bosses over normal enemies
                            if (NPC.boss)
                            {
                                Projectile.NewProjectile(null, Player.Center, Vector2.Zero, ModContent.ProjectileType<PandoraCuffProj>(), 0, 0f, Player.whoAmI, NPC.whoAmI);
                                break;
                            }
                            else
                            {
                                Projectile.NewProjectile(null, Player.Center, Vector2.Zero, ModContent.ProjectileType<PandoraCuffProj>(), 0, 0f, Player.whoAmI, NPC.whoAmI);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                PandoraCuffTimer = 0;
            }
        }
    }
}