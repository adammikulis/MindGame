using Godot;
using System;
using System.Threading.Tasks;

namespace MindGame
{
    [Tool]
    public partial class MindAgent3D : CharacterBody3D
    {
        [Signal]
        public delegate void ChatSessionStatusUpdateEventHandler(bool isLoaded);
        [Signal]
        public delegate void ChatOutputReceivedEventHandler(string text);

        public Label3D ChatLabel3D { get; set; }
        public MindGame.MindAgent MindAgent { get; set; }
        public MindGame.MindManager MindManager { get; set; }
        public AnimationPlayer AnimationPlayer { get; set; }

        public override void _Ready()
        {
            MindManager = GetNode<MindGame.MindManager>("/root/MindManager");
            MindAgent = GetNode<MindGame.MindAgent>("%MindAgent");
            ChatLabel3D = GetNode<Label3D>("%ChatLabel3D");
            AnimationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");

            MindAgent.ChatSessionStatusUpdate += OnChatSessionStatusUpdate;
            MindAgent.ChatOutputReceived += OnChatOutputReceived;
        }

        public async Task InferAsync(string prompt)
        {
            ChatLabel3D.Text = "";
            await MindAgent.InferAsync(prompt);
        }

        public void OnChatOutputReceived(string text)
        {
            ChatLabel3D.Text += text;
            if (AnimationPlayer.GetCurrentAnimation() ==  "")
            {
                AnimationPlayer.SetCurrentAnimation("bobble");
            }

        }

        public void OnChatSessionStatusUpdate(bool isLoaded)
        {
            CallDeferred("emit_signal", SignalName.ChatSessionStatusUpdate, true);
            ChatLabel3D.Text = "Hey there! How can I help you?";
        }
    }
}