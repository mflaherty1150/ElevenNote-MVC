using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevenNote.Models.Note
{
    public class NoteEdit
    {
        [Required]
        public int NoteId { get; set; }
        [Required] 
        public string Title { get; set; }
        [Required] 
        public string Content { get; set; }
        [Required] 
        public int CategoryId { get; set; }
    }
}
