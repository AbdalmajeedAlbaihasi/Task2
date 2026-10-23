namespace Api.DTOs
{
    public record UpdateProductRequestDto(string Name, decimal Price, byte[] RowVersion);
}
