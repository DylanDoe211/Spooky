using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Projectiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave
{
    public class SpiderAntHead : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 12;
			Item.DamageType = DamageClass.Melee;
			Item.noMelee = true;
			Item.autoReuse = true;
            Item.width = 28;
            Item.height = 26;
            Item.useTime = 6;
			Item.useAnimation = 6;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 1;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 3);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<SpiderAntHeadSlash>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			int Slash = Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), ModContent.ProjectileType<SpiderAntHeadSlash>(), damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax);
			Main.projectile[Slash].scale *= Item.scale * (player.meleeScaleGlove ? 0.7f : 0.6f);

            return false;
		}
    }
}