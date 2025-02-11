using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoveParticleEmitter : MonoBehaviour
{
    [SerializeField]
    Pet pet;
    ParticleSystem particle;

    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        pet.OnLove += Emit;
    }

    private void OnDisable()
    {
        pet.OnLove -= Emit;
    }

    private void Emit(int quantity)
    {
        particle.Play();
        particle.Emit(quantity);
        particle.Stop();
    }
}
