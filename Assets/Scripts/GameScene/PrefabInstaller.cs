using UnityEngine;
using Zenject;

// ReSharper disable All

namespace GameScreen {
public class PrefabInstaller : MonoInstaller {
    [Header("Units")] 
    [SerializeField] GameObject warriorPrefab;
    [SerializeField] GameObject rangerPrefab;
    [SerializeField] GameObject artilleryPrefab;
    [Header("Zombie skeletons")]
    [Header("Level 1")]
    [SerializeField] GameObject zombieWarrior1;
    [SerializeField] GameObject zombieRanger1;
    [SerializeField] GameObject zombieArtillery1;
    [Header("Level 2")]
    [SerializeField] GameObject zombieWarrior2;
    [SerializeField] GameObject zombieRanger2;
    [SerializeField] GameObject zombieArtillery2;
    [Header("Level 3")]
    [SerializeField] GameObject zombieWarrior3;
    [SerializeField] GameObject zombieRanger3;
    [SerializeField] GameObject zombieArtillery3;
    [Header("Alien skeletons")]
    [Header("Level 1")]
    [SerializeField] GameObject alienWarrior1;
    [SerializeField] GameObject alienRanger1;
    [SerializeField] GameObject alienArtillery1;
    [Header("Level 2")]
    [SerializeField] GameObject alienWarrior2;
    [SerializeField] GameObject alienRanger2;
    [SerializeField] GameObject alienArtillery2;
    [Header("Level 3")]
    [SerializeField] GameObject alienWarrior3;
    [SerializeField] GameObject alienRanger3;
    [SerializeField] GameObject alienArtillery3;
    [Header("Shells")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject shellPrefab;
    [Header("Misc")]
    [SerializeField] GameObject cellPrefab;
    [SerializeField] GameObject explosion;

    public override void InstallBindings() {
        // unit prefabs
        bind(warriorPrefab, PrefabId.Warrior);
        bind(rangerPrefab, PrefabId.Ranger);
        bind(artilleryPrefab, PrefabId.Artillery);
        // zombie skeletons
        // level 1
        bind(zombieWarrior1, UnitAnimatorId.ZombieWarrior1);
        bind(zombieRanger1, UnitAnimatorId.ZombieRanger1);
        bind(zombieArtillery1, UnitAnimatorId.ZombieArtillery1);
        // level 2
        bind(zombieWarrior2, UnitAnimatorId.ZombieWarrior2);
        bind(zombieRanger2, UnitAnimatorId.ZombieRanger2);
        bind(zombieArtillery2, UnitAnimatorId.ZombieArtillery2);
        // level 3
        bind(zombieWarrior3, UnitAnimatorId.ZombieWarrior3);
        bind(zombieRanger3, UnitAnimatorId.ZombieRanger3);
        bind(zombieArtillery3, UnitAnimatorId.ZombieArtillery3);
        // alien skeletons
        // level 1
        bind(alienWarrior1, UnitAnimatorId.AlienWarrior1);
        bind(alienRanger1, UnitAnimatorId.AlienRanger1);
        bind(alienArtillery1, UnitAnimatorId.AlienArtillery1);
        // level 2
        bind(alienWarrior2, UnitAnimatorId.AlienWarrior2);
        bind(alienRanger2, UnitAnimatorId.AlienRanger2);
        bind(alienArtillery2, UnitAnimatorId.AlienArtillery2);
        // level 3
        bind(alienWarrior3, UnitAnimatorId.AlienWarrior3);
        bind(alienRanger3, UnitAnimatorId.AlienRanger3);
        bind(alienArtillery3, UnitAnimatorId.AlienArtillery3);
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

public enum UnitAnimatorId {
    ZombieWarrior1,
    ZombieRanger1,
    ZombieArtillery1,
    AlienWarrior1,
    AlienRanger1,
    AlienArtillery1,
    ZombieWarrior2,
    ZombieRanger2,
    ZombieArtillery2,
    AlienWarrior2,
    AlienRanger2,
    AlienArtillery2,
    ZombieWarrior3,
    ZombieRanger3,
    ZombieArtillery3,
    AlienWarrior3,
    AlienRanger3,
    AlienArtillery3,
}
}