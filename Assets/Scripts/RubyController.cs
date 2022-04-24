using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public AudioSource BackgroundMusic;
    public AudioSource VictoryMusic;
    public AudioSource DefeatMusic;
    public float speed = 3.0f;
    public Text score;
    public Text stageTwo;
    public Text winText;
    public Text stageThree;
    public Text ammo;
    public int maxHealth = 5;
    public Text loseText;
    public GameObject projectilePrefab;
    public GameObject pickupPrefab;
    public GameObject hurtPrefab;
    private int scoreValue = 0;
    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip powerupMusic;
    public AudioClip ammoMusic;
    public static int level;
    public int health { get { return currentHealth; } }
    int currentHealth;
    private int ammoValue = 6;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    bool gameOver;
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Stage 1"))
        {
            level = 1;
        }
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Stage 2"))
        {
            level = 2;
        }
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Stage 3"))
        {
            level = 3;
        }
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        score.text = scoreValue.ToString();
        winText.text = "";
        currentHealth = maxHealth;
        loseText.text = "";
        stageTwo.text = "";
        stageThree.text = "";
        audioSource = GetComponent<AudioSource>();
        BackgroundMusic.loop = true;
        ammo.text = ammoValue.ToString();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "CollectibleAmmo")
        {
            ammoValue += 4;
            ammo.text = ammoValue.ToString();
            PlaySound(ammoMusic);
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "PowerUp")
        {
            speed = 7.0f;
            PlaySound(powerupMusic);
            Destroy(other.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.R))

        {

            if (gameOver == true)

            {

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene

            }

        }
        if (currentHealth == 0)
        {
            loseText.text = "You Lose! Press R to restart.";
            gameOver = true;
            speed = 0.0f;
            DefeatMusic.enabled = true;
            BackgroundMusic.enabled = false;
        }
        if (level == 2)
        {
            if (scoreValue == 5)
            {
                stageThree.text = "Talk to Jambi to visit stage 3!";
            }
        }
        if (level == 3)
        {
            if (scoreValue == 1)
            {
                winText.text = "You Win!  Game created by Sam Serafini! Press R to restart.";
                speed = 0.0f;
                BackgroundMusic.enabled = false;
                gameOver = true;
                VictoryMusic.enabled = true;
            }
        }
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {

            if (ammoValue > 0)
            {
                Launch();
                UpdateAmmo(-1);
            }

        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
            if (level == 1)
            {
                if (scoreValue == 5)
                {
                    SceneManager.LoadScene("Stage 2");
                }
            }
            if (level == 2)
            {
                if (scoreValue == 5)
                {
                    SceneManager.LoadScene("Stage 3");
                }
            }

        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
            GameObject hurtObject = Instantiate(hurtPrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            PlaySound(hitSound);
        }
        if (amount > 0)
        {
            GameObject pickupObject = Instantiate(pickupPrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }




        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }
    public void UpdateScore(int amount)
    {
        scoreValue += amount;
        score.text = scoreValue.ToString();
        if (level == 1)
        {
            if (scoreValue == 5)
            {
                stageTwo.text = "Talk to Jambi to visit stage 2!";
            }
        }
    }
    public void UpdateAmmo(int amount)
    {
        ammoValue += amount;
        ammo.text = ammoValue.ToString();

    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        PlaySound(throwSound);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
