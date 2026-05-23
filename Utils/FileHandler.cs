namespace project.Utils;

public class FileHandler(string inputFolder, string outputFolder, string errorLogFile, int batchSize) {
    public IEnumerable<FileInfo> RetrieveFiles(string pattern = "*") {
        if (!Directory.Exists(inputFolder)) {
            throw new DirectoryNotFoundException($"{inputFolder} does not exist");
        }

        DirectoryInfo directory = new(inputFolder);
        return directory.GetFiles($"{pattern}.txt").OrderBy(f => f.LastWriteTime);
    }

    public void ProcessFile<TEntity>(FileInfo file, Func<string, TEntity> parse, Action<List<TEntity>> save) {
        if (!file.Exists) {
            throw new FileNotFoundException($"{file.FullName} does not exist");
        }

        if (!Directory.Exists(outputFolder)) {
            Directory.CreateDirectory(outputFolder);
        }

        var batch = new List<TEntity>();

        bool TrySave(in List<TEntity> batch) {
            bool success = false;

            try {
                save(batch);
                success = true;
            } catch (Exception ex) {
                File.AppendAllText("Errors.txt", $"{file.Name} | Batch write | Error: {ex.Message}\n");
            } finally {
                batch.Clear();
            }

            return success;
        }

        int lineNumber = 0;
        foreach (string line in File.ReadLines(file.FullName)) {
            lineNumber++;

            try {
                batch.Add(parse(line));
            } catch (Exception ex) {
                File.AppendAllText(errorLogFile, $"{file.Name} | Row: {lineNumber} | Error: {ex.Message}\n");
                continue;
            }

            if (batch.Count == batchSize && !TrySave(batch)) {
                break;
            }
        }

        if (batch.Count > 0) {
            TrySave(batch);
        }

        string destination = Path.Combine(outputFolder, file.Name);
        if (File.Exists(destination)) {
            File.Delete(destination);
        }

        file.MoveTo(destination);
    }
}
