using Godot;
using GodotUtilities.CSharpExt;

namespace GodotUtilities.DataStructures.Noise;

public class TieredNoise
{
    public List<(Vector2 range, Godot.Noise noise)> Tiers { get; private set; }

    public TieredNoise()
    {
        Tiers = new List<(Vector2 range, Godot.Noise noise)>();
    }

    public FastNoiseLite AddFastNoiseTier(Vector2 range, float frequency)
    {
        var noise = new FastNoiseLite();
        noise.Frequency = frequency;
        Tiers.Add((range, noise));
        return noise;
    }

    public float GetSample2D(Vector2 pos)
    {
        return Tiers.Sum(v =>
        {
            var sample = v.noise.GetNoise2D(pos.X, pos.Y);
            return sample.ProjectToRange(new Vector2(-1f, 1f), v.range);
        });
    }
}