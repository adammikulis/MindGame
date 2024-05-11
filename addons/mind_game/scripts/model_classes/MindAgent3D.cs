 using Godot;
using System;

namespace MindGame
{
    [Tool]
    public partial class MindAgent3D : CharacterBody3D
    {
        [Signal]
        public delegate void ChatSessionStatusUpdateEventHandler(bool isLoaded);
        [Signal]
        public delegate void ChatOutputReceivedEventHandler(string text);

        public Label3D chatLabel3D { get; set; }
        public MindGame.MindAgent mindAgent { get; set; }
        public MindGame.MindManager mindManager { get; set; }
        public AnimationPlayer animationPlayer { get; set; }

        public override void _Ready()
        {
            mindManager = GetNode<MindGame.MindManager>("/root/MindManager");
            mindAgent = GetNode<MindGame.MindAgent>("%MindAgent");
            chatLabel3D = GetNode<Label3D>("%ChatLabel3D");
            animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");

            mindAgent.ChatSessionStatusUpdate += OnChatSessionStatusUpdate;
            mindAgent.ChatOutputReceived += OnChatOutputReceived;
        }

        public async void InferAsync(string prompt)
        {
            chatLabel3D.Text = "";
            await mindAgent.InferAsync(prompt);
        }

        public void OnChatOutputReceived(string text)
        {
            chatLabel3D.Text += text;
            if (animationPlayer.GetCurrentAnimation() ==  "")
            {
                animationPlayer.SetCurrentAnimation("bobble");
            }

        }

        public void OnChatSessionStatusUpdate(bool isLoaded)
        {
            CallDeferred("emit_signal", SignalName.ChatSessionStatusUpdate, true);
            chatLabel3D.Text = "Hey there! How can I help you?";
        }
    }
}
