using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Shipyard;

namespace Spooky.Content.Items.Shipyard
{
    public class BleachedTuna : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
			Item.damage = 10;
			Item.mana = 5;
			Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
			Item.autoReuse = true;
			Item.width = 60;
            Item.height = 60;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 3);
			Item.UseSound = SoundID.Item111 with { Volume = 0.5f, Pitch = -0.5f };     
			Item.shoot = ModContent.ProjectileType<BleachedTunaBubble>();
			Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 55f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(12));
            
            Projectile.NewProjectile(source, position, newVelocity, type, damage, knockback, player.whoAmI, ai0: Main.rand.Next(0, 3));

            return false;
        }
    }
}