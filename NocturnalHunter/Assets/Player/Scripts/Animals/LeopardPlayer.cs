public class LeopardPlayer : AnimalPlayer
{
    protected override string[] AttackParameters() {
        return new string[] { "attack_1", "attack_2", "attack_3" };
    }

    protected override string CreepParameter() {
        return "creep";
    }

    protected override string DieParameter() {
        return "die";
    }

    protected override string[] HitParameters() {
        return new string[] { "hit_1", "hit_2", "hit_3", "hit_4", "hit_5" };
    }

    protected override string JumpParameter() {
        return "jump";
    }

    protected override string[] MoraleParameters() {
        return new string[] {};
    }

    protected override string RunParameter() {
        return "run";
    }

    protected override string[] SpecialIdleParameters() {
        return new string[] { "idle_1", "idle_2", "idle_3" };
    }

    protected override string WalkParameter() {
        return "walk";
    }
}