using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Items.Minibiomes.Vegetable;
using Spooky.Content.Projectiles.Minibiomes.Vegetable;

namespace Spooky.Content.Items.Minibiomes.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class BroccoliHead : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 3;
			Item.width = 26;
			Item.height = 28;
			Item.rare = ItemRarityID.Blue;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<BroccoliBody>() && legs.type == ModContent.ItemType<BroccoliLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.BroccoliArmor");
			player.GetModPlayer<BroccoliArmorPlayer>().BroccoliSet = true;
		}

		public override void UpdateEquip(Player player) 
		{
			player.maxMinions += 1;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<PlantMulch>(), 15)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}

	public class BroccoliArmorPlayer : ModPlayer
    {
		public bool BroccoliSet = false;

		public override void ResetEffects()
        {
			BroccoliSet = false;
		}

		public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
		{
			//broccoli armor spawns broccolis on enemies when hit by summons
			if (BroccoliSet && (proj.minion || ProjectileID.Sets.MinionShot[proj.type]) && proj.type != ModContent.ProjectileType<GrowingBroccoli>() && Main.rand.NextBool(5))
			{
				Vector2 projPos = target.Center + new Vector2(1, 0).RotatedByRandom(360);

				Vector2 Direction = target.Center - projPos;
				Direction.Normalize();

				Vector2 lineDirection = new Vector2(Direction.X, Direction.Y);

				Projectile.NewProjectile(target.GetSource_OnHurt(Player), target.Center, Vector2.Zero,
				ModContent.ProjectileType<GrowingBroccoli>(), proj.damage, 0, Player.whoAmI, ai0: lineDirection.ToRotation() + MathHelper.Pi, ai2: target.whoAmI);
			}
		}
	}
}