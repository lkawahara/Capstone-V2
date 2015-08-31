using System.Collections;
using UnityEngine;

public class CollisionBoxTypeChange {

    private CollisionBox_Script[] colliders;
    private CollisionType type;
    private int damage;

    public CollisionBoxTypeChange(CollisionType type, CollisionBox_Script[] collisionBoxes)
    {
        this.type = type;
        this.damage = 0;
        colliders = collisionBoxes;
    }

    //will currently only affect 
    //provide 
    //set specific colliders to attack/block/invincible/etc
    //set all others to hurt
    public CollisionBoxTypeChange(CollisionType type, int damage, CollisionBox_Script[] collisionBoxes) {
        this.type = type;
        this.damage = damage;
        colliders = collisionBoxes;
    }
    
}
