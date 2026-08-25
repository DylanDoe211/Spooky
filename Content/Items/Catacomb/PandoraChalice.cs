using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Items.Catacomb
{
    public class PandoraChalice : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<PandoraCross>();
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 38;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;  
            Item.value = Item.buyPrice(gold: 20);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<PandoraChalicePlayer>().PandoraChalice = true;
        }
    }

    public class PandoraChalicePlayer : ModPlayer
    {
        public bool PandoraChalice = false;

        public override void ResetEffects()
        {
            PandoraChalice = false;
        }

        public override void GetHealMana(Item item, bool quickHeal, ref int healValue)
        {
            if (PandoraChalice && Player.ownedProjectileCounts[ModContent.ProjectileType<PandoraChaliceOrb>()] < 6)
            {
                Projectile.NewProjectile(Player.GetSource_ItemUse(item), Player.Center, Vector2.Zero,
                ModContent.ProjectileType<PandoraChaliceOrb>(), healValue / 2, 0, Player.whoAmI);
            }
        }
    }
}