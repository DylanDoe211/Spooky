using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[LegacyName("SpookyHead")]
	[AutoloadEquip(EquipType.Head)]
	public class GourdHead : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 4;
			Item.width = 26;
			Item.height = 28;
			Item.rare = ItemRarityID.Blue;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<GourdBody>() && legs.type == ModContent.ItemType<GourdLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.GourdArmor");
			player.GetModPlayer<GourdArmorPlayer>().GourdSet = true;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetCritChance(DamageClass.Melee) += 10;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RottenChunk>(), 12)
			.AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 20)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}

	public class GourdArmorPlayer : ModPlayer
    {
		public bool GourdSet = false;

		public override void ResetEffects()
        {
            GourdSet = false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.active && target.CanBeChasedBy(this) && !target.friendly && !target.dontTakeDamage && !NPCID.Sets.CountsAsCritter[target.type])
            {
                //inflict enemies with gourd decay while wearing the rotten gourd armor
                if (GourdSet && Main.rand.NextBool(8))
                {
                    target.AddBuff(ModContent.BuffType<GourdDecay>(), Main.rand.Next(600, 1200));
                }
			}
		}
	}	
}