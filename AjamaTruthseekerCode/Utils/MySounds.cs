using BaseLib.Audio;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Random;
using Timer = Godot.Timer;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Utils;

public static class MySounds
{
    private static readonly ModSound Prove = new("res://AjamaTruthseeker/audio/prove.ogg");
    public static readonly ModSound Obscure = new("res://AjamaTruthseeker/audio/obscure.ogg");

    private static readonly ModSound Foreseen1 = new("res://AjamaTruthseeker/audio/foreseen/foreseen1.ogg");
    private static readonly ModSound Foreseen2 = new("res://AjamaTruthseeker/audio/foreseen/foreseen2.ogg");
    private static readonly ModSound Foreseen3 = new("res://AjamaTruthseeker/audio/foreseen/foreseen3.ogg");
    public static readonly ModSound[] ForeseenSounds = [Foreseen1, Foreseen2, Foreseen3];

    private static readonly ModSound Theorize1 = new("res://AjamaTruthseeker/audio/theorize/theorize1.ogg");
    private static readonly ModSound Theorize2 = new("res://AjamaTruthseeker/audio/theorize/theorize2.ogg");
    public static readonly ModSound[] TheorizeSounds = [Theorize1, Theorize2];

    public static void PlayRandom(ModSound[] sounds,
        float volumeAdd = 0F, float volumeMult = 1F,
        float pitchVariation = 0F, float basePitch = 1F)
    {
        Rng.Chaotic.NextItem(sounds)!.Play(volumeAdd, volumeMult, pitchVariation, basePitch);
    }
    
    private static Timer? ForeseenTimer { get; set; }
    public static void PlayForeseen()
    {
        ForeseenTimer = PlayWithCooldown(ForeseenTimer, multipleSfx: ForeseenSounds, pitchVariation: 0.12f);
    }
    
    private static Timer? ProveTimer { get; set; }
    public static void PlayProve()
    {
        ProveTimer = PlayWithCooldown(ProveTimer, singleSfx: Prove, pitchVariation: 0.05f);
    }
    
    private static Timer? TheorizeTimer { get; set; }
    public static void PlayTheorize()
    {
        TheorizeTimer = PlayWithCooldown(TheorizeTimer, multipleSfx: TheorizeSounds, pitchVariation: 0.12f);
    }
    
    private static Timer? PlayWithCooldown(Timer? timer,
        ModSound? singleSfx = null, ModSound[]? multipleSfx = null,
        float volumeAdd = 0F, float volumeMult = 1F,
        float pitchVariation = 0F, float basePitch = 1F)
    {
        if ((timer == null || !timer.IsValid()) && NRun.Instance != null)
        {
            timer = new Timer();
            timer.WaitTime = 0.05;
            timer.OneShot = true;
            
            NRun.Instance.GlobalUi.AddChildSafely(timer);
        }

        if (timer == null || !timer.IsValid() || !timer.IsStopped()) return timer;

        if (singleSfx != null)
        {
            singleSfx.Play(volumeAdd, volumeMult, pitchVariation, basePitch);
        }
        else if (multipleSfx != null)
        {
            PlayRandom(multipleSfx, volumeAdd, volumeMult, pitchVariation, basePitch);
        }
        
        timer.Start();

        return timer;
    }
}