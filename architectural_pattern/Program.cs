using System;
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string mvcProjectPath = @"C:\Users\Isaac\Desktop\A\UFLA\Arquitetura de software\TP5\MvcErrorExample";
        string mvpProjectPath = @"C:\Users\Isaac\Desktop\A\UFLA\Arquitetura de software\TP5\Transformed\MvpTransformedProject2";

        try
        {
            if (CompileProject(mvcProjectPath))
            {
                Console.WriteLine("Projeto MVC compilado com sucesso.");
                CreateMvpProject(mvcProjectPath, mvpProjectPath);
                Console.WriteLine("Projeto MVP criado com sucesso.");
            }
            else
            {
                Console.WriteLine("Falha na compilação do projeto MVC.");
                Analyzer analyzer = new Analyzer();
                analyzer.AnalyzeCode(mvcProjectPath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro: {ex.Message}");
        }
    }

    static bool CompileProject(string projectPath)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo("dotnet", $"build {projectPath}")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = Process.Start(startInfo))
        {
            process.WaitForExit();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            Console.WriteLine(output);
            Console.WriteLine(error);

            return process.ExitCode == 0;
        }
    }

    static void CreateMvpProject(string sourcePath, string destinationPath)
    {
        if (Directory.Exists(destinationPath))
        {
            Directory.Delete(destinationPath, true);
        }

        Directory.CreateDirectory(destinationPath);

        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
        }

        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
        }
    }
}
