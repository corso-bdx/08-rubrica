
using System.Text.Json;
using System.Text.Json.Serialization;

// serialize / serializza   = converti da Classe C# a stringa di testo JSON
// deserialize/deserializza = converti da stringa di testo JSON a Classe C#

IEnumerable<Contatto> contatti = File.ReadLines("rubrica.json")
    .Select(linea =>
    {
        Contatto? contatto = JsonSerializer.Deserialize<Contatto>(linea);
        if (contatto == null)
        {
            throw new Exception("JSON non valido.");
        }

        return contatto;
    });

switch (args[0])
{
    case "lista":
        StampaTutti(contatti);
        break;

    case "cerca":
        string query = args[1];
        StampaFiltrati(contatti, query);
        break;

    default:
        throw new Exception("Comando non valido.");
}

void StampaTutti(IEnumerable<Contatto> contatti)
{
    foreach (Contatto contatto in contatti)
    {
        Console.WriteLine($"{contatto.Nome} {contatto.Cognome}: {contatto.Numero}");
    }
}

void StampaFiltrati(IEnumerable<Contatto> contatti, string query)
{
    IEnumerable<Contatto> filtrati = contatti
        .Where(c => c.Nome.Contains(query) || c.Cognome.Contains(query) || c.Numero.Contains(query));

    StampaTutti(filtrati);
}

class Contatto
{
    public string Nome { get; set; } = "";
    public string Cognome { get; set; } = "";
    public string Numero { get; set; } = "";
}
