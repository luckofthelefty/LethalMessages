using System;
using System.Collections.Generic;

namespace com.github.luckofthelefty.LethalMessages.Messages;

internal static class DeathMessages
{
    private static readonly Random _rng = new Random();

    private static readonly List<string> Unknown = new List<string>
    {
        "$$ just... died. Nobody knows how.",
        "$$'s cause of death: ¯\\_(ツ)_/¯",
        "$$ has left the mortal plane.",
        "$$ spontaneously stopped living.",
        "The Company sends its condolences to $$'s family.",
        "$$ died of mysterious circumstances. Totally not suspicious.",
        "$$ ragdolled into the void.",
        "$$ was claimed by the moon itself."
    };

    private static readonly List<string> Bludgeoning = new List<string>
    {
        "$$ got absolutely rocked.",
        "$$'s skull made an interesting sound.",
        "$$ caught something heavy with their face.",
        "$$ found out objects in motion stay in motion.",
        "$$ was on the wrong end of blunt force trauma.",
        "$$ brought a face to a blunt object fight.",
        "$$'s head was used as a stress ball.",
        "Something gave $$ a permanent headache."
    };

    private static readonly List<string> Gravity = new List<string>
    {
        "$$'s last words were 'I can make that jump.'",
        "$$ achieved terminal velocity.",
        "$$ forgot their parachute at home.",
        "$$ tested gravity. Gravity won.",
        "$$ left a $$ shaped crater.",
        "$$ believed they could fly. They were wrong.",
        "$$ speedran the floor.",
        "$$ took the express elevator down.",
        "$$ discovered that fall damage is real."
    };

    private static readonly List<string> Blast = new List<string>
    {
        "$$ was redistributed across the facility.",
        "$$ went out with a bang. Literally.",
        "$$'s atoms are now in several zip codes.",
        "$$ became confetti.",
        "$$ learned about rapid unplanned disassembly.",
        "$$ found the landmine. With their feet.",
        "$$ is now a fine mist.",
        "$$ was vaporized. At least it was quick."
    };

    private static readonly List<string> Strangulation = new List<string>
    {
        "$$ was strangled. That's not a great way to go.",
        "$$'s neck got a very unwanted massage.",
        "$$ forgot how to breathe (with help).",
        "$$ was choked out like a UFC fighter.",
        "Something really wanted $$ to shut up.",
        "$$'s airway was permanently closed for business.",
        "$$ received a complimentary neck compression.",
        "$$ was throttled into the afterlife."
    };

    private static readonly List<string> Suffocation = new List<string>
    {
        "$$ ran out of oxygen. Classic mistake.",
        "$$'s lungs filed for bankruptcy.",
        "$$ tried to breathe. Couldn't relate.",
        "$$ found out air is actually important.",
        "$$'s last breath was... their last breath.",
        "$$ suffocated. Should've brought a spare lung.",
        "$$ forgot that breathing is not optional.",
        "$$ experienced a permanent air shortage."
    };

    private static readonly List<string> Mauling = new List<string>
    {
        "$$ was mauled beyond recognition.",
        "$$'s insurance doesn't cover mauling.",
        "$$ was torn apart like wrapping paper.",
        "$$ became an all-you-can-eat buffet.",
        "Something used $$ as a chew toy.",
        "$$ was violently disassembled.",
        "$$'s organs are now exterior decorations.",
        "$$ found out they're made of meat. The hard way."
    };

    private static readonly List<string> Gunshots = new List<string>
    {
        "$$ brought a body to a gunfight.",
        "$$ now has more holes than swiss cheese.",
        "$$ caught a bullet. With their everything.",
        "$$ was shot. Who gave these things guns?!",
        "$$ got ventilated.",
        "$$ found out they weren't bulletproof.",
        "$$ was used for target practice.",
        "$$ learned that bullets are not friends."
    };

    private static readonly List<string> Crushing = new List<string>
    {
        "$$ became two-dimensional.",
        "$$ was compacted for easy storage.",
        "$$ found out they're not load-bearing.",
        "$$ was crushed like a soda can.",
        "$$ experienced extreme compression.",
        "$$ is now a pancake.",
        "$$'s bones were turned into gravel.",
        "$$ was flattened. Permanently."
    };

