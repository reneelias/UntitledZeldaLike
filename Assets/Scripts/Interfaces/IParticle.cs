using UnityEngine;

public interface IParticle
{
    bool Active {get;}
    // virtual void Activate(float spawnX, float spawnY, float lifeDuration, float xOffset, float yOffset, Color color);
    void Deactivate();
}
