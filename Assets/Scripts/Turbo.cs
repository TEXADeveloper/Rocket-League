using UnityEngine;

public class Turbo : MonoBehaviour
{
    [SerializeField] private float amount;
    [SerializeField] private Animator anim;
    [SerializeField] private float delay;
    bool active = true;
    float time = 0;
    void Update()
    {
        time += Time.deltaTime;
        if (time >= delay && !active)
        {
            active = true;
            anim.SetTrigger("Appear");
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag.Equals("Player") && !col.transform.parent.GetComponent<CarController>().FullTurbo()  && active)
        {
            col.transform.parent.GetComponent<CarController>().AddTurbo(amount);
            anim.SetTrigger("Disappear");
            active = false;
            time = 0;
        }
    }
}
