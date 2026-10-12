using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SpiritReforged.Common.WorldGeneration.GenConfiguration;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.ModLoader;

namespace Spooky.Content.Generation.GenConfig;

#nullable enable

[ExtendsFromMod("SpiritReforged")]
[JITWhenModsEnabled("SpiritReforged")]
public class ShipyardGenerationPage : IGenerationPage
{
	// Page name, page back (the main element for the UI - doesn't need the outline) and page button (for use in the next/back buttons in the UI, also doesn't need outline)
	PageInfo IGenerationPage.Info => new("Shipyard", ConfigLoader.Get("ShipyardBack"), ConfigLoader.Get("ShipyardButton"));

	// This should just point to your own mod.
	Mod IGenerationPage.Mod => Spooky.Instance;
}

[ExtendsFromMod("SpiritReforged")]
[JITWhenModsEnabled("SpiritReforged")]
public class ConfigGenerationLoading
{
	internal static void LoadBindings()
	{
		GenConfigLoader.LoadingMods.Add(ModContent.GetInstance<Spooky>());

		GenConfigLoader.CrossmodConfigurables.Add("Spooky/Shipyard", LoadConfigsFrom<ShipyardConfiguration>);
	}

	private static List<(MemberInfo, ConfigInfo)> LoadConfigsFrom<T>()
	{
		Type type = typeof(T);
		List<(MemberInfo, ConfigInfo)> configs = [];

		foreach (FieldInfo field in type.GetFields())
		{
			if (field.GetCustomAttribute<BindToGenConfigAttribute>() is BindToGenConfigAttribute gen)
			{
				GenConfigParameters parameters = new(gen.Minimum, gen.Maximum, gen.Step);
				configs.Add((field, new ConfigInfo(parameters, gen.ReverseMinMax, gen.Slider, gen.Denominator, gen.PriorityConfig)));
			}
		}

		foreach (PropertyInfo field in type.GetProperties())
		{
			if (field.GetCustomAttribute<BindToGenConfigAttribute>() is BindToGenConfigAttribute gen)
			{
				GenConfigParameters parameters = new(gen.Minimum, gen.Maximum, gen.Step);
				configs.Add((field, new ConfigInfo(parameters, gen.ReverseMinMax, gen.Slider, gen.Denominator, gen.PriorityConfig)));
			}
		}

		return configs;
	}
}

// Classes used to hold the variables for gen configuration (per page), alongside a class safely loading it.
// LoadBindings is necessary to skip JIT on, otherwise ConfigGenerationLoading is discovered by the runtime (and throws).

public class ConfigLoader : ILoadable
{
	public const string FilePath = "Spooky/Content/Generation/GenConfig/";

	public static Asset<Texture2D> Get(string name) => ModContent.Request<Texture2D>(FilePath + name);

	void ILoadable.Load(Mod mod)
	{
		if (!ModLoader.HasMod("SpiritReforged"))
			return;

		LoadBindings();
	}

	[JITWhenModsEnabled("SpiritReforged")]
	private static void LoadBindings() => ConfigGenerationLoading.LoadBindings();

	void ILoadable.Unload()
	{
	}
}

// Wrapper attribute used to mimic the in-house attributes used by Reforged. 
// This can be copied at will for your own mod.

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class BindToGenConfigAttribute(object min, object max, object step, bool slider = false, bool reverseMinMax = false,
	bool denominator = false, string? priorityConfig = null) : Attribute
{
	public readonly object Minimum = min;
	public readonly object Maximum = max;
	public readonly object Step = step;
	public readonly bool Slider = slider;
	public readonly bool ReverseMinMax = reverseMinMax;
	public readonly bool Denominator = denominator;
	public readonly string? PriorityConfig = priorityConfig;
}
