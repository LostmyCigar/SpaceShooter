# Implementation

I start off with a _not as naive_ implementation this time. I already have a spaceshooter working, lets make it less shoot-y and more optimized-y. 

I started off with stipping away the old stuff in the enemies. No more movement, spawning in intervals, getting shot, etc. I now spawn 1 enemy when I press space and to make each enemy heavy they run a good old Thread.Sleep(1).  

I kept the pooling of bullets and enemies, even though they are not my primary focus. 
But as a side note the pooling works well, the only garbage generated in the game comes from Debug.Log and Coroutines (ignoring the first time we instantiate bullets and enemies).

We now have game about remove enemies to improve your Fps, amazing. 

![Gif](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/e671eda3-f2b2-4f7e-a8d3-cfe58add773f)

```
    void SpawnEnemy()
    {
        var enemy = GetEnemy();

        enemy.transform.position = GetRandomSpawnPoint();
        enemy.transform.localScale = transform.localScale * Random.Range(0.5f, 1.2f);

        _enemies.Add(enemy);
    }
```
 Im also updating enemies inside our spawner:
 ```
    private void Update()
    {
        if (!_shouldRunEnemies) return;

        foreach (Enemy enemy in _enemies)
        {
            enemy.UpdateEnemy();
        }
    }
```

We now have one goal in mind; optimize away our Thread.Sleep (by moving it away from the main thread). Incase things goes wrong I am ready to change our Sleepy calculations into this instead:

 ```
    public static float CalculatePi(int iterations)
    {
        float pi = 0.0f;
        for (int i = 0; i < iterations; i++)
        {
            pi += 4.0f * math.pow(-1, i) / (2 * i + 1);
        }
        return pi;
    }
 ```

### Profiling Pictures and Discoveries
All profiling is done in standalone builds (unless stated otherwise)

![1Sleepy](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/8354803b-2224-49d0-87e2-11e4d9944286)  
![2CalcPI](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/af10b393-c127-4f7a-95bc-c85499c13514)  

Here we have our two heavy lifters. I aded input to toggle between them so we can have a more "side by side"-comparision in the profiling session. 
Sleep is a lot more consistant and makes for easier testing. But as of writing this, I have not implemented Jobs yet and am worried that the Burst compiler might just "optmize" away our sleep.  
Also, Sleep seems to last for 1 millisecond longer than it should? Weird, but consistant so it doesnt really matter.  

Before multithreading I wanted to see if we could notice a difference by just using jobs.  
Jobs here are scheduled an then completed before scheduling the next one.  

![JobsVsUnemployed](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/50d4e2cd-91f7-4988-8771-3a2a66e36476)  
(This is in editor profiling)

Now the brust compiler is fantistic and so, but in my case its doing a too good job to be allowed. See when I turn on Burst Compiling it seems to realize that im not acually doing anything with my Pi calculations, and choses to just not do them at all. (Also it doesnt accept Thread.Sleep at all, good we have a backup then)
![4BurstCompilerRuiningMyStuff](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/6dae2a4c-151c-4004-a145-2309c5221cc0)  
Great for making video games, not so great for experimenting with optimization.

Now on to a bigger issue. When disabling the burstcompiler and profliling I get two very different results:  

 - Letting threads sleep gave back the expected results  
The main thread take a slightly shorter nap and the worker threads sleep (instead of being idle, a big difference)

![6MultiSleeping](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/60cc484d-fbb8-44bc-acb3-0bebdb9c5ce2)  ![7SingleSleeping](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/6dc36182-549c-40d3-9940-db829d24021a)  
Very good!

And while I could quit while im ahead, I'll show the sad truth instead 

 - Calculating Pi is significantly slower

![5SlowMulti](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/2a9b0432-fa6d-42b9-863a-7db9d77e2f75)  
![6PiSingleFast](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/8a7c3592-62af-4d59-9ef5-1d15065ba1ee)

Ive not succeded in understanding why this is. When playing without unitys profiler up I see improvements in the fps when multithreading, but as soon as I open the profiler the Fps gets significantly lower (both with and without jobs). If you know why this migth be, please do tell. 

Here is my non-profiling results:

![9OwnProfiling](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/d01c46a5-6d86-4df1-a85f-cd12a37a98c1)

![ProfilerWeirdness](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/bf3f2932-27e4-4b62-97a8-f54bc91bad1d)


# Updated

Its pretty understandable that there were a bunch of flaws in the description since, like many other students this course, had to learn this stuff from the internet. I am expecting you to fail this again since I havnt had found any time to fix it. I understand the flaws in implementation (notes and code) and will improve them for next time. As for the written part, maybe you can take the time as a -teacher- to give feedback or atleast point out where things are simply wrong. That part is slightly harder to do myself and going back to watch the lectures again is less than fruitful. I dont mind passive aggressive-ness as long as i can acually learn from it!

# SpaceShooter
 
