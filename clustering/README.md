# Oportunidades de Modularização

Este projeto analisa um projeto C# para identificar oportunidades de modularização, detectando comunidades dentro da base de código usando o algoritmo de Leiden.

## Pré-requisitos

- .NET SDK
- MSBuild
- Microsoft.Build.Locator
- Microsoft.CodeAnalysis

## Instalação

1. Clone o repositório

2. Instale o .NET SDK e o MSBuild necessários.

3. Restaure os pacotes NuGet:
    ```sh
    dotnet restore
    ```

## Uso

1. Compile o projeto:
    ```sh
    dotnet build
    ```

2. Execute o projeto com o caminho para o projeto C# que você deseja analisar:
    ```sh
    dotnet run -- <caminho-para-o-projeto-csharp>
    ```

## Estrutura do Projeto

- `Program.cs`: Ponto de entrada da aplicação.
- `core/Analyzer.cs`: Analisa o projeto e encontra comunidades.
- `core/Graph.cs`: Representa a estrutura do grafo.
- `core/Leiden.cs`: Implementa o algoritmo de Leiden.
- `core/StructuralCouplingExtractor`: Extrai acoplamento estrutural entre classes.
- `core/SyntaxAnalyzer.cs`: Analisa a sintaxe dos arquivos C#.
- `core/Report.cs`: Gera o relatório.
- `dto/CsFile.cs`: Representa um arquivo C# e suas declarações de classe.
- `utils/FilesManager.cs`: Funções utilitárias para gerenciamento de arquivos.

## Como Funciona

1. **Inicialização**: O programa inicializa o MSBuild e abre o projeto C# especificado.
2. **Análise**: A classe `Analyzer` processa os arquivos do projeto, extrai relacionamentos entre classes e gera um grafo.
3. **Detecção de Comunidades**: O algoritmo de Leiden é aplicado para detectar comunidades dentro do grafo.
4. **Relatório**: Os resultados são impressos, mostrando as comunidades detectadas e suas respectivas classes.

## Exemplo

```sh
dotnet run -- /caminho/para/seu/projeto/csharp
```