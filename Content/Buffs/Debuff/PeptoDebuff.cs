using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Buffs.Debuff
{
	public class PeptoDebuff : ModBuff
	{
		public override string Texture => "Spooky/Content/Buffs/Debuff/DebuffPlaceholder";

		private bool initializeStats;
        Color storedColor;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            BuffGlobal.IsSpookyDebuffForAchievement[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (!initializeStats && npc.buffTime[buffIndex] >= 5)
            {
                npc.damage = (int)(npc.damage * 0.8f);
                npc.defense = (int)(npc.defense * 0.75f);
                storedColor = npc.color;

                initializeStats = true;
            }

            if (npc.buffTime[buffIndex] == 5)
            {
                Player player = Main.LocalPlayer;
                for (int numProjectiles = 0; numProjectiles <= 5; numProjectiles++)
				{
					Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(Main.rand.NextFloat(-12f, 12f), -3f), ModContent.ProjectileType<PeptoBubble>(), npc.damage, 0, player.whoAmI);
				}
            }

            if (npc.buffTime[buffIndex] < 5)
            {
                npc.color = storedColor;
				npc.buffTime[buffIndex] = 0;
            }
            else
            {
                Color color = npc.GetAlpha(Color.DeepPink);
                npc.color = color;
            }
        }
    }
}
