using Godot;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

public partial class VectorDatabase : Resource
{
    private List<EmbeddingData> data = new List<EmbeddingData>();

    public List<EmbeddingData> Data
    {
        get => data;
        set => data = value;
    }

    public void AddData(EmbeddingData data)
    {
        Data.Add(data);
    }

    public void SaveToJson(string filePath)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(Data, options);
        File.WriteAllText(filePath, json);
    }

    public void LoadFromJson(string filePath)
    {
        string json = File.ReadAllText(filePath);
        Data = JsonSerializer.Deserialize<List<EmbeddingData>>(json);
    }

    public void CalculateSimilarities(EmbeddingData targetEmbedding)
    {
        Parallel.ForEach(Data, (embeddingData) =>
        {
            double similarity = VectorUtility.ComputeCosineSimilarity(targetEmbedding.Embedding, embeddingData.Embedding);
            
        });
    }
}

public class EmbeddingData
{
    public double[] Embedding { get; set; }
    public string FileType { get; set; }
    public string DataChunk { get; set; }
}

public class VectorUtility
{
    public static double ComputeCosineSimilarity(double[] vectorA, double[] vectorB)
    {
        double dotProduct = 0.0;
        double normA = 0.0;
        double normB = 0.0;
        for (int i = 0; i < vectorA.Length; i++)
        {
            dotProduct += vectorA[i] * vectorB[i];
            normA += vectorA[i] * vectorA[i];
            normB += vectorB[i] * vectorB[i];
        }
        return dotProduct / (Math.Sqrt(normA) * Math.Sqrt(normB));
    }
}
