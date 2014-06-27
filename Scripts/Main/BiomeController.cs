using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BiomeController : MonoBehaviour {

    public int seed = 1234555;

    List<IBiomeGenerator> _biomes;

    void Awake()
    {
        _biomes = new List<IBiomeGenerator>();
        Random.seed = seed;
    }

	// Use this for initialization
    void Start()
    {
        foreach (Component c in transform.GetComponents<Component>())
        {
            if (c is IBiomeGenerator)
                _biomes.Add((IBiomeGenerator)c);
        }
    }

    public IBiomeGenerator GetBiome()
    {
        return _biomes[0];
    }
	
}
