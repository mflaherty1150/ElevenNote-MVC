using ElevenNote.Data;
using ElevenNote.Models.Category;
using ElevenNote.Models.Note;

namespace ElevenNote.Services.NoteService
{
    public interface INoteService
    {
        bool CreateNote(NoteCreate model);
        IEnumerable<NoteListItem> GetNotes();
        void SetUserId(Guid userId);
        NoteDetail GetNoteById(int id);
        bool UpdateNote(NoteEdit model);
        bool DeleteNote(int noteId);
        IEnumerable<CategoryListItem> CreateCategoryDropDownList();
    }
}