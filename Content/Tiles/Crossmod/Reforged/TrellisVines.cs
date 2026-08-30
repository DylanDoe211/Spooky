using Spooky.Content.Tiles.SpookyBiome;
using System;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Crossmod.Reforged;

/// <summary>
/// Loader class for the crossmod trellis vine tiles.
/// </summary>
internal class TrellisVines : ILoadable
{
	void ILoadable.Load(Mod mod)
	{
		if (!ModLoader.TryGetMod("SpiritReforged", out Mod reforged))
			return;

		// The call is fairly simple. Ignoring the identifier string:
		//		Mod mod: the mod adding this vine
		//		Func<(int, int)[]> styleFunc: the item-style pairs to be fetched when content is loaded.
		//		string name: the name of the new vine tile
		//		string path: the path for the new vine tile's texture
		string path = "Spooky/Content/Tiles/Crossmod/Reforged/SpookyTrellisVines";
		reforged.Call("TrellisVine", ModContent.GetInstance<Spooky>(), (Func<(int, int)[]>)GetStyles, "SpookyTrellisVines", path);
	}

	public static (int, int)[] GetStyles() => [(ModContent.ItemType<SpookySeedsGreen>(), 0), (ModContent.ItemType<SpookySeedsOrange>(), 1),
		(ModContent.ItemType<MushroomMossSeeds>(), 2) ];

	void ILoadable.Unload() { }
}
