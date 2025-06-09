using UnityEngine;

public interface IObjectPoolHandler<T>
{
    public ObjectPooler<T> pool { get; set; }
    public void InitPool();
}
public interface IObjectPoolHandler
{
}
