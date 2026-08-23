using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome
{
    public class SpookFishronYoyo : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
        }

        public override void SetDefaults()
        {
			Item.damage = 100;
			Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.width = 42;
			Item.height = 44;
            Item.useTime = 22;
            Item.useAnimation = 22;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 25);
            Item.UseSound = SoundID.Item1;
            //Item.shoot = ModContent.ProjectileType<SpookFishronYoyoProj>();
            //Item.shootSpeed = 8f;
        }
    }
}