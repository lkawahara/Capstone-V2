using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

//Version 2
public class Fighter_Script : MonoBehaviour
{
    //public GameObject punchCollider_LEFT;
    //public GameObject punchCollider_RIGHT;
    //public GameObject kickCollider_LEFT;
    //public GameObject kickCollider_RIGHT;
    //public bool inputControlled = false;
    //private Animator animator;

    //Set in Editor
    public float horizontalMovementForce;
    public bool voiceOn = true;
    public AudioClip[] sounds;
    //CONST
    private const float MAX_HEALTH = 100;
    private const float blockDamageDampeningPercent = 0.3f;
    //Player vars
    private float health;
    private int roundWins = 0;
    private int hurtCount = 0;
    //INTRODUCTION VARS
    private bool introduced = false;
    private bool inputLocked = true;
    private bool fighterActive = false; //fighter is not active untill the start of the round (after intro animation), fighter becomes inactive at end of round (after someone dies)
    private InputType lastInput = InputType.Null;
    private bool blockingEnabled = true;
    
    //dictionary that links an animation name with associated AttackInfo
        //will be used in animationevent
    //when animation starts, triggers event in player which will loop through child colliders
        //grabbing the ones that will be relevent to the animation (attack/block/etc)
    //when the animator is updated, will check the animation name that is currently playing
    //that animation will change the colliders in the attack info
        //affects all of the colliders linked in the dictionary
        //changing their type
        //the damage they should do

    //TODO get rid of concurrent lists
    //each CollisionBoxTypeChange is linked with an animation,
    //every animation will trigger an event (checkForCollisionBoxChange)
        //this method will check the animation name, grabbing it from this dictionary
        //and change all of the model's named colliders based on the entered change info
    //NOTE all colliders must be entered, or else the non-listed colliders will not change from the last animation
        //TODO add an initial change to the colliders to change them all to hurt
    #region Example
    //colliders change example:
    //fighter attacks
    //go from idle (all coll = hurt)
    //to active damage frame ***change*** (specific coll = damage) ***change***
    //to recovery frame ***change*** (all coll = hurt) ***change***
    //to idle (no change)

    //fighter gets hit
    //from recovery frame (all coll = hurt)
    //to injured frame (all colliders = invincible) to avoid extra hits/damage (time controlled in animation editor)
    //to idle (all coll = hurt)
    #endregion
    //control through events or through code?
    //through events: can set at very specific frame
    //through code: can set many easily in one location
    private Dictionary<string, List<CollisionBoxTypeChange>> animations;
    //TODO currently doesn't set non affected collisionboxes to hurt

    private void Start()
    {
        //animator = GetComponent<Animator>();
        //if (!animator)
        //{
        //    Debug.LogError("Fighter missing animator references");
        //}
        health = MAX_HEALTH;
        InitializeInputDictionary();
        InitializeAnimationEventDictionary();
    }

    private void InitializeAnimationEventDictionary() {
        animations = new Dictionary<string, List<CollisionBoxTypeChange>>();
        CollisionBox_Script[] allColliders = GetComponentsInChildren<CollisionBox_Script>();
        //movement
        animations.Add("block", new List<CollisionBoxTypeChange>() { new CollisionBoxTypeChange(CollisionType.block, 0, allColliders) });
        animations.Add("crouch", new List<CollisionBoxTypeChange>() { new CollisionBoxTypeChange(CollisionType.hurt, 0, allColliders) });
        animations.Add("jump", new List<CollisionBoxTypeChange>() { new CollisionBoxTypeChange(CollisionType.hurt, 0, allColliders) });
        animations.Add("forward", new List<CollisionBoxTypeChange>() { new CollisionBoxTypeChange(CollisionType.hurt, 0, allColliders) });
        animations.Add("back", new List<CollisionBoxTypeChange>() { new CollisionBoxTypeChange(CollisionType.hurt, 0, allColliders) });
        animations.Add("idle", new List<CollisionBoxTypeChange>() { new CollisionBoxTypeChange(CollisionType.hurt, 0, allColliders) });

        //attacks
        animations.Add("light_punch", new List<CollisionBoxTypeChange>() { new CollisionBoxTypeChange(CollisionType.damage, 5, new CollisionBox_Script[]{findCollider("left_hand")}) });
        animations.Add("medium_punch", new List<CollisionBoxTypeChange>() { new CollisionBoxTypeChange(CollisionType.damage, 5, new CollisionBox_Script[]{findCollider("right_hand")}) });
        animations.Add("light_kick", new List<CollisionBoxTypeChange>() { new CollisionBoxTypeChange(CollisionType.damage, 5, new CollisionBox_Script[]{findCollider("left_foot")}) });
        animations.Add("medium_kick", new List<CollisionBoxTypeChange>() { new CollisionBoxTypeChange(CollisionType.damage, 5, new CollisionBox_Script[] { findCollider("right_foot")}) });
    }

