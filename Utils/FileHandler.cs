namespace project.Utils;

public class FileHandler(string inputFolder, string outputFolder, string errorLogFile, int batchSize) {
    public IEnumerable<FileInfo>? RetrieveFiles(string pattern = "*") {
        if (!Directory.Exists(inputFolder)) {
            Console.WriteLine($"{inputFolder} does not exist");
            return null;
        }

        DirectoryInfo directory = new(inputFolder);
        return directory.GetFiles($"{pattern}.txt").OrderBy(f => f.LastWriteTime);
    }

    public void ProcessFile<TEntity>(FileInfo file, Func<string, TEntity> parse, Action<List<TEntity>> save) {
        if (!file.Exists) {
            Console.WriteLine($"{file.FullName} does not exist");
            return;
        }

        var batch = new List<TEntity>();

        int lineNumber = 0;
        foreach (string line in File.ReadLines(file.FullName)) {
            lineNumber++;

            try {
                TEntity entity = parse(line);

                if (entity != null) {
                    batch.Add(entity);
                }
            } catch (Exception ex) {
                File.AppendAllText(errorLogFile, $"{file.Name} | Рядок: {lineNumber} | Помилка: {ex.Message}\n");
            }

            if (batch.Count == batchSize) {
                save(batch);
                batch.Clear();
            }
        }

        if (batch.Count > 0) {
            save(batch);
        }

        string destination = Path.Combine(outputFolder, file.Name);
        if (File.Exists(destination)) {
            File.Delete(destination);
        }

        file.MoveTo(destination);
    }
}
