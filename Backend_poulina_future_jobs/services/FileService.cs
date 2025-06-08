//using Backend_poulina_future_jobs.Dtos;
//using Backend_poulina_future_jobs.Models;

//namespace Backend_poulina_future_jobs.services
//{
//    public interface IFileService
//    {
//        Task<FilePaths> SaveCandidatureFilesAsync(CandidatureCreateDto dto, string userId);
//        Task<FilePaths> UpdateCandidatureFilesAsync(CandidatureUpdateDto dto, Candidature existing);
//    }

//    public class FileService : IFileService
//    {
//        private readonly string _uploadsPath;

//        public FileService(IWebHostEnvironment env)
//        {
//            _uploadsPath = Path.Combine(env.ContentRootPath, "uploads");
//            Directory.CreateDirectory(_uploadsPath);
//        }

//        public async Task<FilePaths> SaveCandidatureFilesAsync(CandidatureCreateDto dto, string userId)
//        {
//            var paths = new FilePaths();

//            if (dto.CvFile != null)
//                paths.CvPath = await SaveFileAsync(dto.CvFile, userId, "cv");

//            if (dto.LettreMotivationFile != null)
//                paths.LettrePath = await SaveFileAsync(dto.LettreMotivationFile, userId, "lettre");

//            return paths;
//        }

//        private async Task<string> SaveFileAsync(IFormFile file, string userId, string type)
//        {
//            // Implémentation de la sauvegarde sécurisée
//        }
//    }
//}
