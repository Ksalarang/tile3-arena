using UnityEngine;
using Zenject;

// ReSharper disable All

namespace GameScreen {
public class PrefabInstaller : MonoInstaller {
    [Header("Units")]
    [SerializeField] GameObject warriorPrefab;
    [SerializeField] GameObject rangerPrefab;
    [SerializeField] GameObject artilleryPrefab;
    [Header("Shells")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject shellPrefab;
    [Header("Misc")]
    [SerializeField] GameObject cellPrefab;
    [SerializeField] GameObject explosion;

    public override void InstallBindings() {
        // units
        bind(warriorPrefab, PrefabId.Warrior);
        bind(rangerPrefab, PrefabId.Ranger);
        bind(artilleryPrefab, PrefabId.Artillery);
        // shells
        bind(bulletPrefab, PrefabId.Bullet);
        bind(shellPrefab, PrefabId.Shell);
        // misc
        bind(cellPrefab, PrefabId.UnitCell);
        bind(explosion, PrefabId.Explosion);
    }
    
    void bind<T>(T instance) {
        Container.BindInstance(instance);
    }
    
    void bind<T>(T instance, object id) {
        Container.Bind<T>().WithId(id).FromInstance(instance);
    }
}

public enum PrefabId {
    Warrior,
    Ranger,
    Artillery,
    Bullet,
    Shell,
    UnitCell,
    Explosion,
}
}