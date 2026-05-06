using System.Threading.Tasks;

namespace MedGyn.MedForce.Service.Interfaces
{
	public interface IBlobStorageService
	{
		Task<string> UploadFileToBlob(string fileName, string base64String);
	}
}
