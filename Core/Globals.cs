﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TheGame
{
    public class Globals
    {
        public static ContentManager content;
        public static GraphicsDeviceManager _graphics;
        public static SpriteBatch spriteBatch;
        public static Viewport viewport;
        public static EffectHandler effectHandler;
        public static EffectHandler effectHandler1;
        public static Matrix projectionMatrix;
        public static Matrix viewMatrix;
        public static Matrix worldMatrix;

        public static int counter = 0;
        public static int moduleCounter = 0;
        public static bool Module2;
        public static bool Module3;
        public static bool Module4;
        public static bool Module5;
        public static bool Start;
        public static bool Death;
        public static bool Tutorial;
        public static bool Pause;
        public static bool TutorialPause;
        public static bool Easy;
        public static bool Hard;
        public static bool LeaderBoard;
        public static float time;
        public static int Score;
        public static int ScoreMultipler;
        public static GamePadState prevState;
        public static ButtonState CheckA;
        public static KeyboardState prevKeyBoardState;
        public static GamePadState prevDeathState;
        public static KeyboardState prevKeyBoardDeathState;
        public static GamePadState prevPauseState;
        public static KeyboardState prevKeyBoardPauseState;
        public static GamePadState prevTutorialState;
        public static KeyboardState prevKeyBoardTutorialState;
        public static GamePadState prevLeaderState;
        public static KeyboardState prevKeyBoardLeaderState;
        public static List<String> learnedRecepture;
        public static int numberOfRecepture = 0;
        public static int maxReceptures = 4;
    }
}
