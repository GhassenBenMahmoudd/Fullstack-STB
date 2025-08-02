namespace stb_backend.DTOs
{
    public class DocumentFileDto
    {
        public long IdFile { get; set; }
        public string FileName { get; set; }

        // On expose l'URL de téléchargement plutôt que le chemin physique du serveur
        public string DownloadUrl { get; set; }

        public DateTime DateUpload { get; set; }
    }
}
