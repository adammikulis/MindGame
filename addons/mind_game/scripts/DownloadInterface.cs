#if TOOLS
using Godot;
using System;
using System.IO;
using System.Threading.Tasks;

[Tool]
public partial class DownloadInterface : Control
{
    private Button chooseDownloadLocationButton, downloadModelButton;
    private OptionButton modelTypeOptionButton, modelSizeOptionButton, modelSubTypeOptionButton, modelQuantizationOptionButton, modelVersionOptionButton;
    private FileDialog downloadModelFileDialog;
    private ProgressBar downloadProgressBar;
    private Label downloadUrlLabel;

    private HttpClient httpClient;

    private const string hugginFaceBaseUrl = "https://huggingface.co/";
    private const string fileExtension = "gguf";
    private string downloadModelDirectoryPath, modelType, modelSubType, modelSize, modelQuantization, fullDownloadUrl;

    public override void _Ready()
    {
        httpClient = new HttpClient();

        modelTypeOptionButton = GetNode<OptionButton>("%ModelTypeOptionButton");
        modelSubTypeOptionButton = GetNode<OptionButton>("%ModelSubTypeOptionButton");
        modelSizeOptionButton = GetNode<OptionButton>("%ModelSizeOptionButton");
        modelQuantizationOptionButton = GetNode<OptionButton>("%ModelQuantizationOptionButton");

        downloadProgressBar = GetNode<ProgressBar>("%DownloadProgressBar");

        chooseDownloadLocationButton = GetNode<Button>("%ChooseDownloadLocationButton");
        downloadModelButton = GetNode<Button>("%DownloadModelButton");
        downloadUrlLabel = GetNode<Label>("%DownloadUrlLabel");

        downloadModelFileDialog = GetNode<FileDialog>("%DownloadModelFileDialog");

        // Signals
        chooseDownloadLocationButton.Pressed += OnChooseDownloadLocationButtonPressed;
        downloadModelButton.Pressed += OnDownloadModelButtonPressed;
        downloadModelFileDialog.DirSelected += OnDownloadModelDirectorySelected;

        modelTypeOptionButton.ItemSelected += OnModelTypeSelected;
        modelSubTypeOptionButton.ItemSelected += OnModelSubTypeSelected;
        modelSizeOptionButton.ItemSelected += OnModelSizeSelected;
        modelQuantizationOptionButton.ItemSelected += OnModelQuantizationSelected;


    }


    private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        throw new NotImplementedException();
    }

    private void OnModelQuantizationSelected(long index)
    {
        modelQuantization = modelQuantizationOptionButton.GetItemText((int)index);
        CheckCanDownloadModel();
    }

    private void OnModelSizeSelected(long index)
    {
        modelSize = modelSizeOptionButton.GetItemText((int)index);
        CheckCanDownloadModel();
    }

    private void OnModelSubTypeSelected(long index)
    {
        modelSubType = modelSubTypeOptionButton.GetItemText((int)index);
        CheckCanDownloadModel();
    }

    private void OnModelTypeSelected(long index)
    {
        modelType = modelTypeOptionButton.GetItemText((int)index);
        CheckCanDownloadModel();
    }

    private void OnDownloadModelDirectorySelected(string dir)
    {
        downloadModelDirectoryPath = dir;
        CheckCanDownloadModel();
    }

    private async void OnDownloadModelButtonPressed()
    {
        // Sample URL: https://huggingface.co/TheBloke/Mistral-7B-Instruct-v0.2-GGUF/resolve/main/mistral-7b-instruct-v0.2.Q2_K.gguf

        //fullDownloadUrl = $"{huggingFaceBaseUrl}" +
        //          $"{char.ToUpper(modelType[0])}{modelType.Substring(1)}-" +
        //          $"{modelSize.ToUpper()}-" +
        //          $"{modelSubType}-" +
        //          $"{modelVersion}-" +
        //          $"{fileExtension.ToUpper()}/resolve/main/" +
        //          $"{modelType}-{modelSize}-{modelSubType}-{modelVersion}.{modelQuantization}.{fileExtension}";

        //downloadUrlLabel.Text = $"{fullDownloadUrl}";
        
        // Need to add download logic here
    }

    private void OnChooseDownloadLocationButtonPressed()
    {
        downloadModelFileDialog.PopupCentered();

    }

    private void CheckCanDownloadModel()
    {
        if (downloadModelDirectoryPath != null && modelType != null && modelSubType != null && modelSize != null && modelQuantization != null)
        {
            downloadModelButton.Disabled = false;

        }
        else
        {
            downloadModelButton.Disabled = true;
        }
    }

  

    public override void _ExitTree()
    {
        chooseDownloadLocationButton.Pressed -= OnChooseDownloadLocationButtonPressed;
        downloadModelButton.Pressed -= OnDownloadModelButtonPressed;
        downloadModelFileDialog.DirSelected -= OnDownloadModelDirectorySelected;

        modelTypeOptionButton.ItemSelected -= OnModelTypeSelected;
        modelSubTypeOptionButton.ItemSelected -= OnModelSubTypeSelected;
        modelSizeOptionButton.ItemSelected -= OnModelSizeSelected;
        modelQuantizationOptionButton.ItemSelected -= OnModelQuantizationSelected;

        // httpRequest.RequestCompleted -= OnRequestCompleted;
    }
}

#endif