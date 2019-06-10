using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Probabilities
{
    public static int GetRandomIndexFromWeights(int[] weights)
    {
        int total = 0;

        foreach (int weight in weights)
        {
            total += weight;
        }

        int selection = Random.Range(0, total);
        int probaCounter = 0;
        int counter = 0;

        foreach (int weight in weights)
        {
            probaCounter += weight;
            if (selection < probaCounter)
                return counter;

            counter++;
        }

        return 0;
    }
}
