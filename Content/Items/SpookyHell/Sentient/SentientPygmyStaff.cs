using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Buffs.Minion;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientPygmyStaff : ModItem, ICauldronOutput
    {
        int numUses = 0;

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.mana = 30;
			Item.DamageType = DamageClass.Summon;
            Item.autoReuse = true;
			Item.noMelee = true;
			Item.width = 48;
			Item.height = 54;
            Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 45);
            Item.UseSound = SoundID.Item79;
            Item.buffType = ModContent.BuffType<GoblinoidBuff>();
			Item.shoot = ModContent.ProjectileType<SentientGoblinFlyMouth>();
            Item.shootSpeed = 0f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            Vector2 offset = Vector2.Zero;

            switch (numUses)
            {
                case 0:
                {
                    //type = ModContent.ProjectileType<SentientGoblinTiny>(); //ModContent.ProjectileType<SentientGoblinFlyMouth>();
                    //numUses++;
                    break;
                }
                case 1:
                {
                    type = ModContent.ProjectileType<SentientGoblinSpit>();
                    offset = new Vector2(0, -15);
                    numUses++;
                    break;
                }
                case 2:
                {
                    type = ModContent.ProjectileType<SentientGoblinTiny>();
                    numUses++;
                    break;
                }
                case 3:
                {
                    //type = ModContent.ProjectileType<SentientGoblinFlyEye>();
                    numUses = 0;
                    break;
                }
            }

            player.AddBuff(Item.buffType, 2);

            var projectile = Projectile.NewProjectileDirect(source, position + offset, velocity, type, damage, knockback, player.whoAmI);
            projectile.originalDamage = Item.damage;

			return false;
		}
    }
}