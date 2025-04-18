using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models.Enums;

namespace TaskManager.Domain.Entities
{
    public class Project : BaseModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        // Data de vencimento do projeto (opcional)
        public DateTime? DueDate { get; set; }

        public string Status { get; set; }

        public virtual User User { get; set; }

        public virtual List<TaskItem> Tasks { get; set; } = new List<TaskItem>();

        //public long ProjectId { get; set; }

        //public override long GetId() => ProjectId;

        //public override void SetId(long id) => ProjectId = id;
    }
}