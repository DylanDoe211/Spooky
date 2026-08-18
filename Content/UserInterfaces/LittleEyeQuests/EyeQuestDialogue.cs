using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;

namespace Spooky.Content.UserInterfaces.LittleEyeQuests;

internal class EyeQuestDialogue
{
	public static void BountyOne()
	{
		DialogueChain chain = new();
		NPC littleEye = Main.npc[LittleEyeQuestUI.LittleEye];
		
		chain.Add(new(LittleEyeQuestUI.UITexture.Value, littleEye,
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest1-1"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest1-1"), LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: littleEye.type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, littleEye,
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest1-2"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest1-2"), LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: littleEye.type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, littleEye,
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest1-3"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest1-3"), LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: littleEye.type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, littleEye,
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest1-4"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest1-4"), LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: littleEye.type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, littleEye, null, null, LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, true));

		TriggerSetupAndStart(LittleEyeQuestUI.PlayerResponse, LittleEyeQuestUI.AcceptOne, chain);
	}

	public static void BountyOneNewItem()
	{
		DialogueChain chain = new();
		chain.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem1-1"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem1-1"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem1-2"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem1-2"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem1-3"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem1-3"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem1-4"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem1-4"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye], null, null, LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, true));

		TriggerSetupAndStart(LittleEyeQuestUI.PlayerResponse, LittleEyeQuestUI.AcceptOne, chain);
	}

	public static void BountyTwo()
	{
		DialogueChain chain = new();
		chain.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest2-1"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest2-1"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest2-2"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest2-2"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest2-3"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest2-3"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest2-4"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest2-4"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye], null, null, LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, true));

		TriggerSetupAndStart(LittleEyeQuestUI.PlayerResponse, LittleEyeQuestUI.AcceptTwo, chain);
	}

	public static void BountyTwoNewItem()
	{
		DialogueChain chain = new();
		chain.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem2-1"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem2-1"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem2-2"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem2-2"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem2-3"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem2-3"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem2-4"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem2-4"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye], null, null, LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, true));

		TriggerSetupAndStart(LittleEyeQuestUI.PlayerResponse, LittleEyeQuestUI.AcceptTwo, chain);
	}

	public static void BountyThree()
	{
		DialogueChain chain = new();
		chain.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest3-1"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest3-1"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest3-2"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest3-2"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest3-3"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest3-3"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye], null, null, LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, true));

		TriggerSetupAndStart(LittleEyeQuestUI.PlayerResponse, LittleEyeQuestUI.AcceptThree, chain);
	}

	public static void BountyThreeNewItem()
	{
		DialogueChain chain = new();
		chain.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem3-1"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem3-1"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem3-2"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem3-2"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem3-3"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem3-3"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye], null, null, LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, true));

		TriggerSetupAndStart(LittleEyeQuestUI.PlayerResponse, LittleEyeQuestUI.AcceptThree, chain);
	}

	public static void BountyFour()
	{
		DialogueChain chain = new();
		chain.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest4-1"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest4-1"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest4-2"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest4-2"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest4-3"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest4-3"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest4-4"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest4-4"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest4-5"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest4-5"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye], null, null, LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, true));

		TriggerSetupAndStart(LittleEyeQuestUI.PlayerResponse, LittleEyeQuestUI.AcceptFour, chain);
	}

	public static void BountyFourNewItem()
	{
		DialogueChain chain = new();
		chain.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem4-1"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem4-1"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem4-2"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem4-2"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem4-3"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem4-3"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem4-4"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem4-4"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem4-5"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem4-5"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye], null, null, LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, true));

		TriggerSetupAndStart(LittleEyeQuestUI.PlayerResponse, LittleEyeQuestUI.AcceptFour, chain);
	}

	public static void OrroborroDialogue(Vector2 modifier )
	{
		DialogueChain chain = new();
		chain.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest5-1"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest5-1"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest5-2"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest5-2"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest5-3"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest5-3"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest5-4"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest5-4"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye],
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest5-5"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest5-5"),
				LittleEyeQuestUI.TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEyeQuestUI.LittleEye].type))
			.Add(new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye], null, null, LittleEyeQuestUI.TalkSound, 2f, 0f, modifier, true));

		TriggerSetupAndStart(LittleEyeQuestUI.PlayerResponse, LittleEyeQuestUI.EndDialogueQuestAccept5, chain);
	}

	private static void TriggerSetupAndStart(DialogueChain.PlayerResponseTrigger trigger, DialogueChain.EndTrigger end, DialogueChain chain)
	{
		chain.OnPlayerResponseTrigger += trigger;
		chain.OnEndTrigger += end;
		DialogueUI.Visible = true;
		DialogueUI.Add(chain);
	}
}
