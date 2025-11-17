using Microsoft.AspNetCore.Mvc.Rendering;

namespace nexumApp.Models
{
    public class Tags
    {
        public List<string> TagsNames = [
            "Proteção Animal", // 0
            "Combate á fome", // 1
            "Meio ambiente", // ...
            "Crianças", 
            "Desenvolvimento", 
            "Educação", 
            "Sustentabilidade",
            "Arte, Cultura e Esporte",
            "Combate á pobreza",
            "Direitos Humanos",
            "Outros"
            ];
        public List<SelectListItem> TagsList { get; set; } = [];

        public Tags()
        {
            for (int i = 0; i < TagsNames.Count; i++)
            {
                var Tag = new SelectListItem
                {
                    Value = i.ToString(),
                    Text = TagsNames[i],
                };
                TagsList.Add(Tag);
            }
        }
    }
}
