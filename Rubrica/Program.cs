
using System.Text.Json;
using System.Text.Json.Serialization;

IEnumerable<string> linee = File.ReadLines("rubrica.json");

string primaLinea = linee.First();
Contatto? contatto = JsonSerializer.Deserialize<Contatto>(primaLinea);
if (contatto == null)
{
    throw new Exception("JSON non valido.");
}

Console.WriteLine($"{contatto.Nome} {contatto.Cognome}: {contatto.Numero}");

class Contatto
{
    public string Nome { get; set; } = "";
    public string Cognome { get; set; } = "";
    public string Numero { get; set; } = "";
}
