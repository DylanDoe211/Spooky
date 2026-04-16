using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.SpiderCave
{
	public class PossessedCrownProj : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 24;
            Projectile.height = 26;
            Projectile.DamageType = DamageClass.Summon;
			Projectile.minion = true;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.timeLeft = 2;
            Projectile.minionSlots = 1;
            Projectile.penetrate = -1;
		}

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

		public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead || !player.active) 
            {
				player.ClearBuff(ModContent.BuffType<PossessedCrownBuff>());
			}

			if (player.HasBuff(ModContent.BuffType<PossessedCrownBuff>()))
            {
				Projectile.timeLeft = 2;
			}

            Projectile.frameCounter++;
			if (Projectile.frameCounter >= 8)
			{
				Projectile.frameCounter = 0;
				
                Projectile.frame++;
				if (Projectile.frame >= 10)
				{
					Projectile.frame = 0;
				}
			}

            Projectile.spriteDirection = player.direction;

            int AdditionalOffsetY = 10 * (int)Projectile.ai[1];

            Vector2 destination = new Vector2(player.Center.X, player.Center.Y - 30 - AdditionalOffsetY);
            Projectile.Center = destination;
        }
    }
}