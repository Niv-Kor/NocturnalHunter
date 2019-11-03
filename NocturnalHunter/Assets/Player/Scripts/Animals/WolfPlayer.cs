public class WolfPlayer : AnimalPlayer
{
    override protected AnimationConstraints AttackConstraints() {
        string[] parameters = new string[] { "attack" };
        return new AnimationConstraints(parameters, false);
    }

    override protected AnimationConstraints DieConstraints() {
        string[] parameters = new string[] { "die" };
        return new AnimationConstraints(parameters, false);
    }

    override protected AnimationConstraints MoraleConstraints() {
        string[] parameters = new string[] { "howl" };
        return new AnimationConstraints(parameters, false);
    }

    override protected AnimationConstraints RunConstraints() {
        string[] parameters = new string[] { "run" };
        return new AnimationConstraints(parameters, false);
    }

    override protected AnimationConstraints WalkConstraints() {
        string[] parameters = new string[] { "walk" };
        return new AnimationConstraints(parameters, false);
    }

    override protected AnimationConstraints JumpConstraints() {
        string[] parameters = new string[] { "jump" };
        return new AnimationConstraints(parameters, true);
    }
}