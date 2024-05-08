using UnityEngine;
using System.Linq;

public static class LayerCollisonMask
{
    static int[] cached_collisionMasks = Enumerable.Repeat(-1, 32).ToArray();

    static public LayerMask GetCollisionMask(int layer)
    {
        if (cached_collisionMasks[layer] == -1)
            cached_collisionMasks[layer] = BuildCollisionMask(layer);

        return cached_collisionMasks[layer];
    }

    static LayerMask BuildCollisionMask(int layer)
    {
        int layerMask = 0;

        for (int i = 0; i < 32; i++)
        {
            // Check if collision is ignored between the given layer and the current layer
            if (!Physics.GetIgnoreLayerCollision(layer, i))
            {
                // Add the current layer to the layer mask
                layerMask |= 1 << i;
            }
        }

        return layerMask;
    }
}
