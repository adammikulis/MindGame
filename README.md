This is a plugin for the Godot game engine that allows the user to load and run a local Large Language Model (LLM) in-engine. Open the .NET version of Godot 4.3 dev5, click Project > Project Settings > Plugins > Enabled (Mind Game)

Download Godot 4.3 dev5 (.NET version) here: https://godotengine.org/article/dev-snapshot-godot-4-3-dev-5/
Download/install .NET8: https://dotnet.microsoft.com/en-us/download

Load a .gguf file of the llama, mistral, mixtral, or phi families to get going!

Recommended model download: https://huggingface.co/TheBloke/Mistral-7B-Instruct-v0.2-GGUF/tree/main

Smaller model for those with less VRAM: https://huggingface.co/TheBloke/phi-2-GGUF/tree/main

The lower quantization (q), the smaller the model is to run but at the cost of accuracy. Mistral-7B Q4_K_M is a great middle-ground for those with 8GB of VRAM. The absolute smallest model (phi-2.Q2_K.gguf) can run on 4GB of VRAM.

Future steps:
- make Download Manager functional
- make singleton functional to be able to access model in-game
- transition to BatchedExecutor and add conversation forking/rewinding
- adding network graph generation
- add project script crawling
- expose LLamaSharp methods like quantization
- integrate Kernel Memory for document ingestion
