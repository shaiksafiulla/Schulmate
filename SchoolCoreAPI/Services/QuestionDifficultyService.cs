using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class QuestionDifficultyService : IQuestionDifficultyService
    {
        private readonly APIContext _context;
        public QuestionDifficultyService(APIContext context)
        {
            _context = context;
        }

        public List<VQuestionDifficulties> GetAllQuestionDifficulty()
        {
            return _context.VQuestionDifficulties.AsNoTracking().OrderBy(x => x.Id).ToList();
        }

        public VQuestionDifficulties ViewQuestionDifficulty(int Id)
        {
            return _context.VQuestionDifficulties.AsNoTracking().SingleOrDefault(m => m.Id == Id);
        }

        public QuestionDifficulty GetQuestionDifficulty(int Id)
        {
            if (Id <= 0) return new QuestionDifficulty();

            var cat = _context.QuestionDifficulty.AsNoTracking().SingleOrDefault(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
            if (cat == null) return new QuestionDifficulty();

            return new QuestionDifficulty
            {
                Id = cat.Id,
                Name = cat.Name,
                Description = cat.Description
            };
        }

        public bool IsRecordExists(string name, int Id)
        {
            return Id == 0
                ? _context.QuestionDifficulty.Any(e => e.Name == name && e.IsActive == ((int)ActiveState.Active).ToString())
                : _context.QuestionDifficulty.Any(e => e.Name == name && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public ServiceResult SaveQuestionDifficulty(QuestionDifficulty model, int UserId)
        {
            var cat = new QuestionDifficulty();
            var result = new ServiceResult();
            if (model.Id > 0)
            {
                cat = _context.QuestionDifficulty.SingleOrDefault(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                if (cat == null) return null;

                cat.Name = model.Name;
                cat.Description = model.Description;
                cat.LastModifiedDate = DateTime.Now;
                cat.LastModifiedByUserId = UserId;
                _context.Entry(cat).State = EntityState.Modified;
            }
            else
            {
                cat = new QuestionDifficulty
                {
                    Name = model.Name,
                    Description = model.Description,
                    IsActive = ((int)ActiveState.Active).ToString(),
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = UserId,
                    LastModifiedDate = DateTime.Now,
                    LastModifiedByUserId = UserId
                };
                _context.QuestionDifficulty.Add(cat);
            }

            var changes = _context.SaveChanges();
            if (changes > 0)
            {
                result.StatusId = model.Id > 0 ? (int)ServiceResultStatus.Updated : (int)ServiceResultStatus.Added;
                result.RecordId = cat.Id;
            }

            return result;
        }

        public ServiceResult DeleteQuestionDifficulty(int Id, int UserId)
        {
            var cat = _context.QuestionDifficulty.SingleOrDefault(m => m.Id == Id);
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
