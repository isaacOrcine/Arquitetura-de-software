using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

public class Analyzer
{
    public void AnalyzeCode(string projectPath)
    {
        using var workspace = MSBuildWorkspace.Create();

        string projectFilePath = Path.Combine(projectPath, "Mvc.csproj");
        if (!File.Exists(projectFilePath))
        {
            Console.WriteLine($"❌ Arquivo de projeto não encontrado: {projectFilePath}");
            return;
        }

        try
        {
            var project = workspace.OpenProjectAsync(projectFilePath).Result;
            Console.WriteLine($"📂 Projeto carregado: {project.Name}");

            foreach (var document in project.Documents)
            {
                Console.WriteLine($"\n🔍 Analisando arquivo: {document.Name}");

                var syntaxTree = document.GetSyntaxTreeAsync().Result;
                if (syntaxTree == null) continue;

                var root = syntaxTree.GetRoot() as CompilationUnitSyntax;
                if (root == null) continue;

                string fixedCode = SuggestFixes(root);

                if (!string.IsNullOrEmpty(fixedCode))
                {
                    Console.WriteLine("\n✅ Código corrigido:");
                    Console.WriteLine(fixedCode);
                }
                else
                {
                    Console.WriteLine($"✔️ Nenhum problema encontrado no arquivo: {document.Name}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Erro durante a análise do código: {ex.Message}");
        }
    }

    private string SuggestFixes(CompilationUnitSyntax root)
    {
        bool hasErrors = false;

        // Corrigir problema de namespace ausente de ponto e vírgula
        foreach (var nsDeclaration in root.DescendantNodes().OfType<NamespaceDeclarationSyntax>())
        {
            if (!nsDeclaration.SemicolonToken.IsKind(SyntaxKind.SemicolonToken))
            {
                hasErrors = true;
                Console.WriteLine("🔧 Erro: ';' ausente no namespace.");
                var fixedNamespace = nsDeclaration.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
                root = root.ReplaceNode(nsDeclaration, fixedNamespace);
            }

            if (!nsDeclaration.OpenBraceToken.IsKind(SyntaxKind.OpenBraceToken))
            {
                hasErrors = true;
                Console.WriteLine("🔧 Erro: '{' ausente no namespace.");
                var fixedNamespace = nsDeclaration.WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken));
                root = root.ReplaceNode(nsDeclaration, fixedNamespace);
            }

            if (!nsDeclaration.CloseBraceToken.IsKind(SyntaxKind.CloseBraceToken))
            {
                hasErrors = true;
                Console.WriteLine("🔧 Erro: '}' ausente no namespace.");
                var fixedNamespace = nsDeclaration.WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken));
                root = root.ReplaceNode(nsDeclaration, fixedNamespace);
            }
        }

        // Corrigir problema de classe ausente de fechamento
        foreach (var classDeclaration in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
        {
            if (!classDeclaration.CloseBraceToken.IsKind(SyntaxKind.CloseBraceToken))
            {
                hasErrors = true;
                Console.WriteLine("🔧 Erro: '}' ausente na classe.");
                var fixedClass = classDeclaration.WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken));
                root = root.ReplaceNode(classDeclaration, fixedClass);
            }
        }

        // Corrigir problema de método ausente de corpo
        foreach (var methodDeclaration in root.DescendantNodes().OfType<MethodDeclarationSyntax>())
        {
            if (methodDeclaration.Body == null && methodDeclaration.ExpressionBody == null)
            {
                hasErrors = true;
                Console.WriteLine("🔧 Erro: corpo do método ausente.");
                var fixedMethod = methodDeclaration.WithBody(SyntaxFactory.Block());
                root = root.ReplaceNode(methodDeclaration, fixedMethod);
            }
        }

        // Corrigir problema de declaração de variável ausente de ponto e vírgula
        foreach (var localDeclaration in root.DescendantNodes().OfType<LocalDeclarationStatementSyntax>())
        {
            if (!localDeclaration.SemicolonToken.IsKind(SyntaxKind.SemicolonToken))
            {
                hasErrors = true;
                Console.WriteLine("🔧 Erro: ';' ausente na declaração de variável.");
                var fixedDeclaration = localDeclaration.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
                root = root.ReplaceNode(localDeclaration, fixedDeclaration);
            }
        }

        return hasErrors ? root.NormalizeWhitespace().ToFullString() : null;
    }
}
