using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave
{
    public class GlaggleOrb : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.mana = 25;
			Item.DamageType = DamageClass.Magic;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.width = 40;
            Item.height = 42;
            Item.useTime = 60;
			Item.useAnimation = 60;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 3);
            Item.UseSound = SoundID.Item20 with { Pitch = -0.5f };
            Item.shoot = ModContent.ProjectileType<GlaggleOrbProj>();
            Item.shootSpeed = 12f;
            Item.scale = 0.75f;
        }

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(0, -3);
		}
    }
}