using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class MortarHead : ModItem
	{
		public override void SetStaticDefaults()
        {
			ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<MortarBody>();
		}

		public override void SetDefaults() 
		{
			Item.defense = 20;
			Item.width = 18;
			Item.height = 22;
			Item.rare = ItemRarityID.Yellow;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<MortarBody>() && legs.type == ModContent.ItemType<MortarLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.MortarArmor");
			player.GetModPlayer<MortarArmorPlayer>().MortarSet = true;
		}

		public override void UpdateEquip(Player player) 
		{
            player.GetCritChance(DamageClass.Generic) += 10;
			player.manaCost -= 0.08f;
			player.endurance += 0.02f;
        }
	}

	public class MortarArmorPlayer : ModPlayer
    {
        public bool MortarSet = false;

		public override void ResetEffects()
        {
            MortarSet = false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			if (MortarSet && Main.rand.NextBool(5) && Player.ownedProjectileCounts[ModContent.ProjectileType<MortarArmorRocket>()] < 5)
			{
				SoundEngine.PlaySound(SoundID.Item42 with { Volume = 0.5f }, Player.Center);
				SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { Volume = 0.5f }, Player.Center);

				Vector2 Velocity = new Vector2(0, Main.rand.Next(-18, -7)).RotatedByRandom(MathHelper.ToRadians(40));

				int RealDamage = damageDone < 50 ? 50 : damageDone;

				Projectile.NewProjectile(target.GetSource_OnHurt(Player), Player.Center, Velocity, ModContent.ProjectileType<MortarArmorRocket>(), RealDamage, 0, Player.whoAmI);
			}
		}
	}
}