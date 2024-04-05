#if TOOLS
using Godot;
using System;

public partial class DownloadInterface : Control
{
    private string downloadModelDirectoryPath;
    private Button chooseDownloadLocationButton, downloadModelButton;
    private FileDialog downloadModelFileDialog;


    public override void _Ready()
    {
        chooseDownloadLocationButton = GetNode<Button>("%ChooseDownloadLocationButton");
        downloadModelButton = GetNode<Button>("%DownloadModelButton");

        downloadModelFileDialog = GetNode<FileDialog>("%DownloadModelFileDialog");

        // Signals
        chooseDownloadLocationButton.Pressed += OnChooseDownloadLocationButtonPressed;
        downloadModelButton.Pressed += OnDownloadModelButtonPressed;
        downloadModelFileDialog.DirSelected += OnDownloadModelDirectorySelected;


    }

    private void OnDownloadModelDirectorySelected(string dir)
    {
        downloadModelDirectoryPath = dir;
        downloadModelButton.Disabled = false;
    }

    private void OnDownloadModelButtonPressed()
    {
        // Need to add HTTP request here
    }

    private void OnChooseDownloadLocationButtonPressed()
    {
        downloadModelFileDialog.PopupCentered();
    }

    public override void _ExitTree()
    {
        chooseDownloadLocationButton.Pressed -= OnChooseDownloadLocationButtonPressed;
        downloadModelButton.Pressed -= OnDownloadModelButtonPressed;
        downloadModelFileDialog.DirSelected -= OnDownloadModelDirectorySelected;
    }

}
#endif