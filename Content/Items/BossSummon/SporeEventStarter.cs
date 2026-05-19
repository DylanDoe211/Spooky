using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

using Spooky.Content.NPCs.Boss.Daffodil;
using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Items.BossSummon
{
    public class SporeEventStarter : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
        }
		
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 42;
			Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.consumable = true;
            Item.useTime = 35;
			Item.useAnimation = 35;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 12;
            Item.maxStack = 9999;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<SporeEventStarterProj>();
			Item.shootSpeed = 12f;
        }
    }
}