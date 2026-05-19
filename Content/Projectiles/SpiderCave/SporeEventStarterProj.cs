using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;
using Spooky.Content.Biomes;
using Spooky.Content.Dusts;
using Spooky.Content.Items.BossSummon;
using Spooky.Content.NPCs.Boss.Daffodil;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class SporeEventStarterProj : ModProjectile
    {
        public override string Texture => "Spooky/Content/Items/BossSummon/SporeEventStarter";

        public static readonly SoundStyle ShatterSound = new("Spooky/Content/Sounds/GlobeShatter", SoundType.Sound) { PitchVariance = 0.6f };

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 1000;
        }

        public override bool? CanDamage()
        {
			return false;
        }

        public override void AI()
        {
            Projectile.rotation += 0.25f * (float)Projectile.direction;

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 20)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;
            }
        }

        public override void OnKill(int timeLeft)
		{
            Player player = Main.player[Projectile.owner];

            if (player.InModBiome(ModContent.GetInstance<SpiderCaveBiome>()))
            {
                SoundEngine.PlaySound(ShatterSound, Projectile.Center);

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)SpookyMessageType.StartSporeEvent);
                    packet.Send();
                }
                else
                {
                    Flags.SporeEventHappening = true;
                    Flags.SporeEventTimeLeft = 54000; //15 real-life minutes
                    Flags.SporeFogIntensity = 0.5f;
                }

                float maxAmount = 50;
                int currentAmount = 0;
                while (currentAmount <= maxAmount)
                {
                    Vector2 velocity = new Vector2(Main.rand.NextFloat(2f, 12f), Main.rand.NextFloat(2f, 12f));
                    Vector2 Bounds = new Vector2(Main.rand.NextFloat(2f, 12f), Main.rand.NextFloat(2f, 12f));
                    float intensity = Main.rand.NextFloat(2f, 12f);

                    Vector2 vector12 = Vector2.UnitX * 0f;
                    vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
                    vector12 = vector12.RotatedBy(velocity.ToRotation(), default);

                    Color[] ColorList = new Color[]
                    {
                        Color.Red, Color.Green, Color.Cyan, Color.Purple, Color.Blue, Color.Orange, Color.Gold
                    };

                    int Smoke = Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Main.rand.Next(ColorList) * 0.5f, Main.rand.NextFloat(0.5f, 1f));
                    Main.dust[Smoke].noGravity = true;
                    Main.dust[Smoke].position = Projectile.Center + vector12;
                    Main.dust[Smoke].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity * 0.2f;

                    currentAmount++;
                }

                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.Find<ModGore>("Spooky/SporeEventStarterGore" + numGores).Type);
                    }
                }
            }
            else
            {
                Item.NewItem(Projectile.GetSource_DropAsItem(), Projectile.position, Projectile.Size, ModContent.ItemType<SporeEventStarter>());
            }
        }
    }
}