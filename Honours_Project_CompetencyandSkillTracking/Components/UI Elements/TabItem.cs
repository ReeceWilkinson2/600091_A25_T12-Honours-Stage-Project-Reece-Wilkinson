using Microsoft.AspNetCore.Components;

namespace Honours_Project_CompetencyandSkillTracking.Components.UI_Elements
{
    public class TabItem
    {
        public string Title { get; set; } = string.Empty;
        public RenderFragment? ChildContent { get; set; }
        public bool Active { get; set; }
    }

}