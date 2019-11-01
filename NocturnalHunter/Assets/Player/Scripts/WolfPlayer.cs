public class WolfPlayer : AnimalPlayer
{
    override protected string[] AttackParameters() {
        return new string[] { "attack" };
    }

    override protected string[] DieParameters() {
        return new string[] { "die" };
    }

    override protected string[] MoraleParameters() {
        return new string[] { "howl" };
    }

    override protected string[] RunParameters() {
        return new string[] { "run" };
    }

    override protected string[] WalkParameters() {
        return new string[] { "walk" };
    }
}