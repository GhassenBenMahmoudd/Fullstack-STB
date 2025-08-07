using stb_backend.Domain;

namespace stb_backend.Interfaces
{
    public interface IDocumentFileService
    {
        Task SaveAsync(DocumentFile file);
        Task UpdateAsync(long cadeauId, List<IFormFile> newFiles, List<long> existingFileIds);


    }
}
