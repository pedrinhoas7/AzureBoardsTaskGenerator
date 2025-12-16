# Azure Boards Task Generator

[![.NET](https://img.shields.io/badge/.NET-6.0-blue)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)
[![GitHub issues](https://img.shields.io/github/issues/pedrinhoas7/AzureBoardsTaskGenerator)](https://github.com/pedrinhoas7/AzureBoardsTaskGenerator/issues)
[![GitHub stars](https://img.shields.io/github/stars/pedrinhoas7/AzureBoardsTaskGenerator)](https://github.com/pedrinhoas7/AzureBoardsTaskGenerator/stargazers)

---

## 📌 Descrição

O **Azure Boards Task Generator** é uma ferramenta que automatiza a criação de tasks no **Azure Boards** a partir da descrição de um Work Item existente. Utiliza Inteligência Artificial para extrair subtarefas e gerar tasks filhas, agilizando o gerenciamento de projetos e garantindo consistência nos títulos e descrições das tasks.

---

## ⚙️ Funcionalidades

- Recebe um **ID de Work Item** do Azure Boards.
- Extrai a descrição completa do Work Item (`System.Description`).
- Analisa o texto utilizando IA para identificar tarefas separadas.
- Gera subtarefas (tasks filhas) com títulos e descrições padronizadas.
- Cria as tasks via API do Azure Boards e associa ao Work Item pai.
- Logging detalhado e tratamento robusto de erros.

---

## 🚀 Tecnologias

- **.NET 7** – Backend
- **Newtonsoft.Json** – Manipulação de JSON
- **Azure DevOps REST API** – Integração com Azure Boards
- **IA** – Processamento de linguagem natural

---

## 📝 Exemplo de uso

```bash
dotnet run --card 1
````

Ou via código C#:

```csharp
var tasks = await TaskManager.GenerateTasksForCardAsync(12345);
foreach(var task in tasks)
{
    Console.WriteLine($"Título: {task.Title}");
    Console.WriteLine($"Descrição: {task.Description}");
}
```

---

## 🛠️ Instalação

1. Clone o repositório:

```bash
git clone https://github.com/pedrinhoas7/AzureBoardsTaskGenerator.git
```

2. Entre na pasta do projeto:

```bash
cd AzureBoardsTaskGenerator
```

3. Restaure as dependências:

```bash
dotnet restore
```

4. Execute a aplicação:

```bash
dotnet run
```

---

## 🔐 Configuração


```json
{
   "AZURE_ORG": "",
   "AZURE_PROJECT": "",
   "AZURE_PAT": "", (Personal Access Token)
   "AI_API_KEY": "",
   "AI_MODEL": "gemini-2.5-flash"
}
```

---


## 📖 Contribuição

Contribuições são bem-vindas! Abra issues ou pull requests com melhorias, correções de bugs ou novas features.

---

## 📝 Licença

Este projeto está licenciado sob a **MIT License**. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

## 💡 Observações

* Certifique-se de ter permissões adequadas para criar Work Items via API.
* A precisão da extração de tasks depende do modelo de IA utilizado.
