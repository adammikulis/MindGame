#if TOOLS
using Godot;
using System;

[Tool]
public partial class DownloadInterface : Control
{
    private string downloadModelDirectoryPath;
    private Button chooseDownloadLocationButton, downloadModelButton;
    private OptionButton modelTypeOptionButton, modelSizeOptionButton, modelSubTypeOptionButton, quantizationOptionButton;
    private FileDialog downloadModelFileDialog;

    private const string theBlokeBaseUrl = "https://huggingface.co/TheBloke/";
    private const string fileExtension = ".gguf";

    public override void _Ready()
    {
        modelTypeOptionButton = GetNode<OptionButton>("%ModelTypeOptionButton");
        modelSubTypeOptionButton = GetNode<OptionButton>("%ModelSubTypeOptionButton");
        modelSizeOptionButton = GetNode<OptionButton>("%ModelSizeOptionButton");
        quantizationOptionButton = GetNode<OptionButton>("%QuantizationOptionButton");


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