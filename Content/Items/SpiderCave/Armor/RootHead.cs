using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

using Spooky.Core;
using Spooky.Content.Tiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class RootHead : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 3;
			Item.width = 24;
			Item.height = 32;
			Item.rare = ItemRarityID.White;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<RootBody>() && legs.type == ModContent.ItemType<RootLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.RootArmor");
			player.GetModPlayer<RootArmorPlayer>().RootSet = true;
		}

		public override void UpdateEquip(Player player) 
		{
            player.GetCritChance(DamageClass.Ranged) += 5;
        }

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RootWoodItem>(), 20)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}

	public class RootArmorPlayer : ModPlayer
    {
        public bool RootSet = false;
		public int RootHealCooldown = 0;

		public override void ResetEffects()
        {
            RootSet = false;
		}

		public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
		{
            if (RootSet && RootHealCooldown <= 0 && proj.DamageType == DamageClass.Ranged && damageDone >= 2)
            {
                if (Main.rand.NextBool(4))
                {
                    //heal based on how much damage was done, with a maximum of 5 health healed
                    int LifeHealed = damageDone > 10 ? 5 : damageDone / 2;
					Player.statLife += LifeHealed;
					Player.HealEffect(LifeHealed, true);
					RootHealCooldown = 300;
                }
            }
		}

		public override void PreUpdate()
        {
            if (RootHealCooldown > 0)
            {
                RootHealCooldown--;
            }
		}
	}
}