using Microsoft.EntityFrameworkCore;
using SchoolCoreAPI.IServices;
using SchoolCoreAPI.Models;
using static SchoolCoreAPI.Helpers.Shared;

namespace SchoolCoreAPI.Services
{
    public class QuestionCategoryService : IQuestionCategoryService
    {
        private readonly APIContext _context;
        public QuestionCategoryService(APIContext context)
        {
            _context = context;
        }

        public List<VQuestionCategories> GetAllQuestionCategory()
        {
            return _context.VQuestionCategories.AsNoTracking().OrderBy(x => x.Id).ToList();
        }

        public VQuestionCategories ViewQuestionCategory(int Id)
        {
            return _context.VQuestionCategories.AsNoTracking().SingleOrDefault(m => m.Id == Id);
        }

        public QuestionCategory GetQuestionCategory(int Id)
        {
            if (Id <= 0) return new QuestionCategory();

            var cat = _context.QuestionCategory.AsNoTracking().SingleOrDefault(x => x.Id == Id && x.IsActive == ((int)ActiveState.Active).ToString());
            if (cat == null) return new QuestionCategory();

            return new QuestionCategory
            {
                Id = cat.Id,
                Name = cat.Name,
                Description = cat.Description
            };
        }

        public bool IsRecordExists(string name, int Id)
        {
            return Id == 0
                ? _context.QuestionCategory.Any(e => e.Name == name && e.IsActive == ((int)ActiveState.Active).ToString())
                : _context.QuestionCategory.Any(e => e.Name == name && e.Id != Id && e.IsActive == ((int)ActiveState.Active).ToString());
        }

        public ServiceResult SaveQuestionCategory(QuestionCategory model, int UserId)
        {
            var cat = new QuestionCategory();
            var result = new ServiceResult();
            if (model.Id > 0)
            {
                cat = _context.QuestionCategory.SingleOrDefault(m => m.Id == model.Id && m.IsActive == ((int)ActiveState.Active).ToString());
                if (cat == null) return null;

                cat.Name = model.Name;
                cat.Description = model.Description;
                cat.LastModifiedDate = DateTime.Now;
                cat.LastModifiedByUserId = UserId;
                _context.Entry(cat).State = EntityState.Modified;
            }
            else
            {
                cat = new QuestionCategory
                {
                    Name = model.Name,
                    Description = model.Description,
                    IsActive = ((int)ActiveState.Active).ToString(),
                    CreatedDate = DateTime.Now,
                    CreatedByUserId = UserId,
                    LastModifiedDate = DateTime.Now,
                    LastModifiedByUserId = UserId
                };
                _context.QuestionCategory.Add(cat);
            }

            var changes = _context.SaveChanges();
            if (changes == 0) return null;

            result.StatusId = model.Id > 0 ? (int)ServiceResultStatus.Updated : (int)ServiceResultStatus.Added;
            result.RecordId = cat.Id;
            return result;
        }

        public ServiceResult DeleteQuestionCategory(int Id, int UserId)
        {
            var cat = _context.QuestionCategory.SingleOrDefault(m => m.Id == Id);
            if (cat == null) return null;

            cat.IsActive = ((int)ActiveState.InActive).ToString();
            cat.LastModifiedByUserId = UserId;
            cat.LastModifiedDate = DateTime.Now;
            _context.Entry(cat).State = EntityState.Modified;

            var changes = _context.SaveChanges();
            if (changes == 0) return null;

            return new ServiceResult
            {
                StatusId = (int)ServiceResultStatus.Deleted,
                RecordId = cat.Id
            };
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