    private CollisionBox_Script findCollider(string name) {
        GameObject foundChild = this.transform.Find(name).gameObject;
        if (!foundChild) {
            Debug.LogError("could not find collider by name: " + name);
        }
        CollisionBox_Script ret = foundChild.GetComponent<CollisionBox_Script>();
        if (!ret) {
            Debug.LogError("found child doesnt have CollisionBox_Script attached; name: " + name);
        }
        return ret;
    }

    public void PlayAudioAtIndex(int index)
    {
        if (!voiceOn)
            return;

        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().clip = sounds[index];
        GetComponent<AudioSource>().Play();
    }

    #region AI STUFF
    //public bool attackingEnabled = true;
    //private float aiTimer = 0;
    //public float aiInputTimeLimit = 5.0f;
    //private void UpdateAI()
    //{
    //    //randomize input based on time
    //    //set animator value
    //    int animatorInputVal = -1;
    //    string[] enumNames = System.Enum.GetNames(typeof(InputType));
    //    int randIndex = (int)(Random.Range(0, enumNames.Length));

    //    aiTimer += Time.deltaTime;
    //    if (aiTimer > aiInputTimeLimit)
    //    {
    //        lastInput = (attackingEnabled) ? (InputType)(System.Enum.Parse(typeof(InputType), enumNames[randIndex])) : InputType.Null;
    //        if (lastInput == InputType.Jump)
    //            lastInput = InputType.Null;
    //        inputAnimatorValues.TryGetValue(lastInput, out animatorInputVal);
    //        aiTimer = 0;
    //    }
    //    animator.SetInteger("Anim_InputVal", animatorInputVal);
    //    //set the time until the next input is made (attack)
    //    //set random movement 
    //    float block = (blockingEnabled) ? 1.0f : 0.0f;
    //    animator.SetFloat("Anim_BlockAxisVal", block);

    //    //movement
    //    int horizontalTemp = Random.Range(-1, 1);
    //    TranslateModel(horizontalTemp);
    //}
    #endregion 
    
    #region Input Set Up
    //movement has priority over attacks,
    //light attacks have priority over medium attacks
    //punches have priority over kicks
    private enum InputType { Null, Crouch, Jump, LP, LK, MP, MK };
    private Dictionary<InputType, KeyCode> inputKeyCodes;
    //used for telling the animator what input was given the current frame, alternatively can be used for identifying move priority (lower the value, higher the priority)
    private Dictionary<InputType, int> inputAnimatorValues;
    private void InitializeInputDictionary()
    {
        inputKeyCodes = new Dictionary<InputType, KeyCode>();
        inputAnimatorValues = new Dictionary<InputType, int>();
        inputKeyCodes.Add(InputType.Null, KeyCode.None);
        inputKeyCodes.Add(InputType.Crouch, KeyCode.S);
        inputKeyCodes.Add(InputType.Jump, KeyCode.W);
        inputKeyCodes.Add(InputType.LP, KeyCode.O);
        inputKeyCodes.Add(InputType.LK, KeyCode.K);
        inputKeyCodes.Add(InputType.MP, KeyCode.P);
        inputKeyCodes.Add(InputType.MK, KeyCode.L);

        //inputs animator values
        int inputVal = -1;
        foreach (InputType input in System.Enum.GetValues(typeof(InputType)))
        {
            Debug.Log("Input: " + input.ToString() + "; val: " + inputVal);
            inputAnimatorValues.Add(input, inputVal);
            inputVal++;
        }
    }
    #endregion

