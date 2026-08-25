using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome
{
    public class CandyBag : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;  
            Item.value = Item.buyPrice(gold: 1);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<CandyBagPlayer>().CandyBag = true;
            
            bool NotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<CandyBagProj>()] <= 0;
			if (NotSpawned && player.whoAmI == Main.myPlayer)
			{
				//leave the source as null for right now
				Projectile.NewProjectile(null, player.position.X + (player.width / 2), player.position.Y + (player.height / 2), 0f, 0f, ModContent.ProjectileType<CandyBagProj>(), 18, 2f, player.whoAmI);
			}
        }
    }

    public class CandyBagPlayer : ModPlayer
    {
        public bool CandyBag = false;
		public bool CandyBagJustHit = false;
        public int CandyBagCooldown = 0;

        public override void ResetEffects()
        {
            CandyBag = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (CandyBag && (hit.DamageType == DamageClass.Summon || hit.DamageType == DamageClass.SummonMeleeSpeed))
            { 
                CandyBagJustHit = true;
            }
        }

        public override void PreUpdate()
        {
            if (!CandyBag)
			{
				CandyBagJustHit = false;
			}

            if (CandyBagCooldown > 0)
            {
                CandyBagCooldown--;
            }
        }
    }
}