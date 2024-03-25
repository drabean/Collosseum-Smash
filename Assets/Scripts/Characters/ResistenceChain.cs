using System;

public class ResistanceEffect
{
    public Func<bool, bool> Effect { get; set; }
    public ResistanceEffect NextEffect { get; set; }

    public ResistanceEffect(Func<bool, bool> effect)
    {
        Effect = effect;
        NextEffect = null;
    }
}

public class ResistanceChain
{
    private ResistanceEffect head;

    public ResistanceChain()
    {
        head = null;
    }

    public void AddResistanceEffect(Func<bool, bool> resistanceEffect)
    {
        var newEffect = new ResistanceEffect(resistanceEffect);

        if (head == null)
        {
            head = newEffect;
        }
        else
        {
            var currentEffect = head;
            while (currentEffect.NextEffect != null)
            {
                // Check for duplicate resistance effects in the chain
                if (currentEffect.NextEffect.Effect.Target == resistanceEffect.Target &&
                    currentEffect.NextEffect.Effect.Method == resistanceEffect.Method)
                {
                    // Avoid adding duplicate effects to the chain
                    return;
                }
                currentEffect = currentEffect.NextEffect;
            }
            currentEffect.NextEffect = newEffect;
        }
    }

    public void RemoveResistanceEffect(Func<bool, bool> resistanceEffect)
    {
        if (head == null)
        {
            // Chain is empty, nothing to remove
            return;
        }

        if (head.Effect.Target == resistanceEffect.Target &&
            head.Effect.Method == resistanceEffect.Method)
        {
            // Remove the head of the chain
            head = head.NextEffect;
            return;
        }

        var currentEffect = head;
        while (currentEffect.NextEffect != null)
        {
            if (currentEffect.NextEffect.Effect.Target == resistanceEffect.Target &&
                currentEffect.NextEffect.Effect.Method == resistanceEffect.Method)
            {
                // Found the matching effect, remove it from the chain
                currentEffect.NextEffect = currentEffect.NextEffect.NextEffect;
                return;
            }
            currentEffect = currentEffect.NextEffect;
        }
    }

    public bool ApplyResistance(bool resistanceSuccess)
    {
        // Apply the resistance effects in the chain sequentially
        var currentEffect = head;

        while (currentEffect != null)
        {
            // Apply the current resistance effect
            resistanceSuccess = currentEffect.Effect(resistanceSuccess);

            // Move to the next resistance effect in the chain
            currentEffect = currentEffect.NextEffect;
        }

        return resistanceSuccess;
    }
}