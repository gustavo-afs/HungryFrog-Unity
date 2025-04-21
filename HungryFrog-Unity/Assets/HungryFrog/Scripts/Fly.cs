using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Fly : MonoBehaviour
{
    [SerializeField] private float flySpeed = 1f;
    [SerializeField] private float flyRange = 5f;
     
    ObjectPool<GameObject> fliesPool;
    public void Catched()
    {
        Die();
    }

    public void StartFly(Vector3 position, float time, ObjectPool<GameObject> pool)
    {
        this.transform.position = position;
        StartCoroutine(StartLifeTime(time));
        fliesPool = pool;
    }

    public IEnumerator StartLifeTime(float time)
    {
        float counter = 0f;
        bool isIdle = true;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos;
        Vector3 direction;

        while (counter < time)
        {
            if (isIdle)
            {
                float offsetX = (Random.value < 0.5f)
                    ? Random.Range(-flyRange, -1)
                    : Random.Range(2, flyRange + 1);

                float offsetY = (Random.value < 0.5f)
                    ? Random.Range(-flyRange, -1)
                    : Random.Range(2, flyRange + 1);

                startPos = transform.position;
                targetPos = (startPos + new Vector3(offsetX, offsetY, 0f))*-1;

                direction = targetPos - startPos;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);

                isIdle = false;
            }
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * flySpeed);
            
            if (Vector3.Distance(transform.position, targetPos) < 0.8f)
            {
                isIdle = true;
            }

            counter += Time.deltaTime;
            yield return null;
        }

        Die();
    }

    private void Die()
    {
        StopAllCoroutines();
        fliesPool.Release(this.gameObject);
    }
    
}