    #region Update
    private void Update()
    {
        //if the intro is done
        if (fighterActive)
        {
            if (!inputLocked)
            {
                //handle input from last frame
                int animatorInputVal = -1;
                inputAnimatorValues.TryGetValue(lastInput, out animatorInputVal);
                //animator.SetInteger("Anim_InputVal", animatorInputVal);

                UpdateAnimator();
                UpdatePosition();
                UpdateInput();
            }
            UpdateHurtCount();
            UpdateColliders();
        }
    }

    /// <summary>
    /// Updates the animation state of the object based on input
    /// </summary>
    private void UpdateAnimator() {
        //animator.SetFloat("Anim_HorizontalAxisVal", Input.GetAxisRaw("Horizontal"));
        //animator.SetFloat("Anim_VerticalAxisVal", Input.GetAxisRaw("Vertical"));
        //animator.SetInteger("Anim_HurtCount", hurtCount);
        //animator.SetFloat("Anim_AttackAxisVal", Input.GetAxisRaw("Attack"));
        //animator.SetFloat("Anim_MediumAttackAxisVal", Input.GetAxisRaw("MediumAttack"));
            
        if (blockingEnabled) {
            //animator.SetFloat("Anim_BlockAxisVal", Input.GetAxisRaw("Block"));
        }
    }

    /// <summary>
    /// Updates position of gameObject based on horizontal input axis
    /// </summary>
    private void UpdatePosition() {
        int direction = (int)Input.GetAxisRaw("Horizontal");
        if (!IsInCrouchAnimation() && IsInIdleAnimation() || IsInMoveAnimation())
        {
            transform.Translate(new Vector3(0, 0, direction * horizontalMovementForce * Time.deltaTime));
        }
    }

    /// <summary>
    /// hurtCount should be reset when the player gets back up
    /// reset hurtCount if IsIdle()
    /// </summary>
    private void UpdateHurtCount()
    {
        if (IsInIdleAnimation())
        {
            hurtCount = 0;
        }
    }

    /// <summary>
    /// When the player goes back to the idle state, disable damage colliders
    /// </summary>
    private void UpdateColliders() {
        //if player is idle and not yet hit
        //disable damage colliders 
        if (IsInIdleAnimation())
        {
            //disableAllDamageColliders();
        }
    }

    /// <summary>
    /// Gets the input this frame based on keyDown() and updates the animatorsInputVal
    /// Input has priority: listed above
    /// if more than one key is pressed simultaneously, only the key with the highest priority will register (all other commands are lost)
    /// if no keys are pressed down, the animator's value is still updated except with a value of -1 (null)
    /// </summary>
    private void UpdateInput()
    {
        //iterate over each key
        foreach (InputType input in System.Enum.GetValues(typeof(InputType)))
        {
            if (input != InputType.Null) //skip KeyCode.None
            {
                KeyCode keyCode;
                inputKeyCodes.TryGetValue(input, out keyCode);

                //if key was just pressed this frame store the input that was received, and lock input until it is handled
                if (Input.GetKeyDown(keyCode))
                {
                    lastInput = input;
                    inputLocked = true;
                    //lock input once the command is received, lockInput
                    //input is recorded everytime the player presses a button (while the input is not locked) 
                    //input is locked until the start of each the round (so that the player cannot move before the notifications)
                    //input is then unlocked until a player dies
                    break;
                }
            }
        }
    }

    #endregion

    #region Reset Functions
    //player is only introduced right after selected: doesnt show intro animation more than once
    public void MatchReset()
    {
        //introduced = false;
        roundWins = 0;
        RoundReset();
    }

