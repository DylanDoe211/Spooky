using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Items.Minibiomes.Vegetable;
using Spooky.Content.Projectiles.Minibiomes.Vegetable;

namespace Spooky.Content.Items.Minibiomes.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class HazmatHead : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 10;
			Item.width = 26;
			Item.height = 30;
			Item.rare = ItemRarityID.Pink;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<HazmatBody>() && legs.type == ModContent.ItemType<HazmatLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.HazmatArmor");
			player.GetModPlayer<HazmatArmorPlayer>().HazmatSet = true;

			int MaxMinions = 0;
			foreach (string var in player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots)
			{
				if (var != string.Empty)
				{
					MaxMinions++;
				}
			}

			if (player.ownedProjectileCounts[ModContent.ProjectileType<HazmatArmorMinion>()] < MaxMinions)
			{
				Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<HazmatArmorMinion>(), 60, 0f, player.whoAmI);
			}
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetModPlayer<HazmatArmorPlayer>().HazmatMinionCrit = true;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
			.AddIngredient(ItemID.HallowedBar, 12)
            .AddIngredient(ModContent.ItemType<PlantMulch>(), 18)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
	}

	public class HazmatArmorPlayer : ModPlayer
    {
        public bool HazmatSet = false;
		public bool HazmatMinionCrit = false;
		public bool DrawHazmatBack = false;

		private static Asset<Texture2D> HazmatArmorBackTex;

		public override void ResetEffects()
        {
            HazmatSet = false;
			HazmatMinionCrit = false;
			DrawHazmatBack = false;
		}

		public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
		{
			if (DrawHazmatBack)
            {
				if (!drawInfo.drawPlayer.dead && !drawInfo.drawPlayer.frozen && drawInfo.shadow == 0f)
				{
					HazmatArmorBackTex ??= ModContent.Request<Texture2D>("Spooky/Content/Items/Minibiomes/Armor/HazmatBackpack");

					SpriteEffects spriteEffects = drawInfo.drawPlayer.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
				
					int xOffset = 5;

					DrawData PlayerBack = new DrawData(HazmatArmorBackTex.Value,
					new Vector2((int)(drawInfo.drawPlayer.MountedCenter.X - Main.screenPosition.X - (xOffset * drawInfo.drawPlayer.direction)) - 4f * drawInfo.drawPlayer.direction, (int)(drawInfo.drawPlayer.MountedCenter.Y - Main.screenPosition.Y + 2f * drawInfo.drawPlayer.gravDir - 8f * drawInfo.drawPlayer.gravDir + drawInfo.drawPlayer.gfxOffY)),
					new Rectangle(0, 0, HazmatArmorBackTex.Width(), HazmatArmorBackTex.Height()),
					drawInfo.colorArmorBody,
					drawInfo.drawPlayer.bodyRotation,
					new Vector2(HazmatArmorBackTex.Width() / 2, HazmatArmorBackTex.Height() / 2),
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