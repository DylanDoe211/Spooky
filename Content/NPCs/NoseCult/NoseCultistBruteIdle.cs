using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.UserInterfaces;

namespace Spooky.Content.NPCs.NoseCult
{
	public class NoseCultistBruteIdle : ModNPC
	{
		public Vector2 modifier = new(-200, -75);

		private static Asset<Texture2D> UITexture;

        public static readonly SoundStyle TalkSound = new("Spooky/Content/Sounds/TalkSounds/NoseCultistTalk", SoundType.Sound) { Volume = 0.35f, Pitch = -1f, PitchVariance = 0.75f };

		public override void Load()
		{
			UITexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/DialogueUINoseCultist");
		}

		public override void SetStaticDefaults()
		{	
			Main.npcFrameCount[NPC.type] = 3;
			NPCID.Sets.CantTakeLunchMoney[Type] = true;
			NPCID.Sets.ShimmerTownTransform[Type] = false;
			NPCID.Sets.NoTownNPCHappiness[Type] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 5;
			NPC.damage = 0;
			NPC.defense = 0;
            NPC.width = 122;
			NPC.height = 112;
			NPC.npcSlots = 0f;
			NPC.knockBackResist = 0f;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.dontCountMe = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
		}

		public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 15)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 3)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}

		public override bool CheckActive()
        {
            return false;
        }

		public override bool CanChat()
		{
			return Main.LocalPlayer.GetModPlayer<SpookyPlayer>().NoseCultistDisguise1 && Main.LocalPlayer.GetModPlayer<SpookyPlayer>().NoseCultistDisguise2;
		}

		public override void SetChatButtons(ref string button, ref string button2)
		{
			button = "";
		}

		public override string GetChat()
		{
            DialogueChain chain = new();
            chain.Add(new(UITexture.Value, NPC,
            Language.GetTextValue("Mods.Spooky.Dialogue.NoseBrute.Dialogue" + Main.rand.Next(1, 4)),
            Language.GetTextValue("..."),
            TalkSound, 2f, 0f, modifier, NPCID: NPC.type))
            .Add(new(UITexture.Value, NPC, null, null, TalkSound, 2f, 0f, modifier, true));
            chain.OnPlayerResponseTrigger += PlayerResponse;
            chain.OnEndTrigger += EndDialogue;
            DialogueUI.Visible = true;
            DialogueUI.Add(chain);

            return string.Empty;
        }

		public override void AI()
		{
			NPC Parent = Main.npc[(int)NPC.ai[0]];

			NPC.spriteDirection = NPC.Center.X > Parent.Center.X ? -1 : 1;

			int[] Idols = { ModContent.NPCType<MocoIdol1>(), ModContent.NPCType<MocoIdol2>(), ModContent.NPCType<MocoIdol3>(), ModContent.NPCType<MocoIdol4>(), ModContent.NPCType<MocoIdol5>() };

			if (!Parent.active || !Idols.Contains(Parent.type))
			{
				NPC.active = false;
			}

			if (Parent.ai[1] == 1)
			{
				NPC.ai[1]++;

				if (NPC.ai[1] == 30)
				{
					Dust.NewDustPerfect(new Vector2(NPC.Center.X, NPC.Center.Y - NPC.height), ModContent.DustType<CultistExclamation>(), Vector2.Zero, 0, default, 1f);
				}

				if (NPC.ai[1] >= 60)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						int SpawnedNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + NPC.height / 2, ModContent.NPCType<NoseCultistBrute>());

						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.SyncNPC, number: SpawnedNPC);
						}
					}

					NPC.active = false;
				}
			}
        }

		public void PlayerResponse(Dialogue dialogue, string Text, int ID)
		{
			Dialogue newDialogue = new(ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/DialogueUIPlayer").Value, Main.LocalPlayer,
			Text, null, SoundID.Item1, 2f, 0f, default, NotPlayer: false);
			DialogueUI.Visible = true;
			DialogueUI.Add(newDialogue);
		}

		public void EndDialogue(Dialogue dialogue, int ID)
		{
			DialogueUI.Visible = false;
		}
    }
}