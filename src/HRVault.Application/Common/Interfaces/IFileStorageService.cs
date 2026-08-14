namespace HRVault.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadAsync(
        Stream stream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);

    Task<Stream> DownloadAsync(
        string storageName,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string storageName,
        CancellationToken cancellationToken = default);
}