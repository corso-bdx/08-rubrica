
using System.Text.Json;
using System.Text.Json.Serialization;

// serialize / serializza   = converti da Classe C# a stringa di testo JSON
// deserialize/deserializza = converti da stringa di testo JSON a Classe C#

const string FileName = "rubrica.json";

string ChiediParametro(int index, string messaggio)
{
    if (args.Length > index)
    {
        return args[index];
    }

    Console.Write(messaggio);
    string? input = Console.ReadLine();
    if (input == null)
    {
        throw new Exception("Nessun input");
    }

    return input;
}

string operazione = ChiediParametro(0, "Operazioni disponibili: lista, cerca, nuovo\nCosa vuoi fare? ");
switch (operazione)
{
    case "lista":
        StampaTutti(LeggiContatti(FileName));
        break;

    case "cerca":
        string query = ChiediParametro(1, "Query: ");
        StampaFiltrati(LeggiContatti(FileName), query);
        break;

    case "nuovo":
        Contatto contatto = new Contatto();
        contatto.Nome = ChiediParametro(1, "Nome: ");
        contatto.Cognome = ChiediParametro(2, "Cognome: ");
        contatto.Numero = ChiediParametro(3, "Numero: ");
        AggiungiContatto(contatto, FileName);
        break;

    default:
        throw new Exception("Comando non valido.");
}

IEnumerable<Contatto> LeggiContatti(string fileName)
{
    return File.ReadLines(fileName)
        .Select(linea =>
        {
            Contatto? contatto = JsonSerializer.Deserialize<Contatto>(linea);
            if (contatto == null)
            {
                throw new Exception("JSON non valido.");
            }

            return contatto;
        });
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

void AggiungiContatto(Contatto contatto, string fileName)
{
    string json = JsonSerializer.Serialize(contatto);

    File.AppendAllText(fileName, $"\n{json}");

    Console.WriteLine("Contatto aggiunto!");
}

class Contatto
{
    public string Nome { get; set; } = "";
    public string Cognome { get; set; } = "";
    public string Numero { get; set; } = "";
}
