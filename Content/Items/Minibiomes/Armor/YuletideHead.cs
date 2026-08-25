using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Projectiles.Minibiomes.Christmas;
using Spooky.Content.Tiles.Minibiomes.Christmas;

namespace Spooky.Content.Items.Minibiomes.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class YuletideHead : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 3;
			Item.width = 34;
			Item.height = 24;
			Item.rare = ItemRarityID.Blue;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<YuletideBody>() && legs.type == ModContent.ItemType<YuletideLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.YuletideArmor");
			player.GetModPlayer<YuletideArmorPlayer>().YuletideSet = true;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Magic) += 0.1f;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<ChristmasWoodItem>(), 20)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}

	public class YuletideArmorPlayer : ModPlayer
    {
		public bool YuletideSet = false;
		public int YuletideFireTimer = 0;
		public float YuletideFireAlpha = 0f;

		public override void ResetEffects()
        {
			YuletideSet = false;
		}

		public override void ArmorSetBonusActivated()
		{
			if (YuletideSet && YuletideFireTimer <= 0 && !Player.HasBuff(ModContent.BuffType<YuletideArmorCooldown>()))
			{
				YuletideFireTimer = 300;
			}
		}

		public override void PreUpdate()
        {
			//yuletide flaming timer and projectiles
			if (YuletideSet && YuletideFireTimer > 0)
			{
				YuletideFireTimer--;

				if (YuletideFireTimer % 10 == 0)
				{
					SoundEngine.PlaySound(SoundID.Item42, Player.Center);

					Vector2 velocity = new Vector2(0, Main.rand.Next(-5, -2)).RotatedByRandom(MathHelper.ToRadians(65));

					//400 - 402 are friendly greek fire projectiles
					Projectile.NewProjectile(null, Player.Top, velocity, Main.rand.Next(400, 403), 35, 0, Player.whoAmI);
				}

                if (YuletideFireTimer == 2)
                {
                    Player.AddBuff(ModContent.BuffType<YuletideArmorCooldown>(), 1200);
                }

				if (Main.rand.NextBool(10))
				{
					Color[] colors = new Color[] { Color.Gray, Color.DarkGray };

					int DustEffect = Dust.NewDust(Player.position, Player.width, 3, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Main.rand.Next(colors) * 0.5f, Main.rand.NextFloat(0.2f, 0.4f));
					Main.dust[DustEffect].velocity.X = 0;
					Main.dust[DustEffect].velocity.Y = -2;
					Main.dust[DustEffect].alpha = 100;
				}
			}
		}

		public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
			//yuletide fire colors
			if (YuletideSet && YuletideFireTimer > 0)
			{
				if (YuletideFireAlpha < 0.8f)
				{
					YuletideFireAlpha += 0.02f;
				}
			}
			else
			{
				if (YuletideFireAlpha > 0f)
				{
					YuletideFireAlpha -= 0.02f;
				}
			}

			if (YuletideFireAlpha > 0f)
			{
				r *= 1f - (YuletideFireAlpha * 0.15f);
				g *= 1f - (YuletideFireAlpha * 0.75f);
				b *= 1f - (YuletideFireAlpha * 1f);
			}
		}
	}
}