using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.TestSupport;

namespace AjamaTruthseeker.AjamaTruthseekerCode.Nodes;

public partial class ForesightNCardFlyVfx : NCardFlyVfx
{
    public override void _Ready()
    {
        _vfx = NCardTrailVfx.Create(_card, _trailPath);
        if (_vfx != null) GetParent().AddChildSafely(_vfx);
        
        _controlPointOffset = Rng.Chaotic.NextFloat(-50f, 200f);
        _speed = 3.2f;
        _accel = Rng.Chaotic.NextFloat(2f, 2.5f);
        _arcDir = 50f + _controlPointOffset;
        _duration = 1.5f;
        
        _card.Connect(Node.SignalName.TreeExited, Callable.From(OnCardExitedTree));
        
        if (NCombatUi.IsDebugHidingPlayContainer)
        {
            _card.Modulate = Colors.Transparent;
            _card.Visible = false;
            Visible = false;
        }
        
        TaskHelper.RunSafely(ForesightPlayAnim());
    }
    
    public static ForesightNCardFlyVfx? ForesightCreate(NCard card, Creature target, string trailPath)
    {
        if (TestMode.IsOn) return null;
        if (NCombatRoom.Instance == null) return null;
        
        NCreature? creatureNode = NCombatRoom.Instance.GetCreatureNode(target);
        if (creatureNode == null) return null;
        
        GodotObject? originalVfx = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NCardFlyVfx>();
        var godotObjectId = originalVfx.GetInstanceId();
        originalVfx.SetScript(ResourceLoader.Load("AjamaTruthseekerCode/Nodes/ForesightNCardFlyVfx.cs"));
        ForesightNCardFlyVfx newVfx = (InstanceFromId(godotObjectId) as ForesightNCardFlyVfx)!;
        
        newVfx._startPos = card.GlobalPosition;
        newVfx._endPos = creatureNode.VfxSpawnPosition;
        newVfx._card = card;
        newVfx._isAddingToPile = false;
        newVfx._trailPath = trailPath;
        
        return newVfx;
    }
    
    private async Task ForesightPlayAnim()
    {
        CardPile? pile = _card.Model!.Pile;
        if (pile != null)
        {
            SfxCmd.PlayCardSwooshSfx(pile);
        }
        SwooshAwayCompletion = new TaskCompletionSource();
        float time = 0f;
        while (time / _duration <= 1f)
        {
            await this.AwaitProcessFrame();
            if (_cancelToken.IsCancellationRequested)
            {
                SwooshAwayCompletion?.SetResult();
                return;
            }
            float num = (float)GetProcessDeltaTime();
            time += _speed * num;
            _speed += _accel * num;
            Vector2 c = _startPos + (_endPos - _startPos) * 0.5f;
            c.Y -= _arcDir;
            Vector2 vector = MathHelper.BezierCurve(_startPos, _endPos, c, (time + 0.05f) / _duration);
            _card.GlobalPosition = MathHelper.BezierCurve(_startPos, _endPos, c, time / _duration);
            float num2 = (vector - _card.GlobalPosition).Angle() + (float)Math.PI / 2f;
            Node parent = _card.GetParent();
            if (parent is Control control)
            {
                num2 -= control.Rotation;
            }
            else if (parent is Node2D node2D)
            {
                num2 -= node2D.Rotation;
            }
            _card.Rotation = Mathf.LerpAngle(_card.Rotation, num2, num * 12f);
            _card.Body.Modulate = Colors.White.Lerp(Colors.Black, Mathf.Clamp(time * 3f / _duration, 0f, 1f));
            _card.Body.Scale = Vector2.One * Mathf.Lerp(1f, 0.1f, Mathf.Clamp(time * 3f / _duration, 0f, 1f));
        }
        _card.GlobalPosition = _endPos;
        if (_isAddingToPile)
        {
            _card.Model.Pile?.InvokeCardAddFinished();
        }
        time = 0f;
        while (time / _duration <= 1f)
        {
            await this.AwaitProcessFrame();
            if (_cancelToken.IsCancellationRequested)
            {
                SwooshAwayCompletion?.SetResult();
                return;
            }
            // time += _speed * (float) GetProcessDeltaTime();
            time += (float) GetProcessDeltaTime();
            // if (time / _duration > 0.25f && !_vfxFading)
            if (!_vfxFading)
            {
                if (_vfx != null)
                {
                    _ = TaskHelper.RunSafely(_vfx.FadeOut());
                }
                _vfxFading = true;
            }
            _card.Body.Scale = Vector2.One * Mathf.Max(Mathf.Lerp(0.1f, -0.15f, time / _duration), 0f);
        }
        SwooshAwayCompletion?.SetResult();
        _card.QueueFreeSafely();
    }
}