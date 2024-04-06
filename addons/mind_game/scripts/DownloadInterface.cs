#if TOOLS
using Godot;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

[Tool]
public partial class DownloadInterface : Control
{
    private Button chooseDownloadLocationButton, downloadModelButton;
    private OptionButton modelTypeOptionButton, modelSizeOptionButton, modelSubTypeOptionButton, modelQuantizationOptionButton, modelVersionOptionButton;
    private FileDialog downloadModelFileDialog;
    // private HttpRequest httpRequest;
    private ProgressBar downloadProgressBar;
    private Label downloadUrlLabel;

    private const string theBlokeBaseUrl = "https://huggingface.co/TheBloke/";
    private const string fileExtension = "gguf";
    private string downloadModelDirectoryPath, modelType, modelSubType, modelSize, modelQuantization, modelVersion, fullDownloadUrl;

    public override void _Ready()
    {
        modelTypeOptionButton = GetNode<OptionButton>("%ModelTypeOptionButton");
        modelSubTypeOptionButton = GetNode<OptionButton>("%ModelSubTypeOptionButton");
        modelVersionOptionButton = GetNode<OptionButton>("%ModelVersionOptionButton");
        modelSizeOptionButton = GetNode<OptionButton>("%ModelSizeOptionButton");
        modelQuantizationOptionButton = GetNode<OptionButton>("%ModelQuantizationOptionButton");

        downloadProgressBar = GetNode<ProgressBar>("%DownloadProgressBar");

        chooseDownloadLocationButton = GetNode<Button>("%ChooseDownloadLocationButton");
        downloadModelButton = GetNode<Button>("%DownloadModelButton");
        downloadUrlLabel = GetNode<Label>("%DownloadUrlLabel");

        downloadModelFileDialog = GetNode<FileDialog>("%DownloadModelFileDialog");
        // httpRequest = GetNode<HttpRequest>("%HTTPRequest");

        // Signals
        chooseDownloadLocationButton.Pressed += OnChooseDownloadLocationButtonPressed;
        downloadModelButton.Pressed += OnDownloadModelButtonPressed;
        downloadModelFileDialog.DirSelected += OnDownloadModelDirectorySelected;

        modelTypeOptionButton.ItemSelected += OnModelTypeSelected;
        modelSubTypeOptionButton.ItemSelected += OnModelSubTypeSelected;
        modelVersionOptionButton.ItemSelected += OnModelVersionSelected;
        modelSizeOptionButton.ItemSelected += OnModelSizeSelected;
        modelQuantizationOptionButton.ItemSelected += OnModelQuantizationSelected;

        // httpRequest.RequestCompleted += OnRequestCompleted;

    }


    private void OnModelVersionSelected(long index)
    {
        modelVersion = modelVersionOptionButton.GetItemText((int)index);
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

        fullDownloadUrl = $"{theBlokeBaseUrl}" +
                  $"{char.ToUpper(modelType[0])}{modelType.Substring(1)}-" +
                  $"{modelSize.ToUpper()}-" +
                  $"{modelSubType}-" +
                  $"{modelVersion}-" +
                  $"{fileExtension.ToUpper()}/resolve/main/" +
                  $"{modelType}-{modelSize}-{modelSubType}-{modelVersion}.{modelQuantization}.{fileExtension}";

        downloadUrlLabel.Text = $"{fullDownloadUrl}";
        //httpRequest.Request(fullDownloadUrl);
        await DownloadModelAsync();

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

    public async Task DownloadModelAsync()
    {
        using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient())
        {
            try
            {
                int bufferSize = 8192;

                // Assuming fullDownloadUrl and downloadModelDirectoryPath are already set
                string fileName = Path.GetFileName(fullDownloadUrl);
                string destinationPath = Path.Combine(downloadModelDirectoryPath, fileName);

                // Download the file
                HttpResponseMessage response = await httpClient.GetAsync(fullDownloadUrl, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                await WriteStreamToFile(bufferSize, destinationPath, response);
            }
            catch (Exception ex)
            {
                GD.PrintErr($"Error downloading file: {ex.Message}");
            }
        }
    }


    // This method writes the downloaded model to a file while displaying to the user the percentage downloaded
    private async Task WriteStreamToFile(int bufferSize, string destinationPath, HttpResponseMessage response)
    {
        // Used for tracking download progress
        var totalBytes = response.Content.Headers.ContentLength ?? 0;
        long totalReadBytes = 0;
        int readBytes;
        double lastProgress = 0;

        // This sets up the stream to receive the model download (initially into a buffer)
        using (Stream contentStream = await response.Content.ReadAsStreamAsync(), fileStream = new FileStream(destinationPath, FileMode.Create, System.IO.FileAccess.Write, FileShare.None, bufferSize, true))
        {
            byte[] buffer = new byte[bufferSize];

            while ((readBytes = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, readBytes);
                totalReadBytes += readBytes;
                var progress = (double)totalReadBytes / totalBytes;

                // Update progress for every 1% increase or more
                if (progress - lastProgress >= 0.01)
                {
                    downloadProgressBar.Value = progress;
                    lastProgress = progress;
                }
            }
        }
    }


    public override void _ExitTree()
    {
        chooseDownloadLocationButton.Pressed -= OnChooseDownloadLocationButtonPressed;
        downloadModelButton.Pressed -= OnDownloadModelButtonPressed;
        downloadModelFileDialog.DirSelected -= OnDownloadModelDirectorySelected;

        modelTypeOptionButton.ItemSelected -= OnModelTypeSelected;
        modelSubTypeOptionButton.ItemSelected -= OnModelSubTypeSelected;
        modelVersionOptionButton.ItemSelected -= OnModelVersionSelected;
        modelSizeOptionButton.ItemSelected -= OnModelSizeSelected;
        modelQuantizationOptionButton.ItemSelected -= OnModelQuantizationSelected;

        // httpRequest.RequestCompleted -= OnRequestCompleted;
    }
}

#endif