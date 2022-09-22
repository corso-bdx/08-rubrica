
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

    case "cancella":
        string vittima = ChiediParametro(1, "Chi vuoi cancellare? ");
        CancellaContatto(vittima, FileName);
        break;

    default:
        throw new Exception("Comando non valido.");
}

Contatto DeserializzaContatto(string json)
{
    Contatto? contatto = JsonSerializer.Deserialize<Contatto>(json);
    if (contatto == null)
    {
        throw new Exception("JSON non valido.");
    }

    return contatto;
}

IEnumerable<Contatto> LeggiContatti(string fileName)
{
    return File.ReadLines(fileName)
        .Select(DeserializzaContatto);
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

void CancellaContatto(string vittima, string fileName)
{
    string[] linee = File.ReadAllLines(fileName);

    List<string> mantenere = new List<string>();

    foreach (string linea in linee)
    {
        Contatto c = DeserializzaContatto(linea);

        if (c.Nome.Contains(vittima) || c.Cognome.Contains(vittima) || c.Numero.Contains(vittima))
            continue;

        mantenere.Add(linea);
    }

    IEnumerable<Contatto> contattiCancellati = linee.Except(mantenere).Select(DeserializzaContatto);

    if (contattiCancellati.Count() > 1)
    {
        Console.WriteLine("Nome ambiguo:");
        StampaTutti(contattiCancellati);
    }
    else if (contattiCancellati.Count() == 1)
    {
        Contatto c2 = contattiCancellati.First();

        File.WriteAllLines(fileName, mantenere);
        Console.WriteLine($"Cancellato {c2.Nome} {c2.Cognome} ({c2.Numero})");
    }
    else
    {
        Console.WriteLine($"{vittima} non trovato.");
    }
}

class Contatto
{
    public string Nome { get; set; } = "";
    public string Cognome { get; set; } = "";
    public string Numero { get; set; } = "";
}
