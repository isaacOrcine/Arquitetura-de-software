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

        // Conversão de MVC para MVP
        ConvertMvcToMvp(destinationPath);
    }

    static void ConvertMvcToMvp(string projectPath)
    {
        // Exemplo de conversão de um controlador MVC para um presenter MVP
        string controllerPath = Path.Combine(projectPath, "Controllers", "UserController.cs");
        string presenterPath = Path.Combine(projectPath, "Presenters", "UserPresenter.cs");
        string viewInterfacePath = Path.Combine(projectPath, "Views", "IUserView.cs");

        if (File.Exists(controllerPath))
        {
            string controllerCode = File.ReadAllText(controllerPath);

            // Exemplo simples de transformação de código
            string presenterCode = controllerCode.Replace("UserController", "UserPresenter")
                                                 .Replace("Controller", "Presenter");

            string viewInterfaceCode = @"
public interface IUserView
{
    void DisplayUser(UserModel user);
}";

            // Cria diretórios se não existirem
            Directory.CreateDirectory(Path.Combine(projectPath, "Presenters"));
            Directory.CreateDirectory(Path.Combine(projectPath, "Views"));

            // Salva o código transformado
            File.WriteAllText(presenterPath, presenterCode);
            File.WriteAllText(viewInterfacePath, viewInterfaceCode);

            Console.WriteLine("Conversão de MVC para MVP concluída.");
        }
        else
        {
            Console.WriteLine("Controlador MVC não encontrado.");
        }
    }
}
