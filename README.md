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

![Naive500Ast](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/b9bbdf08-0081-4f72-9d3f-0093b73c5143)

 FPS ~140
 
![Naive500AstGCAloc](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/886e368c-b72d-462b-bd75-c53758eab558)

GC alloc: 0.7mb

Spawning takes 146ms 

![Naive5000Ast](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/37aea58a-6e41-46ed-a56c-a8d7b622b158)

5000 asteroids: ~30fps

![Naive5000Ast](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/dfdc2a46-f530-45de-9cc9-a1c68e0c98ba)

50000 asteroids: ~0.9fps

 Comparison 5000 to 50000 enemies in memory 
 
 ![500to5000Compare](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/86ac4f74-b982-4277-b1f3-248f3d56fbb4)


Scared of whatever this is:

![ScaredOfThis](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/3f6dc11a-32e7-4366-b113-3af45881b36f)



Spawning with set intervals:

50 enemies every second:
(Around 100 enemies active at any time)

![50AstPerSec](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/c67dcb8b-5ef3-416b-b442-225a310d7388)




Fun little spike on one single enemy. (Why?)

![SingleEnemySpike](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/35411d33-778c-4939-a9c5-c6fc12fb21aa)

With Enemies Pooled:

![50AstPerSecPooled](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/ba619034-893a-4ffb-b175-8b57dac0aab3)

Cant say i understand why pooling doesnt seem to have an impact unless im doing something wrong when releasing pooled enemies (Setting them inactive). But the allocation seems to be happening OnTriggerEnter. 


Update: 

Me being somewhat unfamiliar to the profiler have been profiling the editor so far and not standalone builds. This makes a lot of my comparisons so far useless since a big portion of GC allocation has been done by the editor it seems.


I have these odd spikes when running the standalone build:

![OddSpikes](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/47b37a49-d2da-43e9-ad78-290d63c16c96)



With pooling so far all the GC aloc that is happening comes from coroutine calls (Expected since we are using new WaitForSeconds)


![CoroutineGCAloc](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/44079684-5a35-4d8c-a2dd-e928acb1ca08)



Using jobs and burst compiler:


Spawning 500 enemies every second

![500PerSecBurst](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/779be01c-172f-4688-a5c5-5f565ad267cf)


Spawning 50 enemies every second in editor profiling

![50AstPerSecBurst](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/fbd6b778-a1e0-4930-8bd0-812f2e4f6f45)

Standalone build

![50AstPerSecBurstStandalone](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/d3f3db73-989b-4fc0-a244-2791715c825e)

~6 ms per frame (aprox 166 fps)

