using ElevenNote.Data;
using ElevenNote.Models.Category;
using ElevenNote.Models.Note;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevenNote.Services.NoteService
{
    public class NoteService : INoteService
    {
        private readonly ApplicationDbContext _context;
        private Guid _userId;

        public NoteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool CreateNote(NoteCreate model)
        {
            var noteEntity = new Note
            {
                OwnerId = _userId,
                Title = model.Title,
                Content = model.Content,
                CategoryId = model.CategoryId,
                CreatedUtc = DateTimeOffset.Now
            };

            _context.Notes.Add(noteEntity);
            return _context.SaveChanges() == 1;
        }

        public IEnumerable<NoteListItem> GetNotes()
        {
            var notes = _context.Notes
                .Where(e => e.OwnerId == _userId)
                .Select(e =>
                    new NoteListItem()
                    {
                        NoteId = e.NoteId,
                        Title = e.Title,
                        IsStarred = e.IsStarred,
                        CreatedUtc = e.CreatedUtc,
                        CategoryName = e.Category.Name
                    }).ToList();
            return notes;
        }

        public NoteDetail GetNoteById(int id)
        {
            var note = _context.Notes
                .Single(e => e.NoteId == id && e.OwnerId == _userId);
            return new NoteDetail()
            {
                NoteId = note.NoteId,
                Title = note.Title,
                Content = note.Content,
                CreatedUtc = note.CreatedUtc,
                ModifiedUtc = note.ModifiedUtc
            };
        }

        public bool UpdateNote(NoteEdit model)
        {
            var note = _context.Notes
                        .Single(e => e.NoteId == model.NoteId && e.OwnerId == _userId);
            note.Title = model.Title;
            note.Content = model.Content;
            note.ModifiedUtc = DateTimeOffset.UtcNow;

            return _context.SaveChanges() == 1;
        }

        public bool DeleteNote(int noteId)
        {
            var entity = _context.Notes
                .SingleOrDefault(e => e.NoteId == noteId && e.OwnerId == _userId);

            _context.Notes.Remove(entity);

            return _context.SaveChanges() == 1;
        }

        public IEnumerable<CategoryListItem> CreateCategoryDropDownList()
        {
            var categoryService = new CategoryService.CategoryService(_context);
            categoryService.SetUserId(_userId);
            var userCategories = categoryService.GetAllCategories().Where(e => e.OwnwerId == _userId);
            return userCategories;
        }


        public void SetUserId(Guid userId) => _userId = userId;
    }
}
