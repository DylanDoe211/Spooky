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
using Spooky.Content.Projectiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class SporeShroomHead : ModItem
	{
		private static Asset<Texture2D> HeadTexture;

		public override void Load()
		{
			HeadTexture = ModContent.Request<Texture2D>(Texture + "Head");
		}

		public override void SetStaticDefaults()
        {
			ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SporeShroomBody>();
		}

		public override void SetDefaults() 
		{
			Item.defense = 12;
			Item.width = 24;
			Item.height = 32;
			Item.rare = ItemRarityID.LightRed;
		}

		public override bool ModifyEquipTextureDraw(ref PlayerDrawSet drawInfo, ref DrawData drawData, EquipTexture equipTexture, string methodName)
		{
			//manually add drawdata here first so the actual helmet head texture draws
			drawInfo.DrawDataCache.Add(drawData);

			Rectangle TextureRectangle = new(0, 0, 46, 97);
			Vector2 origin = TextureRectangle.Size() / 2f;

			Player player = drawInfo.drawPlayer;
			float yOffset = Main.OffsetsPlayerHeadgear[player.bodyFrame.Y / player.bodyFrame.Height].Y * player.gravDir;
			Vector2 offset = new Vector2(3 * player.direction, (player.gravDir == -1 ? 11 : 0) + yOffset);
			drawInfo.DrawDataCache.Add(drawData with { texture = HeadTexture.Value, position = drawData.position + offset, sourceRect = TextureRectangle, origin = origin });

			return false;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<SporeShroomBody>() && legs.type == ModContent.ItemType<SporeShroomLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			string TapKey = Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN");
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.SporeShroomArmor", TapKey);
			player.GetModPlayer<SporeShroomArmorPlayer>().SporeShroomSet = true;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetCritChance(DamageClass.Generic) += 10;
        }
	}

	public class SporeShroomArmorPlayer : ModPlayer
    {
        public bool SporeShroomSet = false;

		public override void ResetEffects()
        {
            SporeShroomSet = false;
		}

		public override void ArmorSetBonusActivated()
		{
            if (SporeShroomSet && Player.ownedProjectileCounts[ModContent.ProjectileType<SporeShroom>()] < 3)
            {
                SoundEngine.PlaySound(SoundID.Item112 with { Pitch = 0.5f }, Player.Center);

                int Mushroom = Projectile.NewProjectile(null, Player.Bottom, new Vector2(0, -10), ModContent.ProjectileType<SporeShroom>(), 65, 0f, Player.whoAmI, ai0: Main.rand.Next(0, 6));
				Main.projectile[Mushroom].scale = 0f;
            }
		}
	}
}