    public void RoundReset()
    {
        lastInput = InputType.Null;
        health = MAX_HEALTH;
        hurtCount = 0;
        ResetAnimatorParams();
        UnlockInput();
        //var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        //animator.CrossFade(stateInfo.nameHash, 0);
    }

    private void ResetAnimatorParams()
    {
        //animator.SetInteger("Anim_HurtCount", 0);
        //animator.SetFloat("Anim_VerticalAxisVal", 0);
        //animator.SetFloat("Anim_HorizontalAxisVal", 0);
        //animator.SetFloat("Anim_AttackAxisVal", 0);
        //animator.SetFloat("Anim_MediumAttackAxisVal", 0);
        //animator.SetInteger("Anim_InputVal", 0);
        //animator.SetFloat("Anim_BlockAxisVal", 0);
        //animator.SetBool("Anim_Introduced", !introduced);
    }
    #endregion

    //call to start the introduction animation
    /// <summary>
    /// Sets animator value, to start fighter's introduction animation
    /// the animation should call an event which signifies the introduction is called
    /// </summary>
    public void IntroduceFighter()
    {
        //introduction boolean is originally False
        //so this will set the animator val to True while the character is not yet introduced
        //if the character has already been introduced, if this method is called then the introduction animation will not be able to fire
        //i.e. !introduction = !true = false
        //animator.SetBool("Anim_Introduced", true); //change animation state to introduction animation, when finishes sets introduction to false
    }

    #region Public Accessors/Setters
    public bool IsIntroduced()
    {
        return (introduced);
    }


    public bool IsDead()
    {
        return (health <= 0);
    }

    public bool isIdle() {
        return IsInIdleAnimation();
    }

    public int GetNumWins()
    {
        return roundWins;
    }

    public float GetHealthPercentage()
    {
        return health / MAX_HEALTH;
    }

    public int GetHurtCount()
    {
        return hurtCount;
    }

    public bool UnlockInput()
    {
        return inputLocked = false;
    }

    public void SetActive(bool act)
    {
        fighterActive = act;
    }
    #endregion

    #region collider stuff
    //public void activateLPunchColliders(float damage)
    //{
    //    punchCollider_LEFT.GetComponent<CollisionBox_Script>().setActiveType(CollisionType.damage);
    //    punchCollider_LEFT.GetComponent<CollisionBox_Script>().setDamage(damage);
    //}

    //public void activateRPunchColliders(float damage)
    //{
    //    punchCollider_RIGHT.GetComponent<CollisionBox_Script>().setActiveType(CollisionType.damage);
    //    punchCollider_RIGHT.GetComponent<CollisionBox_Script>().setDamage(damage);
    //}

    //public void activateLKickColliders(float damage)
    //{
    //    kickCollider_LEFT.GetComponent<CollisionBox_Script>().setActiveType(CollisionType.damage);
    //    kickCollider_LEFT.GetComponent<CollisionBox_Script>().setDamage(damage);
    //}


    //public void activateRKickColliders(float damage)
    //{
    //    kickCollider_RIGHT.GetComponent<CollisionBox_Script>().setActiveType(CollisionType.damage);
    //    kickCollider_RIGHT.GetComponent<CollisionBox_Script>().setDamage(damage);
    //}


    ////call when the animation goes into the recovery frames
    ////this method will change all colliders back to hurt colliders and unlock the character input
    ////the input is locked when the player keys down a input key 
    ////the input is locked to ensure that input is handled properly before the player can input another command
    ////this method has to be called to ensure the input is unlocked 
    //public void disableAllDamageColliders()
    //{
    //    punchCollider_LEFT.GetComponent<CollisionBox_Script>().setActiveType(CollisionType.hurt);
    //    punchCollider_RIGHT.GetComponent<CollisionBox_Script>().setActiveType(CollisionType.hurt);
    //    kickCollider_LEFT.GetComponent<CollisionBox_Script>().setActiveType(CollisionType.hurt);
    //    kickCollider_RIGHT.GetComponent<CollisionBox_Script>().setActiveType(CollisionType.hurt);
    //    UnlockInput();
    //    lastInput = InputType.Null;
    //}

