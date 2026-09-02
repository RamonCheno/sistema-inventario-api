namespace SistemaInventarioApi.DTOs
{
    public class CategoriaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class CreateCategoriaDto
    {
        public string Nombre { get; set; }
    }

    public class UpdateCategoriaDto
    {
        public string Nombre { get; set; }
    }
}
