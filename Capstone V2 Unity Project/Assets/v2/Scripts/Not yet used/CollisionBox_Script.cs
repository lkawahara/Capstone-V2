using UnityEngine;
using System.Collections;

public enum CollisionType
{
    hurt,
    damage,
    block,
    //invincible
};

public class CollisionBox_Script : MonoBehaviour
{
    #region How Collision Boxes
    //fighters start off with collision boxes in hurt state (they can get hurt when intercepted by a damage collider)
    //when a fighter makes an attack the collision boxes remain in hurt state untill the damage frame of the animation plays
    //at this point the collision boxes are set to damage (they cannot be hurt, but can hurt the other player)
    //when the attack recedes, the collision boxes change their active type to hurt (allowing them the player still in recovery frames of the attack animation to get hurt)
    //When the player blocks: sets their collision boxes to block (dampening the amount of damage they receive)
    #endregion

    #region HOW DAMAGE WORKS
    //different attacks can do different amounts of damage
        //i.e. medium punch vs light punch
    //uses same collider but with different damage value
    //animation determines the attack (so set the damage value as an animationEvent)
    #endregion
   
    private Fighter_Script fighter;
    private CollisionType activeType = CollisionType.hurt;
    private float damage = 5;

    public void Start()
    {
        if (!GetComponent<Fighter_Script>())
        {
            fighter = gameObject.GetComponentInParent<Fighter_Script>();
        }
        else {
            Debug.LogError("No fighterscript found on this gameobject");
        }
    }

    public float getDamage()
    {
        return damage;
    }
    public void setDamage(float dam)
    {
        damage = dam;
    }

    public CollisionType getActiveType()
    {
        return activeType;
    }
    public void setActiveType(CollisionType newType)
    {
        activeType = newType;
    }

    public void setCollisionBox() { 
        //convenience method to use as animationEvent to be called when an attack animation is issued
            
            //player will have a map of attacks and collider names that will be activated and changed to a type when an attack goes through
        //i.e. block (change all colliders to block)
        //i.e. mediumPunch (change punch collider to attack with certain damage)

        //when attack goes through
        //
    }

    public void Update()
    {
        //disable attack state if the player has been hit
        if (fighter.GetHurtCount() > 0 && activeType != CollisionType.hurt)
        {
            activeType = CollisionType.hurt;
        }

        if (fighter.isInBlockAnimation())
        {
            activeType = CollisionType.block;
        }
    }


   

    //hurtboxes check for incoming collisions
    //when two boxes collide only the hurt box reacts:
    //the hurt box takes in the damage info and hurts the parent fighter (changing the hitThisFrame value)
        //the fighter then updates his information
    //from this point on all other collision boxes tied to the hurt player should be turned into hurt boxes
    void OnTriggerEnter(Collider collider)
    {
        CollisionBox_Script otherGuy = collider.gameObject.GetComponent<CollisionBox_Script>();
        
        if (!otherGuy) return;
        //possible collisions: 
        //hurt collide with damage
        //hurt collide with hurt
        //damage collide with damage
        //my hurt got hit by another damage box
        if ((activeType == CollisionType.hurt || activeType == CollisionType.block) && otherGuy.getActiveType() == CollisionType.damage && otherGuy.gameObject != this.gameObject)
        {
            //Debug.Log("Collision box receiving damage");

            //this player's hurt box is hit by an opposing 
            fighter.GetHurt(otherGuy.getDamage());
            //damage box of the other player no
            otherGuy.setActiveType(CollisionType.hurt);
        }

        if ((activeType == CollisionType.damage) && otherGuy.getActiveType() == CollisionType.damage && otherGuy.gameObject != this.gameObject)
        {
            //higher damage amount wins
            if (otherGuy.getDamage() > this.getDamage())
            {
                fighter.GetHurt(otherGuy.getDamage());
                otherGuy.setActiveType(CollisionType.hurt);
            }
        }
        //things to keep in mind
        //order of resolves between both players
        //both players collision enter will be called
    }
}