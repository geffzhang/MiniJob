using System.ComponentModel.DataAnnotations;

namespace MiniJob.Jobs
{
    public class CreateUpdateAppInfoDto
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        [Required]
        [StringLength(128)]
        public virtual string AppName { get; set; }

        /// <summary>
        /// 应用描述
        /// </summary>
        [StringLength(512)]
        public virtual string Description { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [Required]
        public virtual bool IsEnabled { get; set; }
    }
}