    //to be called by hurt boxes
    //called at any point in time
    //only called when a damage box collides with a hurt box, that damage box is then disabled preventing further calls
    //should only result in a maximum of 1 call per frame (shouldnt get hurt multiple times in one frame)
    public void GetHurt(float damageDealt)
    {
        if (isInBlockAnimation())
        {
            damageDealt *= blockDamageDampeningPercent;
        }
        health -= damageDealt;
        hurtCount++;
        if (health <= 0)
        {
            //Debug.Log("Player Died");
            health = 0;
            //animator.SetBool("Anim_Alive", false);
            fighterActive = false;
            ResetAnimatorParams();
        }
    }
    #endregion

    #region Animation state Queries
    /// <summary>
    /// NOTIMPLEMENTED gets idle state based on gameobject's current animation state
    /// </summary>
    private bool IsInIdleAnimation()
    {
        return true;
        //return (animator.GetCurrentAnimatorStateInfo(0).IsTag("Idle"));
    }

    /// <summary>
    /// NOTIMPLEMENTED gets moving state based on gameobject's current animation state
    /// </summary>
    private bool IsInMoveAnimation()
    {
        return false;
        //return animator.GetNextAnimatorStateInfo(0).IsTag("Move");
    }

    /// <summary>
    /// NOTIMPLEMENTED gets crouch state based on gameobject's current animation state
    /// </summary>
    private bool IsInCrouchAnimation()
    {
        return false;
        //Input.GetAxisRaw("Vertical");
    }

    /// <summary>
    /// NOT IMPLEMENTED Gets blocking state based on animation state
    /// </summary>
    /// <returns></returns>
    public bool isInBlockAnimation()
    {
        return false;
        //if (!blockingEnabled)
        //    return false;

        //return (animator.GetFloat("Anim_BlockAxisVal") == 1.0f);
    }
    #endregion

    #region AnimationEvents
    //Animation events can only take up to one optional parameter

    //damage box collides with the hurt box
    //player has been hit!
    //change animation state of hurt player
    //deal damage to hurt player's health

    //hurt box gets the information associated from the hit
    //transfer hit information to the player on update
    //update decreases health and initiates damage animation
    //from already damaged state, a player can be hit again (if the reaction hit animation is longer than the attack animation)
    //this would result in

    //damage box becomes inactive
    //player finishes his animation
    //hurt box becomes active

    //***ATTACK PROCESS***//
    //the player will get damaged when an opposing damage box collides with the player's hurt box
        //when the player is hit their hit Count is increased, the hit count will determine the next hit animation that is initiated
    //attacks have damage dealt
    //attacks can have effects (knockback, sweep, grab, push, etc)


    /// <summary>
    /// call at the end of the fighter's intro animation
    /// </summary>
    public void IntroAnimationEvent()
    {
        introduced = true;
        //was getting stuck on the frame
        //animator.SetBool("Anim_Introduced", false);
    }

    public void IdleAnimationEvent()
    {
        //beginning of idle animation:
        //  1) clear hurt count
        //  2) change state of all attached colliders to hurt
        //  3) (player input should already be unlocked)
        if (fighterActive)
        {
            //currently have very specific references to all of the colliders that could cause damage
            //I could have an array of damage colliders and then tag the objects with the appropriate damage name
            //this would make looping through them easier, but not make setting specific ones easier
            //disableAllDamageColliders();
            hurtCount = 0;
        }
    }

    //call during hit animations
    public void DecreaseHitCount()
    {
        hurtCount--;
        if (hurtCount < 0)
            hurtCount = 0;
    }

    //call at the end of the winDie animation
    public void EndRoundAnimationEvent()
    {
        ResetAnimatorParams();
    }

    public void WinLoseAnimation(bool won)
    {
        lastInput = InputType.Null;
        fighterActive = false;
        string animationName = "Base Layer.Swept";
        if (won)
        {
            animationName = "Base Layer.Victory";
            roundWins++;
        }

        //animator.CrossFade(animationName, 0);
    }
    
    #endregion

}