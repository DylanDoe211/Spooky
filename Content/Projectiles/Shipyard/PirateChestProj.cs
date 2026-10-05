using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Shipyard
{
    public class PirateChestProj : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 5;
            Projectile.alpha = 255;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void AI()
        {
            Flags.PirateChestSpawn = Projectile.Center;

            if (Projectile.timeLeft == 2)
            {
                if (Main.netMode != NetmodeID.SinglePlayer) 
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)SpookyMessageType.OpenPirateChest);
                    packet.Send();
                }
                else 
                {
                    NPC.NewNPC(Projectile.GetSource_FromAI(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<PirateChestVisuals>());
                }

                Projectile.netUpdate = true;
            }
        }
    }
}