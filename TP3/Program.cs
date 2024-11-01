using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

class Program
{
    static void Main(string[] args)
    {
        // Definindo o caminho do arquivo na pasta "testes" na raiz do projeto
        string rootPath = Directory.GetCurrentDirectory();
        string filePath = Path.Combine(rootPath, "testes", "arquivo.cs");

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Arquivo não encontrado no caminho: {filePath}");
            return;
        }

        string code = File.ReadAllText(filePath);
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
        SyntaxNode root = syntaxTree.GetRoot();

        // Coletar todas as declarações de classe no arquivo
        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
        var classGroups = new Dictionary<ClassDeclarationSyntax, HashSet<ClassDeclarationSyntax>>();

        // Comparar classes para encontrar grupos com membros comuns
        for (int i = 0; i < classDeclarations.Count; i++)
        {
            for (int j = i + 1; j < classDeclarations.Count; j++)
            {
                if (HaveCommonMembers(classDeclarations[i], classDeclarations[j]))
                {
                    if (!classGroups.ContainsKey(classDeclarations[i]))
                        classGroups[classDeclarations[i]] = new HashSet<ClassDeclarationSyntax>();

                    if (!classGroups.ContainsKey(classDeclarations[j]))
                        classGroups[classDeclarations[j]] = new HashSet<ClassDeclarationSyntax>();

                    classGroups[classDeclarations[i]].Add(classDeclarations[j]);
                    classGroups[classDeclarations[j]].Add(classDeclarations[i]);
                }
            }
        }

        // Criar uma lista de conjuntos de classes relacionadas
        var relatedClassGroups = new List<HashSet<ClassDeclarationSyntax>>();
        var visited = new HashSet<ClassDeclarationSyntax>();

        foreach (var classDeclaration in classGroups.Keys)
        {
            if (!visited.Contains(classDeclaration))
            {
                var group = new HashSet<ClassDeclarationSyntax>();
                BuildClassGroup(classDeclaration, classGroups, group, visited);
                relatedClassGroups.Add(group);
            }
        }

        // Exibir oportunidades de refatoração para grupos de classes
        Console.WriteLine("Oportunidades de refatoração para herança encontradas:");
        foreach (var group in relatedClassGroups)
        {
            if (group.Count > 1)
            {
                var classNames = string.Join(", ", group.Select(c => $"{c.Identifier} na linha {GetLineNumber(c)}"));
                Console.WriteLine($"Classes {classNames} têm membros em comum.");
                Console.WriteLine("Sugestão: Considere criar uma classe base para compartilhar membros comuns.");

                // Sugerir o nome da classe base e os membros a serem movidos
                var commonMembers = GetCommonMembers(group);
                Console.WriteLine("\nSugestão de classe base:");
                Console.WriteLine("class BaseClass");
                Console.WriteLine("{");
                foreach (var member in commonMembers)
                {
                    Console.WriteLine($"    {member};");
                }
                Console.WriteLine("}");

                // Sugerir modificações para as classes derivadas
                foreach (var classDeclaration in group)
                {
                    Console.WriteLine($"\nSugestão de modificação para {classDeclaration.Identifier}:");
                    Console.WriteLine($"class {classDeclaration.Identifier} : BaseClass");
                    Console.WriteLine("{");
                    Console.WriteLine("    // Mantenha apenas membros específicos dessa classe");
                    Console.WriteLine("}");
                }
            }
        }
    }

    static bool HaveCommonMembers(ClassDeclarationSyntax class1, ClassDeclarationSyntax class2)
    {
        var members1 = class1.Members.Select(m => m.ToString()).ToHashSet();
        var members2 = class2.Members.Select(m => m.ToString()).ToHashSet();
        return members1.Overlaps(members2);
    }

    static void BuildClassGroup(ClassDeclarationSyntax classDeclaration, Dictionary<ClassDeclarationSyntax, HashSet<ClassDeclarationSyntax>> classGroups, HashSet<ClassDeclarationSyntax> group, HashSet<ClassDeclarationSyntax> visited)
    {
        if (visited.Contains(classDeclaration))
            return;

        visited.Add(classDeclaration);
        group.Add(classDeclaration);

        if (classGroups.ContainsKey(classDeclaration))
        {
            foreach (var relatedClass in classGroups[classDeclaration])
            {
                BuildClassGroup(relatedClass, classGroups, group, visited);
            }
        }
    }

    static int GetLineNumber(SyntaxNode node)
    {
        var lineSpan = node.SyntaxTree.GetLineSpan(node.Span);
        return lineSpan.StartLinePosition.Line + 1;
    }

    static IEnumerable<string> GetCommonMembers(HashSet<ClassDeclarationSyntax> classes)
    {
        // Identificar membros comuns entre todas as classes no grupo
        var memberSets = classes.Select(c => c.Members.Select(m => m.ToString()).ToHashSet()).ToList();
        var commonMembers = memberSets.Aggregate((set1, set2) => new HashSet<string>(set1.Intersect(set2)));
        return commonMembers;
    }
}
