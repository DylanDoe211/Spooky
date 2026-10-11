using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.NPCs.Shipyard
{
	public class QueenConchIntro : ModNPC
	{
	 	bool holdFrame = true;
		float progress = 1;

        public enum AnimationState
		{
			Emerge, PrepareThrow, Throw
		}

		private AnimationState CurrentAnimation
        {
			get => (AnimationState)NPC.ai[3];
			set => NPC.ai[3] = (float)value;
		}

        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 19;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 100;
			NPC.height = 84;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 50, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ShipyardBiome>().Type };
        }

        public override void FindFrame(int frameHeight)
        {
			NPC.frameCounter++;
			if (NPC.frameCounter > 6)
			{
				NPC.frame.Y = NPC.frame.Y + frameHeight;
				NPC.frameCounter = 0;
			}

            //emerge frame sits on the ground
			if (CurrentAnimation == AnimationState.Emerge)
			{
				if (NPC.frame.Y >= frameHeight * 3 && holdFrame)
				{
					NPC.frame.Y = 2 * frameHeight;
					NPC.frameCounter = 0;
					holdFrame = false;
				}
				if (NPC.frame.Y >= frameHeight * 7)
				{
					NPC.frame.Y = 6 * frameHeight;
				}
			}
            else if (CurrentAnimation == AnimationState.PrepareThrow)
			{
				if (NPC.frame.Y >= frameHeight * 13)
				{
					NPC.frame.Y = 12 * frameHeight;
				}
			}
            else if (CurrentAnimation == AnimationState.Throw)
			{
				if (NPC.frame.Y >= frameHeight * 19)
				{
					NPC.frame.Y = 18 * frameHeight;
				}
			}
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new Vector2(NPCTexture.Width() * 0.5f - 10, NPC.height * 0.5f);
            spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            
            return false;
        }
		
        public override void AI()
		{
			int StartSpinning = 50;
			int BeginThrow = 132;
			int SpawnQueenConch = BeginThrow + 35;

			NPC.ai[0]++;
            if (NPC.ai[0] == 1)
            {
                CurrentAnimation = AnimationState.Emerge;
            }

            if (NPC.ai[0] == StartSpinning)
            {
				SoundEngine.PlaySound(SoundID.NPCDeath19, NPC.Center);

				CurrentAnimation = AnimationState.PrepareThrow;
				NPC.velocity = new Vector2(Main.rand.Next(-3, 4), -11f);
                NPC.netUpdate = true;
            }

            //have it float up while queen conch is throwing the hermit crab
            if (NPC.ai[0] >= StartSpinning && NPC.ai[0] < BeginThrow + 30)
            {
				NPC.velocity.Y += 0.15f;
			}

            //dont start rotating until the throwing animation begins
            if (NPC.ai[0] >= StartSpinning)
            {
				if (NPC.ai[0] < BeginThrow -7) 
				{
					NPC.rotation = SpinEasing(StartSpinning, BeginThrow - 7, NPC.ai[0]);
				}
				else
				{
					NPC.TargetClosest();
					Player player = Main.player[NPC.target];

					if (NPC.ai[0] <= SpawnQueenConch - 8) 
					{
						progress = 0.4118f + (NPC.ai[0] - BeginThrow -7) / (SpawnQueenConch - (BeginThrow + 1));
					}

					Vector2 RotateTowards = player.Center - NPC.Center;
					float RotateDirection = (float)Math.Atan2(RotateTowards.Y, RotateTowards.X) + 4.71f;
					float targetSpeed = 0.05f;
					float RotateSpeed = targetSpeed * progress;
					NPC.rotation = NPC.rotation.AngleTowards(RotateDirection - MathHelper.TwoPi, RotateSpeed);

					NPC.ai[1] += 0.005f;

					float theta = MathHelper.PiOver2 * MathF.Sin(NPC.ai[1] * 12) * 0.4f;
					Vector2 GoTo = player.Center + new Vector2(0, -280).RotatedBy(theta);

					Vector2 desiredVelocity = NPC.DirectionTo(GoTo) * 12;
					NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20 * progress);
				}
            }

			if (NPC.ai[0] == BeginThrow - 18) 
            {
                CurrentAnimation = AnimationState.Throw;
                NPC.netUpdate = true;
            }
            
			if (NPC.ai[0] == BeginThrow)
			{
				SoundEngine.PlaySound(SoundID.DD2_GoblinBomberThrow with { Volume = 3f }, NPC.Center);

				Vector2 CrabSpawnPosition = NPC.Center + new Vector2(55, 30).RotatedBy(NPC.rotation);
                Vector2 CrabLaunchToPosition = NPC.Center + new Vector2(60, 100).RotatedBy(NPC.rotation);

				Vector2 ShootSpeed = CrabLaunchToPosition - NPC.Center;
				ShootSpeed.Normalize();
				ShootSpeed *= 12f;

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					int NewNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)CrabSpawnPosition.X, (int)CrabSpawnPosition.Y, NPCID.Crab);
					Main.npc[NewNPC].velocity = ShootSpeed;
					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.SyncNPC, number: NewNPC);
					}
				}

                NPC.netUpdate = true;
			}

			if (NPC.ai[0] == SpawnQueenConch)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int NewNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + NPC.height / 4, ModContent.NPCType<QueenConch>());
                    Main.npc[NewNPC].velocity = NPC.velocity;
                    Main.npc[NewNPC].rotation = NPC.rotation;
                    Main.npc[NewNPC].alpha = 255;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: NewNPC);
                    }
                }

                NPC.active = false;
                NPC.netUpdate = true;
            }
        }

        private float SpinEasing(int start, int end, float time)
		{
			double progress = (time - start) / (end - start);
			double y = 1 + (progress * 0.5);
			double x = Math.Pow(progress, y);
			double degrees = 720 * (x < 0.5 ? 2 * x * x : 1 - Math.Pow(-2 * x + 2, 2) / 2);
			return (float) (degrees * 0.017453292519943295769236907684886); //radians 💅
		}
    }
}