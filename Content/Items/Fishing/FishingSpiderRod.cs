using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Items.Fishing
{
    public class FishingSpiderRod : ModItem
    {
        public override void SetDefaults()
        {
            Item.fishingPole = 40;
            Item.width = 50;
            Item.height = 38;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 10);
            Item.shoot = ModContent.ProjectileType<FishingSpiderBobber>();
            Item.shootSpeed = 12f;
        }

        public override void ModifyFishingLine(Projectile bobber, ref Vector2 lineOriginOffset, ref Color lineColor)
        {
			lineOriginOffset = new Vector2(46, -30);
			lineColor = Color.White;
        }
    }
}