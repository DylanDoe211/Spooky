using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Quest;

namespace Spooky.Content.NPCs.Quest
{
	public class Chudshroom : ModNPC
	{
        int SoundDelay = 0;

        public static readonly SoundStyle GruntSound = new("Spooky/Content/Sounds/ChudGrunt", SoundType.Sound) { PitchVariance = 0.5f };

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 9;
            NPCGlobal.IsSpookyModMiniboss[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
				Velocity = 1f
			};
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 2500;
            NPC.damage = 55;
            NPC.defense = 0;
			NPC.width = 66;
			NPC.height = 62;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 10, 0);
            NPC.noGravity = false;
			NPC.HitSound = SoundID.NPCHit7;
			NPC.DeathSound = SoundID.NPCDeath2 with { Pitch = 1.2f };
            NPC.rarity = 1;
			NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiomeUg>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Chudshroom"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiomeUg>().ModBiomeBestiaryInfoElement)
			});
		}
        
        public override void FindFrame(int frameHeight)
		{
            //moving frames
            if (NPC.velocity.X != 0)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > 8 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y < frameHeight * 5)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
                if (NPC.frame.Y >= frameHeight * 9)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
            }
            //idle frames
            else
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > 5)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 4)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
		}

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            bool HasLineOfSight = Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height);
            if ((NPC.Distance(player.Center) <= 370f && HasLineOfSight) || NPC.ai[1] > 0)
            {
                if (NPC.ai[0] == 0)
                {
                    SoundEngine.PlaySound(GruntSound, NPC.Center);
                    NPC.ai[0]++;
                }

                if (NPC.ai[1] > 0)
                {
                    NPC.ai[1]--;
                }

                NPC.spriteDirection = NPC.direction = NPC.velocity.X <= 0 ? -1 : 1;
                MoveBackAndFourth(player.Center, 8f, 0.75f, 125);
            }
            else
            {
                NPC.spriteDirection = NPC.direction;
                NPC.velocity.X = 0;
                NPC.ai[0] = 0;
            }
        }

        public void MoveBackAndFourth(Vector2 Center, float MaxSpeed, float Acceleration, int Distance)
        {
            NPC.ai[2]++;
            if (NPCGlobalHelper.IsCollidingWithFloor(NPC) && NPC.ai[2] > 60 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X) * 7)
            {
                SoundEngine.PlaySound(SoundID.DeerclopsStep with { Volume = 0.2f, Pitch = 1f }, NPC.Center);
                NPC.ai[2] = 0;
            }

            //prevents the pet from getting stuck on sloped tiles
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

            Vector2 DistToPlayer = Center - NPC.Center;
            float CenterDistance = DistToPlayer.Length();

            if (NPC.velocity.Y == 0 && HoleBelow() && CenterDistance > 100f)
            {
                NPC.velocity.Y = -10f;
                NPC.netUpdate = true;
            }

            if ((NPC.velocity.X < 0f && NPC.direction == -1) || (NPC.velocity.X > 0f && NPC.direction == 1))
            {
                if (NPC.velocity.Y == 0 && Collision.SolidTilesVersatile((int)(NPC.Center.X / 16f), (int)(NPC.Center.X + NPC.spriteDirection * 65) / 16, (int)NPC.Top.Y / 16, (int)NPC.Bottom.Y / 16 - 3))
                {
                    NPC.velocity.Y = -10f;
                    NPC.netUpdate = true;
                }
            }

            if (NPC.collideX)
            {
                NPC.velocity.X = -NPC.velocity.X;
            }

            NPC.velocity.Y += 0.15f;

            if (NPC.velocity.Y > 10f)
            {
                NPC.velocity.Y = 10f;
            }

            if (CenterDistance > Distance)
            {
                if (Center.X - NPC.Center.X > 0f)
                {
                    NPC.velocity.X += Acceleration;
                    if (NPC.velocity.X > MaxSpeed)
                    {
                        NPC.velocity.X = MaxSpeed;
                    }
                }
                else
                {
                    NPC.velocity.X -= Acceleration;
                    if (NPC.velocity.X < -MaxSpeed)
                    {
                        NPC.velocity.X = -MaxSpeed;
                    }
                }
            }
            else
            {
                if (NPC.velocity.X >= 0)
                {
                    NPC.velocity.X += Acceleration;
                    if (NPC.velocity.X > MaxSpeed)
                    {
                        NPC.velocity.X = MaxSpeed;
                    }
                }
                else
                {
                    NPC.velocity.X -= Acceleration;
                    if (NPC.velocity.X < -MaxSpeed)
                    {
                        NPC.velocity.X = -MaxSpeed;
                    }
                }
            }
        }

        public bool HoleBelow()
        {
            int tileWidth = 4;
            int tileX = (int)(NPC.Center.X / 16f) - tileWidth;
            if (NPC.velocity.X > 0)
            {
                tileX += tileWidth;
            }
            int tileY = (int)((NPC.position.Y + NPC.height) / 16f);
            for (int y = tileY; y < tileY + 2; y++)
            {
                for (int x = tileX; x < tileX + tileWidth; x++)
                {
                    if (Main.tile[x, y].HasTile && (Main.tile[x - 1, y].HasTile || Main.tile[x + 1, y].HasTile))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OldHunterMushroom>()));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 7; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ChudshroomGore" + numGores).Type);
                    }
                }
            }
            else
            {
                NPC.ai[1] = 180;
            }
        }
	}
}