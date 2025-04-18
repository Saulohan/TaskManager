namespace TaskManagerAPI.Utils
{
    public static class Utils
    {
        public static void SaveLogError(Exception ex)
        {
            string logFilePath = "logs/error_log.txt"; // Caminho para o arquivo de log

            // Certificar-se de que o diretório existe
            var logDirectory = Path.GetDirectoryName(logFilePath);
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            string logMessage = $"{DateTime.UtcNow}: {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}\n\n";

            try
            {
                File.AppendAllText(logFilePath, logMessage);
            }
            catch (Exception fileEx)
            {
                Console.WriteLine($"Erro ao tentar registrar log: {fileEx.Message}");
            }

            //salvar log na tabela de log do banco de dados
        }
    }
}
