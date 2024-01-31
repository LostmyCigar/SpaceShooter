If something is inaccurate, please tell me WHAT and WHY. Receiving the "feedback" "stuffs wrong, redo" did not help much.

Sidenote: Sorry I did not bother to resize the picutres here, it should still be readable though 

# Multithreading and Jobs

This time around I decided to spend my time on multithreading. 

Multithreading is making use of the fact that computers can compute multiple threads over its different cores at the same time. By dividing our code into diffrent threads, the OS can run the threads in separate cores, processing the threads in parallel and thus decreasing our games processing time. 

Unitys job system works to make it easy for developers to multithread without risking running into some of the more common issues that may occur when doing so. The Job system works by creating jobs rather than threads, putting them in a job queue and then letting worker threads pick them up and execute them. The job system handles 1 worker thread per available logical core (approximatly) to avoid context switching (Basicly the OS putting a stop to our thread and let other threads through, to then come back to our thread later. This makes it so that a single thread doesnt grind a core to a halt). Processing data in parallel means that we do not have the same control as a single thread would. If worker thread one want to work on the same data as worker thread two we might run into bugs due to threads overwriting eachother while processing the data, something that would not happen if we worked in linear fashion on a single thread. The job system solves this by only letting jobs work with copies instead of references. This way data get encapsulated by jobs and only readable by other threads before or after being modified (and sent back to the main thread). But jobs encapsulating their work also means we cannot get the data back from them. This is where Native Containers are comes in. Natived Containers exists in shared memory and work as the bridge between our main thread and worker threads. If multiple jobs are working in the same Native Container it should throw an error (I have not tried this myself) as we are back to risking race conditions (aka reading and writing to the same data). Native Containers can have ownership transfered between jobs as a way to handle dependancies, although jobs that work on the same native container can understandably not work in parallel (again, race conditions). 

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
The way it looks its like the same calculation just gets a lot heavier for no reason (going from a consistant 2ms up to as much as 20). There must be something Ive missed but I have not been able to figure it out. Im at the point where I have deemed it some low level voodoo/curse cast on me by a compiler witchdoctor or OS wizard

Here is my non-profiling results:

![9OwnProfiling](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/d01c46a5-6d86-4df1-a85f-cd12a37a98c1)

![ProfilerWeirdness](https://github.com/LostmyCigar/SpaceShooter/assets/60781151/bf3f2932-27e4-4b62-97a8-f54bc91bad1d)

The same drop happens when running Sleep, but to a way smaller extent and does not affect Jobs more than it does our regular implementation (the way it does with PI). 



