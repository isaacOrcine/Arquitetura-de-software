using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ModularizationOportunities.core;

public class SyntaxAnalyzer
{
    private readonly SemanticModel _semanticModel;

    public SyntaxAnalyzer(SemanticModel semanticModel)
    {
        _semanticModel = semanticModel;
    }

    public IEnumerable<INamedTypeSymbol> GetExplicitlyReferencedClasses(MethodDeclarationSyntax methodDeclaration)
    {
        var referencedClasses = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

        var nodes = methodDeclaration.DescendantNodes();
        foreach (var node in nodes)
        {
            var symbolInfo = _semanticModel.GetSymbolInfo(node);
            if (symbolInfo.Symbol is ITypeSymbol typeSymbol && typeSymbol.TypeKind == TypeKind.Class)
            {
                referencedClasses.Add((INamedTypeSymbol)typeSymbol);
            }
        }

        return referencedClasses;
    }
    
    public IEnumerable<IMethodSymbol> GetCalledMethods(MethodDeclarationSyntax methodDeclaration)
    {
        var calledMethods = new List<IMethodSymbol>();

        var invocationExpressions = methodDeclaration.DescendantNodes().OfType<InvocationExpressionSyntax>();
        foreach (var invocation in invocationExpressions)
        {
            var symbolInfo = _semanticModel.GetSymbolInfo(invocation);
            if (symbolInfo.Symbol is IMethodSymbol methodSymbol)
            {
                calledMethods.Add(methodSymbol);
            }
        }

        return calledMethods;
    }
}