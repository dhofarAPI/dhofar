using Microsoft.AspNetCore.Mvc;
using dhofarAPI.Model;
using dhofarAPI.InterFaces;
using dhofarAPI.DTOS.Complaint;

namespace dhofarAPI.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class ComplaintsController : ControllerBase
    {
        private readonly IComplaint _complaint;

        public ComplaintsController(IComplaint complaint)
        {
            _complaint = complaint;
        }

        // GET: api/Complaint
        [HttpGet]
        public async Task<ActionResult<List<GetComplaintdto>>> GetComplaints()
        {
            var complaints = await _complaint.GetAll();
            return Ok(complaints);
        }

        // GET: api/Complaint/bydate
        [HttpGet]
        public async Task<ActionResult<List<GetComplaintdto>>> GetComplaintsByDateRange( DateTime from,  DateTime to)
        {
            if (from > to)
            {
                return BadRequest("From date must be earlier than To date.");
            }

            var complaints = await _complaint.GetByDate(from, to);
            return Ok(complaints);
        }

        // POST: api/Complaint
        [HttpPost]
        public async Task<ActionResult> PostComplaint(PostComplaintdto complaint,List<IFormFile> Files)
        {
            var createdComplaint = await _complaint.Create(complaint, Files);
            return Ok(createdComplaint);
        }

        // DELETE: api/Complaint/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComplaint(int id , string why)
        {
            await _complaint.Delete(id, why);
            return NoContent();
        }

        // PUT: api/Complaint/EditStatus
        [HttpPut("EditStatus")]
        public async Task<IActionResult> PUTCompliment(EditComplaintStatus ST)
        {

            var editedComplaint = await _complaint.EditStatus(ST);
            return Ok(editedComplaint);
        }

        [HttpPut("Accept")]
        public async Task<IActionResult> AcceptComplaint(AcceptedComplaint ST)
        {

            var editedComplaint = await _complaint.Accept(ST);
            return Ok(editedComplaint);
        }

        [HttpGet("mycomplaint")]
        public async Task<ActionResult<List<GetComplaintdto>>> GetMyComplaints()
        {
            var myComplaints = await _complaint.GetMyComplaints();
            return Ok(myComplaints);
        }
    }
}
