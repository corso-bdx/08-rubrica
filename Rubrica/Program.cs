
using System.Text.Json;
using System.Text.Json.Serialization;

IEnumerable<string> linee = File.ReadLines("rubrica.json");

foreach (string linea in linee)
{
    Contatto? contatto = JsonSerializer.Deserialize<Contatto>(linea);
    if (contatto == null)
    {
        throw new Exception("JSON non valido.");
    }

    Console.WriteLine($"{contatto.Nome} {contatto.Cognome}: {contatto.Numero}");
}

class Contatto
{
    public string Nome { get; set; } = "";
    public string Cognome { get; set; } = "";
    public string Numero { get; set; } = "";
}
