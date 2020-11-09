using TMPro.EditorUtilities;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Rocket : MonoBehaviour
{

    [SerializeField]
    float rcsThrust = 100f;
    [SerializeField]
    float mainThrust = 100f;
    [SerializeField]
    int reloadTime = 3;
    [SerializeField]
    int loadDelay;



    [SerializeField]
    AudioClip mainEngine;
    [SerializeField]
    AudioClip levelFinish;
    [SerializeField]
    AudioClip death;
    [SerializeField]
    ParticleSystem mainEngineParticles;
    [SerializeField]
    ParticleSystem successParticles;
    [SerializeField]
    ParticleSystem deathParticles;


    AudioSource audioSource;
    Rigidbody rigidBody;

    bool isTransitioning = false;
    bool collisionsDisabled = false;

    void Start()
    {

        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(!isTransitioning)
        {
            RespondToRotateInput();
            RespondToThrustInput();
        }
        if(Debug.isDebugBuild)
        {
            RespondToDebug();
        }    
        
    }

    void RespondToRotateInput()
    {
        rigidBody.angularVelocity = Vector3.zero;

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        { 
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

    }

    void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            StopApplyingThrust();
        }
    }

    private void StopApplyingThrust()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();


    }

    private void OnCollisionEnter(Collision collision)
    {
        if(isTransitioning || collisionsDisabled)
        {
            return;
        }
        
        switch(collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;

        }
    }


    private void StartSuccessSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(levelFinish);
        successParticles.Play();
        Invoke("LoadNextScene", loadDelay);
        
    }
    private void StartDeathSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("ReloadGame", reloadTime);
    }


    void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if(nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
        
    }
    void ReloadGame()
    {
        SceneManager.LoadScene(0);
    }

    void RespondToDebug()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
        else if(Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled;
        }
    }


}



