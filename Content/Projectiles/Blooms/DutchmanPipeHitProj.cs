using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Items;

namespace Spooky.Content.Projectiles.Blooms
{
    public class DutchmanPipeHitProj : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.alpha = 255;
        }

        public override bool? CanDamage()
        {
			return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }
    
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            NPC Parent = Main.npc[(int)Projectile.ai[0]];

            if (Projectile.ai[1] == 0)
            {
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.whoAmI != Projectile.ai[0] && npc.Distance(Projectile.Center) <= 240f && !npc.friendly && !npc.dontTakeDamage && !NPCID.Sets.CountsAsCritter[npc.type])
                    {
                        //damage enemies
                        player.ApplyDamageToNPC(npc, Projectile.damage, 0, 0, false, null, true);
                    }
                }

                Projectile.ai[1]++;
            }
        }
    }
}