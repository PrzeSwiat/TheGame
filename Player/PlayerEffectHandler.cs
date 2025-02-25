﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Timers;

namespace TheGame
{
    internal class PlayerEffectHandler
    {
        private Player player;
        private IntervalTimer regenarationTimer;
        private IntervalTimer damageTimer;
        private NormalTimer stunTimer, speedTimer, powerTimer;
        private NormalTimer immortalTimer;

        int damage = 2;
        int HPregen = 0;

        public PlayerEffectHandler(Player player)
        {
            this.player = player;
            regenarationTimer = new IntervalTimer(5, 1, EffectAddHealth);
            damageTimer = new IntervalTimer(0, 1, EffectTakeHealth);
            stunTimer = new NormalTimer(0, undoStun);
            speedTimer = new NormalTimer(0, undoSpeed);
            powerTimer = new NormalTimer(0, undoStrenght);
            immortalTimer = new NormalTimer(0, undoImmortal);
        }

        public void Start()
        {
            damageTimer.Start();
        }

        public void Stop()
        {
            damageTimer.Stop();
        }

        // FUNKCJE PLAYEROWE

        public void DamagePlayer(int damage, int time)
        {
            this.damage = damage;
            damageTimer.setTimerMaxTime(time);
            damageTimer.Start();
        }

        public void RegenarateHP(int HP) // single use
        {
            player.AddHealth(HP);
        }

        public void RegenarateHP(int health, int time) //interwal
        {
            HPregen = health;
            regenarationTimer.setTimerMaxTime(time);
            regenarationTimer.Start();
        }

        public void Stun(int time) 
        {
            player.setcanMove(false);
            if (Globals.playerActiveEffects.Contains("stun"))
            {
                Globals.playerActiveEffects.Add("stun");
            }
            stunTimer.setTimerMaxTime(time);
            stunTimer.Start();
        }

        public void Haste(float speed, int time)
        {
            player.ActualSpeed += speed;
            speedTimer.setTimerMaxTime(time);
            speedTimer.Start();
            if (!Globals.playerActiveEffects.Contains("haste"))
            {
                Globals.playerActiveEffects.Add("haste");
            }
        }

        public void BuffStrenght(int DMG, int time)
        {
            player.Strength += DMG;
            powerTimer.setTimerMaxTime(time);
            powerTimer.Start();
            if (!Globals.playerActiveEffects.Contains("strenght"))
            {
                Globals.playerActiveEffects.Add("strenght");
            }
        }

        public void MakeImmortal(int time)
        {
            player.immortal = true;
            immortalTimer.setTimerMaxTime(time);
            immortalTimer.Start();
            damageTimer.Stop();
            if (!Globals.playerActiveEffects.Contains("immortal"))
            {
                Globals.playerActiveEffects.Add("immortal");
            }
        }


        // FUNKCJE DO TIMEROW
        private void undoSpeed()
        {
            player.setOriginalSpeed();
            Globals.playerActiveEffects.Remove("haste");
        }

        private void undoStun()
        {
            player.setcanMove(true);
            Globals.playerActiveEffects.Remove("stun");
        }

        private void undoStrenght()
        {
            player.setOriginalStrenght();
            Globals.playerActiveEffects.Remove("strenght");
        }

        private void undoImmortal()
        {
            player.immortal = false;
            damageTimer.Start();
            Globals.playerActiveEffects.Remove("immortal");
        }

        private void EffectAddHealth()
        {
            player.AddHealth(HPregen);
        }

        private void EffectTakeHealth()
        {
            player.Hit2(damage);
        }


        private class IntervalTimer
        {
            private Timer timer;
            private Action timerCallBack;
            private int maxTime;
            private int elapsedTime;

            public IntervalTimer(int maxTime, int intervalTime, Action timerCallBack)
            {
                timer = new System.Timers.Timer();
                timer.Interval = intervalTime * 1000;
                this.maxTime = maxTime * 1000;
                this.timerCallBack = timerCallBack;
                this.elapsedTime = 0;

                timer.Elapsed += Timer_Elapsed;
            }

            public void Start()
            {
                timer.Start();
            }

            public void Stop()
            {
                timer.Stop();
            }

            public void setTimerMaxTime(int maxTime)
            {
                this.maxTime = maxTime * 1000;
            }

            public void setTimerInterval(int interval)
            {
                this.timer.Interval = interval * 1000;
            }

            void Timer_Elapsed(object sender, ElapsedEventArgs e)
            {
                // Sprawdzenie, czy upłynął już maksymalny czas
                if (this.maxTime == 0)
                {
                    timerCallBack();
                }
                else
                {
                    if (this.elapsedTime >= this.maxTime)
                    {
                        // Zatrzymanie timera
                        this.elapsedTime = 0;
                        timer.Stop();
                    }
                    else
                    {
                        // Dodanie czasu interwału timera do zmiennej elapsedTime
                        this.elapsedTime += (int)timer.Interval;
                        timerCallBack();
                    }
                }

            }
        }


        private class NormalTimer
        {
            private Timer timer;
            private Action timerCallBack;
            private int maxTime;
            private int elapsedTime;

            public NormalTimer(int maxTime, Action timerCallBack)
            {
                timer = new System.Timers.Timer();
                this.maxTime = maxTime * 1000;
                this.timerCallBack = timerCallBack;
                this.elapsedTime = 0;

                timer.Elapsed += Timer_Elapsed;
            }

            public void Start()
            {
                timer.Start();
            }

            public void setTimerMaxTime(int maxTime) // maxTime w sekundach
            {
                this.maxTime = maxTime * 1000;
            }

            void Timer_Elapsed(object sender, ElapsedEventArgs e)
            {
                // Sprawdzenie, czy upłynął już maksymalny czas
                if (this.elapsedTime < this.maxTime)
                {
                    this.elapsedTime += (int)timer.Interval;
                    
                } else
                {
                    timerCallBack();
                    this.elapsedTime = 0;
                    timer.Stop();
                }


            }


        }

    }
}