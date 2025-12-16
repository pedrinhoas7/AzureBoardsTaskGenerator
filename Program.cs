class Program
{
    static async Task Main(string[] args)
    {
        // =========================
        // Setup de variáveis
        // =========================
        var org = Environment.GetEnvironmentVariable("AZURE_ORG");
        var project = Environment.GetEnvironmentVariable("AZURE_PROJECT");
        var pat = Environment.GetEnvironmentVariable("AZURE_PAT");
        var apiKey = Environment.GetEnvironmentVariable("AI_API_KEY");
        var model = Environment.GetEnvironmentVariable("AI_MODEL");

        if (string.IsNullOrWhiteSpace(org) ||
            string.IsNullOrWhiteSpace(project) ||
            string.IsNullOrWhiteSpace(pat) ||
            string.IsNullOrWhiteSpace(apiKey))
        {
            Console.WriteLine("Erro: variáveis de ambiente não configuradas.");
            Console.WriteLine("Necessário: AZURE_ORG, AZURE_PROJECT, AZURE_PAT, AI_API_KEY, AI_MODEL");
            return;
        }

        var azureBoardsService = new AzureBoardsService(org, project, pat);
        var iaService = new IAService(apiKey, model);
        var taskManager = new TaskManager(azureBoardsService, iaService);

        // =========================
        // DEBUG MODE
        // =========================
#if DEBUG
        if (args.Length == 0)
        {
            Console.WriteLine("DEBUG MODE");
            Console.WriteLine("Informe o comando (ex: --card 2) ou 'exit':");

            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input) ||
                input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                return;

            args = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }
#endif

        // =========================
        // CLI MODE
        // =========================
        if (args.Length == 0 || HasArg(args, "--help", "-h"))
        {
            PrintHelp();
            return;
        }

        var cardArg = GetArg(args, "--card");
        if (string.IsNullOrWhiteSpace(cardArg) || !int.TryParse(cardArg, out var cardId))
        {
            Console.WriteLine("Erro: informe um ID válido com --card <id>");
            return;
        }

        Console.WriteLine($"Gerando tasks para o card {cardId}...");
        await taskManager.GenerateTasksForCardAsync(cardId);
    }

    // =========================
    // Helpers
    // =========================
    static string? GetArg(string[] args, string name)
    {
        var index = Array.IndexOf(args, name);
        return index >= 0 && index < args.Length - 1
            ? args[index + 1]
            : null;
    }

    static bool HasArg(string[] args, params string[] names)
    {
        foreach (var arg in args)
        {
            foreach (var name in names)
            {
                if (arg.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }
        return false;
    }

    static void PrintHelp()
    {
        Console.WriteLine(@"
Generate Tasks - Azure Boards

Uso:
  dotnet run -- --card <id>

Parâmetros:
  --card <id>        ID do card (User Story / BUG /...)
  -h, --help         Exibe esta ajuda

Variáveis de ambiente:
  AZURE_ORG
  AZURE_PROJECT
  AZURE_PAT
  AI_API_KEY

DEBUG:
  Execute sem argumentos e informe o comando via console.
");
    }
}
