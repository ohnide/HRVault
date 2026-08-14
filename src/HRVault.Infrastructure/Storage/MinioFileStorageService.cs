using HRVault.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace HRVault.Infrastructure.Storage;

public class MinioFileStorageService
    : IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioOptions _options;

    public MinioFileStorageService(
		IOptions<MinioOptions> options)
	{
		_options = options.Value;

		var client = new MinioClient()
			.WithEndpoint(_options.Endpoint)
			.WithCredentials(
				_options.AccessKey,
				_options.SecretKey);

		if (_options.UseSsl)
		{
			client = client.WithSSL();
		}

		_minioClient = client.Build();
	}

    public async Task<string> UploadAsync(
        Stream stream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        await EnsureBucketExistsAsync(
            cancellationToken);

        var extension =
            Path.GetExtension(fileName);

        var storageName =
            $"{Guid.NewGuid():N}{extension}";

        if (stream.CanSeek)
            stream.Position = 0;

        var args = new PutObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(storageName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(contentType);

        await _minioClient.PutObjectAsync(
            args,
            cancellationToken);

        return storageName;
    }

    public async Task<Stream> DownloadAsync(
        string storageName,
        CancellationToken cancellationToken = default)
    {
        var memoryStream = new MemoryStream();

        var args = new GetObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(storageName)
            .WithCallbackStream(
                stream => stream.CopyTo(memoryStream));

        await _minioClient.GetObjectAsync(
            args,
            cancellationToken);

        memoryStream.Position = 0;

        return memoryStream;
    }

    public async Task DeleteAsync(
        string storageName,
        CancellationToken cancellationToken = default)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(storageName);

        await _minioClient.RemoveObjectAsync(
            args,
            cancellationToken);
    }

    private async Task EnsureBucketExistsAsync(
        CancellationToken cancellationToken)
    {
        var existsArgs = new BucketExistsArgs()
            .WithBucket(_options.BucketName);

        var exists =
            await _minioClient.BucketExistsAsync(
                existsArgs,
                cancellationToken);

        if (exists)
            return;

        var createArgs = new MakeBucketArgs()
            .WithBucket(_options.BucketName);

        await _minioClient.MakeBucketAsync(
            createArgs,
            cancellationToken);
    }
}