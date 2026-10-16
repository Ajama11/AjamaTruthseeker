using BaseLib.Audio;
using MegaCrit.Sts2.Core.Random;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Utils;

public static class MySounds
{
    public static readonly ModSound Prove = new("res://AjamaTruthseeker/audio/prove.ogg");
    public static readonly ModSound Obscure = new("res://AjamaTruthseeker/audio/obscure.ogg");

    private static readonly ModSound Foreseen1 = new("res://AjamaTruthseeker/audio/foreseen/foreseen1.ogg");
    private static readonly ModSound Foreseen2 = new("res://AjamaTruthseeker/audio/foreseen/foreseen2.ogg");
    private static readonly ModSound Foreseen3 = new("res://AjamaTruthseeker/audio/foreseen/foreseen3.ogg");
    public static readonly ModSound[] ForeseenSounds = [Foreseen1, Foreseen2, Foreseen3];

    public static void PlayRandom(ModSound[] sounds,
        float volumeAdd = 0F, float volumeMult = 1F,
        float pitchVariation = 0F, float basePitch = 1F)
    {
        Rng.Chaotic.NextItem(sounds)!.Play(volumeAdd, volumeMult, pitchVariation, basePitch);
    }
}