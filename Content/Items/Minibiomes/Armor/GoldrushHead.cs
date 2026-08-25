using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

using Spooky.Core;

namespace Spooky.Content.Items.Minibiomes.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class GoldrushHead : ModItem
	{
		public override void SetStaticDefaults()
        {
			ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<GoldrushBody>();
		}

		public override void SetDefaults() 
		{
			Item.defense = 2;
			Item.width = 30;
			Item.height = 22;
			Item.rare = ItemRarityID.Blue;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<GoldrushBody>() && legs.type == ModContent.ItemType<GoldrushLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.GoldrushArmor");
			player.GetModPlayer<GoldrushArmorPlayer>().GoldrushSet = true;
		}

		public override void UpdateEquip(Player player) 
		{
			player.pickSpeed -= 0.1f;
		}
	}

	public class GoldrushArmorPlayer : ModPlayer
    {
		public bool GoldrushSet = false;

		public override void ResetEffects()
        {
			GoldrushSet = false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			//goldrush set inflicts midas forever
			if (GoldrushSet)
			{
				target.AddBuff(BuffID.Midas, int.MaxValue);
			}
		}
	}
}