Due to me working on a fulltime project I choose to not use ECS and instead only tried some optimizations using jobs and the burst compiler. Also please correct me if I am wrong on anything as I feel that my knowledge is a bit surface level :(

# Implementation

I started by creating the game with a "Naive" approach (I.E a using Mono OOP). Since I knew from the start of the project that i would try to push the game through the use of "Enemies" I decided to use pooling for the bullets right from the get-go. This way I could focus a little bit more on the performance of my Enemies since they would (hopefully) be the sole bottleneck in performance. Profiling confirmed my suspicions as it proved that running multiple thousands of enemies were slightly more demanding than 30 bullets. I started by pooling the enemies so that we no longer use instantiate() and destroy(), removing some GC overhead. I then tried changing how enemies work by having them use unitys job system and burst compiler using DoD. This is done by separating the behaviors of the enemies from the enemies themselves, moving them into structs that has an execute function (from the IJob interface). The jobs are then scheduled, something that is currently done through Enemy instance mono scripts, although should be moved and handled by a single enemy manager that keeps track of active enemies.

I tried to implement the Player logic in a way that is inspired by DoD (although in the end uses a OOP approach) by separating player behaviors such as shooting and moving into their own behaviors that we then contain in the same chunk of memory (although since im using a list for practicality i might be completely off here since im unsure how how lists store memory, using a Array is for sure the proper choice but it also not something im bothering with here) and running their logic. This is primarily a way to keep behaviors separate from each other to make a project more manageable and some improvement could be made from a performance standpoint such as making the behaviors into struct (that they already basically work as) as this would avoid allocating the classes on the heap. 

My primary optimization (and usage of DoD) was using unitys Job system. The Job system supports use of the burst compiler which in itself is a good optimization (when used) since it optimizes the code into native code and does not compile at runtime (From my understanding it technically compiles at the first instance of a scheduled job?). Burst compiler also does away with overhead such as exception handling and garbage collection. Another performance advantage with Jobs is the possibility to use multithreading. Although my project does not properly implement it the way it could (by using ScheduleParallel() ), It does use Schedule() which moves the work away from the main thread into a worker thread. Even if I were to implement multithreading I am unsure of the performance bonus it would gain since the calculations we are running in jobs are very light and we still need to move the data back into the main thread eventually. Jobs are, even if we are not using them to their full effect, at the very least cache friendly due to their DoD nature. Jobs make sure that the data has both Temporal and Spatial locality, thus making more efficient use of how a computer fetches memory from the cache.



# Notes

Full Naive implementation 
(IS OOP)

This includes pooling on bullets. With pooling the total bullets can't go over 30 due to destruction time and firerate. 

I’ll mainly try optimizing the game with the use of enemies (asteroids).

Only spawning at start:

Enemy size: 88b

5 asteroids: ~185fps
50 asteroids: ~180fps

500 asteroids:

![Naive500Ast](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/99b83ef9-5ebb-4b2d-92aa-5a91b25b8588)

 FPS ~140
 
 ![Naive500AstGCAloc](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/cae3bcc0-c96c-4357-9ff6-760c5161d8b0)

GC alloc: 0.7mb

Spawning takes 146ms 

![Naive5000Ast](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/bca46253-bc27-4b04-8a1f-4ce13fee7a58)

5000 asteroids: ~30fps

Comparison 5000 to 50000 enemies in memory 
 
![500to5000Compare](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/b236c119-5098-4602-a4d0-9897ea10fcc9)


Scared of whatever this is:

![ScaredOfThis](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/f1be5ffc-1cc2-40e4-ae1b-ab3af5f74e07)


Spawning with set intervals:

50 enemies every second:
(Around 100 enemies active at any time)

![50AstPerSec](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/c62e90c3-3f5b-42c8-9345-f445aa617f6a)



Fun little spike on one single enemy. (Why?)

![SingleEnemySpike](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/ebcf999f-5212-4b12-9435-c5f7a4628cb2)

With Enemies Pooled:

![50AstPerSecPooled](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/02c9109c-ba5e-4011-b0d0-4bc3d74f4d89)

Cant say i understand why pooling doesnt seem to have an impact unless im doing something wrong when releasing pooled enemies (Setting them inactive). But the allocation seems to be happening OnTriggerEnter. 


Update: 

Me being somewhat unfamiliar to the profiler have been profiling the editor so far and not standalone builds. This makes a lot of my comparisons so far useless since a big portion of GC allocation has been done by the editor it seems.


I have these odd spikes when running the standalone build:

![OddSpikes](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/9ca760ff-3444-4ac6-bedb-c5c8383f9c09)



With pooling so far all the GC aloc that is happening comes from coroutine calls (Expected since we are using new WaitForSeconds)

![CoroutineGCAloc](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/302cf13e-33a5-4816-b760-31297a770278)



Using jobs and burst compiler:


Spawning 500 enemies every second

![500PerSecBurst](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/a14f8ef9-3fc0-40b0-abad-b3579cc4ed79)


Spawning 50 enemies every second in editor profiling

![50AstPerSecBurst](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/aaf876b5-db8f-43a5-b53e-63f002be9527)

Standalone build

![50AstPerSecBurstStandalone](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/70245bac-6665-460a-9ef3-f69c4d78fc16)

~6 ms per frame (aprox 166 fps)

