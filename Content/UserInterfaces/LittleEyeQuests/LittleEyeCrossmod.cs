using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Spooky.Content.NPCs.Friendly;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Spooky.Content.UserInterfaces.LittleEyeQuests;

#nullable enable

public readonly record struct CrossmodQuest(Mod Mod, string Name, Asset<Texture2D> Icon, Func<bool> CompleteCheck, Func<DialogueChain> Chain,
	Func<DialogueChain> RecoverChain, Func<bool>? IsLocked = null);

public class LittleEyeCrossmod
{
	internal readonly static Dictionary<string, List<CrossmodQuest>> QuestsByMod = [];

	private static string ErrorStart(int paramIndex) => $"Parameters {paramIndex} for EyeQuest ";

	internal static bool Call(object[] objects)
	{
		if (objects.Length <= 6)
			throw new ArgumentException("EyeQuest takes at least 7 parameters (Mod mod, string questName, Asset<Texture2D> icons, Action<bool> onActivate, Func<bool> completeCheck, " +
				"(string npcText, string playerText)[] dialogue, (string npcText, string playerText)[] recoverDialogue, [Func<bool>? isLocked = null]");

		if (objects[0] is not Mod mod)
			throw new ArgumentException(ErrorStart(0) + "must be a Mod (mod)!");

		if (objects[1] is not string name)
			throw new ArgumentException(ErrorStart(1) + "must be a string (questName)!");

		if (objects[2] is not Asset<Texture2D> icons)
			throw new ArgumentException(ErrorStart(2) + "must be an Asset<Texture2D> (icons)!");

		if (objects[3] is not Action<bool> onActivate)
			throw new ArgumentException(ErrorStart(3) + "must be an Action<bool> (onActivate)!");

		if (objects[4] is not Func<bool> completeCheck)
			throw new ArgumentException(ErrorStart(4) + "must be an Action<bool> (onActivate)!");

		if (objects[5] is not (string npcText, string playerText)[] dialogue)
			throw new ArgumentException(ErrorStart(5) + "must be an (string, string)[] (dialogue)! Note that these are the localization keys used, not the localized text.");

		if (objects[6] is not (string npcText, string playerText)[] recoverDialogue)
			throw new ArgumentException(ErrorStart(6) + "must be an (string, string)[] (recoverDialogue)! Note that these are the localization keys used, not the localized text.");

		Func<bool>? isLocked = null;

		if (objects.Length == 8) 
		{
			if (objects[7] is not Func<bool> locked)
				throw new ArgumentException(ErrorStart(6) + "must be an Func<bool> (isLocked), or be omitted!");
			else
				isLocked = locked;
		}

		AddQuest(mod, name, icons, onActivate, completeCheck, dialogue, recoverDialogue, isLocked);
		return true;
	}

	public static void AddQuestDirect(CrossmodQuest quest)
	{
		QuestsByMod.TryAdd(quest.Mod.Name, []);
		QuestsByMod[quest.Mod.Name].Add(quest);
	}

	private static void AddQuest(Mod mod, string questName, Asset<Texture2D> icon, Action<bool> onActivate, Func<bool> completeCheck, (string npcText, string playerText)[] dialogue, 
		(string npcText, string playerText)[] recoverDialogue, Func<bool>? isLocked)
	{
		CrossmodQuest quest = new(mod, questName, icon, completeCheck, Chain, Recover, isLocked);
		QuestsByMod.TryAdd(mod.Name, []);
		QuestsByMod[mod.Name].Add(quest);

		return;

		// Delegate local methods; the triggers are simply to simplify the call signature, the chains are to construct chains without a reference

		void EndTrigger(Dialogue dialogue, int id) => onActivate.Invoke(false);
		void RecoverTrigger(Dialogue dialogue, int id) => onActivate.Invoke(true);

		DialogueChain Chain()
		{
			DialogueChain chain = new();

			foreach ((string npc, string player) in dialogue)
				chain.Add(ConstructDialogue(npc, player));

			NPC entity = Main.npc[LittleEyeQuestUI.LittleEye];
			chain.Add(new(LittleEyeQuestUI.UITexture.Value, entity, null, null, LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, true));
			chain.OnPlayerResponseTrigger += LittleEyeQuestUI.PlayerResponse;
			chain.OnEndTrigger += EndTrigger;
			return chain;
		}

		DialogueChain Recover()
		{
			DialogueChain chain = new();

			foreach ((string npc, string player) in recoverDialogue)
				chain.Add(ConstructDialogue(npc, player));

			NPC entity = Main.npc[LittleEyeQuestUI.LittleEye];
			chain.Add(new(LittleEyeQuestUI.UITexture.Value, entity, null, null, LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, true));
			chain.OnPlayerResponseTrigger += LittleEyeQuestUI.PlayerResponse;
			chain.OnEndTrigger += RecoverTrigger;
			return chain;
		}
	}

	private static Dialogue ConstructDialogue(string npc, string player)
	{
		int id = ModContent.NPCType<LittleEye>();

		return new(LittleEyeQuestUI.UITexture.Value, Main.npc[LittleEyeQuestUI.LittleEye], Language.GetTextValue(npc), Language.GetTextValue(player),
			LittleEyeQuestUI.TalkSound, 2f, 0f, LittleEyeQuestUI.PositionModifier, NPCID: id);
	}
}
