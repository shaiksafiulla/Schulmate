using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolCoreAPI.Helpers;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;

namespace SchoolCoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionController : ControllerBase
    {
        private readonly ISectionService _sectionService;

        public SectionController(ISectionService sectionService)
        {
            _sectionService = sectionService;
        }
        private UserParam GetUserParam()
        {
            return Shared.GetUserParamFromHeader<UserParam>(Request.Headers, "UserParam");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var lst = await _sectionService.GetAllSection(userParam.UserId);
            return Ok(lst);
        }

        [HttpGet("View/{id}")]
        [Authorize]
        public async Task<IActionResult> View(int id)
        {
            var cls = await _sectionService.ViewSection(id);
            return Ok(cls);
        }

        [HttpGet("AddEdit/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEdit(int id)
        {
            var cls = await _sectionService.GetSection(id);
            return Ok(cls);
        }

        [HttpGet("IsExist/{name},{branchClassId},{id}")]
        [Authorize]
        public async Task<IActionResult> IsExist(string name, int branchClassId, int id)
        {
            var isExists = await _sectionService.IsRecordExists(name, branchClassId, id);
            return Ok(isExists);
        }

        [HttpPost("Save")]
        [Authorize]
        public async Task<IActionResult> Save(Section model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _sectionService.SaveSection(model, userParam.UserId);
            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var result = await _sectionService.DeleteSection(id, userParam.UserId);
            return Ok(result);
        }

        [HttpGet("GetClassesByBranch/{branchId}")]
        [Authorize]
        public async Task<IActionResult> GetClassesByBranch(string branchId)
        {
            var lst = await _sectionService.GetClassesByBranch(branchId);
            return Ok(lst);
        }

        [HttpGet("GetSectionsByBranchAndClass/{branchId},{classId}")]
        [Authorize]
        public async Task<IActionResult> GetSectionsByBranchAndClass(string branchId, string classId)
        {
            var lst = await _sectionService.GetSectionsByBranchAndClass(branchId, classId);
            return Ok(lst);
        }

        [HttpGet("SectionTeacher/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEditSectionTeacher(int id)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var vm = await _sectionService.GetSectionVM(id, userParam.UserId);
            return Ok(vm);
        }

        [HttpGet("MbSection")]
        [Authorize]
        public async Task<IActionResult> GetSectionsByPersonnelId()
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var vm = await _sectionService.GetSectionsByPersonnelId(userParam.PersonId, userParam.UserType, userParam.BranchId, userParam.SessionYearId);
            return Ok(vm);
        }

        [HttpGet("MbBranchClass")]
        [Authorize]
        public async Task<IActionResult> GetBranchClassesByPersonnelId()
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var vm = await _sectionService.GetBranchClassesByPersonnelId(userParam.PersonId, userParam.UserType, userParam.SessionYearId);
            return Ok(vm);
        }

        [HttpGet("SubjectLesson/{branchclassid}")]
        [Authorize]
        public async Task<IActionResult> GetSubjectLessonsByClassId(int branchClassId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var vm = await _sectionService.GetSubjectLessonsByClassId(branchClassId, userParam.SessionYearId);
            return Ok(vm);
        }

        [HttpGet("SectionActivityPersonnel/{id}")]
        [Authorize]
        public async Task<IActionResult> AddEditSectionActivityPersonnel(int id)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var vm = await _sectionService.GetSectionActivityVM(id, userParam.UserId);
            return Ok(vm);
        }

        [HttpPost("SectionTeacher/Save")]
        [Authorize]
        public async Task<IActionResult> UpdateSectionTeacher(SectionVM model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var vm = await _sectionService.UpdateBranchClassSectionTeacher(model, userParam.UserId, userParam.SessionYearId);
            return Ok(vm);
        }

        [HttpPost("SectionActivity/Save")]
        [Authorize]
        public async Task<IActionResult> UpdateSectionActivity(SectionActivityVM model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var vm = await _sectionService.UpdateBranchClassSectionActivityPersonnel(model, userParam.UserId);
            return Ok(vm);
        }

        [HttpGet("TimeTable/{id}")]
        [Authorize]
        public async Task<IActionResult> GetSectionTimeTable(int id)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var cls = await _sectionService.GetSectionTimeTableVM(id, userParam.UserId);
            return Ok(cls);
        }

        [HttpGet("TransTimeTable/{id}")]
        [Authorize]
        public async Task<IActionResult> GetSectionTransTimeTable(int id)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var cls = await _sectionService.GetSectionTransTimeTableVM(id, userParam.SessionYearId);
            return Ok(cls);
        }

        [HttpGet("MbTimeTable/{sectionId}")]
        [Authorize]
        public async Task<IActionResult> GetTimeTableBySectionAndSessionYear(int sectionId)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var lst = await _sectionService.GetTimeTable(sectionId, userParam.SessionYearId, userParam.UserId);
            return Ok(lst);
        }

        [HttpPost("TimeTable/Save")]
        [Authorize]
        public async Task<IActionResult> UpdateSectionTimeTable(TimeTableDTO model)
        {
            var userParam = GetUserParam();
            if (userParam == null)
            {
                return BadRequest("Missing or Invalid Header");
            }
            var vm = await _sectionService.UpdateBranchClassSectionTimeTable(model, userParam.UserId);
            return Ok(vm);
        }
    }
}
