using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IDamageable
{
    // this one shot blast is for rocket launcher and false is for machine guns and smaller damaging assets
    public void Damage(float damageAmount, bool oneShotBlast = true);
}
