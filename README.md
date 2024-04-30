This is a plugin for the Godot game engine that allows the user to load and run a local Large Language Model (LLM) in-engine using the [LLamaSharp](https://github.com/SciSharp/LLamaSharp) (v0.11.1) C# library.

1) Download and extract [Godot 4.3 dev5 (.NET version)](https://godotengine.org/article/dev-snapshot-godot-4-3-dev-5/)

2) Download/install [.NET8](https://dotnet.microsoft.com/en-us/download)

3) Clone/download this repo, open it with Godot 4.3 .NET, click Project > Project Settings > Plugins > Enabled (Mind Game) to make it appear in the bottom bar.

4) Currently you have to manually add the [LLamaSharp](https://www.nuget.org/packages/LLamaSharp) and [Cuda12 backend](https://www.nuget.org/packages/LLamaSharp.Backend.Cuda12) or [Cpu backend](https://www.nuget.org/packages/LLamaSharp.Backend.Cpu) Nuget packages to the project solution, use Visual Studio Community for this or a visual NuGet Manager extension for VS Code.

5) Load a .gguf file of the Llama, Mistral, Mixtral, or Phi families to get going!

Recommended model: [Llama3-8B-Instruct](https://huggingface.co/bartowski/Meta-Llama-3-8B-Instruct-GGUF/tree/main)

Next best: [Mistral-7B-Instruct-v0.2](https://huggingface.co/TheBloke/Mistral-7B-Instruct-v0.2-GGUF/tree/main)

Smaller model for those with less VRAM: [Phi-2](https://huggingface.co/TheBloke/phi-2-GGUF/tree/main)

The lower quantization (q), the smaller the model is to run but at the cost of accuracy. Llama-3-8B-Instruct.Q4_K_M is a great middle-ground for those with 8GB of VRAM. The absolute smallest model (phi-2.Q2_K.gguf) can run on 4GB of VRAM.

Future steps:
- Implement LLaVa support (including viewport analysis)
- Automatically include and reference LLamaSharp nuget packages
- Make Download Manager functional
- Make a singleton to be able to access currently loaded model in-game
- Transition to BatchedExecutor and add conversation forking/rewinding
- Add network graph generation
- Add project script crawling
- Expose LLamaSharp methods like quantization
- Integrate Kernel Memory for document ingestion
