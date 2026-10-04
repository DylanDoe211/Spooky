using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Projectiles.Shipyard;
 
namespace Spooky.Content.Items.Shipyard
{
	public class PirateBlunderbuss : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 20;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 46;
			Item.height = 26;
			Item.useTime = 60;
			Item.useAnimation = 60;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 1;
			Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.Item1;
			Item.shoot = ModContent.ProjectileType<PirateBlunderbussProj>();
			Item.useAmmo = AmmoID.Bullet;
			Item.shootSpeed = 50f;
		}

		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<PirateBlunderbussProj>()] < 1;
		}
		
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position.X, position.Y, 0, 0, ModContent.ProjectileType<PirateBlunderbussProj>(), damage, knockback, player.whoAmI);

			return false;
		}
	}

	public class BlunderbussDrawPlayer : ModPlayer
	{
		private static Asset<Texture2D> BlunderbussBackTex;

		public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
		{
			if (drawInfo.shadow != 0f)
			{
				return;
			}

			if (!drawInfo.drawPlayer.frozen && !drawInfo.drawPlayer.dead && !drawInfo.drawPlayer.wet)
			{
                SpriteEffects spriteEffects = drawInfo.drawPlayer.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                if (ItemGlobal.ActiveItem(drawInfo.drawPlayer).type == ModContent.ItemType<PirateBlunderbuss>() && drawInfo.drawPlayer.ownedProjectileCounts[ModContent.ProjectileType<PirateBlunderbussProj>()] > 0)
                {
                    BlunderbussBackTex ??= ModContent.Request<Texture2D>("Spooky/Content/Items/Shipyard/PirateBlunderbussBack");

                    int xOffset = 10;

                    DrawData PlayerBack = new DrawData(BlunderbussBackTex.Value,
					new Vector2((int)(drawInfo.drawPlayer.MountedCenter.X - Main.screenPosition.X - (xOffset * drawInfo.drawPlayer.direction)) - 4f * drawInfo.drawPlayer.direction, (int)(drawInfo.drawPlayer.MountedCenter.Y - Main.screenPosition.Y + 2f * drawInfo.drawPlayer.gravDir - 8f * drawInfo.drawPlayer.gravDir + drawInfo.drawPlayer.gfxOffY)),
					new Rectangle(0, 0, BlunderbussBackTex.Width(), BlunderbussBackTex.Height()),
                    drawInfo.colorArmorBody,
                    drawInfo.drawPlayer.bodyRotation,
                    new Vector2(BlunderbussBackTex.Width() / 2, BlunderbussBackTex.Height() / 2),
                    1f, 
                    spriteEffects, 
                    0);

                    PlayerBack.shader = 0;
                    drawInfo.DrawDataCache.Add(PlayerBack);
                }
			}
		}
	}
}
