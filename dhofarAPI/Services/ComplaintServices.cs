using dhofarAPI.Data;
using dhofarAPI.DTOS.Complaint;
using dhofarAPI.DTOS.ComplaintFiles;
using dhofarAPI.InterFaces;
using dhofarAPI.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using Xabe.FFmpeg;

namespace dhofarAPI.Services
{
    public class ComplaintServices : IComplaint
    {
        private readonly dhofarDBContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ComplaintServices(dhofarDBContext db , IWebHostEnvironment webHostEnvironment )
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<List<GetComplaintdto>> GetAll()
        {
            var x = await _db.Complaints.ToListAsync();
            var y = new List<GetComplaintdto>();
            foreach (var item in y)
            {
                foreach (var item1 in x)
                {
                    item.Description = item1.Description;
                    item.Location = item1.Location;
                    item.State = item1.State;
                    item.Time = item1.Time;
                    item.Title = item1.Title;
                    item.Type = item1.Type;
                    item.Status = item1.Status;
                }
            };
            return y;
        }
        public async Task<GetComplaintdto> Create(PostComplaintdto complaint, List<IFormFile> files)
        {
            var now = DateTime.UtcNow;

            Complaint newComplaint = new Complaint()
            {
                CategoryId = complaint.CategoryId,
                Description = complaint.Description,
                Location = complaint.Location,
                State = complaint.State,
                Title = complaint.Title,
                Status = "New",
                IsAccepted = false,
                Type = complaint.Type,
                Time = now,
                Files = new List<ComplaintsFile>()
            };

            // Process files concurrently
            var tasks = files.Select(async file =>
            {
                if (file == null || file.Length == 0)
                    return;

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);

                    if (IsImageFile(file.FileName))
                    {
                        // Handle image resizing and compression
                        var imageBytes = await ResizeAndCompressImage(stream.ToArray());
                        string fileName = $"{Guid.NewGuid()}_{file.FileName}";
                        string filePath = SaveFileToServer(fileName, imageBytes);

                        var newComplaintFile = new ComplaintsFile
                        {
                            Filename = fileName,
                            FilePaths = filePath
                        };
                        lock (newComplaint.Files)
                        {
                            newComplaint.Files.Add(newComplaintFile);
                        }
                    }
                    else if (IsVideoFile(file.FileName))
                    {
                        // Handle video transcoding and compression
                        var videoBytes = await TranscodeAndCompressVideo(stream.ToArray());
                        string fileName = $"{Guid.NewGuid()}_{file.FileName}";
                        string filePath = SaveFileToServer(fileName, videoBytes);

                        var newComplaintFile = new ComplaintsFile
                        {
                            Filename = fileName,
                            FilePaths = filePath
                        };
                        lock (newComplaint.Files)
                        {
                            newComplaint.Files.Add(newComplaintFile);
                        }
                    }
                }
            });

            await Task.WhenAll(tasks);

            // Save the complaint to the database
            // _db.Complaints.Add(newComplaint);
            // await _db.SaveChangesAsync();

            // Prepare and return response DTO
            var getComplaintdto = new GetComplaintdto
            {
                Description = newComplaint.Description,
                Location = newComplaint.Location,
                State = newComplaint.State,
                Title = newComplaint.Title,
                Status = "New",
                Type = newComplaint.Type,
            };
            return getComplaintdto;
        }

        private async Task<byte[]> ResizeAndCompressImage(byte[] imageBytes)
        {
            using (var image = SixLabors.ImageSharp.Image.Load(imageBytes))
            {
                var width = 800;
                var height = 600;
                image.Mutate(x => x.Resize(width, height));

                var quality = 75;
                using (var outputStream = new MemoryStream())
                {
                    var encoder = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder
                    {
                        Quality = quality
                    };
                    await image.SaveAsync(outputStream, encoder);
                    return outputStream.ToArray();
                }
            }
        }

        private async Task<byte[]> TranscodeAndCompressVideo(byte[] videoBytes)
        {
            string inputFilePath = Path.GetTempFileName() + ".mp4";
            string outputFilePath = Path.GetTempFileName() + ".mp4";

            await File.WriteAllBytesAsync(inputFilePath, videoBytes);

            var mediaInfo = await FFmpeg.GetMediaInfo(inputFilePath);
            var videoStream = mediaInfo.VideoStreams.Single();

            var conversion = FFmpeg.Conversions.New()
                .AddStream(videoStream)
                .AddParameter($"-r 30")
                .AddParameter($"-s {videoStream.Width}x{videoStream.Height}")
                .AddParameter($"-b:v {videoStream.Bitrate * 0.8}k")
                .SetOutput(outputFilePath);

            await conversion.Start();

            byte[] result = await File.ReadAllBytesAsync(outputFilePath);

            File.Delete(inputFilePath);
            File.Delete(outputFilePath);

            return result;
        }

        private string SaveFileToServer(string fileName, byte[] fileBytes)
        {
            string directoryPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            string filePath = Path.Combine(directoryPath, fileName);

            Directory.CreateDirectory(directoryPath);
            File.WriteAllBytes(filePath, fileBytes);

            return filePath;
        }


        private bool IsImageFile(string fileName)
        {
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            return Array.Exists(imageExtensions, ext => fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsVideoFile(string fileName)
        {
            string[] videoExtensions = { ".mp4", ".avi", ".mov", ".mkv" };
            return Array.Exists(videoExtensions, ext => fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }

 

    public async Task<string> Delete(int Id , string why)
        {
            var deletedComplaint = await _db.Complaints.FindAsync(Id);
            if (deletedComplaint != null)
            {
                _db.Entry(deletedComplaint).State = EntityState.Deleted;
                await _db.SaveChangesAsync();
                return why;
            }
            return "Not Found";
            
        }

        public async Task<Complaint> EditStatus(EditComplaintStatus ST)
        {
            var EditedComplaint = await _db.Complaints.FindAsync(ST.Id);
            if (EditedComplaint != null)
            {
                EditedComplaint.Status = ST.Status;
                _db.Entry(EditedComplaint).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }
            return EditedComplaint;

        }

        public async Task<Complaint> Accept(AcceptedComplaint ST)
        {
            var EditedComplaint = await _db.Complaints.FindAsync(ST.Id);
            if (EditedComplaint != null)
            {
                EditedComplaint.IsAccepted = ST.IsAccepted;
                _db.Entry(EditedComplaint).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }
            return EditedComplaint;

        }

        public async Task<List<GetComplaintdto>> GetByDate(DateTime From, DateTime To)
        {
          var x=  await _db.Complaints.Where(x => x.Time >= From && x.Time <= To).ToListAsync();
            var y = new List<GetComplaintdto>();
            foreach (var item in y)
            {
                foreach (var item1 in x)
                {
                    item.Description = item1.Description;
                    item.Location = item1.Location;
                    item.State = item1.State;
                    item.Time = item1.Time;
                    item.Title = item1.Title;
                    item.Type = item1.Type;
                    item.Status = item1.Status;
                }
            };
            return y;
        }

        public async Task<List<GetComplaintdto>> GetMyComplaints()
        {
            var x = await _db.Complaints.Where(x => x.IsAccepted == true).ToListAsync();
            var y = new List<GetComplaintdto>() ;
            foreach (var item in y)
            {
                foreach (var item1 in x)
                {
                    item.Description = item1.Description;
                    item.Location = item1.Location;
                    item.State = item1.State;
                    item.Time = item1.Time;
                    item.Title = item1.Title;
                    item.Type = item1.Type;
                    item.Status = item1.Status;
                }
            };
            return y;

        }
    }
}
