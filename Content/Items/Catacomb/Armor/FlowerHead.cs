using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Items.Catacomb.Misc;
using Spooky.Content.Projectiles.Catacomb;
using Humanizer;

namespace Spooky.Content.Items.Catacomb.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class FlowerHead : ModItem
	{
		private static Asset<Texture2D> HeadTexture;

		public override void Load()
		{
			HeadTexture = ModContent.Request<Texture2D>(Texture + "RealHead");
		}

		public override void SetDefaults() 
		{
			Item.defense = 8;
			Item.width = 38;
			Item.height = 36;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override bool ModifyEquipTextureDraw(ref PlayerDrawSet drawInfo, ref DrawData drawData, EquipTexture equipTexture, string methodName)
		{
			Rectangle TextureRectangle = new(0, 0, 38, 53);
			Vector2 origin = TextureRectangle.Size() / 2f;

			Player player = drawInfo.drawPlayer;
			float yOffset = Main.OffsetsPlayerHeadgear[player.bodyFrame.Y / player.bodyFrame.Height].Y * player.gravDir;
			Vector2 offset = new Vector2(-1 * player.direction, (player.gravDir == -1 ? 11 : 0) + yOffset);
			drawInfo.DrawDataCache.Add(drawData with { texture = HeadTexture.Value, position = drawData.position + offset, sourceRect = TextureRectangle, origin = origin });

			return false;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<FlowerBody>() && legs.type == ModContent.ItemType<FlowerLegs>();
		}

        public override void UpdateArmorSet(Player player) 
		{
			string TapKey = Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN");
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.FlowerArmor", TapKey);
            player.GetModPlayer<FlowerArmorPlayer>().FlowerArmorSet = true;
            player.lifeRegen += 5;
		}

		public override void UpdateEquip(Player player)
        {
			player.manaCost -= 0.15f;
			player.maxMinions += 2;
			player.maxTurrets += 1;
        }

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<PlantChunk>(), 15)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }

	public class FlowerArmorPlayer : ModPlayer
    {
        public bool FlowerArmorSet = false;

		public override void ResetEffects()
        {
            FlowerArmorSet = false;
		}

		public override void ArmorSetBonusActivated()
		{
			//flower armor setbonus
			if (FlowerArmorSet && !Player.HasBuff(ModContent.BuffType<FlowerArmorCooldown>()))
			{
				SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, Player.Center);

				for (int numProjectiles = 0; numProjectiles < 12; numProjectiles++)
				{
					Projectile.NewProjectile(null, Player.Center.X + Main.rand.Next(-30, 30),
					Player.Center.Y + Main.rand.Next(-30, 30), 0, 0, ModContent.ProjectileType<FlowerArmorPollen>(), 55, 2f, Player.whoAmI);
				}

				Player.AddBuff(ModContent.BuffType<FlowerArmorCooldown>(), 900);
			}
		}
	}
}