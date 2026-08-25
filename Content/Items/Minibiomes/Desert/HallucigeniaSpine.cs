using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Minibiomes.Desert;

namespace Spooky.Content.Items.Minibiomes.Desert
{
	public class HallucigeniaSpine : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 42;
			Item.value = Item.buyPrice(gold: 15);
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.GetModPlayer<HallucigeniaSpinePlayer>().HallucigeniaSpine = true;
        }
	}

	public class HallucigeniaSpinePlayer : ModPlayer
    {
		public bool HallucigeniaSpine = false;

		public override void ResetEffects()
        {
			HallucigeniaSpine = false;
		}

		public override void OnHurt(Player.HurtInfo info)
        {
            if (HallucigeniaSpine)
            {
                int[] Types = { ModContent.ProjectileType<HallucigeniaSpineProj1>(), ModContent.ProjectileType<HallucigeniaSpineProj2>() };

                int MinDamage = 40; //minimum damage
				float Damage = (info.Damage / 2) < MinDamage ? MinDamage : info.Damage / 2;

				float maxAmount = 3;
				int currentAmount = 0;

                float RandomRotation = MathHelper.ToRadians(Main.rand.NextFloat(0f, 360f));

				while (currentAmount < maxAmount)
				{
					Vector2 velocity = new Vector2(3f, 3f);
					Vector2 Bounds = new Vector2(3f, 3f);
					float intensity = 3f;

					Vector2 vector12 = Vector2.UnitX * 0f;
					vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
					vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
					Vector2 ShootVelocity = (velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity).RotatedBy(RandomRotation);

					Projectile.NewProjectile(Player.GetSource_OnHurt(info.DamageSource), Player.Center, ShootVelocity, Main.rand.Next(Types), (int)Damage, 4.5f, Player.whoAmI);

					currentAmount++;
				}
            }
		}
	}
}