    private static readonly List<string> Drowning = new List<string>
    {
        "$$ forgot how to swim.",
        "$$ is sleeping with the fishes.",
        "$$ tried to drink the entire ocean.",
        "$$'s lungs were not designed for water.",
        "$$ went for a swim. Didn't come back.",
        "$$ discovered they're not a fish.",
        "$$ drowned. Turns out water is wet AND deadly.",
        "$$ sank like a rock. Because they're not a boat."
    };

    private static readonly List<string> Abandoned = new List<string>
    {
        "$$ was left behind. Oops.",
        "The ship left without $$. That's rough.",
        "$$ was voted off the moon.",
        "Nobody waited for $$. Cold.",
        "$$ learned the ship waits for no one.",
        "$$'s coworkers chose profits over friendship.",
        "$$ was sacrificed for the quota.",
        "The Company doesn't do rescue missions for $$."
    };

    private static readonly List<string> Electrocution = new List<string>
    {
        "$$ became a lightning rod.",
        "$$'s hair is standing up. Forever.",
        "$$ found out watt happens when you touch that.",
        "$$ was deep fried.",
        "$$ got a shocking surprise.",
        "$$ conducted electricity. Poorly.",
        "$$'s nervous system got a factory reset.",
        "$$ was zapped into the shadow realm."
    };

    private static readonly List<string> Kicking = new List<string>
    {
        "$$ was kicked into next week.",
        "$$ got booted. Literally.",
        "$$ caught a foot to the everything.",
        "$$ was punted like a football.",
        "$$ found out legs are weapons too.",
        "Somebody dropkicked $$ into orbit.",
        "$$ was on the receiving end of a boot.",
        "$$ was yeeted by foot."
    };

    private static readonly List<string> Burning = new List<string>
    {
        "$$ is extra crispy.",
        "$$ was roasted. Not the funny kind.",
        "$$ achieved a nice golden brown.",
        "$$ discovered they're flammable.",
        "$$'s marshmallow impression was too convincing.",
        "$$ was cooked to perfection. Medium well.",
        "$$ spontaneously combusted. Probably.",
        "$$ was flame-broiled."
    };

    private static readonly List<string> Stabbing = new List<string>
    {
        "$$ was turned into a pincushion.",
        "$$ got the point. Several points actually.",
        "Something sharp disagreed with $$.",
        "$$ was perforated.",
        "$$ brought their torso to a knife fight.",
        "$$ was skewered like a kebab.",
        "$$ discovered a new hole in their body.",
        "$$ was impaled. Ouch doesn't cover it."
    };

    private static readonly List<string> Fan = new List<string>
    {
        "$$ became a smoothie.",
        "$$ was blended on the 'puree' setting.",
        "$$ walked into a fan. A big one.",
        "$$ was diced, sliced, and julienned.",
        "$$ tried to high-five a giant fan blade.",
        "$$ was turned into confetti by a fan.",
        "$$ found the industrial fan. It found them back.",
        "$$ was shredded. Not the cool guitar kind."
    };

    internal static string Get(CauseOfDeath cause, string username)
    {
        var pool = cause switch
        {
            CauseOfDeath.Unknown => Unknown,
            CauseOfDeath.Bludgeoning => Bludgeoning,
            CauseOfDeath.Gravity => Gravity,
            CauseOfDeath.Blast => Blast,
            CauseOfDeath.Strangulation => Strangulation,
            CauseOfDeath.Suffocation => Suffocation,
            CauseOfDeath.Mauling => Mauling,
            CauseOfDeath.Gunshots => Gunshots,
            CauseOfDeath.Crushing => Crushing,
            CauseOfDeath.Drowning => Drowning,
            CauseOfDeath.Abandoned => Abandoned,
            CauseOfDeath.Electrocution => Electrocution,
            CauseOfDeath.Kicking => Kicking,
            CauseOfDeath.Burning => Burning,
            CauseOfDeath.Stabbing => Stabbing,
            CauseOfDeath.Fan => Fan,
            _ => Unknown
        };

        return pool[_rng.Next(pool.Count)].Replace("$$", username);
    }
}
