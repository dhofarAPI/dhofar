using dhofarAPI.DTOS.Complaint;
using dhofarAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace dhofarAPI.InterFaces
{
    public interface IComplaint
    {
        public Task<List<GetComplaintdto>> GetAll();
        // public Task<List<Complaint>> GetAll(int userId);
        public Task<Complaint> EditStatus(EditComplaintStatus ST);

        public Task<GetComplaintdto> Create(PostComplaintdto complaint, List<IFormFile> Files);
        public Task<string> Delete(int Id , string why);

        public Task<List<GetComplaintdto>> GetByDate(DateTime From , DateTime To);

        public Task<List<GetComplaintdto>> GetMyComplaints();
        public Task<Complaint> Accept(AcceptedComplaint ST);




    }
}
