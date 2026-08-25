using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Cemetery.Contraband
{
    public class RedMistClarinet : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 58;
            Item.accessory = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 30);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<RedMistClarinetPlayer>().RedMistClarinet = true;
        }
    }

    public class RedMistClarinetPlayer : ModPlayer
    {
        public bool RedMistClarinet = false;

        public static readonly SoundStyle ClarinetSound = new("Spooky/Content/Sounds/Clarinet", SoundType.Sound) { Volume = 0.7f, PitchVariance = 0.6f };

        public override void ResetEffects()
        {
            RedMistClarinet = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (RedMistClarinet && Main.rand.NextBool() && hit.Crit && Player.ownedProjectileCounts[ModContent.ProjectileType<RedMistNote>()] < 5 && !target.friendly && !target.dontTakeDamage && !NPCID.Sets.CountsAsCritter[target.type])
            {
                SoundEngine.PlaySound(ClarinetSound, Player.Center);

                //dont cap the damage if the player has the combined creepypasta accessory
                int damage = Player.GetModPlayer<CreepyPastaPlayer>().CreepyPasta ? hit.Damage : (hit.Damage >= 70 ? 70 : hit.Damage);

                Projectile.NewProjectile(target.GetSource_OnHit(target), Player.Center, Vector2.Zero, ModContent.ProjectileType<RedMistNote>(), damage, hit.Knockback, Player.whoAmI, 0, 0, Main.rand.Next(0, 2));
            }
        }
    }
}