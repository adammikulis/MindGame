using Godot;
using System;

namespace MindGame
{
    [Tool]
    public partial class InferenceConfigController : Control
    {




        private uint calculateExpMaxTokens(double value)
        {
            return (uint)Math.Pow(2, value) * 1000;
        }

        private double calculateLogMaxTokens(uint value)
        {
            return (double)Math.Log2(value / 1000);
        }
    }
}

    
