# SpaceShooter
 
Due to me working on a fulltime project I choose to not use ECS and instead only tried some optimizations using jobs and the burst compiler. Also please correct me if I am wrong on anything as I feel that my knowledge is a bit surface level :(

# Implementation

I started by creating the game with a "Naive" approach (I.E a using Mono OOP). Since I knew from the start of the project that i would try to push the game through the use of "Enemies" I decided to use pooling for the bullets right from the get-go. This way I could focus a little bit more on the performance of my Enemies since they would (hopefully) be the sole bottleneck in performance. Profiling confirmed my suspicions as it proved that running multiple thousands of enemies were slightly more demanding than 30 bullets. I started by pooling the enemies so that we no longer use instantiate() and destroy(), removing some GC overhead. I then tried changing how enemies work by having them use unitys job system and burst compiler using DoD. This is done by separating the behaviors of the enemies from the enemies themselves, moving them into structs that has an execute function (from the IJob interface). The jobs are then scheduled, something that is currently done through Enemy instance mono scripts, although should be moved and handled by a single enemy manager that keeps track of active enemies.

I tried to implement the Player logic in a way that is inspired by DoD (although in the end uses a OOP approach) by separating player behaviors such as shooting and moving into their own behaviors that we then contain in the same chunk of memory (although since im using a list for practicality i might be completely off here since im unsure how how lists store memory, using a Array is for sure the proper choice but it also not something im bothering with here) and running their logic. This is primarily a way to keep behaviors separate from each other to make a project more manageable and some improvement could be made from a performance standpoint such as making the behaviors into struct (that they already basically work as) as this would avoid allocating the classes on the heap. 

My primary optimization (and usage of DoD) was using unitys Job system. The Job system supports use of the burst compiler which in itself is a good optimization (when used) since it optimizes the code into native code and does not compile at runtime (From my understanding it technically compiles at the first instance of a scheduled job?). Burst compiler also does away with overhead such as exception handling and garbage collection. Another performance advantage with Jobs is the possibility to use multithreading. Although my project does not properly implement it the way it could (by using ScheduleParallel() ), It does use Schedule() which moves the work away from the main thread into a worker thread. Even if I were to implement multithreading I am unsure of the performance bonus it would gain since the calculations we are running in jobs are very light and we still need to move the data back into the main thread eventually. Jobs are, even if we are not using them to their full effect, at the very least cache friendly due to their DoD nature. Jobs make sure that the data has both Temporal and Spatial locality, thus making more efficient use of how a computer fetches memory from the cache.


# Updated

Its hard to say what exactly was wrong in my previous documentation since I didnt receive any feedback and everything I acually learned during the course had to come from the internet. That being said, ill try to improve it. 

Since its a bit clearer on how the implementation and profiling is lacking, that where ill start. 

To make sure i can get propper information from the profiler ill make things a bit more consistant:

Enemies no longer die (unless you shoot them), and we spawn them by pressing space.
They no longer move so we wont have to worry about that either.
Basicly im stripping the game into something very simple to profile.


A thread.Sleep() was added in the update method of the enemies to simulate lag. 



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

