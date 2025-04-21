using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class QuestionTypeService : IQuestionTypeService
    {
        private readonly APIContext _context;
        public QuestionTypeService(APIContext context)
        {
            _context = context;
        }

        public List<VQuestionTypes> GetAllQuestionType()
        {
            return _context.VQuestionTypes.AsNoTracking().OrderBy(x => x.Id).ToList();
        }

        public VQuestionTypes ViewQuestionType(int Id)
        {
            return _context.VQuestionTypes.AsNoTracking().SingleOrDefault(m => m.Id == Id);
        }

        public QuestionType GetQuestionType(int Id)
        {
            if (Id <= 0) return new QuestionType();

            var cat = _context.QuestionType.AsNoTracking().SingleOrDefault(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
            if (cat == null) return new QuestionType();

            return new QuestionType
            {
                Id = cat.Id,
                Name = cat.Name,
                Description = cat.Description
            };
        }

        public bool IsRecordExists(string name, int Id)
        {
            return Id == 0
                ? _context.QuestionType.Any(e => e.Name == name && e.IsActive == ((int)ActiveState.Active).ToString())
                : _context.QuestionType.Any(e => e.Name == name && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public ServiceResult SaveQuestionType(QuestionType model, int UserId)
        {
            var cat = new QuestionType();
            var result = new ServiceResult();
            if (model.Id > 0)
            {
                cat = _context.QuestionType.SingleOrDefault(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                if (cat == null) return null;

                cat.Name = model.Name;
                cat.Description = model.Description;
                cat.LastModifiedDate = DateTime.Now;
                cat.LastModifiedByUserId = UserId;
                _context.Entry(cat).State = EntityState.Modified;
            }
            else
            {
                cat = new QuestionType
                {
                    Name = model.Name,
                    Description = model.Description,
                    IsActive = ((int)ActiveState.Active).ToString(),
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = UserId,
                    LastModifiedDate = DateTime.Now,
                    LastModifiedByUserId = UserId
                };
                _context.QuestionType.Add(cat);
            }

            var changes = _context.SaveChanges();
            if (changes > 0)
            {
                result.StatusId = model.Id > 0 ? (int)ServiceResultStatus.Updated : (int)ServiceResultStatus.Added;
                result.RecordId = cat.Id;
            }

            return result;
        }

        public ServiceResult DeleteQuestionType(int Id, int UserId)
        {
            var cat = _context.QuestionType.SingleOrDefault(m => m.Id == Id);
            if (cat == null) return null;

            cat.IsActive = ((int)ActiveState.InActive).ToString();
            cat.LastModifiedByUserId = UserId;
            cat.LastModifiedDate = DateTime.Now;
            _context.Entry(cat).State = EntityState.Modified;

            var changes = _context.SaveChanges();
            if (changes > 0)
            {
                return new ServiceResult
                {
                    StatusId = (int)ServiceResultStatus.Deleted,
                    RecordId = cat.Id
                };
            }

            return null;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
