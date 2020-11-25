using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public int playerNumber;
    public float speed;
    public bool ableToMove;
    public float force;
    public GameObject swingPivot;
    [SerializeField] GameObject punch = null;
    public int hp;
    public int startingHp;
    public Action<PlayerController> OnDeath;
    CharacterController cController;
    Animator animator;
    Vector3 dir;
    Vector3 rot;
    public float rotSpeed;
    string playerAxisX;
    string playerAxisY;
    string playerAxisHit;
    string playerAxisInteract;
    Item grabbedItem;
    public List<Item> itemsInRange = new List<Item>();
    float distOnGround = 0.3f;
    [Header("Physics")]
    [SerializeField] Vector3 velocity = Vector3.zero;
    [SerializeField] Vector3 momentum = Vector3.zero;
    [SerializeField] float drag = 0;
    [Header("Gravity related")]
    [SerializeField] bool isGrounded = false;
    [SerializeField] float baseGravity = 0;
    [SerializeField] float gravityAcceleration = 0;
    [SerializeField] float currentGravity = 0;
    [Space]
    LayerMask groundLayer;
    float punchTime;
    float swingTime;
    [SerializeField] float immunityTime = 0;
    [SerializeField] bool immune = false;
    Coroutine pushedCor;
    public Action UpdateUI;
    public int modelIndex = 0;
    [SerializeField] GameObject[] availableModels = null;
    [SerializeField] Sprite[] availableSprites = null;
    [SerializeField] Image worldspaceImg = null;

    string damageRecieveSound;
    Vector3 startingPos;

    public enum State
    {
        idle,
        walking,
        punching,
        carrying
    }
    public State currentState;

    private void Awake()
    {
        startingHp = hp;   
    }
    void Start()
    {
        GameObject modelGo = Instantiate(availableModels[modelIndex], transform.position, Quaternion.identity ,transform);
        PlayerGFX model = modelGo.GetComponent<PlayerGFX>();
        if (modelIndex <= 1) damageRecieveSound = "Recieve_duck";
        else damageRecieveSound = "Recieve_frog";
        GetComponentInChildren<ImpactCollider>().gameObject.layer = LayerMask.NameToLayer("ImpactColl" + playerNumber);
        worldspaceImg.sprite = availableSprites[playerNumber-1];
        swingPivot = model.swingPivot;
        punch = model.punch;
        startingPos = transform.position;
        cController = GetComponent<CharacterController>();
        playerAxisX = "HorizontalP" + playerNumber;
        playerAxisY = "VerticalP" + playerNumber;
        playerAxisHit = "HitP" + playerNumber;
        playerAxisInteract = "Grab-ThrowP" + playerNumber;
        animator = GetComponentInChildren<Animator>();
        currentState = State.idle;
        groundLayer = LayerMask.GetMask("Ground");
        currentGravity = baseGravity;
        GetAnimationTimes(); //se reemplaza por animation events mmmm o no
    }

    void Update()
    {    
        float hor;
        float ver;
        isGrounded = Physics.CheckSphere(transform.position, distOnGround, groundLayer);
        hor = Input.GetAxis(playerAxisX);
        ver = Input.GetAxis(playerAxisY);
        rot = new Vector3(Input.GetAxis(playerAxisX), 0, Input.GetAxis(playerAxisY));
        dir = new Vector3(hor, 0, ver);
        if (Input.GetButtonDown(playerAxisHit) && currentState != State.punching)
        {
            if (currentState == State.carrying)
                StartCoroutine(Swing());
            else
                StartCoroutine(Hit());
        }
        if (Input.GetButtonDown(playerAxisInteract))
        {
            if (currentState == State.idle)
            {
                Item closestAvaiableItem = GetClosestAvaiableItem();
                if (closestAvaiableItem)
                {
                    currentState = State.carrying;
                    grabbedItem = GetClosestAvaiableItem();
                    grabbedItem.transform.parent = swingPivot.transform;
                    grabbedItem.SetAsGrabbed(this);
                    animator.SetBool("carrying", true);
                    AkSoundEngine.PostEvent("Pick", gameObject);
                }
            }
            else if (currentState == State.carrying)
            {
                currentState = State.idle;
                grabbedItem.Throw();
                animator.SetBool("carrying", false);
                AkSoundEngine.PostEvent("Throw", gameObject);
            }
        }
    }


    private void FixedUpdate()
    {
        if (isGrounded)
        {
            currentGravity = baseGravity;
        }
        else
        {
            currentGravity += gravityAcceleration * Time.fixedDeltaTime;
        }
        velocity = Vector3.zero;
        velocity += momentum;
        if (currentGravity > 0)
        {
            velocity += (new Vector3(0.0f, -currentGravity));
            //ableToMove 
        }
        cController.Move(velocity * Time.fixedDeltaTime);
        if (ableToMove && dir != Vector3.zero)
        {
            cController.Move(Vector3.ClampMagnitude(dir, 1) * speed * Time.fixedDeltaTime);
            if (rot != Vector3.zero) transform.forward = dir.normalized;
            animator.SetFloat("walkingSpeed", dir.magnitude);
            // transform.forward = Vector3.Lerp(transform.forward,rot,Time.deltaTime * rotSpeed).normalized; //poner axis raw y un lerp en la rotacion en vez de esta wea y mover hacia dir en vez de forward
        }
        else
        {
            animator.SetFloat("walkingSpeed", 0);
        }
        momentum = Vector3.zero;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, distOnGround);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!immune)
        {
            if (other.gameObject.CompareTag("Punch") && other.gameObject != punch)
            {
                Vector3 funElevation = new Vector3(0.0f, 0.35f);
                PlayerController hitBy = other.transform.parent.parent.parent.parent.parent.gameObject.GetComponent<PlayerController>(); //desde el "Punch" busco al PlayerController
                Vector3 dir = transform.position - hitBy.transform.position;
                pushedCor = StartCoroutine(Pushed((dir.normalized + funElevation) * hitBy.force));
                hp -= (int)hitBy.force;
                if (hp <= 0)
                {
                    hp = 0;
                    OnDeath(this);
                }
                UpdateUI();
                AkSoundEngine.PostEvent(damageRecieveSound, gameObject);
                StartCoroutine(ImmunityTime());
            }
        }
    }

    public void OnItemCollision(Item hitBy)
    {
        if (!immune)
            if (hitBy.playerGrabbing && hitBy.playerGrabbing != this)
            {
                Vector3 dir;
                Vector3 horizontalDir;
                bool meleeHit = false;
                if (hitBy.itemState == Item.State.midAir)
                {
                    dir = transform.position - hitBy.transform.position;
                    Debug.Log("Golpeado a distancia");
                }
                else 
                {
                    dir = transform.position - hitBy.playerGrabbing.transform.position;
                    Debug.Log("Golpeado a melee");
                    meleeHit = true;
                }
                hitBy.GetDamaged();
                horizontalDir = Vector3.Project(dir, new Vector3(dir.x, 0, dir.z));
                pushedCor = StartCoroutine(Pushed(horizontalDir.normalized * hitBy.playerGrabbing.force));//rb.AddForce(horizontalDir.normalized * hitBy.playerGrabbing.force);
                Debug.Log("hit");
                hp -= (int)(hitBy.playerGrabbing.force * hitBy.damageMultiplier);
                if (!meleeHit) hitBy.SetAsGrabbable();
                if (hp <= 0)
                {
                    hp = 0;
                    OnDeath(this);
                }
                UpdateUI();
                AkSoundEngine.PostEvent(damageRecieveSound, gameObject);
                StartCoroutine(ImmunityTime());
            }
    }
    IEnumerator Pushed(Vector3 initImpulse)
    {
        Vector3 impulse = initImpulse;
        float t = 0.0f;
        while (impulse != Vector3.zero)
        {
            t += drag * Time.deltaTime;
            impulse = Vector3.Lerp(impulse, Vector3.zero, t);
            momentum += impulse;
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator Hit()
    {
        currentState = State.punching;
        animator.SetTrigger("hit");
        yield return new WaitForSeconds(punchTime);
        currentState = State.idle;
    }

    IEnumerator Swing()
    {
        int itemOrigLayer = grabbedItem.gameObject.layer;

        currentState = State.punching;
        animator.SetTrigger("hit");
        
        grabbedItem.gameObject.layer = LayerMask.NameToLayer("ItemColl" + playerNumber);

        yield return new WaitForSeconds(swingTime);
 
        grabbedItem.gameObject.layer = itemOrigLayer;
        currentState = State.carrying;
        CheckItemBreak();
    }

    IEnumerator ImmunityTime()
    {
        immune = true;
        yield return new WaitForSeconds(immunityTime);
        immune = false;
    }

    Item GetClosestAvaiableItem()
    {
        int closest = -1;
        for (int i = 0; i < itemsInRange.Count; i++)
        {
            if (itemsInRange[i].playerGrabbing == null)
            {
                closest = i;
                break;
            }
        }
        if (closest == -1) return null;
        for (int i = closest + 1; i < itemsInRange.Count; i++)
        {
            if (!itemsInRange[i].playerGrabbing && Vector3.Distance(itemsInRange[i].transform.position, transform.position) < Vector3.Distance(itemsInRange[closest].transform.position, transform.position))
                closest = i;
        }
        return itemsInRange[closest];
    }
    void GetAnimationTimes() // esto se reemplaza por animation events o tal vez no :S
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < clips.Length; i++)
        {
            switch (clips[i].name)
            {
                case "Punching":
                    punchTime = clips[i].length;
                    break;
                case "Swing":
                    swingTime = clips[i].length;
                    break;
            }
        }
    }
    public void ResetPlayer()
    {
        cController.enabled = false;
        transform.position = startingPos;
        cController.enabled = true;
        hp = startingHp;
        momentum = Vector3.zero;
        if (pushedCor != null) StopCoroutine(pushedCor);
        UpdateUI();
    }

    public void CheckItemBreak()
    {
        if (grabbedItem.transform.parent == swingPivot.transform)
            return;
        currentState = State.idle;
        animator.SetBool("carrying", false);
    }
}
