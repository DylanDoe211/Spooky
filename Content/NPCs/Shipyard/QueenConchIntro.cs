using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Shipyard
{
	public class QueenConchIntro : ModNPC
	{
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
				if (NPC.frame.Y >= frameHeight * 3)
				{
					NPC.frame.Y = 2 * frameHeight;
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
			int SpawnQueenConch = BeginThrow + 15;

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
            if (NPC.ai[0] >= StartSpinning + 35)
            {
                //this is where the npc should rotate alongside the throwing charge up animation
                //nothing for now, look into proper ease-in rotation later
            }

			if (NPC.ai[0] == BeginThrow - 24) 
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
                    int NewNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<QueenConch>());
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

        private float spinEasing(int start, int end, float time)
		{
			// (time - start) / (end - start) gives a value between 0 and 1 
			// implement something from https://easings.net/ 👍
			return 0;
		}
    }
}