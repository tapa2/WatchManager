using System.Text.Json.Serialization;

public class Watch
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("brand")]
    public string Brand { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("inStock")]
    public bool InStock { get; set; }
}
