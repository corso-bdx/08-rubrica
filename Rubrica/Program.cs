
using System.Text.Json;
using System.Text.Json.Serialization;

// serialize / serializza   = converti da Classe C# a stringa di testo JSON
// deserialize/deserializza = converti da stringa di testo JSON a Classe C#

// Nome del file contentente la rubrica.
// Ogni riga di questo file è un documento JSON.
const string FileName = "rubrica.json";

// funzione che permette di chiedere un parametro.
// Se il paramentro "index" è stato indicato da linea di comando usa quello,
// altrimenti mostra un "messaggio" all'utente chiedendo il parametro.
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

Dictionary<string, Action> operazioni = new Dictionary<string, Action>()
{
    ["lista"] = () =>
    {
        StampaTutti(LeggiContatti(FileName));
    },

    ["cerca"] = () =>
    {
        string query = ChiediParametro(1, "Query: ");
        StampaFiltrati(LeggiContatti(FileName), query);
    },

    ["nuovo"] = () =>
    {
        Contatto contatto = new Contatto();
        contatto.Nome = ChiediParametro(1, "Nome: ");
        contatto.Cognome = ChiediParametro(2, "Cognome: ");
        contatto.Numero = ChiediParametro(3, "Numero: ");
        AggiungiContatto(contatto, FileName);
    },

    ["cancella"] = () =>
    {
        string vittima = ChiediParametro(1, "Chi vuoi cancellare? ");
        CancellaContatto(vittima, FileName);
    },
};

// Chiede all'utente che operazione vuole esseguire sulla rubrica.
string operazione = ChiediParametro(0, $"Operazioni disponibili: {string.Join(", ", operazioni.Keys)}\nCosa vuoi fare? ");
if (!operazioni.TryGetValue(operazione, out Action? action))
{
    throw new Exception("Comando non valido.");
}

action();

// Deserializza un contatto dal JSON.
Contatto DeserializzaContatto(string json)
{
    Contatto? contatto = JsonSerializer.Deserialize<Contatto>(json);
    if (contatto == null)
    {
        throw new Exception("JSON non valido.");
    }

    return contatto;
}

// Legge i contatti dal file indicato.
IEnumerable<Contatto> LeggiContatti(string fileName)
{
    return File.ReadLines(fileName)
        .Select(DeserializzaContatto);
}

// Stampa una lista di contatti.
void StampaTutti(IEnumerable<Contatto> contatti)
{
    foreach (Contatto contatto in contatti)
    {
        Console.WriteLine($"{contatto.Nome} {contatto.Cognome}: {contatto.Numero}");
    }
}

// Stampa da una lista di contatti quelli che hanno "query" nel nome, cognome o numero.
void StampaFiltrati(IEnumerable<Contatto> contatti, string query)
{
    IEnumerable<Contatto> filtrati = contatti
        .Where(c => c.Nome.Contains(query) || c.Cognome.Contains(query) || c.Numero.Contains(query));

    StampaTutti(filtrati);
}

// Aggiunge un contatto come ultima riga del file.
void AggiungiContatto(Contatto contatto, string fileName)
{
    string json = JsonSerializer.Serialize(contatto);

    File.AppendAllText(fileName, $"\n{json}");

    Console.WriteLine("Contatto aggiunto!");
}

// Cancella dal file il contatto che ha "vittima" nel nome, cognome o numero.
void CancellaContatto(string vittima, string fileName)
{
    // leggiamo tutte le righe del file, e carichiamole in un array
    string[] linee = File.ReadAllLines(fileName);
    
    // creiamo una lista vuota delle righe che intendiamo mantenere (non cancellare)
    List<string> mantenere = new List<string>();

    // iteriamo ciascuna delle righe lette dal file
    foreach (string linea in linee)
    {
        // converte la riga da JSON a classe C# "Contatto"
        Contatto c = DeserializzaContatto(linea);

        // controlliamo se la striga "vittima" appare nel nome, cognome o numero del contatto
        if (c.Nome.Contains(vittima) || c.Cognome.Contains(vittima) || c.Numero.Contains(vittima))
            continue;  // interrompe l'iterazione del ciclo, ovvero non esegue il codice seguente nel ciclo

        // se "vittima" non appare nel nome, cognome o numero del contatto, aggiungiamo alla lista di contatti da mantenere
        mantenere.Add(linea);
    }

    // tutte le linee eccetto quelle da mantenere, ovvero solo le linee da mantenere
    // per ciascuna delle linee trovate, chiamiamo "DeserializzaContatto" per convertire da JSON a classe C# "Contatto"
    IEnumerable<Contatto> contattiCancellati = linee.Except(mantenere).Select(DeserializzaContatto);

    // se c'è più di un contatto che risponde alla query "vittima"...
    if (contattiCancellati.Count() > 1)
    {
        // ...diciamo che la query è ambigua, non cancelliamo nulla,
        // e facciamo vedere tutti i contatti che rispondono alla query
        Console.WriteLine("Nome ambiguo:");
        StampaTutti(contattiCancellati);
    }

    // se ce n'è esattamente uno solo...
    else if (contattiCancellati.Count() == 1)
    {
        // ...leggiamo il primo, ovvero l'unico
        Contatto c2 = contattiCancellati.First();

        // scriviamo nel file tutte le righe da mantenere
        File.WriteAllLines(fileName, mantenere);

        // diciamo qual'era il contatto cancellato
        Console.WriteLine($"Cancellato {c2.Nome} {c2.Cognome} ({c2.Numero})");
    }

    // se non ce ne sono...
    else
    {
        // diciamo che l'operazione non è valida
        Console.WriteLine($"{vittima} non trovato.");
    }
}

class Contatto
{
    public string Nome { get; set; } = "";
    public string Cognome { get; set; } = "";
    public string Numero { get; set; } = "";
}
