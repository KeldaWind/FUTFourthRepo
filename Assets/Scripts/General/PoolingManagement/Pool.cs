using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool<T> where T : MonoBehaviour
{
    [SerializeField] protected T objectPrefab;
    [SerializeField] protected Transform poolParent = null;
    [SerializeField] protected int initialPoolSize = 5;
    protected Queue<T> objectsQueue;

    public virtual void SetUpPool()
    {
        objectsQueue = new Queue<T>();

        for(int i = 0; i < initialPoolSize; i++)
        {
            T instantiatedObj = Object.Instantiate(objectPrefab, poolParent);
            objectsQueue.Enqueue(instantiatedObj);
            instantiatedObj.gameObject.SetActive(false);
        }
    }

    public T PeekPrefab()
    {
        return objectPrefab;
    }

    public T PeekObjectFromPool()
    {
        CheckForQueueEmpty();
        return objectsQueue.Peek();
    }

    public T GetObjectFromPool()
    {
        CheckForQueueEmpty();

        return objectsQueue.Dequeue();
    }

    public void AddObjectInPool(T objToAdd)
    {
        objectsQueue.Enqueue(objToAdd);
    }

    public virtual void CheckForQueueEmpty()
    {
        if(objectsQueue.Count < 1)
        {
            T instantiatedObj = Object.Instantiate(objectPrefab, poolParent);
            objectsQueue.Enqueue(instantiatedObj);
            instantiatedObj.gameObject.SetActive(false);
        }
    }
}
